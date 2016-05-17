using System;
using System.Collections;
using System.IO;

using muscle.iogateway;
using muscle.message;

namespace muscle.client
{
    public class MessageDecoder
    {

        const int BUFFER_SIZE = 262144;

        private byte[] buffer = null;
        private int length = 0;
        private ArrayList receiveList = null;
        private MessageIOGateway gw = null;
        private int messageSize = 0;
        private bool haveSize = false;
        private MemoryStream memStream = null;

        public MessageDecoder()
        {
            receiveList = new ArrayList();
            gw = new MessageIOGateway();
            buffer = new byte[BUFFER_SIZE];
            memStream = new MemoryStream(buffer);
        }

        public void Decode(byte[] buf, int len)
        {
            int bytesToCopy = (buffer.Length - length < len) ? buffer.Length - length : len;
            int bytesRemaining = (bytesToCopy < len) ? len - bytesToCopy : 0;
            Buffer.BlockCopy(buf, 0, buffer, length, bytesToCopy);
            length += bytesToCopy;

            while (bytesRemaining > 0 || length > 0)
            {
                if (!haveSize)                     // get message size
                {
                    if (length < 4)
                        return;

                    messageSize = BitConverter.ToInt32(buffer, 0);
                    if (!BitConverter.IsLittleEndian)
                    {
                        messageSize = (int) ((((uint)messageSize) << 24) | ((((uint)messageSize) & 0xff00) << 8) | ((((uint)messageSize) & 0xff0000) >> 8) | ((((uint)messageSize) >> 24)));
                    }

                    messageSize += MessageIOGateway.MESSAGE_HEADER_SIZE;

                    if (messageSize > buffer.Length)
                    {
                        byte[] tmpArray = buffer;
                        buffer = new byte[messageSize];
                        memStream = new MemoryStream(buffer);
                        Buffer.BlockCopy(tmpArray, 0, buffer, 0, length);
                    }

                    haveSize = true;
                }
                else if (length < messageSize)     // build buffer out to at least message size
                {
                    int bytesAvailableInBuffer = buffer.Length - length;
                    bytesToCopy = (bytesRemaining > bytesAvailableInBuffer) ? bytesAvailableInBuffer : bytesRemaining;

                    if (bytesToCopy == 0)
                        return;

                    Buffer.BlockCopy(buf, len - bytesRemaining, buffer, length, bytesToCopy);
                    length += bytesToCopy;
                    bytesRemaining -= bytesToCopy;
                }
                else  // enough in buffer to process message
                {
                    memStream.Seek(0, SeekOrigin.Begin);
                    Message m = gw.unflattenMessage(memStream);
                    receiveList.Add(m);
                    Buffer.BlockCopy(buffer, messageSize, buffer, 0, length - messageSize);
                    length -= messageSize;
                    haveSize = false;
                }
            }
        }

        public ArrayList Received
        {
            get { return receiveList; }
            set { receiveList = value; }
        }
    }
}
