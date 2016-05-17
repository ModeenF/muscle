///<summary>
/// Very similar to DataInputStream except it reads little-endian instead of
/// big-endian binary data.
/// We can't extend DataInputStream directly since it has only methods.
/// This forces us implement LEDataInputStream with a DataInputStream object,
/// and use wrapper methods.
/// </summary>
namespace muscle.support {
    public class LEDataInputStream : InputStream, DataInput {
        private static string _embeddedCopyright = "Copyright 1998 Roedy Green, Canadian Mind Products, http://mindpro_dataInputStream.com";
        private DataInputStream _dataInputStream; // to get at high level readFully methods of DataInputStream
        private InputStream _inStream;    // to get at the low-level read methods of InputStream
        private byte[] _byteArray; // work array for buffering input

        public LEDataInputStream(InputStream inStream) {
            _inStream = inStream;
            _dataInputStream = new DataInputStream(inStream);
            _byteArray = new byte[8];
        }

        ///<summary>
        /// L I T T L E   E N D I A N   R E A D E R S
        /// Little endian methods for multi-byte numeric types.
        /// Big-endian do fine for single-byte types and strings.
        /// like DataInputStream.readShort except little endian.</summary>
        ///<returns>short</returns>
        ///<exception cref="IOException"></exception>
        public short readShort() 
        {
            _dataInputStream.readFully(_byteArray, 0, 2);
            return (short)((_byteArray[1]&0xff) << 8 | (_byteArray[0]&0xff));
        }

        ///<summary>
        /// like DataInputStream.readUnsignedShort except little endian.
        /// Note, returns int even though it reads a short.</summary>
        ///<returns>int</returns>
        ///<exception cref="IOException"></exception>
        public int readUnsignedShort()
        {
            _dataInputStream.readFully(_byteArray, 0, 2);
            return (
            (_byteArray[1]&0xff) << 8 |
            (_byteArray[0]&0xff));
        }

        ///<summary>like DataInputStream.readChar except little endian.</summary>
        ///<returns>char</returns>
        ///<exception cref="IOException"></exception>
        public char readChar()
        {
            _dataInputStream.readFully(_byteArray, 0, 2);
            return (char) (
            (_byteArray[1]&0xff) << 8 |
            (_byteArray[0]&0xff));
        }

        ///<summary>like DataInputStream.readInt except little endian.</summary>
        ///<returns>int</returns>
        ///<exception cref="IOException"></exception>
        public int readInt()
        {
            _dataInputStream.readFully(_byteArray, 0, 4);
            return
            (_byteArray[3])      << 24 |
            (_byteArray[2]&0xff) << 16 |
            (_byteArray[1]&0xff) <<  8 |
            (_byteArray[0]&0xff);
        }

        ///<summary>like DataInputStream.readLong except little endian.</summary>
        ///<returns>long</returns>
        ///<exception cref="IOException"></exception>
        public long readLong()
        {
            _dataInputStream.readFully(_byteArray, 0, 8);
            return
            (long)(_byteArray[7])      << 56 |  /* long cast needed or shift done modulo 32 */
            (long)(_byteArray[6]&0xff) << 48 |
            (long)(_byteArray[5]&0xff) << 40 |
            (long)(_byteArray[4]&0xff) << 32 |
            (long)(_byteArray[3]&0xff) << 24 |
            (long)(_byteArray[2]&0xff) << 16 |
            (long)(_byteArray[1]&0xff) <<  8 |
            (long)(_byteArray[0]&0xff);
        }

        ///<summary>like DataInputStream.readFloat except little endian.</summary>
        ///<returns>float</returns>
        ///<exception cref="IOException"></exception>
        public float readFloat()
        {
            return Float.intBitsToFloat(readInt());
        }

        ///<summary>like DataInputStream.readDouble except little endian.</summary>
        ///<returns>double</returns>
        ///<exception cref="IOException"></exception>
        public double readDouble()
        {
            return Double.longBitsToDouble(readLong());
        }

        // p u r e l y   w r a p p e r   m e t h o d s
        // We can't simply inherit since dataInputStream is final.
        
        ///<summary>Watch out, may return fewer bytes than requested.</summary>
        ///<returns>int</returns>
        ///<exception cref="IOException"></exception>
        public int read(byte[] b, int off, int len)
        {
            // For efficiency, we avoid one layer of wrapper
            return _dataInputStream.read(b, off, len);
        }

        ///<summary></summary>
        ///<exception cref="IOException"></exception>
        public void readFully(byte[] b)
        {
            _dataInputStream.readFully(b, 0, b.length);
        }

        ///<summary></summary>
        ///<exception cref="IOException"></exception>
        public void readFully(byte[] b, int off, int len) 
        {
            _dataInputStream.readFully(b, off, len);
        }

        ///<summary></summary>
        ///<returns>int</returns>
        ///<exception cref="IOException"></exception>
        public int skipBytes(int n)
        {
            return _dataInputStream.skipBytes(n);
        }
        
        ///<summary>only reads one byte</summary>
        ///<returns>bool</returns>
        ///<exception cref="IOException"></exception>
        public bool readBoolean()
        {
            return _dataInputStream.readBoolean();
        }

        ///<summary></summary>
        ///<returns>byte</returns>
        ///<exception cref="IOException"></exception>
        public byte readByte()
        {
            return _dataInputStream.readByte();
        }

        ///<summary></summary>
        ///<returns>int</returns>
        ///<exception cref="IOException"></exception>
        public int read()
        {
            return _dataInputStream.read();
        }
        
        ///<summary>note: returns an int, even though says Byte.</summary>
        ///<returns>int</returns>
        ///<exception cref="IOException"></exception>
        public int readUnsignedByte()
        {
            return _dataInputStream.readUnsignedByte();
        }

        /** @deprecated */
        ///<summary></summary>
        ///<returns>string</returns>
        ///<exception cref="IOException"></exception>
        public string readLine()
        {
            return _dataInputStream.readLine();
        }

        ///<summary></summary>
        ///<returns>string</returns>
        ///<exception cref="IOException"></exception>
        public string readUTF()
        {
            return _dataInputStream.readUTF();
        }

        ///<summary></summary>
        ///<returns>string</returns>
        ///<exception cref="IOException"></exception>
        public static string readUTF(DataInput di)
        {
            return DataInputStream.readUTF(di);
        }

        ///<summary></summary>
        public void close()
        {
            _dataInputStream.close();
        }
    }
}