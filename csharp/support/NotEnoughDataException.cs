
using System;

/// <summary>
/// Exception that is thrown when there is enough data in a buffer to unflatten an entire Message.
/// </summary>

namespace muscle.support
{
    public class NotEnoughDataException : IOException
    {
        private static long serialVersionUID = 234010774012207621L;
        private int _numMissingBytes;

        public NotEnoughDataException(int numMissingBytes)
        {
            _numMissingBytes = numMissingBytes;
        }

        public int getNumMissingBytes() { return _numMissingBytes; }
    }
}
