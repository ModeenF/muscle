
/* This file is Copyright 2001 Level Control Systems.  See the included LICENSE.TXT file for details. */

  /// <summary>
  /// Base class for the various Exceptions that the Message class may throw
  /// </summary>

using System;

namespace muscle.message {
    public class MessageException : Exception
    {
        private static readonly long serialVersionUID = -7107595031653595454L;
        public MessageException(string s) : base(s) { }
        public MessageException() : base("Error accessing a Message field") { }
    }
}
