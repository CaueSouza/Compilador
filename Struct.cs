using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    class Struct
    {
        public string lexema { get; set; }
        public string nome { get; set; }
        public int nivel { get; set; }
        public int rotulo { get; set; }
        public string tipo { get; set; }

        public Struct(string lexema, string nome, int nivel, int rotulo)
        {
            this.lexema = lexema;
            this.nome = nome;
            this.nivel = nivel;
            this.rotulo = rotulo;
        }
    }
}
