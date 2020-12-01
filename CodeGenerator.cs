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

        public static void cleanCommands()
        {
            VMCommands.Clear();
        }

        public static List<string> getVMCommands()
        {
            return VMCommands;
        }

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
            VMCommands.Add("L" + rotulo + " " + NULL);
        }

        private static void geraComando2Param(string comando, string parametro1, string parametro2)
        {
            VMCommands.Add(comando + " " + parametro1 + "," + parametro2);
        }

        private static void geraComando1Param(string comando, string parametro)
        {
            VMCommands.Add(comando + " " + parametro);
        }

        public static void geraCodeExpression(List<string> posFixExpression)
        {
            foreach (String field in posFixExpression)
            {
                switch (field)
                {
                    case "*":
                        gera(EMPTY_STRING, MULT, EMPTY_STRING, EMPTY_STRING);
                        break;
                    case "div":
                        gera(EMPTY_STRING, DIVI, EMPTY_STRING, EMPTY_STRING);
                        break;
                    case "=":
                        gera(EMPTY_STRING, CEQ, EMPTY_STRING, EMPTY_STRING);
                        break;
                    case "!=":
                        gera(EMPTY_STRING, DIF, EMPTY_STRING, EMPTY_STRING);
                        break;
                    case ">":
                        gera(EMPTY_STRING, CMA, EMPTY_STRING, EMPTY_STRING);
                        break;
                    case ">=":
                        gera(EMPTY_STRING, CMAQ, EMPTY_STRING, EMPTY_STRING);
                        break;
                    case "<":
                        gera(EMPTY_STRING, CME, EMPTY_STRING, EMPTY_STRING);
                        break;
                    case "<=":
                        gera(EMPTY_STRING, CMEQ, EMPTY_STRING, EMPTY_STRING);
                        break;
                    case "e":
                        gera(EMPTY_STRING, AND, EMPTY_STRING, EMPTY_STRING);
                        break;
                    case "ou":
                        gera(EMPTY_STRING, OR, EMPTY_STRING, EMPTY_STRING);
                        break;
                    case "nao":
                        gera(EMPTY_STRING, NEG, EMPTY_STRING, EMPTY_STRING);
                        break;
                    case "+":
                        gera(EMPTY_STRING, ADD, EMPTY_STRING, EMPTY_STRING);
                        break;
                    case "+u":
                        break;
                    case "-u":
                        gera(EMPTY_STRING, INV, EMPTY_STRING, EMPTY_STRING);
                        break;
                    case "-":
                        gera(EMPTY_STRING, SUB, EMPTY_STRING, EMPTY_STRING);
                        break;
                    case "verdadeiro":
                        gera(EMPTY_STRING, LDC, "1", EMPTY_STRING);
                        break;
                    case "falso":
                        gera(EMPTY_STRING, LDC, "0", EMPTY_STRING);
                        break;

                    default:
                        if (field.All(char.IsDigit)) //é digito
                        {
                            gera(EMPTY_STRING, LDC, field, EMPTY_STRING);
                        }
                        else
                        {
                            Struct structField = Semantico.pesquisaTabela(field,0);

                            if (structField.nome.Equals(NOME_FUNCAO))
                            {
                                CodeGenerator.gera(EMPTY_STRING, CALL, structField.rotulo.ToString(), EMPTY_STRING);
                                CodeGenerator.gera(EMPTY_STRING, LDV, FUNCTION_RETURN_LABEL, EMPTY_STRING);
                            }
                            else
                            {
                                CodeGenerator.gera(EMPTY_STRING, LDV, structField.rotulo.ToString(), EMPTY_STRING);
                            }
                        }

                        break;
                }
            }
        }
    }
}
