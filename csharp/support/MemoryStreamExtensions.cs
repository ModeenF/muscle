using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace muscle.support
{
    public class MemoryStreamExtended : MemoryStream
    {

        public static void Clear(this MemoryStream stream)
        {
            stream.SetLength(0);
        }

        public static long Remaining(this MemoryStream stream)
        {
            return stream.Length - stream.Position;
        }
    }
}
