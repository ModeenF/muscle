using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using muscle.iogateway;
using muscle.message;

namespace muscle.client
{

    public delegate void DisconnectCallback(MessageTransceiver client, Exception err, object state);
    public delegate void MessagesCallback(Message [] messages, MessageTransceiver client, object state);

    public class MessageTransceiver : IDisposable 
    {

        private int encoding;
        private Socket socket = null;
        private IPEndPoint endPoint = null;
        private Queue sendQueue = null;
        private DisconnectCallback disconnectCallback = null;
        private MessagesCallback messagesCallback = null;
        private object disconnectState = null;
        private object messagesState = null;
        private bool run = true;
        private Thread processThread = null;
        byte[] write_buffer = null;
        byte[] read_buffer = null;
        int write_pos = 0;

        const int MAX_RECEIVED_BEFORE_CALLBACK = 200;
        const int RECEIVE_BUFFER_SIZE = 16384;

        public MessageTransceiver(string host, int port) : this(host, port, MessageIOGateway.MUSCLE_MESSAGE_ENCODING_DEFAULT){ }

        public MessageTransceiver(string host, int port, int encoding) 
        {
	        this.encoding = encoding;
            read_buffer = new byte[RECEIVE_BUFFER_SIZE];
            write_buffer = null;
            write_pos = 0;
            sendQueue = new Queue();
            run = true;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 

            IPHostEntry hEntry = Dns.GetHostByName(host);
            IPAddress ipaddress = hEntry.AddressList[0];
            endPoint = new IPEndPoint(ipaddress, port);   
        }

        public IAsyncResult BeginConnect(AsyncCallback callback, object state) 
        {
            return socket.BeginConnect(endPoint, callback, state);
        }

        public void Connect() 
        {
            lock (this) 
            {
                if (!run) 
                    throw new ObjectDisposedException("MessageTransceiver already disposed or connection terminated");
                socket.Connect(endPoint);
                socket.Blocking = false;
                StartThreads();
            }
        }

        public void EndConnect(IAsyncResult ar) 
        {
            lock (this) 
            {
                if (!run) 
                    throw new ObjectDisposedException("MessageTransceiver already disposed or connection terminated");

                socket.EndConnect(ar);
                socket.Blocking = false;

                if (run) 
                {
                    StartThreads();
                }
            }
        }

        private void StartThreads() 
        {
            processThread = new Thread(new ThreadStart(this.ThreadProc));
            processThread.Start();
        }

        public void RegisterForDisconnect(DisconnectCallback callback, object state) 
        {
            lock (this) 
            {
                if (!run) 
                {
                    throw new ObjectDisposedException("MessageTransceiver already disposed or connection terminated");
                }
                disconnectCallback = callback;
                disconnectState = state;
            }
        }
        public void RegisterForMessages(MessagesCallback callback, object state) 
        {
            lock (this) 
            {
                if (!run) 
                {
                    throw new ObjectDisposedException("MessageTransceiver already disposed or connection terminated");
                }
                messagesCallback = callback;
                messagesState = state;
            }
        }

        public void Send(Message message) 
        {
            lock (this) 
            {
                if (!run) 
                {
                    throw new ObjectDisposedException("MessageTransceiver already disposed or connection terminated");
                }

                lock (sendQueue) 
                {
                    bool notifyAll = false;

                    if (sendQueue.Count == 0)
                        notifyAll = true;

                    sendQueue.Enqueue(message);

                    if (notifyAll)
                        Monitor.PulseAll(sendQueue);

                }
            }
        }

        public void Send(Message [] messages) 
        {
            lock (this) 
            {
                if (!run) 
                {
                    throw new ObjectDisposedException("MessageTransceiver already disposed or connection terminated");
                }

                lock (sendQueue) 
                {
                    bool notifyAll = false;

                    if (sendQueue.Count == 0)
                        notifyAll = true;

                    foreach (Message message in messages) 
                    {
                        sendQueue.Enqueue(message);
                    }

                    if (notifyAll)
                        Monitor.PulseAll(sendQueue);
                }
            }
        }

        public void Close() 
        {
            Dispose();
        }

        public void Dispose() 
        {
            lock (this) 
            {
                run = false;
                try 
                {
                    processThread.Interrupt();
                }
                catch (Exception) 
                {
                    processThread.Abort();
                }
                finally 
                {
                    sendQueue = null;
                    write_buffer = null;
                    read_buffer = null;
                    socket.Close();
                }
            }
        }

        private void DoCheckRead(Socket s, MessageDecoder decoder)
        {
            if (s != null) 
            {
                while (run && socket.Available > 0) 
                {
                    int bytesRead = 0;
                    bytesRead = socket.Receive(read_buffer);
                    decoder.Decode(read_buffer, bytesRead);	

                    if (run && (bytesRead < read_buffer.Length || decoder.Received.Count >= MAX_RECEIVED_BEFORE_CALLBACK)) 
                    {
                        ArrayList received = decoder.Received;
                        Message [] array = (Message []) received.ToArray(typeof(Message));
                        received.Clear();

                        if (run && messagesCallback != null)
                            messagesCallback(array, this, messagesState);	  
                        break;
                    }
                }
            }
        }

        private void DoCheckWrite(Socket s, MessageEncoder encoder)
        {
            if (s != null) 
            {
                if (write_buffer != null) 
                {
                    while (run) 
                    {
                        int bytes_sent = socket.Send(write_buffer, write_pos, write_buffer.Length - write_pos, SocketFlags.None);

                        if (bytes_sent > 0) 
                        {
                            write_pos += bytes_sent;

                            if (write_pos == write_buffer.Length) 
                            {
                                write_buffer = null;
                                write_pos = 0;
                                break;
                            }
                        }
                        else if (bytes_sent == 0) 
                        {
                            break;
                        }
                    }
                }
                else 
                {
                    lock (sendQueue) 
                    {
                        if (sendQueue.Count > 0) 
                        {
                            while (run && sendQueue.Count > 0) 
                            {
                                Message m = (Message) sendQueue.Peek();
                                bool success = encoder.Encode(m);

                                if (success)
                                    sendQueue.Dequeue();
                                else
                                    break;
                            }

                            write_buffer = encoder.GetAndResetBuffer();
                        }
                    }
                }
            }
        }

        private void ProcessSocket() 
        {
            MessageEncoder encoder = new MessageEncoder();
            MessageDecoder decoder = new MessageDecoder();
            ArrayList list = new ArrayList();
            list.Add(socket);

            while (run) 
            {
                ArrayList checkRead = null;
                ArrayList checkWrite = null;
                ArrayList checkError = null;

                checkRead = (ArrayList) list.Clone();
                checkError = (ArrayList) list.Clone();

                lock (sendQueue) 
                {
                    if (sendQueue.Count > 0 || write_buffer != null)
                        checkWrite = (ArrayList) list.Clone();
                }

                Socket.Select(checkRead, checkWrite, checkError, 500000);

                if (checkRead != null && checkRead.Count > 0) 
                {
                    Socket s = (Socket) checkRead[0];
                    if (s.Available == 0)
                        throw new Exception("Remote side disconnected");

                    DoCheckRead(s, decoder);
                }

                if (checkWrite != null && checkWrite.Count > 0) 
                {
                    Socket s = (Socket) checkWrite[0];
                    DoCheckWrite(s, encoder);
                }
            }
        }

        internal void ThreadProc() 
        {
            try 
            {
                ProcessSocket();
            }
            catch (Exception e) 
            {
                try 
                {
                    disconnectCallback(this, e, disconnectState);
                }
                finally 
                {
                    Dispose();
                }
            }
        }


        public Socket Socket 
        {
            get { return socket; }
        }

        public IPEndPoint IPEndPoint 
        {
            get { return endPoint; }
        }        
    }
}

