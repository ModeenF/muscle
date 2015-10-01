
/* This file is Copyright 2001 Level Control Systems.  See the included LICENSE.TXT file for details. */

/// <summary>
/// Exception that is thrown if you try to access a Message field that isn't present in the Message.
/// </summary>

namespace muscle.message {
    public class FieldNotFoundException : MessageException
    {
       private static readonly long serialVersionUID = -1387490200161526582L;
       public FieldNotFoundException(string s) : base(s){ }
       public FieldNotFoundException() : base("Message entry not found"){ }
    }
}