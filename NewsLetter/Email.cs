using System;
using System.Collections.Generic;
using System.Text;

namespace NewsLetter
{
    class Email
    {        public bool Error { get; set; }
        public string ExceptionMessage { get; set; }
        public List<string> Emails { get; set; }
    }
}
