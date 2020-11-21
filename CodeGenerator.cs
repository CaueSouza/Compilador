using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Compilador.Constantes;

namespace Compilador
{
    class CodeGenerator
    {
        private static List<string> VMCommands = new List<string>();

        public static void gera(string rotulo, string comando, string parametro1, string parametro2)
        {
            switch (comando)
            {
                case LDC:
                    break;
                case LDV:
                    break;
                case STR:
                    break;

                case ADD:
                case SUB:
                case MULT:
                case DIVI:
                case INV:
                case AND:
                case OR:
                case NEG:
                case CME:
                case CMA:
                case CEQ:
                case CDIF:
                case CMEQ:
                case CMAQ:
                case START:
                case HLT:
                case RETURN:
                case RD:
                case PRN:
                    geraComandoSemParam(comando);
                    break;

                
                case JMP:
                case JMPF:
                case CALL:
                    geraLabelCommand(comando, parametro1);
                    break;

                case NULL:
                    geraLabel(rotulo);
                    break;

                case ALLOC:
                    break;
                case DALLOC:
                    break;
            }
        }

        public static void geraComandoSemParam(string comando)
        {
            VMCommands.Add(comando);
        }

        public static void geraLabelCommand(string comando, string rotulo)
        {
            VMCommands.Add(comando + " L" + rotulo);
        }

        public static void geraLabel(string rotulo)
        {
            VMCommands.Add("L" + rotulo + NULL);
        }
    }
}
