
/* This file is Copyright 2001 Level Control Systems.  See the included LICENSE.TXT file for details. */

/// <summary>
/// Exception that is thrown if you try to access a field in a Message by the wrong type (eg calling getInt() on a string field or somesuch)
/// </summary>

namespace muscle.message {
    public class FieldTypeMismatchException : MessageException
    {
        private static readonly long serialVersionUID = -8562539748755552587L;
        public FieldTypeMismatchException(string s) : base(s) { }
        public FieldTypeMismatchException() : base ("Message entry type mismatch") { }
    }
}
