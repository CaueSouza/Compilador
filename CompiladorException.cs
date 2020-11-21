using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    class CompiladorException : Exception
    {
        //Constructors. It is recommended that at least all the
        //constructors of
        //base class Exception are implemented
        public CompiladorException() : base() { }
        public CompiladorException(string message) : base(message) { }
        public CompiladorException(string message, Exception e) : base(message, e) { }
        //If there is extra error information that needs to be captured
        //create properties for them.
    }
}
