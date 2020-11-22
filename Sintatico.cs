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
        public Token errorToken { get; set; }
        private bool hasEndedTokens = false;
        private Semantico semantico;
        private int parentesisCount = 0;
        private int analyzeExpressionStarterLine = 0;
        private string returnType = "";
        private Token tokenAtribuicao;
        private int rotulo = 1;
        private int functionReturnsExpected = 0;
        private Stack<int> functionLine = new Stack<int>();
        private Struct structReceivedForAssignment = null;
        private bool returnMade = false;
        private int returnsMade = 0;
        private Stack<string> actualFunctionName = new Stack<string>();
        private bool returnAlreadyMade = false;

        private void resetValidators()
        {
            tokenCount = 0;
            hasEndedTokens = false;
            errorToken = null;
            actualToken = null;
            tokenList = null;
            semantico.resetStack();
            parentesisCount = 0;
            analyzeExpressionStarterLine = 0;
            returnType = "";
            tokenAtribuicao = null;
            rotulo = 1;
            functionReturnsExpected = 0;
            functionLine.Clear();
            structReceivedForAssignment = null;
            returnMade = false;
            returnsMade = 0;
            actualFunctionName.Clear();
            returnAlreadyMade = false;
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
                        CodeGenerator.gera(EMPTY_STRING, START, EMPTY_STRING, EMPTY_STRING);

                        updateToken();

                        if (!hasEndedTokens && isSimbol(PONTO_VIRGULA))
                        {
                            analisaBloco();

                            if (isSimbol(PONTO))
                            {
                                updateToken();

                                if (!hasEndedTokens)
                                {
                                    throwError(new CompiladorException(ERRO_SINTATICO));
                                }

                                CodeGenerator.gera(EMPTY_STRING, HLT, EMPTY_STRING, EMPTY_STRING);
                            }
                            else
                            {
                                throwError(new CompiladorException(ERRO_SINTATICO));
                            }
                        }
                        else
                        {
                            throwError(new CompiladorException(ERRO_SINTATICO));
                        }
                    }
                    else
                    {
                        throwError(new CompiladorException(ERRO_SINTATICO));
                    }
                }
                else
                {
                    throwError(new CompiladorException(ERRO_SINTATICO));
                }
            }
        }

        private void throwError(CompiladorException exception)
        {
            errorToken = actualToken;
            throw exception;
        }

        private void throwError(CompiladorException exception, int errorType)
        {
            errorToken = new Token(actualToken.lexem, actualToken.line, errorType);
            throw exception;
        }

        private void throwError(CompiladorException exception, int errorType, int errorLine)
        {
            errorToken = new Token(actualToken.lexem, errorLine, errorType);
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
                throwError(new CompiladorException(ERRO_LEXICO));
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
                            throwError(new CompiladorException(ERRO_SINTATICO));
                        }
                    }
                }
                else
                {
                    throwError(new CompiladorException(ERRO_SINTATICO));
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
                                    throwError(new CompiladorException(ERRO_SINTATICO));
                                }
                            }
                        }
                        else
                        {
                            throwError(new CompiladorException(ERRO_SINTATICO));
                        }
                    }
                    else
                    {
                        throwError(new CompiladorException(ERRO_SEMANTICO), DUPLIC_VAR_ERROR);
                    }
                }
                else
                {
                    throwError(new CompiladorException(ERRO_SINTATICO));
                }
            } while (!hasEndedTokens && !isSimbol(DOIS_PONTOS));

            updateToken();

            analisaTipo();
        }

        private void analisaTipo()
        {
            if (!hasEndedTokens && !isSimbol(INTEIRO) && !isSimbol(BOOLEANO))
            {
                throwError(new CompiladorException(ERRO_SINTATICO));
            }
            else
            {
                semantico.colocaTipoTabela(isSimbol(INTEIRO) ? TIPO_INTEIRO : TIPO_BOOLEANO);
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
                        throwError(new CompiladorException(ERRO_SINTATICO));
                    }
                }

                returnAlreadyMade = false;

                updateToken();
            }
            else
            {
                throwError(new CompiladorException(ERRO_SINTATICO));
            }
        }

        private void analisaComandoSimples()
        {
            if (returnAlreadyMade)
            {
                throwError(new CompiladorException(ERRO_SEMANTICO), UNREACHABLE_CODE, functionLine.Peek());
            }

            returnMade = false;

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
                    if (semantico.pesquisaDeclVarFuncTabela(actualToken.lexem))
                    {
                        updateToken();

                        if (!hasEndedTokens && isSimbol(FECHA_PARENTESES))
                        {
                            updateToken();
                        }
                        else
                        {
                            throwError(new CompiladorException(ERRO_SINTATICO));
                        }
                    }
                    else
                    {
                        throwError(new CompiladorException(ERRO_SEMANTICO), DECL_VAR_FUNC_ERROR);
                    }
                }
                else
                {
                    throwError(new CompiladorException(ERRO_SINTATICO));
                }
            }
            else
            {
                throwError(new CompiladorException(ERRO_SINTATICO));
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
                            throwError(new CompiladorException(ERRO_SINTATICO));
                        }
                    }
                    else
                    {
                        throwError(new CompiladorException(ERRO_SEMANTICO), DECL_VAR_ERROR);
                    }
                }
                else
                {
                    throwError(new CompiladorException(ERRO_SINTATICO));
                }
            }
            else
            {
                throwError(new CompiladorException(ERRO_SINTATICO));
            }
        }

        private void analisaEnquanto()
        {
            int auxrot1, auxrot2;

            auxrot1 = rotulo;
            CodeGenerator.gera(rotulo.ToString(), NULL, EMPTY_STRING, EMPTY_STRING);
            rotulo++;

            updateToken();

            semantico.cleanExpression();
            analisaExpressao();

            try
            {
                returnType = semantico.analyzeExpression();
            }
            catch (CompiladorException e)
            {
                throwError(e, ANALYZING_EXPRESSION_ERROR, analyzeExpressionStarterLine);
            }
            
            if (!returnType.Equals(TIPO_BOOLEANO))
            {
                throwError(new CompiladorException(ERRO_SEMANTICO), EXPRESSION_MUST_BE_BOOL, analyzeExpressionStarterLine);
            }
            else
            {
                //para usar a lista use posfixExpression, para ter a string completa use finalPosFixExpression
                List<string> posFixExpression = semantico.getPosFixExpression();

                //TODO GERAR CODIGO PARA A POSFIXA
            }

            if (!hasEndedTokens && isSimbol(FACA))
            {
                auxrot2 = rotulo;
                CodeGenerator.gera(EMPTY_STRING, JMPF, rotulo.ToString(), EMPTY_STRING);
                rotulo++;

                updateToken();

                analisaComandoSimples();

                returnMade = false;

                CodeGenerator.gera(EMPTY_STRING, JMP, auxrot1.ToString(), EMPTY_STRING);
                CodeGenerator.gera(auxrot2.ToString(), NULL, EMPTY_STRING, EMPTY_STRING);
            }
            else
            {
                throwError(new CompiladorException(ERRO_SINTATICO));
            }
        }

        private void analisaSe()
        {
            updateToken();

            semantico.cleanExpression();
            analisaExpressao();

            if (!hasEndedTokens && isSimbol(ENTAO))
            {
                try
                {
                    returnType = semantico.analyzeExpression();
                }
                catch (CompiladorException e)
                {
                    throwError(e, ANALYZING_EXPRESSION_ERROR, analyzeExpressionStarterLine);
                }

                if (!returnType.Equals(TIPO_BOOLEANO))
                {
                    throwError(new CompiladorException(ERRO_SEMANTICO), EXPRESSION_MUST_BE_BOOL, analyzeExpressionStarterLine);
                }
                else
                {
                    //para usar a lista use posfixExpression, para ter a string completa use finalPosFixExpression
                    List<string> posFixExpression = semantico.getPosFixExpression();

                    //TODO GERAR CODIGO PARA A POSFIXA
                }

                bool entaoReturnMade = false;
                bool senaoReturnMade = false;
                
                updateToken();

                analisaComandoSimples();

                if (functionReturnsExpected > 0)
                {
                    entaoReturnMade = returnMade;

                    if (!hasEndedTokens && isSimbol(SENAO))
                    {
                        updateToken();

                        analisaComandoSimples();

                        senaoReturnMade = returnMade;
                    }

                    returnMade = entaoReturnMade && senaoReturnMade;
                    returnAlreadyMade = returnMade;
                }
                else
                {
                    if (!hasEndedTokens && isSimbol(SENAO))
                    {
                        updateToken();
                        analisaComandoSimples();
                    }
                }
            }
            else
            {
                throwError(new CompiladorException(ERRO_SINTATICO));
            }
        }

        private void analisaAtribChamadaProc()
        {
            tokenAtribuicao = actualToken;
            structReceivedForAssignment = semantico.pesquisaTabela(tokenAtribuicao.lexem, 0);

            if (structReceivedForAssignment == null)
            {
                throwError(new CompiladorException(ERRO_SEMANTICO), ITEM_NOT_FOUND);
            }
            
            updateToken();
            bool hasSameName;

            try
            {
                if (structReceivedForAssignment != null && structReceivedForAssignment.nome.Equals(NOME_FUNCAO))
                {
                    hasSameName = structReceivedForAssignment.lexema.Equals(actualFunctionName.Peek());
                } 
                else
                {
                    hasSameName = false;
                }
            }
            catch (Exception)
            {
                hasSameName = false;
            }

            if (!hasEndedTokens && isSimbol(ATRIBUICAO))
            {
                if (structReceivedForAssignment.nome.Equals(NOME_VARIAVEL))
                {
                    analisaAtribuicao();
                }
                else if (hasSameName && structReceivedForAssignment.nome.Equals(NOME_FUNCAO) && functionReturnsExpected > 0)
                {
                    analisaAtribuicao();
                    returnsMade++;
                    returnMade = true;
                    returnAlreadyMade = true;
                }
                else
                {
                    if (!hasSameName && structReceivedForAssignment.nome.Equals(NOME_FUNCAO))
                    {
                        throwError(new CompiladorException(ERRO_SEMANTICO), INVALID_FUNCTION_NAME, tokenAtribuicao.line);
                    }
                    else
                    {
                        if (structReceivedForAssignment.tipo.Equals(TIPO_BOOLEANO))
                        {
                            throwError(new CompiladorException(ERRO_SEMANTICO), ASSIGNMENT_EXPRESSION_MUST_BE_BOOL, tokenAtribuicao.line);
                        }
                        else
                        {
                            throwError(new CompiladorException(ERRO_SEMANTICO), ASSIGNMENT_EXPRESSION_MUST_BE_INT, tokenAtribuicao.line);
                        }
                    }
                }
            }
            else
            {
                analisaChamadaProcedimento();
            }
        }

        private void analisaSubRotinas()
        {
            int flag = 0;
            int auxrot = rotulo;

            if (!hasEndedTokens && (isSimbol(PROCEDIMENTO) || isSimbol(FUNCAO)))
            {
                CodeGenerator.gera(EMPTY_STRING, JMP, rotulo.ToString(), EMPTY_STRING);
                rotulo++;
                flag = 1;
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
                    throwError(new CompiladorException(ERRO_SINTATICO));
                }
            }

            if (flag == 1)
            {
                CodeGenerator.gera(auxrot.ToString(), NULL, EMPTY_STRING, EMPTY_STRING);
            }
        }

        private void analisaDeclaracaoProcedimento()
        {
            updateToken();

            if (!hasEndedTokens && isSimbol(IDENTIFICADOR))
            {
                if (!semantico.pesquisaDeclProcTabela(actualToken.lexem))
                {
                    semantico.insereTabela(actualToken.lexem, NOME_PROCEDIMENTO, rotulo);
                    CodeGenerator.gera(rotulo.ToString(), NULL, EMPTY_STRING, EMPTY_STRING);
                    rotulo++;

                    semantico.increaseLevel();
                    updateToken();

                    if (!hasEndedTokens && isSimbol(PONTO_VIRGULA))
                    {
                        analisaBloco();
                    }
                    else
                    {
                        throwError(new CompiladorException(ERRO_SINTATICO));
                    }
                }
                else
                {
                    throwError(new CompiladorException(ERRO_SEMANTICO), DECL_PROC_ERROR);
                }
            }
            else
            {
                throwError(new CompiladorException(ERRO_SINTATICO));
            }

            semantico.voltaNivel();
        }

        private void analisaDeclaracaoFuncao()
        {
            updateToken();

            if (!hasEndedTokens && isSimbol(IDENTIFICADOR))
            {
                actualFunctionName.Push(actualToken.lexem);

                if (!semantico.pesquisaDeclFuncTabela(actualToken.lexem))
                {
                    semantico.insereTabela(actualToken.lexem, NOME_FUNCAO, rotulo);
                    CodeGenerator.gera(rotulo.ToString(), NULL, EMPTY_STRING, EMPTY_STRING);//TODO: CHECK IF THIS IS RIGHT
                    rotulo++;

                    semantico.increaseLevel();
                    functionLine.Push(actualToken.line);
                    updateToken();

                    if (!hasEndedTokens && isSimbol(DOIS_PONTOS))
                    {
                        updateToken();

                        if (!hasEndedTokens && (isSimbol(INTEIRO) || isSimbol(BOOLEANO)))
                        {
                            string type = isSimbol(INTEIRO) ? TIPO_INTEIRO : TIPO_BOOLEANO;
                            semantico.colocaTipoTabela(type);
                            updateToken();

                            if (!hasEndedTokens && isSimbol(PONTO_VIRGULA))
                            {
                                functionReturnsExpected++;
                                analisaBloco();
                                functionReturnsExpected--;

                                if (!returnMade)
                                {
                                    throwError(new CompiladorException(ERRO_SEMANTICO, new CompiladorException(actualFunctionName.Peek())), returnsMade > 0 ? FUNCTION_LAST_LINE_NOT_RETURN : EXPECTED_FUNCTION_RETURN, functionLine.Peek());
                                }

                                actualFunctionName.Pop();
                                functionLine.Pop();
                            }
                        }
                        else
                        {
                            throwError(new CompiladorException(ERRO_SINTATICO));
                        }
                    }
                    else
                    {
                        throwError(new CompiladorException(ERRO_SINTATICO));
                    }
                } 
                else
                {
                    throwError(new CompiladorException(ERRO_SEMANTICO), DECL_FUNC_ERROR);
                }
            }
            else
            {
                throwError(new CompiladorException(ERRO_SINTATICO));
            }

            semantico.voltaNivel();
        }

        private void analisaExpressao()
        {
            if (parentesisCount == 0)
            {
                analyzeExpressionStarterLine = actualToken.line;
            }

            analisaExpressaoSimples();

            while (!hasEndedTokens &&
                (isSimbol(MAIOR) || isSimbol(MAIORIG) || isSimbol(IGUAL) || isSimbol(MENOR) || isSimbol(MENORIG) || isSimbol(DIF)))
            {
                semantico.addCharToExpression(actualToken);
                updateToken();

                analisaExpressaoSimples();
            }
        }

        private void analisaExpressaoSimples()
        {
            if (!hasEndedTokens && (isSimbol(MAIS) || isSimbol(MENOS)))
            {
                semantico.addCharToExpression(actualToken);
                updateToken();
            }

            analisaTermo();

            while (!hasEndedTokens && (isSimbol(MAIS) || isSimbol(MENOS) || isSimbol(OU)))
            {
                semantico.addCharToExpression(actualToken);
                updateToken();

                analisaTermo();
            }
        }

        private void analisaTermo()
        {
            analisaFator();

            while (!hasEndedTokens && (isSimbol(MULTI) || isSimbol(DIV) || isSimbol(E)))
            {
                semantico.addCharToExpression(actualToken);
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
                    semantico.addCharToExpression(actualToken);

                    if (actualItem.tipo == TIPO_INTEIRO || actualItem.tipo == TIPO_BOOLEANO)
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
                    throwError(new CompiladorException(ERRO_SEMANTICO), ITEM_NOT_FOUND);
                }
            }
            else if (!hasEndedTokens && isSimbol(NUMERO))
            {
                semantico.addCharToExpression(actualToken);
                updateToken();
            }
            else if (!hasEndedTokens && isSimbol(NAO))
            {
                semantico.addCharToExpression(actualToken);
                updateToken();

                analisaFator();
            }
            else if (!hasEndedTokens && isSimbol(ABRE_PARENTESES))
            {
                parentesisCount++;
                semantico.addCharToExpression(actualToken);
                updateToken();

                analisaExpressao();

                if (!hasEndedTokens && isSimbol(FECHA_PARENTESES))
                {
                    parentesisCount--;
                    semantico.addCharToExpression(actualToken);
                    updateToken();
                }
                else
                {
                    throwError(new CompiladorException(ERRO_SINTATICO));
                }
            }
            else if (!hasEndedTokens && (isSimbol(VERDADEIRO) || isSimbol(FALSO)))
            {
                semantico.addCharToExpression(actualToken);
                updateToken();
            }
            else
            {
                throwError(new CompiladorException(ERRO_SINTATICO));
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

            semantico.cleanExpression();
            analisaExpressao();

            try
            {
                returnType = semantico.analyzeExpression();
            }
            catch (CompiladorException e)
            {
                throwError(e, ANALYZING_EXPRESSION_ERROR, analyzeExpressionStarterLine);
            }

            if (!returnType.Equals(structReceivedForAssignment.tipo))
            {
                if (structReceivedForAssignment.tipo.Equals(TIPO_BOOLEANO))
                {
                    throwError(new CompiladorException(ERRO_SEMANTICO), ASSIGNMENT_EXPRESSION_MUST_BE_BOOL, analyzeExpressionStarterLine);
                }
                else
                {
                    throwError(new CompiladorException(ERRO_SEMANTICO), ASSIGNMENT_EXPRESSION_MUST_BE_INT, analyzeExpressionStarterLine);
                }
            }
            else
            {
                //para usar a lista use posfixExpression, para ter a string completa use finalPosFixExpression
                List<string> posFixExpression = semantico.getPosFixExpression();

                //TODO GERAR CODIGO PARA A POSFIXA
            }
        }
    }
}
