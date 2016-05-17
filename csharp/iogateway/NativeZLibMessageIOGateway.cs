using System.IO;

namespace muscle.iogateway {
    class NativeZLibMessageIOGateway : MessageIOGateway 
    {
        protected Inflater _inflater;
        protected Deflater _deflater;
        protected ByteArrayOutputStream _baos;
        protected DataOutput _dos;
   
        public NativeZLibMessageIOGateway() 
        {
        }

        public NativeZLibMessageIOGateway(int encoding) : base(encoding)
        {
        }

        override public void SetOutgoingEncoding(int newEncoding)
        {
            _deflater = null;
        }
        public void setOutgoingEncoding(int newEncoding) 
        {

        }

        public void flattenMessage(ByteChannel out, Message msg) throws IOException
        {
        int oge = getOutgoingEncoding();

        if (oge <= MUSCLE_MESSAGE_DEFAULT_ENCODING) 
        {
        super.flattenMessage(out, msg);
        return;
        }
        int flatSize = msg.flattenedSize();
        if (flatSize < 32) 
        {
        super.flattenMessage(out, msg);
        return;
        }

        boolean independent = false;
        if (_deflater == null) 
        {
        _deflater = new Deflater(oge - MUSCLE_MESSAGE_DEFAULT_ENCODING, false);
        _baos = new ByteArrayOutputStream(flatSize);
        _dos = new DataOutputStream(new DeflaterStreamResetHack(_baos, _deflater, oge - MUSCLE_MESSAGE_DEFAULT_ENCODING));
        independent = true;
        }

        ByteBuffer data = ByteBuffer.allocate(flatSize);
        data.order(ByteOrder.LITTLE_ENDIAN);
      
        msg.flatten(data);
        data.rewind();
      
        _dos.write(data.array());
        ((OutputStream) _dos).flush();
      
        data = ByteBuffer.allocate(16 + _baos.size());
        data.order(ByteOrder.LITTLE_ENDIAN);

        data.putInt(_baos.size() + 8);
        data.putInt(oge);
        data.putInt(independent ? ZLIB_CODEC_HEADER_INDEPENDENT : ZLIB_CODEC_HEADER_DEPENDENT);
        data.putInt(flatSize);
        data.put(_baos.toByteArray());
        out.write(data);
        _baos.reset();
        }
   
        public void flattenMessage(DataOutput out, Message msg) throws IOException
        {
        int oge = getOutgoingEncoding();
        int flatSize = msg.flattenedSize();
        if ((oge <= MUSCLE_MESSAGE_DEFAULT_ENCODING) || (flatSize < 32))
        {
        super.flattenMessage(out, msg);
        return;
        }

        boolean independent = false;
        if (_deflater == null) 
        {
        _deflater = new Deflater(oge - MUSCLE_MESSAGE_DEFAULT_ENCODING, false);
        _baos = new ByteArrayOutputStream(flatSize);
        _dos = new LEDataOutputStream(new DeflaterStreamResetHack(_baos, _deflater, oge - MUSCLE_MESSAGE_DEFAULT_ENCODING));
        independent = true;
        }

        msg.flatten(_dos);
        ((OutputStream)_dos).flush();
        out.writeInt(_baos.size() + 8);
        out.writeInt(oge);
        out.writeInt(independent ? ZLIB_CODEC_HEADER_INDEPENDENT : ZLIB_CODEC_HEADER_DEPENDENT);
        out.writeInt(flatSize);
        out.write(_baos.toByteArray());
        _baos.reset();
        }
   
        static class DeflaterStreamResetHack extends DeflaterOutputStream 
        {
        private int _level;

        public DeflaterStreamResetHack(OutputStream out, Deflater def, int level) 
        {
        super(out, def, 1024);
        this._level = level;
        }

        private static final byte [] EMPTYBYTEARRAY = new byte [0];

        /**
        * Insure all remaining data will be output.
        */
        public void flush() throws IOException 
        {
        /**
        * Now this is tricky: We force the Deflater to flush
        * its data by switching compression level.
        * As yet, a perplexingly simple workaround for 
        * http://developer.java.sun.com/developer/bugParade/bugs/4255743.html 
        */
        def.setInput(EMPTYBYTEARRAY, 0, 0);
        def.setLevel(Deflater.NO_COMPRESSION);
        deflate();
        def.setLevel(_level);
        deflate();
        out.flush();
        }
        }
    }
}