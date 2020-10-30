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
        public Token errorToken;
        private bool hasEndedTokens = false;
        private Semantico semantico;

        private void resetValidators()
        {
            tokenCount = 0;
            hasEndedTokens = false;
            errorToken = null;
            actualToken = null;
            tokenList = null;
            semantico.resetStack();
        }

        public void executeSintatico(List<Token> tokens, Semantico semantico)
        {
            this.semantico = semantico;
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
                        semantico.insereTabela(actualToken.lexem, NOME_PROGRAMA, 0);
                        updateToken();

                        if (!hasEndedTokens && isSimbol(PONTO_VIRGULA))
                        {
                            analisaBloco();

                            if (isSimbol(PONTO))
                            {
                                updateToken();

                                if (!hasEndedTokens)
                                {
                                    throwError(new Exception(ERRO_SINTATICO));
                                }
                            }
                            else
                            {
                                throwError(new Exception(ERRO_SINTATICO));
                            }
                        }
                        else
                        {
                            throwError(new Exception(ERRO_SINTATICO));
                        }
                    }
                    else
                    {
                        throwError(new Exception(ERRO_SINTATICO));
                    }
                }
                else
                {
                    throwError(new Exception(ERRO_SINTATICO));
                }
            }
        }

        private void throwError(Exception exception)
        {
            errorToken = actualToken;
            throw exception;
        }

        private void throwError(Exception exception, int errorType)
        {
            errorToken = new Token(actualToken.lexem, actualToken.line, errorType);
            throw exception;
        }

        private bool isSimbol(string Simbol)
        {
            return actualToken.simbol.Equals(Simbol);
        }

        private void updateToken()
        {
            actualToken = getActualToken();

            if (actualToken.isError)
            {
                throwError(new Exception(ERRO_LEXICO));
            }
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
                            throwError(new Exception(ERRO_SINTATICO));
                        }
                    }
                }
                else
                {
                    throwError(new Exception(ERRO_SINTATICO));
                }
            }
        }

        private void analisaVariaveis()
        {
            do
            {
                if (!hasEndedTokens && isSimbol(IDENTIFICADOR))
                {
                    if (!semantico.pesquisaDuplicVarTabela(actualToken.lexem))
                    {
                        semantico.insereTabela(actualToken.lexem, NOME_VARIAVEL, 0);

                        updateToken();

                        if (!hasEndedTokens && (isSimbol(VIRGULA) || isSimbol(DOIS_PONTOS)))
                        {
                            if (!hasEndedTokens && isSimbol(VIRGULA))
                            {
                                updateToken();

                                if (!hasEndedTokens && isSimbol(DOIS_PONTOS))
                                {
                                    throwError(new Exception(ERRO_SINTATICO));
                                }
                            }
                        }
                        else
                        {
                            throwError(new Exception(ERRO_SINTATICO));
                        }
                    }
                    else
                    {
                        throwError(new Exception(ERRO_SEMANTICO), DUPLIC_VAR_ERROR);
                    }
                }
                else
                {
                    throwError(new Exception(ERRO_SINTATICO));
                }
            } while (!hasEndedTokens && !isSimbol(DOIS_PONTOS));

            updateToken();

            analisaTipo();
        }

        private void analisaTipo()
        {
            if (!hasEndedTokens && !isSimbol(INTEIRO) && !isSimbol(BOOLEANO))
            {
                throwError(new Exception(ERRO_SINTATICO));
            }
            else
            {
                semantico.colocaTipoTabela(actualToken.lexem);
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
                        throwError(new Exception(ERRO_SINTATICO));
                    }
                }

                updateToken();
            }
            else
            {
                throwError(new Exception(ERRO_SINTATICO));
            }
        }

        private void analisaComandoSimples()
        {
            if (!hasEndedTokens)
            {
                switch (actualToken.simbol)
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
                    if (semantico.pesquisaDeclVarTabela(actualToken.lexem))
                    {
                        updateToken();

                        if (!hasEndedTokens && isSimbol(FECHA_PARENTESES))
                        {
                            updateToken();
                        }
                        else
                        {
                            throwError(new Exception(ERRO_SINTATICO));
                        }
                    }
                    else
                    {
                        throwError(new Exception(ERRO_SEMANTICO), DECL_VAR_ERROR);
                    }
                }
                else
                {
                    throwError(new Exception(ERRO_SINTATICO));
                }
            }
            else
            {
                throwError(new Exception(ERRO_SINTATICO));
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
                    if (semantico.pesquisaDeclVarTabela(actualToken.lexem))
                    {
                        updateToken();

                        if (!hasEndedTokens && isSimbol(FECHA_PARENTESES))
                        {
                            updateToken();
                        }
                        else
                        {
                            throwError(new Exception(ERRO_SINTATICO));
                        }
                    }
                    else
                    {
                        throwError(new Exception(ERRO_SEMANTICO), DECL_VAR_ERROR);
                    }
                }
                else
                {
                    throwError(new Exception(ERRO_SINTATICO));
                }
            }
            else
            {
                throwError(new Exception(ERRO_SINTATICO));
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
                throwError(new Exception(ERRO_SINTATICO));
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
                throwError(new Exception(ERRO_SINTATICO));
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
                    throwError(new Exception(ERRO_SINTATICO));
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
                if (!semantico.pesquisaDeclProcTabela(actualToken.lexem))
                {
                    semantico.insereTabela(actualToken.lexem, NOME_PROCEDIMENTO, 0);
                    semantico.increaseLevel();
                    updateToken();

                    if (!hasEndedTokens && isSimbol(PONTO_VIRGULA))
                    {
                        analisaBloco();
                    }
                    else
                    {
                        throwError(new Exception(ERRO_SINTATICO));
                    }
                }
                else
                {
                    throwError(new Exception(ERRO_SEMANTICO), DECL_PROC_ERROR);
                }
            }
            else
            {
                throwError(new Exception(ERRO_SINTATICO));
            }

            semantico.voltaNivel();
        }

        private void analisaDeclaracaoFuncao()
        {
            updateToken();

            if (!hasEndedTokens && isSimbol(IDENTIFICADOR))
            {
                if (!semantico.pesquisaDeclFuncTabela(actualToken.lexem))
                {
                    semantico.insereTabela(actualToken.lexem, NOME_FUNCAO, 0);
                    semantico.increaseLevel();
                    updateToken();

                    if (!hasEndedTokens && isSimbol(DOIS_PONTOS))
                    {
                        updateToken();

                        if (!hasEndedTokens && (isSimbol(INTEIRO) || isSimbol(BOOLEANO)))
                        {
                            semantico.colocaTipoTabela(isSimbol(INTEIRO) ? TIPO_INTEIRO : TIPO_BOOLEANO);
                            updateToken();

                            if (!hasEndedTokens && isSimbol(PONTO_VIRGULA))
                            {
                                analisaBloco();
                            }
                        }
                        else
                        {
                            throwError(new Exception(ERRO_SINTATICO));
                        }
                    }
                    else
                    {
                        throwError(new Exception(ERRO_SINTATICO));
                    }
                } 
                else
                {
                    throwError(new Exception(ERRO_SEMANTICO), DECL_FUNC_ERROR);
                }
            }
            else
            {
                throwError(new Exception(ERRO_SINTATICO));
            }

            semantico.voltaNivel();
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
                Struct actualItem = semantico.pesquisaTabela(actualToken.lexem, 0);

                if (actualItem != null)
                {
                    if (actualItem.tipo == "inteiro" || actualItem.tipo == "booleano")
                    {
                        analisaChamadaFuncao();
                    }
                    else
                    {
                        updateToken();
                    }
                }
                else
                {
                    throwError(new Exception(ERRO_SEMANTICO), ITEM_NOT_FOUND);
                }
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
                    throwError(new Exception(ERRO_SINTATICO));
                }
            }
            else if (!hasEndedTokens && (isSimbol(VERDADEIRO) || isSimbol(FALSO)))
            {
                updateToken();
            }
            else
            {
                throwError(new Exception(ERRO_SINTATICO));
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
