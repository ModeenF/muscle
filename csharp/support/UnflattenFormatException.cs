using System.IO;
   
/// <summary>
/// Exception that is thrown when an unflatten() attempt fails, usually due to unrecognized data in the input stream, or a type mismatch.
/// </summary>

namespace muscle.support {  
    public class UnflattenFormatException : IOException {
        private static final long serialVersionUID = 234010774012207620L;
        public UnflattenFormatException() : base("unexpected bytes during Flattenable unflatten") { }
        public UnflattenFormatException(string s) : base(s) { }
    }
}


