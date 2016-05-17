using System;
using System.IO;

using muscle.iogateway;
using muscle.message;


namespace muscle.client
{
    public class MessageEncoder
    {
        private byte[] buffer = null;
        private int pos = 0;
        private MessageIOGateway gw = null;
        private MemoryStream memStream = null;

        const int BUFFER_SIZE = 1048576;

        public MessageEncoder() : this(MessageIOGateway.MUSCLE_MESSAGE_ENCODING_DEFAULT)
        { }

        public MessageEncoder(int encoding)
        {
            gw = new MessageIOGateway();
            buffer = new byte[BUFFER_SIZE];
            pos = 0;
            memStream = new MemoryStream(buffer, 0, BUFFER_SIZE);
        }

        public bool Encode(Message m)
        {
            int required = m.flattenedSize();
            required += MessageIOGateway.MESSAGE_HEADER_SIZE;

            if (buffer.Length - pos < required)
                return false;
            else
            {
                gw.flattenMessage(memStream, m);
                pos += required;
                return true;
            }
        }

        public byte[] GetAndResetBuffer()
        {
            byte[] buffer_copy = new byte[pos];
            Buffer.BlockCopy(buffer, 0, buffer_copy, 0, pos);
            memStream.Seek(0, SeekOrigin.Begin);
            pos = 0;
            return buffer_copy;
        }
    }
}