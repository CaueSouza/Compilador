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
                case LDV:
                case STR:
                    geraComando1Param(comando, parametro1);
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
                case DALLOC:
                    geraComando2Param(comando, parametro1, parametro2);
                    break;
            }
        }

        private static void geraComandoSemParam(string comando)
        {
            VMCommands.Add(comando);
        }

        private static void geraLabelCommand(string comando, string rotulo)
        {
            VMCommands.Add(comando + " L" + rotulo);
        }

        private static void geraLabel(string rotulo)
        {
            VMCommands.Add("L" + rotulo + NULL);
        }

        private static void geraComando2Param(string comando, string parametro1, string parametro2)
        {
            VMCommands.Add(comando + " " + parametro1 + "," + parametro2);
        }

        private static void geraComando1Param(string comando, string parametro)
        {
            VMCommands.Add(comando + " " + parametro);
        }
    }
}
