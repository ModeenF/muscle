using System;

using muscle.message;
using muscle.client;
using muscle.iogateway;

namespace muscle.test
{
    public class TestDecoding : StorageReflectConstants {
        const int MESSAGE_COUNT = 50000;

        static void Main() {
            MessageIOGateway gw = new MessageIOGateway();
            MessageDecoder d = new MessageDecoder();
            MessageEncoder e = new MessageEncoder();

            Message m = new Message();
            m.what = PR_COMMAND_GETDATA;
            string text = "*";
            m.setString(PR_NAME_KEYS, text);

            e.Encode(m);

            byte [] buffer = e.GetAndResetBuffer();
            long start = DateTime.Now.Ticks;

            for (int i = 0; i < MESSAGE_COUNT; ++i) {
                d.Decode(buffer, buffer.Length);
            }

            long end = DateTime.Now.Ticks;
            long elapsed = end - start;

            DateTime t = new DateTime(elapsed);

            Console.WriteLine("Elapsed time: " + t.ToString());
            Console.WriteLine("Messages flattened and unflattened: " + MESSAGE_COUNT.ToString());
        }
    }
}
