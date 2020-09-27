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
        private int tokenCount = 0;

        public void executeSintatico(List<Token> tokens)
        {
            tokenList = tokens;

            updateToken();

            if (!actualToken.getIsError() && isSimbol(PROGRAMA))
            {
                updateToken();

                if (!actualToken.getIsError() && isSimbol(IDENTIFICADOR))
                {
                    updateToken();

                    if (!actualToken.getIsError() && isSimbol(PONTO_VIRGULA))
                    {
                        analisaBloco();

                        if (!actualToken.getIsError() && isSimbol(PONTO))
                        {
                            if (!hasEndedTokens())
                            {
                                //erro
                            }
                        }
                        else
                        {
                            //erro
                        }
                    }
                    else
                    {
                        //erro
                    }
                }
                else
                {
                    //erro
                }
            }
            else
            {
                //erro
            }
        }

        private bool hasEndedTokens()
        {
            return tokenCount == tokenList.Count();
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
            Token token = tokenList[tokenCount];
            tokenCount++;

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
            if (!actualToken.getIsError() && isSimbol(VAR))
            {
                updateToken();

                if (!actualToken.getIsError() && isSimbol(IDENTIFICADOR))
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
                            //erro
                        }
                    }
                }
                else
                {
                    //erro
                }
            }
        }

        private void analisaVariaveis()
        {
            do
            {
                if (!actualToken.getIsError() && isSimbol(IDENTIFICADOR))
                {
                    updateToken();

                    if (!actualToken.getIsError() && (isSimbol(VIRGULA) || isSimbol(DOIS_PONTOS)))
                    {
                        if (!actualToken.getIsError() && isSimbol(VIRGULA))
                        {
                            updateToken();

                            if (!actualToken.getIsError() && isSimbol(DOIS_PONTOS))
                            {
                                //erro
                            }
                        }
                    }
                    else
                    {
                        //erro
                    }
                }
                else
                {
                    //erro
                }
            } while (!actualToken.getIsError() && !isSimbol(DOIS_PONTOS));

            updateToken();

            analisaTipo();
        }

        private void analisaTipo()
        {
            if (!actualToken.getIsError() && !isSimbol(INTEIRO) && !isSimbol(BOOLEANO))
            {
                //erro
            }
            else
            {
                updateToken();
            }
        }

        private void analisaComandos()
        {
            if (!actualToken.getIsError() && isSimbol(INICIO))
            {
                updateToken();

                analisaComandoSimples();

                while (!actualToken.getIsError() && !isSimbol(FIM))
                {
                    if (!actualToken.getIsError() && isSimbol(PONTO_VIRGULA))
                    {
                        updateToken();

                        if (!actualToken.getIsError() && !isSimbol(FIM))
                        {
                            analisaComandoSimples();
                        }
                    }
                    else
                    {
                        //erro
                    }
                }

                updateToken();
            }
            else
            {
                //erro
            }
        }

        private void analisaComandoSimples()
        {
            if (!actualToken.getIsError())
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

            if (!actualToken.getIsError() && isSimbol(ABRE_PARENTESES))
            {
                updateToken();

                if (!actualToken.getIsError() && isSimbol(IDENTIFICADOR))
                {
                    updateToken();

                    if (!actualToken.getIsError() && isSimbol(FECHA_PARENTESES))
                    {
                        updateToken();
                    }
                    else
                    {
                        //erro
                    }
                }
                else
                {
                    //erro
                }
            }
            else
            {
                //erro
            }
        }

        private void analisaLeia()
        {
            updateToken();

            if (!actualToken.getIsError() && isSimbol(ABRE_PARENTESES))
            {
                updateToken();

                if (!actualToken.getIsError() && isSimbol(IDENTIFICADOR))
                {
                    updateToken();

                    if (!actualToken.getIsError() && isSimbol(FECHA_PARENTESES))
                    {
                        updateToken();
                    }
                    else
                    {
                        //erro
                    }
                }
                else
                {
                    //erro
                }
            }
            else
            {
                //erro
            }
        }

        private void analisaEnquanto()
        {
            updateToken();

            analisaExpressao();

            if (!actualToken.getIsError() && isSimbol(FACA))
            {
                updateToken();

                analisaComandoSimples();
            }
            else
            {
                //erro
            }
        }

        private void analisaSe()
        {
            updateToken();

            analisaExpressao();

            if (!actualToken.getIsError() && isSimbol(ENTAO))
            {
                updateToken();

                analisaComandoSimples();

                if (!actualToken.getIsError() && isSimbol(SENAO))
                {
                    updateToken();

                    analisaComandoSimples();
                }
            }
            else
            {
                //erro
            }
        }

        private void analisaAtribChamadaProc()
        {
            updateToken();

            if (!actualToken.getIsError() && isSimbol(ATRIBUICAO))
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

            if (!actualToken.getIsError() && (isSimbol(PROCEDIMENTO) || isSimbol(FUNCAO)))
            {
                //cod semantico
            }

            while (!actualToken.getIsError() && (isSimbol(PROCEDIMENTO) || isSimbol(FUNCAO)))
            {
                if (!actualToken.getIsError() && isSimbol(PROCEDIMENTO))
                {
                    analisaDeclaracaoProcedimento();
                }
                else
                {
                    analisaDeclaracaoFuncao();
                }

                if (!actualToken.getIsError() && isSimbol(PONTO_VIRGULA))
                {
                    updateToken();
                }
                else
                {
                    //erro
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

            if (!actualToken.getIsError() && isSimbol(IDENTIFICADOR))
            {
                updateToken();

                if (!actualToken.getIsError() && isSimbol(PONTO_VIRGULA))
                {
                    analisaBloco();
                }
                else
                {
                    //erro
                }
            }
            else
            {
                //erro
            }
        }

        private void analisaDeclaracaoFuncao()
        {
            updateToken();

            if (!actualToken.getIsError() && isSimbol(IDENTIFICADOR))
            {
                updateToken();

                if (!actualToken.getIsError() && isSimbol(DOIS_PONTOS))
                {
                    updateToken();

                    if (!actualToken.getIsError() && (isSimbol(INTEIRO) || isSimbol(BOOLEANO)))
                    {
                        updateToken();

                        if (!actualToken.getIsError() && isSimbol(PONTO_VIRGULA))
                        {
                            analisaBloco();
                        }
                    }
                    else
                    {
                        //erro
                    }
                }
                else
                {
                    //erro
                }
            }
            else
            {
                //erro
            }
        }

        private void analisaExpressao()
        {
            analisaExpressaoSimples();

            if (!actualToken.getIsError() &&
                (isSimbol(MAIOR) || isSimbol(MAIORIG) || isSimbol(IGUAL) || isSimbol(MENOR) || isSimbol(MENORIG) || isSimbol(DIF)))
            {
                updateToken();

                analisaExpressaoSimples();
            }
        }

        private void analisaExpressaoSimples()
        {
            if (!actualToken.getIsError() && (isSimbol(MAIS) || isSimbol(MENOS)))
            {
                updateToken();
            }

            analisaTermo();

            while (!actualToken.getIsError() && (isSimbol(MAIS) || isSimbol(MENOS) || isSimbol(OU)))
            {
                updateToken();

                analisaTermo();
            }
        }

        private void analisaTermo()
        {
            analisaFator();

            if (!actualToken.getIsError() && (isSimbol(MULT) || isSimbol(DIV) || isSimbol(E)))
            {
                updateToken();

                analisaFator();
            }
        }

        private void analisaFator()
        {
            if (!actualToken.getIsError() && isSimbol(IDENTIFICADOR))
            {
                analisaChamadaFuncao();
            }
            else if (!actualToken.getIsError() && isSimbol(NUMERO))
            {
                updateToken();
            }
            else if (!actualToken.getIsError() && isSimbol(NAO))
            {
                updateToken();

                analisaFator();
            }
            else if (!actualToken.getIsError() && isSimbol(ABRE_PARENTESES))
            {
                updateToken();

                analisaExpressao();

                if (!actualToken.getIsError() && isSimbol(FECHA_PARENTESES))
                {
                    updateToken();
                }
                else
                {
                    //erro
                }
            }
            else if (!actualToken.getIsError() && (isSimbol(VERDADEIRO) || isSimbol(FALSO)))
            {
                updateToken();
            }
            else
            {
                //erro
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
