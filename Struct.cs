using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    class Struct
    {
        string lexema;
        string nome;
        int nivel;
        int rotulo;

        public Struct(string lexema, string nome, int nivel, int rotulo)
        {
            this.lexema = lexema;
            this.nome = nome;
            this.nivel = nivel;
            this.rotulo = rotulo;
        }
    }
}
