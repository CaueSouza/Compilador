using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Compilador.Constantes;

namespace Compilador
{
    class Semantico
    {
        private Stack stack;
        private int actualLevel = 0;
        private List<string> expression = new List<string>();
        private List<string> posFixExpression = new List<string>();
        string finalPosFixExpression = "";

        public Semantico()
        {
            stack = new Stack();
        }

        public void increaseLevel()
        {
            actualLevel++;
        }

        public void decreaseLevel()
        {
            actualLevel--;
        }

        public void resetStack()
        {
            stack.cleanStack();
        }

        public void insereTabela(string lexema, string nome, int rotulo)
        {
            stack.push(new Struct(lexema, nome, actualLevel, rotulo));
        }

        public bool pesquisaDuplicVarTabela(string lexema)
        {
            int stackSize = stack.getLength();

            for (int i = stackSize; i >= 0; i--)
            {
                Struct actualItem = stack.getPosition(i);

                if (actualItem.lexema.Equals(lexema))
                {
                    if (actualItem.nome.Equals(NOME_PROCEDIMENTO) || actualItem.nome.Equals(NOME_FUNCAO))
                    {
                        return true;
                    }
                    else if (actualItem.nome.Equals(NOME_VARIAVEL) && actualItem.nivel == actualLevel)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void colocaTipoTabela(string lexema)
        {
            int stackSize = stack.getLength();

            for (int i = stackSize; i >= 0; i--)
            {
                Struct actualItem = stack.getPosition(i);

                if ((actualItem.nome.Equals(NOME_VARIAVEL) || actualItem.nome.Equals(NOME_FUNCAO)) && actualItem.tipo == null)
                {
                    actualItem.tipo = lexema;
                }
                else
                {
                    return;
                }
            }
        }

        public bool pesquisaDeclVarTabela(string lexema)
        {
            int stackSize = stack.getLength();

            for (int i = stackSize; i >= 0; i--)
            {
                Struct actualItem = stack.getPosition(i);

                if (actualItem.nome.Equals(NOME_VARIAVEL) && actualItem.lexema.Equals(lexema))
                {
                    return true;
                }
            }

            return false;
        }

        public bool pesquisaDeclVarFuncTabela(string lexema)
        {
            int stackSize = stack.getLength();

            for (int i = stackSize; i >= 0; i--)
            {
                Struct actualItem = stack.getPosition(i);

                if ((actualItem.nome.Equals(NOME_VARIAVEL) || actualItem.nome.Equals(NOME_FUNCAO)) && actualItem.lexema.Equals(lexema))
                {
                    return true;
                }
            }

            return false;
        }

        public bool pesquisaDeclProcTabela(string lexema)
        {
            int stackSize = stack.getLength();

            for (int i = stackSize; i >= 0; i--)
            {
                Struct actualItem = stack.getPosition(i);

                if (actualItem.lexema.Equals(lexema))
                {
                    return true;
                }
            }

            return false;
        }

        public bool pesquisaDeclFuncTabela(string lexema)
        {
            int stackSize = stack.getLength();

            for (int i = stackSize; i >= 0; i--)
            {
                Struct actualItem = stack.getPosition(i);

                if (actualItem.lexema.Equals(lexema))
                {
                    return true;
                }
            }

            return false;
        }

        public Struct pesquisaTabela(string lexema, int indice)
        {
            int stackSize = stack.getLength();

            for (int i = stackSize; i >= 0; i--)
            {
                Struct actualItem = stack.getPosition(i);

                if (actualItem.lexema.Equals(lexema) && (actualItem.nome.Equals(NOME_VARIAVEL) || actualItem.nome.Equals(NOME_FUNCAO) || actualItem.nome.Equals(NOME_PROCEDIMENTO)))
                {
                    return actualItem;
                }
            }

            return null;
        }

        public void voltaNivel()
        {
            int stackSize = stack.getLength();

            for (int i = stackSize; i >= 0; i--)
            {
                Struct actualItem = stack.getPosition(i);

                if (actualItem.nivel == actualLevel)
                {
                    stack.pop();
                }
                else
                {
                    decreaseLevel();
                    return;
                }
            }
        }

        public void addCharToExpression(Token caractere)
        {
            expression.Add(caractere.lexem);
        }

        public void cleanExpression()
        {
            expression.Clear();
            posFixExpression.Clear();
            finalPosFixExpression = "";
        }

        public List<string> getPosFixExpression()
        {
            return posFixExpression;
        }

        public string analyzeExpression()
        {
            convertExpressionToPosFix();
            string expResult = validateExpressionReturnType();

            foreach (string expressao in posFixExpression)
            {
                finalPosFixExpression += expressao;
            }

            return expResult;
        }

        private void convertExpressionToPosFix()
        {
            Stack<string> posFixStack = new Stack<string>();

            for (int i = 0; i < expression.Count; i++)
            {
                switch (expression[i])
                {
                    case "*":
                    case "div":
                    case "=":
                    case "!=":
                    case ">":
                    case ">=":
                    case "<":
                    case "<=":
                    case "e":
                    case "ou":
                    case "nao":
                    case "+":
                    case "-":
                        if (posFixStack.Count > 0)
                        {
                            if (((expression[i].Equals("+") || expression[i].Equals("-")) && isUnary(i)) || expression[i].Equals("nao"))
                            {
                                expression[i] += "u";
                            }

                            int myPriority = getPriority(expression[i]);
                            int topStackPriority = getPriority(posFixStack.Peek());

                            if (myPriority <= topStackPriority)
                            {
                                if (!expression[i].Equals("naou") && !posFixStack.Peek().Equals("naou"))
                                {
                                    try
                                    {
                                        do
                                        {
                                            posFixExpression.Add(posFixStack.Pop());
                                            topStackPriority = getPriority(posFixStack.Peek());
                                        } while (topStackPriority >= myPriority);
                                    }
                                    catch (InvalidOperationException)
                                    {

                                    }
                                }
                                
                                posFixStack.Push(expression[i]);
                            }
                            else
                            {
                                posFixStack.Push(expression[i]);
                            }
                        }
                        else
                        {
                            if (((expression[i].Equals("+") || expression[i].Equals("-")) && isUnary(i)) || expression[i].Equals("nao"))
                            {
                                expression[i] += "u";
                            }
                            
                            posFixStack.Push(expression[i]);

                        }
                        break;

                    case "(":
                        posFixStack.Push(expression[i]);
                        break;
                    case ")":
                        try
                        {
                            while (posFixStack.Peek() != "(")
                            {
                                posFixExpression.Add(posFixStack.Pop());
                            }

                            posFixStack.Pop();
                        }
                        catch (InvalidOperationException)
                        {

                        }
                        break;

                    default:
                        posFixExpression.Add(expression[i]);
                        break;
                }
            }

            while (posFixStack.Count > 0)
            {
                posFixExpression.Add(posFixStack.Pop());
            }
        }

        private int getPriority(string caractere)
        {
            switch (caractere)
            {
                case "ou":
                    return 1;

                case "e":
                    return 2;

                case "=":
                case "!=":
                    return 3;

                case ">":
                case ">=":
                case "<":
                case "<=":
                    return 4;

                case "+":
                case "-":
                    return 5;

                case "*":
                case "div":
                    return 6;

                case "-u":
                case "+u":
                case "naou":
                    return 7;

                default:
                    return 0;
            }
        }

        private bool isUnary(int position)
        {
            if (position == 0) return true;

            string pastChar = expression.ElementAt(position - 1);

            switch (pastChar)
            {
                case "*":
                case "div":
                case "=":
                case "!=":
                case ">":
                case ">=":
                case "<":
                case "<=":
                case "e":
                case "ou":
                case "nao":
                case "+":
                case "-":
                case "(":
                case ")":
                    return true;
                default:
                    return false;
            }
        }

        private string validateExpressionReturnType()
        {
            Stack<string> typesValidationStack = new Stack<string>();
            string tipoPos1;
            string tipoPos2;

            for (int i = 0; i < posFixExpression.Count; i++)
            {
                switch (posFixExpression[i])
                {
                    case "*":
                    case "div":
                    case "+":
                    case "-":
                    case ">":
                    case ">=":
                    case "<":
                    case "<=":
                        tipoPos1 = typesValidationStack.Pop();
                        tipoPos2 = typesValidationStack.Pop();

                        if (tipoPos1 == TIPO_INTEIRO && tipoPos2 == TIPO_INTEIRO)
                        {
                            if (posFixExpression[i].Equals("*") || posFixExpression[i].Equals("div") || posFixExpression[i].Equals("+") || posFixExpression[i].Equals("-"))
                            {
                                typesValidationStack.Push(TIPO_INTEIRO);
                            }
                            else
                            {
                                typesValidationStack.Push(TIPO_BOOLEANO);
                            }
                        }
                        else
                        {
                            throw new CompiladorException(ERRO_SEMANTICO, new CompiladorException(String.Format(ERROR_MESSAGE_NORMAL_OP, posFixExpression[i])));
                        }

                        break;

                    case "=":
                    case "!=":
                        tipoPos1 = typesValidationStack.Pop();
                        tipoPos2 = typesValidationStack.Pop();

                        if (tipoPos1 == TIPO_INTEIRO && tipoPos2 == TIPO_INTEIRO)
                        {
                            typesValidationStack.Push(TIPO_BOOLEANO);
                        }
                        else if(tipoPos1 == TIPO_BOOLEANO && tipoPos2 == TIPO_BOOLEANO)
                        {
                            typesValidationStack.Push(TIPO_BOOLEANO);
                        }
                        else
                        {
                            throw new CompiladorException(ERRO_SEMANTICO, new CompiladorException(ERROR_MESSAGE_EQUAL_DIF));
                        }
                        break;
                    case "e":
                    case "ou":
                        tipoPos1 = typesValidationStack.Pop();
                        tipoPos2 = typesValidationStack.Pop();

                        if (tipoPos1 == TIPO_BOOLEANO && tipoPos2 == TIPO_BOOLEANO)
                        {
                            typesValidationStack.Push(TIPO_BOOLEANO);
                        }
                        else
                        {
                            throw new CompiladorException(ERRO_SEMANTICO, new CompiladorException(ERROR_MESSAGE_E_OU));
                        }
                        break;

                    case "naou":
                        tipoPos1 = typesValidationStack.Pop();

                        if (tipoPos1 == TIPO_BOOLEANO)
                        {
                            typesValidationStack.Push(TIPO_BOOLEANO);
                        }
                        else
                        {
                            throw new CompiladorException(ERRO_SEMANTICO, new CompiladorException(ERROR_MESSAGE_BOOL_UNARY));
                        }
                        break;

                    case "+u":
                    case "-u":
                        tipoPos1 = typesValidationStack.Pop();

                        if (tipoPos1 == TIPO_INTEIRO)
                        {
                            typesValidationStack.Push(TIPO_INTEIRO);
                        }
                        else
                        {
                            throw new CompiladorException(ERRO_SEMANTICO, new CompiladorException(ERROR_MESSAGE_INT_UNARY));
                        }
                        break;

                    default:
                        if (int.TryParse(posFixExpression[i], out _))
                        {
                            typesValidationStack.Push(TIPO_INTEIRO);
                        }
                        else if (posFixExpression[i].Equals(TIPO_INTEIRO) || posFixExpression[i].Equals(TIPO_BOOLEANO))
                        {
                            typesValidationStack.Push(posFixExpression[i]);
                        }
                        else if (posFixExpression[i].Equals("verdadeiro") || posFixExpression[i].Equals("falso"))
                        {
                            typesValidationStack.Push(TIPO_BOOLEANO);
                        }
                        else
                        {
                            typesValidationStack.Push(getIdentifierType(posFixExpression[i]));
                        }
                        break;
                }
            }

            return typesValidationStack.Pop();
        }

        private string getIdentifierType(string identificador)
        {
            int stackSize = stack.getLength();

            for (int i = stackSize; i >= 0; i--)
            {
                Struct actualItem = stack.getPosition(i);

                if (actualItem.lexema.Equals(identificador) && (actualItem.nome.Equals(NOME_VARIAVEL) || actualItem.nome.Equals(NOME_FUNCAO)))
                {
                    return actualItem.tipo;
                }
            }

            throw new CompiladorException(ERRO_SEMANTICO, new CompiladorException(ERROR_MESSAGE_IDENTIFIER_NOT_FOUND));
        }
    }
}
