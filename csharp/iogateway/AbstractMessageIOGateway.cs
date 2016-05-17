using System.IO;

using muscle.support;
using muscle.message;

/// <summary>
/// Interface for an object that knows how to translate bytes into Messages, and vice versa
/// </summary>

namespace muscle.iogateway {
    public interface AbstractMessageIOGateway {
       public static int MUSCLE_MESSAGE_DEFAULT_ENCODING =  1164862256;  // 'Enc0'  ... our default (plain-vanilla) encoding 
       public static int MUSCLE_MESSAGE_ENCODING_ZLIB_1  =  1164862257;  
       public static int MUSCLE_MESSAGE_ENCODING_ZLIB_2  =  1164862258;  
       public static int MUSCLE_MESSAGE_ENCODING_ZLIB_3  =  1164862259;  
       public static int MUSCLE_MESSAGE_ENCODING_ZLIB_4  =  1164862260;  
       public static int MUSCLE_MESSAGE_ENCODING_ZLIB_5  =  1164862261;  
       /** This is the recommended CPU vs space-savings setting for zlib */
       public static int MUSCLE_MESSAGE_ENCODING_ZLIB_6  =  1164862262;  
       public static int MUSCLE_MESSAGE_ENCODING_ZLIB_7  =  1164862263;  
       public static int MUSCLE_MESSAGE_ENCODING_ZLIB_8  =  1164862264;  
       public static int MUSCLE_MESSAGE_ENCODING_ZLIB_9  =  1164862265;

        /// <summary>
        /// Reads from the input stream until a Message can be assembled and returned.
        ///</summary>
        ///
        /// <param name="dataIn">The input stream to read from.</param>
        /// <exception cref="IOException">if there is an error reading from the stream.</exception>
        /// <exception cref="UnflattenFormatException">if there is an error parsing the data in the stream.</exception>
        /// <returns>The next assembled Message.</returns>
        /// 
        public Message unflattenMessage(DataInput dataIn);
        //    Message unflattenMessage(Stream inputStream);

        /// <summary>
        /// Unflattens the Message based on the contents of the supplied ByteBuffer instead.
        ///</summary>
        ///
        /// <param name="dataIn">The ByteBuffer to read from.</param>
        /// <exception cref="IOException">if there is an error reading from the byte buffer.</exception>
        /// <exception cref="UnflattenFormatException">if there is an error parsing the data in the byte buffer.</exception>
        /// <exception cref="NotEnoughDataException">if the byte buffer doesn't contain enough data to unflatten a message.</exception>
        /// <returns>The next assembled Message.</returns>
        ///
        public Message unflattenMessage(ByteBuffer dataIn);
        //    Message unflattenMessage(Stream inputStream);

        /// <summary>
        /// Converts the given Message into bytes and sends it out the stream.
        /// </summary>
        ///
        /// <param name="dataOut">The data stream to send the converted bytes to.</param>
        /// <param name="msg">The Message to convert.</param>
        /// <exception cref="IOException">If there is an error writing to the stream.</exception>
        public void flattenMessage(DataOutput dataOut, Message msg);
        //    void flattenMessage(Stream outputStream, Message msg);
   
        /// <summary>
        /// Fattens the Message to supplied ByteChannel instead.
        /// </summary>
        ///
        /// <param name="dataOut">The ByteChannel send the converted bytes to.</param>
        /// <param name="msg">The Message to convert.</param>
        /// <exception cref="IOException">If there is an error writing to the ByteChannel.</exception>
        public void flattenMessage(ByteChannel dataOut, Message msg);
        //    void flattenMessage(Stream outputStream, Message msg);

        /// <summary>
        /// Converts the given Message into a ByteBuffer and returns the filled buffer to the caller. 
        /// The Buffer's limit will be set to the end of the message, and the buffer's position will be set to the start of the message.
        /// </summary>
        ///
        /// <param name="msg">The Message to flatten.</param>
        /// <returns>A filled ByteBuffer</returns>
        /// <exception cref="IOException">If there is an error writing to the ByteChannel.</exception>
        public ByteBuffer flattenMessage(Message msg);
        //    void flattenMessage(Stream outputStream, Message msg);

        /// <summary>
        /// Should be implemented to return the largest allowable incoming message size, in bytes.
        /// If there is no limit, this method should be implemented to return Integer.MAX_VALUE.
        /// </summary>
        ///
        /// <returns>return the largest allowable incoming message size</returns>
        public int getMaximumIncomingMessageSize();

        /// <summary>
        /// Change the currently used outgoing encoding. 
        /// </summary>
        ///
        /// <param name="newEncoding"></param>
        public void setOutgoingEncoding(int newEncoding);
    }
}
