
using System;
using System.IO;

using muscle.support;
using muscle.message;

/// <summary>
/// A gateway that converts to and from the 'standard' MUSCLE 
/// flattened message byte stream.
/// </summary>
/// 
namespace muscle.iogateway {
    public class MessageIOGateway : AbstractMessageIOGateway
    {
        private int _outgoingEncoding;
        private ByteBuffer _outgoing;
        public const int MESSAGE_HEADER_SIZE = 8;

        // 'Enc0' -- just plain ol' flattened Message objects, with no 
        // special encoding 
        public const int MUSCLE_MESSAGE_ENCODING_DEFAULT = 1164862256;

        private int _maximumIncomingMessageSize = Integer.MAX_VALUE;

        // zlib encoding levels
        public const int MUSCLE_MESSAGE_ENCODING_ZLIB_1 = 1164862257;
        public const int MUSCLE_MESSAGE_ENCODING_ZLIB_2 = 1164862258;
        public const int MUSCLE_MESSAGE_ENCODING_ZLIB_3 = 1164862259;
        public const int MUSCLE_MESSAGE_ENCODING_ZLIB_4 = 1164862260;
        public const int MUSCLE_MESSAGE_ENCODING_ZLIB_5 = 1164862261;
        public const int MUSCLE_MESSAGE_ENCODING_ZLIB_6 = 1164862262;
        public const int MUSCLE_MESSAGE_ENCODING_ZLIB_7 = 1164862263;
        public const int MUSCLE_MESSAGE_ENCODING_ZLIB_8 = 1164862264;
        public const int MUSCLE_MESSAGE_ENCODING_ZLIB_9 = 1164862265;
        public const int MUSCLE_MESSAGE_ENCODING_END_MARKER = 1164862266;

        public const int ZLIB_CODEC_HEADER_INDEPENDENT = 2053925219; // 'zlic'    
        public const int ZLIB_CODEC_HEADER_DEPENDENT = 2053925218; // 'zlib'
        
        public MessageIOGateway() : this(MUSCLE_MESSAGE_ENCODING_DEFAULT)
        {
            _outgoingEncoding = MUSCLE_MESSAGE_ENCODING_DEFAULT;
        }
    
        /// <summary>
        /// Constructs a MessageIOGateway whose outgoing encoding is one of MUSCLE_MESSAGE_ENCODING_*.
        ///</summary>
        private MessageIOGateway(int encoding) 
        {
            setOutgoingEncoding(encoding);
        }

        public void setOutgoingEncoding(int newEncoding)
        {
            if ((newEncoding < MUSCLE_MESSAGE_DEFAULT_ENCODING) || (newEncoding > MUSCLE_MESSAGE_ENCODING_ZLIB_9)) throw new UnsupportedOperationException("The argument is not a supported encoding");
            _outgoingEncoding = newEncoding;
        }

        /// <summary>
        /// Returns this gateway's current MUSCLE_MESSAGE_ENCODING_* value */
        /// </summary>
        /// 
        public int getOutgoingEncoding()
        {
            return _outgoingEncoding;
        }

        /// <summary>
        /// Set the largest allowable size for incoming Message objects.  Default value is Integer.MAX_VALUE. */
        /// </summary>
        /// 
        public void setMaximumIncomingMessageSize(int maxSize) { _maximumIncomingMessageSize = maxSize; }
    
        /// <summary>
        /// Returns the current maximum-incoming-message-size.  Default value is Integer.MAX_VALUE. */
        /// </summary>
        /// 
        public int getMaximumIncomingMessageSize() { return _maximumIncomingMessageSize; }

        /// <summary>
        /// Reads from the input stream until a Message can be assembled 
        /// and returned.
        /// </summary>
        ///
        /// <param name="reader">the input stream from which to read</param>
        /// <exception cref="IOException"/>
        /// <exception cref="UnflattenFormatException"/>
        /// <returns>The next assembled Message</returns>
        ///
        public Message unflattenMessage(Stream inputStream)
        {
            BinaryReader reader = new BinaryReader(inputStream);
            int numBytes = reader.ReadInt32();
            int encoding = reader.ReadInt32();

            int independent = reader.ReadInt32();
            inputStream.Seek(-4, SeekOrigin.Current);

            Message pmsg = new Message();
            pmsg.unflatten(reader, numBytes);

            return pmsg;
        }
        
        /* Java
        public Message unflattenMessage(ByteBuffer in) throws IOException, UnflattenFormatException, NotEnoughDataException
        {
        if (in.remaining() < 8) {
        in.position(in.limit());
        throw new NotEnoughDataException(8-in.remaining());
        }

        int numBytes = in.getInt();
        if (numBytes > getMaximumIncomingMessageSize()) throw new UnflattenFormatException("Incoming message was too large! (" + numBytes + " bytes, " + getMaximumIncomingMessageSize() + " allowed!)");

        int encoding = in.getInt();
        if (encoding != MUSCLE_MESSAGE_DEFAULT_ENCODING) throw new IOException("ByteBuffer: " + in + " numBytes: " + numBytes + " encoding: " + encoding);
        if (in.remaining() < numBytes) 
        {
        in.position(in.limit());
        throw new NotEnoughDataException(numBytes-in.remaining()); 
        }

        Message pmsg = new Message();
        pmsg.unflatten(in, numBytes);
        return pmsg;
        }
*/    
   /** Reads from the input stream until a Message can be assembled and returned.
     * @param in The input stream to read from.
     * @throws IOException if there is an error reading from the stream.
     * @throws UnflattenFormatException if there is an error parsing the data in the stream.
     * @return The next assembled Message.
     */
/* Java
   public Message unflattenMessage(DataInput in) throws IOException, UnflattenFormatException
   {
      int numBytes = in.readInt();
      if (numBytes > getMaximumIncomingMessageSize()) throw new UnflattenFormatException("Incoming message was too large! (" + numBytes + " bytes, " + getMaximumIncomingMessageSize() + " allowed!)");

      int encoding = in.readInt();
      if (encoding != MUSCLE_MESSAGE_DEFAULT_ENCODING) throw new IOException(); 
      Message pmsg = new Message();
      pmsg.unflatten(in, numBytes);
      return pmsg;
   }
*/
   /** Converts the given Message into bytes and sends it out the ByteChannel.
     * This method will block if necessary, even if the ByteChannel is
     * non-blocking.  If you want to do 100% proper non-blocking I/O,
     * you'll need to use the version of flattenMessage() that returns
     * a ByteBuffer, and then handle the byte transfers yourself.
     * @param out the ByteChannel to send the converted bytes to.
     * @param msg the Message to convert.
     * @throws IOException if there is an error writing to the stream.
     */
/* Java
        public void flattenMessage(ByteChannel out, Message msg) throws IOException
   {
      int flattenedSize = msg.flattenedSize();
      if ((_outgoing == null)||(_outgoing.capacity() < 8+flattenedSize))
      {
          _outgoing = ByteBuffer.allocate(8+flattenedSize);
          _outgoing.order(ByteOrder.LITTLE_ENDIAN);
      }
      _outgoing.rewind();
      _outgoing.limit(8+flattenedSize);
      
      _outgoing.putInt(flattenedSize);
      _outgoing.putInt(MUSCLE_MESSAGE_DEFAULT_ENCODING);
      msg.flatten(_outgoing);
      _outgoing.rewind();

      if (out instanceof SelectableChannel)
      {
         SelectableChannel sc = (SelectableChannel) out;
         if (!sc.isBlocking()) 
         {
           int numBytesWritten = 0;
           
           Selector selector = Selector.open();
           sc.register(selector, SelectionKey.OP_WRITE);
           while(_outgoing.remaining() > 0) 
           {
              if (numBytesWritten == 0) 
              {
                 selector.select();

                 // Slight shortcut here - there's only one key registered in this selector object.
                 if (selector.selectedKeys().isEmpty()) continue;
                                                   else selector.selectedKeys().clear();
              }
              numBytesWritten = out.write(_outgoing);
           }
           selector.close();
         }
         else out.write(_outgoing);
      }
      else out.write(_outgoing);
   }
*/

        /// <summary>
        /// Converts the given Message into bytes and sends it out the stream.
        /// </summary>
        ///
        /// <param name="outDO">the data stream to send the converted bytes to.</param>
        /// <param name="msg">the Message to convert.</param>
        /// <exception cref="IOException"> if there is an error writing to the stream.</exception>
        /// <returns>The next assembled Message</returns>
        ///
        public void flattenMessage(DataOutput outDO, Message msg)
        {
            outDO.writeInt(msg.flattenedSize());
            outDO.writeInt(MUSCLE_MESSAGE_DEFAULT_ENCODING);
            msg.flatten(outDO);
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        /// <param name="msg">a message</param>
        /// <exception cref="IOException"/>
        /// <returns>The next assembled Message</returns>
        ///
        public ByteBuffer flattenMessage(Message msg)
        {
            ByteBuffer buffer;
            int flattenedSize = msg.flattenedSize();
            buffer = ByteBuffer.allocate(flattenedSize+8);
            buffer.order(ByteOrder.LITTLE_ENDIAN);
            buffer.rewind();
            buffer.limit(8+flattenedSize);
       
            buffer.putInt(flattenedSize);
            buffer.putInt(MUSCLE_MESSAGE_DEFAULT_ENCODING);
            msg.flatten(buffer);
            buffer.rewind();
            return buffer;
        }
    
        /// <summary>
        /// Converts the given Message into bytes and sends it out the stream.
        /// </summary>
        /// <param name="outputStream">the data stream to send the converted bytes
        /// </param>
        /// <param name="msg">the message to convert</param>
        /// <exception cref="IOException"/>
        ///
        public void flattenMessage(Stream outputStream, Message msg)
        {
            int flattenedSize = msg.flattenedSize();

            if (flattenedSize >= 32 &&
                _encoding >= MUSCLE_MESSAGE_ENCODING_ZLIB_1 &&
                _encoding <= MUSCLE_MESSAGE_ENCODING_ZLIB_9) {

                // currently do not compress outgoing messages do later
                BinaryWriter writer = new BinaryWriter(outputStream);
                writer.Write((int) flattenedSize);
                writer.Write((int) MessageIOGateway.MUSCLE_MESSAGE_ENCODING_DEFAULT);
                msg.flatten(writer);
                writer.Flush();
            } else {
                BinaryWriter writer = new BinaryWriter(outputStream);
                writer.Write((int) flattenedSize);
                writer.Write((int) MessageIOGateway.MUSCLE_MESSAGE_ENCODING_DEFAULT);
                msg.flatten(writer);
                writer.Flush();
            }
        }    
    }
}
