
namespace muscle.iogateway {
    public class MessageIOGatewayFactory 
    {
        // This class is a factory and shouldn't be created anywhere
        private MessageIOGatewayFactory() 
        {
            // empty
        }
   
        public static AbstractMessageIOGateway getMessageIOGateway() 
        {
            return MessageIOGatewayFactory.getMessageIOGateway(AbstractMessageIOGateway.MUSCLE_MESSAGE_DEFAULT_ENCODING);
        }
   
        public static AbstractMessageIOGateway getMessageIOGateway(int encoding)
        {
            return getMessageIOGateway(encoding, int.MaxValue);
        }
   
        public static AbstractMessageIOGateway getMessageIOGateway(int encoding, int maxIncomingMessageSize) 
        {
            MessageIOGateway ret = null;
      
            if (encoding == AbstractMessageIOGateway.MUSCLE_MESSAGE_DEFAULT_ENCODING)
                ret = new MessageIOGateway();
            else
            {
                // else, try to get the best zlib implementation currently available
                try {
                    if (Class.forName("com.jcraft.jzlib.JZlib") != null)
                    {
                        // Found the JZlib library on the classpath, now return a reference to JZLibMessageIOGateway
                        Type gatewayClass = Class.forName("com.meyer.muscle.iogateway.JZLibMessageIOGateway");
                        ret = (MessageIOGateway) gatewayClass.getConstructor(new Type[] {Integer.TYPE}).newInstance(new object[] {new int?(encoding)});
                    }
                } catch (Exception e) {
                    // If any problems occur, we will fall back to the native class!
                }
                if (ret == null)
                    ret = new NativeZLibMessageIOGateway(encoding);
            }

            if (ret != null)
                ret.SetMaximumIncomingMessageSize(maxIncomingMessageSize);

            return ret;
        }
    }
}