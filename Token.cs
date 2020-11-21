using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    class Token
    {
        public String simbol { get; }
        public String lexem { get; }
        public bool isError { get; }
        public int line { get; }
        public int errorType { get; }

        public Token(string simbol, string lexem, int line)
        {
            this.simbol = simbol;
            this.lexem = lexem;
            isError = false;
            this.line = line;
        }

        public Token(string lexem, int errorLine, int errorType)
        {
            this.lexem = lexem;
            isError = true;
            line = errorLine;
            this.errorType = errorType;
        }
    }
}
