using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    class Token
    {
        private String simbol;
        private String lexem;
        private bool isError;
        private int line;
        private int errorType;

        public Token(string simbol, string lexem, int line)
        {
            this.simbol = simbol;
            this.lexem = lexem;
            isError = false;
            this.line = line;
        }

        public Token(int errorLine, int errorType)
        {
            isError = true;
            line = errorLine;
            this.errorType = errorType;
        }

        public string getSimbol() { return simbol; }

        public string getLexem() { return lexem; }

        public bool getIsError() { return isError; }

        public int getLine() { return line; }

        public int getErrorType() { return errorType; }
    }
}
