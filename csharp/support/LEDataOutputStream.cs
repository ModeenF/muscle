using System.IO;
/**
* Very similar to DataOutputStream except it writes little-endian instead of
* big-endian binary data.
* We can't extend DataOutputStream directly since it has only methods.
* This forces us implement LEDataOutputStream with a DataOutputStream object,
* and use wrapper methods.
*/
namespace muscle.support {
    public class LEDataOutputStream : OutputStream, DataOutput {

        private DataOutputStream d; // to get at high level write methods of DataOutputStream
        private byte w[]; // work array for composing output
        private static string _embeddedCopyright = "Copyright 1998 Roedy Green, Canadian Mind Products, http://mindprod.com";

        public LEDataOutputStream(Stream sOut) {
            this.d = new Stream(sOut);
            w = new byte[8]; // work array for composing output
        }

        // L I T T L E   E N D I A N   W R I T E R S
        // Little endian methods for multi-byte numeric types.
        // Big-endian do fine for single-byte types and strings.

        ///<summary>like DataOutputStream.writeShort. Also acts as a writeUnsignedShort</summary>
        ///<param name="v"></param>
        ///<exception cref="IOException"></exception>
        public void writeShort(int v)
        {
            w[0] = (byte) v;
            w[1] = (byte)(v >> 8);
            d.write(w, 0, 2);
        }


        ///<summary>like DataOutputStream.writeChar.
        ///Note the parm is an int even though this as a writeChar
        /// </summary>
        ///<param name="v"></param>
        ///<exception cref="IOException"></exception>
        public void writeChar(int v)
        {
        // same code as writeShort
            w[0] = (byte) v;
            w[1] = (byte)(v >> 8);
            d.write(w, 0, 2);
        }


        ///<summary>like DataOutputStream.writeInt.</summary>
        ///<param name="v"></param>
        ///<exception cref="IOException"></exception>
        public void writeInt(int v)
        {
            w[0] = (byte) v;
            w[1] = (byte)(v >> 8);
            w[2] = (byte)(v >> 16);
            w[3] = (byte)(v >> 24);
            d.write(w, 0, 4);
        }

        ///<summary>like DataOutputStream.writeLong.</summary>
        ///<param name="v"></param>
        ///<exception cref="IOException"></exception>
        public void writeLong(long v)
        {
            w[0] = (byte) v;
            w[1] = (byte)(v >> 8);
            w[2] = (byte)(v >> 16);
            w[3] = (byte)(v >> 24);
            w[4] = (byte)(v >> 32);
            w[5] = (byte)(v >> 40);
            w[6] = (byte)(v >> 48);
            w[7] = (byte)(v >> 56);
            d.write(w, 0, 8);
        }


        ///<summary>like DataOutputStream.writeFloat.</summary>
        ///<param name="v"></param>
        ///<exception cref="IOException"></exception>
        public void writeFloat(float v)
        {
            writeInt(Float.floatToIntBits(v));
        }


        ///<summary>like DataOutputStream.writeDouble.</summary>
        ///<param name="v"></param>
        ///<exception cref="IOException"></exception>
        public void writeDouble(double v)
        {
            writeLong(Double.doubleToLongBits(v));
        }

        ///<summary>like DataOutputStream.writeChars, flip each char.</summary>
        ///<param name="s"></param>
        ///<exception cref="IOException"></exception>
        public void writeChars(string s)
        {
            int len = s.length();
            for ( int i = 0 ; i < len ; i++ ) {
                writeChar(s.charAt(i));
            }
        } // end writeChars

        // p u r e l y   w r a p p e r   m e t h o d s
        // We cannot inherit since DataOutputStream is final.

        /* This method writes only one byte, even though it says int */
        public void write(int b) {
            d.write(b);
        }

        public void write(byte b[], int off, int len) 
        {
            d.write(b, off, len);
        }

        public void flush() {
            d.flush();
        }

        /* Only writes one byte */
        public void writeBoolean(boolean v) {
            d.writeBoolean(v);
        }

        public void writeByte(int v){
            d.writeByte(v);
        }

        public void writeBytes(string s) {
            d.writeBytes(s);
        }

        public void writeUTF(string str)
        {
            d.writeUTF(str);
        }

        public int size() {
            return d.size();
        }

        public void write(byte b[]) {
            d.write(b, 0, b.length);
        }

        public  void close()
        {
            d.close();
        }
    } // end LEDataOutputStream
}