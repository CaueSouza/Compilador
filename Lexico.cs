﻿using System;
using System.Collections.Generic;
using static Compilador.Constantes;

namespace Compilador
{
    class Lexico
    {
        private char actualChar = ' ';
        private int count = -1;
        private int lineCount;
        private string fullString = "";
        private List<Token> tokenList = new List<Token>();
        private bool notEOF = true;

        private void resetValidators()
        {
            actualChar = ' ';
            count = -1;
            lineCount = 1;
            fullString = "";
            notEOF = true;
            tokenList = new List<Token>();
        }

        public void executeLexico(String fullString)
        {
            resetValidators();
            this.fullString = fullString;

            readCaracter();

            while (notEOF)
            {
                while ((actualChar == '{' || actualChar == ' ' || actualChar == '/') && notEOF)
                {
                    if (actualChar == '{')
                    {
                        int lineCommentStarted = lineCount;

                        while (actualChar != '}' && notEOF)
                        {
                            readCaracter();
                        }

                        if (!notEOF && actualChar != '}') createErrorToken(lineCommentStarted, COMENTARIO_ERROR);

                        readCaracter();
                    }


                    if (actualChar == '/' && notEOF)
                    {
                        readCaracter();
                        if (actualChar == '*' && notEOF)
                        {
                            int lineCommentStarted = lineCount;

                            readCaracter();
                            bool endedComment = true;

                            while (endedComment && notEOF)
                            {
                                while (actualChar != '*' && notEOF)
                                {
                                    readCaracter();
                                }

                                if (notEOF)
                                {
                                    readCaracter();

                                    if (notEOF)
                                    {
                                        if (actualChar == '/')
                                        {
                                            readCaracter();
                                            endedComment = false;
                                        }
                                        else
                                        {
                                            readCaracter();
                                        }
                                    }
                                    else createErrorToken(lineCommentStarted, COMENTARIO_ERROR);
                                }
                                else createErrorToken(lineCommentStarted, COMENTARIO_ERROR);
                            }
                        }
                        else
                        {
                            createErrorToken('/'.ToString(), lineCount, CARACTER_ERROR);
                        }
                    }


                    while (actualChar == ' ' && notEOF)
                    {
                        readCaracter();
                    }
                }

                if (notEOF)
                {
                    Token token = readToken();
                    tokenList.Add(token);
                }
            }
        }

        private void readCaracter()
        {
            if (count == fullString.Length - 1)
            {
                notEOF = false;
            }
            else
            {
                count++;
                actualChar = fullString[count];

                if (actualChar == '\n')
                {
                    lineCount++;
                    if (count != fullString.Length - 1)
                    {
                        count++;
                        actualChar = fullString[count];
                    }
                    else
                    {
                        notEOF = false;
                    }
                }
            }
        }

        private void createErrorToken(int errorLine, int errorType)
        {
            notEOF = false;
            tokenList.Add(new Token(actualChar.ToString(), errorLine, errorType));
        }

        private void createErrorToken(string lexem, int errorLine, int errorType)
        {
            notEOF = false;
            tokenList.Add(new Token(lexem, errorLine, errorType));
        }

        private bool isDigit()
        {
            return Char.IsDigit(actualChar);
        }
        
        private bool isLetter()
        {
            return Char.IsLetter(actualChar);
        }

        private bool isAssignment()
        {
            return actualChar == ':';
        }

        private bool isArithmetic()
        {
            return actualChar == '+' || actualChar == '-' || actualChar == '*';
        }

        private bool isRelational()
        {
            return actualChar == '<' || actualChar == '>' || actualChar == '=' || actualChar == '!';
        }

        private bool isPunctuation()
        {
            return actualChar == ';' || actualChar == ',' || actualChar == '(' || actualChar == ')' || actualChar == '.';
        }

        private Token readToken()
        {
            if (isDigit())
            {
                return treatDigit();
            }
            else if (isLetter())
            {
                return treatIdentifierAndReservedWord();
            }
            else if (isAssignment())
            {
                return treatAssignment();
            }
            else if (isArithmetic())
            {
                return treatArithmetic();
            }
            else if (isRelational())
            {
                return treatRelational();
            }
            else if (isPunctuation())
            {
                return treatPunctuation();
            }
            else
            {
                if (actualChar.Equals('_'))
                {
                    notEOF = false;
                    return new Token(actualChar.ToString(), lineCount, IDENTIFICADOR_COM_UNDERLINE);
                }
                else
                {
                    return new Token(actualChar.ToString(), lineCount, CARACTER_ERROR);
                }
                
            }
        }

        private Token treatDigit()
        {
            string num = actualChar.ToString();
            readCaracter();

            while (isDigit() && notEOF)
            {
                num += actualChar.ToString();
                readCaracter();
            }

            return new Token(Constantes.NUMERO, num, lineCount);
        }

        private Token treatIdentifierAndReservedWord()
        {
            string id = actualChar.ToString();
            readCaracter();

            while ((isLetter() || isDigit() || actualChar.Equals('_')) && notEOF)
            {
                id += actualChar.ToString();
                readCaracter();
            }

            string simbol;

            switch (id)
            {
                case "programa":
                    simbol = PROGRAMA;
                    break;
                case "se":
                    simbol = SE;
                    break;
                case "entao":
                    simbol = ENTAO;
                    break;
                case "senao":
                    simbol = SENAO;
                    break;
                case "enquanto":
                    simbol = ENQUANTO;
                    break;
                case "faca":
                    simbol = FACA;
                    break;
                case "inicio":
                    simbol = INICIO;
                    break;
                case "fim":
                    simbol = FIM;
                    break;
                case "escreva":
                    simbol = ESCREVA;
                    break;
                case "leia":
                    simbol = LEIA;
                    break;
                case "var":
                    simbol = VAR;
                    break;
                case "inteiro":
                    simbol = INTEIRO;
                    break;
                case "booleano":
                    simbol = BOOLEANO;
                    break;
                case "verdadeiro":
                    simbol = VERDADEIRO;
                    break;
                case "falso":
                    simbol = FALSO;
                    break;
                case "procedimento":
                    simbol = PROCEDIMENTO;
                    break;
                case "funcao":
                    simbol = FUNCAO;
                    break;
                case "div":
                    simbol = DIV;
                    break;
                case "e":
                    simbol = E;
                    break;
                case "ou":
                    simbol = OU;
                    break;
                case "nao":
                    simbol = NAO;
                    break;
                default:
                    simbol = IDENTIFICADOR;
                    break;
            }

            return new Token(simbol, id, lineCount);
        }

        private Token treatAssignment()
        {
            string assignment = actualChar.ToString();
            readCaracter();

            if (actualChar.ToString().Equals("="))
            {
                string caracter = actualChar.ToString();
                readCaracter();
                return new Token(ATRIBUICAO, assignment + caracter, lineCount);
            }
            else
            {
                return new Token(DOIS_PONTOS, assignment, lineCount);
            }
        }

        private Token treatArithmetic()
        {
            string aritmetico = actualChar.ToString();
            readCaracter();

            switch (aritmetico)
            {
                case "+":
                    return new Token(MAIS, aritmetico, lineCount);
                case "-":
                    return new Token(MENOS, aritmetico, lineCount);
                case "*":
                    return new Token(MULTI, aritmetico, lineCount);
                default:
                    return new Token(actualChar.ToString(), lineCount, 3);
            }
        }

        private Token treatRelational()
        {
            string relacional = actualChar.ToString();
            readCaracter();
            string caracter = actualChar.ToString();

            switch (relacional)
            {
                case "<":
                    if (caracter.Equals("="))
                    {
                        readCaracter();
                        return new Token(MENORIG, relacional + caracter, lineCount);
                    }
                    else return new Token(MENOR, relacional, lineCount);

                case ">":
                    if (caracter.Equals("="))
                    {
                        readCaracter();
                        return new Token(MAIORIG, relacional + caracter, lineCount);
                    }
                    else return new Token(MAIOR, relacional, lineCount);


                case "!":
                    if (caracter.Equals("="))
                    {
                        readCaracter();
                        return new Token(DIF, relacional + caracter, lineCount);
                    }
                    else return new Token(relacional.ToString(), lineCount, CARACTER_ERROR);

                case "=":
                    return new Token(IGUAL, relacional, lineCount);

                default:
                    return new Token(relacional.ToString(), lineCount, CARACTER_ERROR);
            }
        }

        private Token treatPunctuation()
        {
            string punctuation = actualChar.ToString();
            string simbolo = "";

            switch (punctuation)
            {
                case ";":
                    simbolo = PONTO_VIRGULA;
                    break;
                case ",":
                    simbolo = VIRGULA;
                    break;
                case ".":
                    simbolo = PONTO;
                    break;
                case "(":
                    simbolo = ABRE_PARENTESES;
                    break;
                case ")":
                    simbolo = FECHA_PARENTESES;
                    break;
            }

            readCaracter();
            return new Token(simbolo, punctuation, lineCount);
        }

        public List<Token> getTokens()
        {
            return tokenList;
        }
    }
}
