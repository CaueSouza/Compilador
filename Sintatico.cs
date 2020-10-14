using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Compilador.Constantes;

namespace Compilador
{
    class Sintatico
    {
        private Token actualToken;
        private List<Token> tokenList;
        private int tokenCount;
        public int errorLine = 1;
        public string errorMessage;
        private bool hasEndedTokens = false;
        private int oldTokenLine;

        private void resetValidators()
        {
            tokenCount = 0;
            hasEndedTokens = false;
            errorLine = 1;
            oldTokenLine = 0;
            errorMessage = "";
            actualToken = null;
            tokenList = null;
        }

        public void executeSintatico(List<Token> tokens)
        {
            resetValidators();
            tokenList = tokens;

            if (tokenList.Count() > 0)
            {
                updateToken();

                if (!hasEndedTokens && isSimbol(PROGRAMA))
                {
                    updateToken();

                    if (!hasEndedTokens && isSimbol(IDENTIFICADOR))
                    {
                        updateToken();

                        if (!hasEndedTokens && isSimbol(PONTO_VIRGULA))
                        {
                            analisaBloco();

                            if (isSimbol(PONTO))
                            {
                                updateToken();

                                if (!hasEndedTokens)
                                {
                                    throwError(ERROR_ENDED_TOKENS);
                                }
                            }
                            else
                            {
                                throwError(ERROR_MISSING_PONTO);
                            }
                        }
                        else
                        {
                            throwError(ERROR_MISSING_PONTO_VIRGULA);
                        }
                    }
                    else
                    {
                        throwError(ERROR_MISSING_IDENTIFICADOR);
                    }
                }
                else
                {
                    throwError(ERROR_MISSING_PROGRAMA);
                }
            }
        }

        private void throwError(string errorMessage)
        {
            if (oldTokenLine < actualToken.getLine() && oldTokenLine > 0)
            {
                errorLine = oldTokenLine;
            }
            else
            {
                errorLine = actualToken.getLine();
            }
            
            this.errorMessage = errorMessage;
            throw new Exception(ERRO_SINTATICO);
        }


        private bool isSimbol(string Simbol)
        {
            return actualToken.getSimbol().Equals(Simbol);
        }

        private void updateToken()
        {
            actualToken = getActualToken();
        }

        private Token getActualToken()
        {
            Token token;

            if (tokenCount == tokenList.Count())
            {
                hasEndedTokens = true;
                token = actualToken;
            } 
            else
            {
                
                oldTokenLine = actualToken == null ? 0 : actualToken.getLine();
                token = tokenList[tokenCount];
                tokenCount++;
            }

            return token;
        }

        private void analisaBloco()
        {
            updateToken();

            analisaEtVariaveis();
            analisaSubRotinas();
            analisaComandos();
        }

        private void analisaEtVariaveis()
        {
            if (!hasEndedTokens && isSimbol(VAR))
            {
                updateToken();

                if (!hasEndedTokens && isSimbol(IDENTIFICADOR))
                {
                    while (isSimbol(IDENTIFICADOR))
                    {
                        analisaVariaveis();

                        if (isSimbol(PONTO_VIRGULA))
                        {
                            updateToken();
                        }
                        else
                        {
                            throwError(ERROR_MISSING_PONTO_VIRGULA);
                        }
                    }
                }
                else
                {
                    throwError(ERROR_MISSING_IDENTIFICADOR);
                }
            }
        }

        private void analisaVariaveis()
        {
            do
            {
                if (!hasEndedTokens && isSimbol(IDENTIFICADOR))
                {
                    updateToken();

                    if (!hasEndedTokens && (isSimbol(VIRGULA) || isSimbol(DOIS_PONTOS)))
                    {
                        if (!hasEndedTokens && isSimbol(VIRGULA))
                        {
                            updateToken();

                            if (!hasEndedTokens && isSimbol(DOIS_PONTOS))
                            {
                                throwError(ERROR_MISSING_DOIS_PONTOS);
                            }
                        }
                    }
                    else
                    {
                        throwError(ERROR_MISSING_VIRGULA_DOIS_PONTOS);
                    }
                }
                else
                {
                    throwError(ERROR_MISSING_IDENTIFICADOR);
                }
            } while (!hasEndedTokens && !isSimbol(DOIS_PONTOS));

            updateToken();

            analisaTipo();
        }

        private void analisaTipo()
        {
            if (!hasEndedTokens && !isSimbol(INTEIRO) && !isSimbol(BOOLEANO))
            {
                throwError(ERROR_MISSING_TIPO);
            }
            else
            {
                updateToken();
            }
        }

        private void analisaComandos()
        {
            if (!hasEndedTokens && isSimbol(INICIO))
            {
                updateToken();

                analisaComandoSimples();

                while (!hasEndedTokens && !isSimbol(FIM))
                {
                    if (!hasEndedTokens && isSimbol(PONTO_VIRGULA))
                    {
                        updateToken();

                        if (!hasEndedTokens && !isSimbol(FIM))
                        {
                            analisaComandoSimples();
                        }
                    }
                    else
                    {
                        throwError(ERROR_MISSING_PONTO_VIRGULA);
                    }
                }

                updateToken();
            }
            else
            {
                throwError(ERROR_MISSING_INICIO);
            }
        }

        private void analisaComandoSimples()
        {
            if (!hasEndedTokens)
            {
                switch (actualToken.getSimbol())
                {
                    case IDENTIFICADOR:
                        analisaAtribChamadaProc();
                        break;
                    case SE:
                        analisaSe();
                        break;
                    case ENQUANTO:
                        analisaEnquanto();
                        break;
                    case LEIA:
                        analisaLeia();
                        break;
                    case ESCREVA:
                        analisaEscreva();
                        break;
                    default:
                        analisaComandos();
                        break;
                }
            }
        }

        private void analisaEscreva()
        {
            updateToken();

            if (!hasEndedTokens && isSimbol(ABRE_PARENTESES))
            {
                updateToken();

                if (!hasEndedTokens && isSimbol(IDENTIFICADOR))
                {
                    updateToken();

                    if (!hasEndedTokens && isSimbol(FECHA_PARENTESES))
                    {
                        updateToken();
                    }
                    else
                    {
                        throwError(ERROR_MISSING_FECHA_PARENTESES);
                    }
                }
                else
                {
                    throwError(ERROR_MISSING_IDENTIFICADOR);
                }
            }
            else
            {
                throwError(ERROR_MISSING_ABRE_PARENTESES);
            }
        }

        private void analisaLeia()
        {
            updateToken();

            if (!hasEndedTokens && isSimbol(ABRE_PARENTESES))
            {
                updateToken();

                if (!hasEndedTokens && isSimbol(IDENTIFICADOR))
                {
                    updateToken();

                    if (!hasEndedTokens && isSimbol(FECHA_PARENTESES))
                    {
                        updateToken();
                    }
                    else
                    {
                        throwError(ERROR_MISSING_FECHA_PARENTESES);
                    }
                }
                else
                {
                    throwError(ERROR_MISSING_IDENTIFICADOR);
                }
            }
            else
            {
                throwError(ERROR_MISSING_ABRE_PARENTESES);
            }
        }

        private void analisaEnquanto()
        {
            updateToken();

            analisaExpressao();

            if (!hasEndedTokens && isSimbol(FACA))
            {
                updateToken();

                analisaComandoSimples();
            }
            else
            {
                throwError(ERROR_MISSING_FACA);
            }
        }

        private void analisaSe()
        {
            updateToken();

            analisaExpressao();

            if (!hasEndedTokens && isSimbol(ENTAO))
            {
                updateToken();

                analisaComandoSimples();

                if (!hasEndedTokens && isSimbol(SENAO))
                {
                    updateToken();

                    analisaComandoSimples();
                }
            }
            else
            {
                throwError(ERROR_MISSING_ENTAO);
            }
        }

        private void analisaAtribChamadaProc()
        {
            updateToken();

            if (!hasEndedTokens && isSimbol(ATRIBUICAO))
            {
                analisaAtribuicao();
            }
            else
            {
                analisaChamadaProcedimento();
            }
        }

        private void analisaSubRotinas()
        {
            int flag = 0;

            if (!hasEndedTokens && (isSimbol(PROCEDIMENTO) || isSimbol(FUNCAO)))
            {
                //cod semantico
            }

            while (!hasEndedTokens && (isSimbol(PROCEDIMENTO) || isSimbol(FUNCAO)))
            {
                if (!hasEndedTokens && isSimbol(PROCEDIMENTO))
                {
                    analisaDeclaracaoProcedimento();
                }
                else
                {
                    analisaDeclaracaoFuncao();
                }

                if (!hasEndedTokens && isSimbol(PONTO_VIRGULA))
                {
                    updateToken();
                }
                else
                {
                    throwError(ERROR_MISSING_PONTO_VIRGULA);
                }
            }

            if (flag == 1)
            {
                //cod semantico
            }
        }

        private void analisaDeclaracaoProcedimento()
        {
            updateToken();

            if (!hasEndedTokens && isSimbol(IDENTIFICADOR))
            {
                updateToken();

                if (!hasEndedTokens && isSimbol(PONTO_VIRGULA))
                {
                    analisaBloco();
                }
                else
                {
                    throwError(ERROR_MISSING_PONTO_VIRGULA);
                }
            }
            else
            {
                throwError(ERROR_MISSING_IDENTIFICADOR);
            }
        }

        private void analisaDeclaracaoFuncao()
        {
            updateToken();

            if (!hasEndedTokens && isSimbol(IDENTIFICADOR))
            {
                updateToken();

                if (!hasEndedTokens && isSimbol(DOIS_PONTOS))
                {
                    updateToken();

                    if (!hasEndedTokens && (isSimbol(INTEIRO) || isSimbol(BOOLEANO)))
                    {
                        updateToken();

                        if (!hasEndedTokens && isSimbol(PONTO_VIRGULA))
                        {
                            analisaBloco();
                        }
                    }
                    else
                    {
                        throwError(ERROR_MISSING_TIPO);
                    }
                }
                else
                {
                    throwError(ERROR_MISSING_DOIS_PONTOS);
                }
            }
            else
            {
                throwError(ERROR_MISSING_IDENTIFICADOR);
            }
        }

        private void analisaExpressao()
        {
            analisaExpressaoSimples();

            if (!hasEndedTokens &&
                (isSimbol(MAIOR) || isSimbol(MAIORIG) || isSimbol(IGUAL) || isSimbol(MENOR) || isSimbol(MENORIG) || isSimbol(DIF)))
            {
                updateToken();

                analisaExpressaoSimples();
            }
        }

        private void analisaExpressaoSimples()
        {
            if (!hasEndedTokens && (isSimbol(MAIS) || isSimbol(MENOS)))
            {
                updateToken();
            }

            analisaTermo();

            while (!hasEndedTokens && (isSimbol(MAIS) || isSimbol(MENOS) || isSimbol(OU)))
            {
                updateToken();

                analisaTermo();
            }
        }

        private void analisaTermo()
        {
            analisaFator();

            if (!hasEndedTokens && (isSimbol(MULT) || isSimbol(DIV) || isSimbol(E)))
            {
                updateToken();

                analisaFator();
            }
        }

        private void analisaFator()
        {
            if (!hasEndedTokens && isSimbol(IDENTIFICADOR))
            {
                analisaChamadaFuncao();
            }
            else if (!hasEndedTokens && isSimbol(NUMERO))
            {
                updateToken();
            }
            else if (!hasEndedTokens && isSimbol(NAO))
            {
                updateToken();

                analisaFator();
            }
            else if (!hasEndedTokens && isSimbol(ABRE_PARENTESES))
            {
                updateToken();

                analisaExpressao();

                if (!hasEndedTokens && isSimbol(FECHA_PARENTESES))
                {
                    updateToken();
                }
                else
                {
                    throwError(ERROR_MISSING_FECHA_PARENTESES);
                }
            }
            else if (!hasEndedTokens && (isSimbol(VERDADEIRO) || isSimbol(FALSO)))
            {
                updateToken();
            }
            else
            {
                throwError(ERROR_MISSING_FATOR);
            }
        }

        private void analisaChamadaProcedimento()
        {
            //nao faz nada
        }

        private void analisaChamadaFuncao()
        {
            updateToken();
        }

        private void analisaAtribuicao()
        {
            updateToken();

            analisaExpressao();
        }
    }
}
