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
        private int totalVariables = 0;
        private Stack<int> totalVarsCreatedLocally = new Stack<int>();
        private Stack<int> createdVarsLocally = new Stack<int>();
        private int declaredVar = 0;

        private void resetValidators()
        {
            tokenCount = 0;
            hasEndedTokens = false;
            errorToken = null;
            actualToken = null;
            tokenList = null;
            Semantico.resetStack();
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
            totalVariables = 0;
            CodeGenerator.cleanCommands();
            totalVarsCreatedLocally.Clear();
            declaredVar = 0;
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
                        Semantico.insereTabela(actualToken.lexem, NOME_PROGRAMA, 0);

                        CodeGenerator.gera(EMPTY_STRING, START, EMPTY_STRING, EMPTY_STRING);
                        CodeGenerator.gera(EMPTY_STRING, ALLOC, "0", "1");
                        totalVariables++;

                        updateToken();

                        if (!hasEndedTokens && isSimbol(PONTO_VIRGULA))
                        {
                            analisaBloco();

                            if (isSimbol(PONTO))
                            {
                                updateToken();

                                if (!hasEndedTokens)
                                {
                                    throwError(new CompiladorException(ERRO_SINTATICO), ERRO_FALTA);
                                }

                                CodeGenerator.gera(EMPTY_STRING, DALLOC, "0", "1");
                                CodeGenerator.gera(EMPTY_STRING, HLT, EMPTY_STRING, EMPTY_STRING);
                            }
                            else
                            {
                                throwError(new CompiladorException(ERRO_SINTATICO), ERRO_PONTO_FALTA);
                            }
                        }
                        else
                        {
                            throwError(new CompiladorException(ERRO_SINTATICO), ERRO_PV);
                        }
                    }
                    else
                    {
                        throwError(new CompiladorException(ERRO_SINTATICO), ERRO_FALTA);
                    }
                }
                else
                {
                    throwError(new CompiladorException(ERRO_SINTATICO), ERRO_NOME);
                }
            }
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
                throwError(new CompiladorException(ERRO_LEXICO), actualToken.errorType);
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

            int totalCreatedLocally = createdVarsLocally.Pop();

            while (totalCreatedLocally > 0)
            {
                int totalVarsPopped = totalVarsCreatedLocally.Pop();
                totalVariables -= totalVarsPopped;
                CodeGenerator.gera(EMPTY_STRING, DALLOC, totalVariables.ToString(), totalVarsPopped.ToString());
                totalCreatedLocally--;
            }
        }

        private void analisaEtVariaveis()
        {
            declaredVar = 0;

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
                            throwError(new CompiladorException(ERRO_SINTATICO), ERRO_PV);
                        }
                    }
                }
                else
                {
                    throwError(new CompiladorException(ERRO_SINTATICO), ERRO_NOME);
                }
            }

            createdVarsLocally.Push(declaredVar);
        }

        private void analisaVariaveis()
        {
            int varsCreated = 0;
            do
            {
                if (!hasEndedTokens && isSimbol(IDENTIFICADOR))
                {
                    if (!Semantico.pesquisaDuplicVarTabela(actualToken.lexem))
                    {
                        Semantico.insereTabela(actualToken.lexem, NOME_VARIAVEL, totalVariables + varsCreated);
                        varsCreated++;

                        updateToken();

                        if (!hasEndedTokens && (isSimbol(VIRGULA) || isSimbol(DOIS_PONTOS)))
                        {
                            if (!hasEndedTokens && isSimbol(VIRGULA))
                            {
                                updateToken();

                                if (!hasEndedTokens && !isSimbol(IDENTIFICADOR))
                                {
                                    throwError(new CompiladorException(ERRO_SINTATICO), ERRO_VAR);
                                }
                            }
                        }
                        else
                        {
                            throwError(new CompiladorException(ERRO_SINTATICO), ERRO_DOIS_PONTOS);
                        }
                    }
                    else
                    {
                        throwError(new CompiladorException(ERRO_SEMANTICO), DUPLIC_VAR_ERROR);
                    }
                }
                else
                {
                    throwError(new CompiladorException(ERRO_SINTATICO), ERRO_NOME);
                }
            } while (!hasEndedTokens && !isSimbol(DOIS_PONTOS));

            updateToken();

            declaredVar++;
            totalVarsCreatedLocally.Push(varsCreated);
            CodeGenerator.gera(EMPTY_STRING, ALLOC, totalVariables.ToString(), varsCreated.ToString());
            totalVariables += varsCreated;

            analisaTipo();
        }

        private void analisaTipo()
        {
            if (!hasEndedTokens && !isSimbol(INTEIRO) && !isSimbol(BOOLEANO))
            {
                throwError(new CompiladorException(ERRO_SINTATICO), ERRO_TIPO);
            }
            else
            {
                Semantico.colocaTipoTabela(isSimbol(INTEIRO) ? TIPO_INTEIRO : TIPO_BOOLEANO);
                updateToken();
            }
        }

        private void analisaComandos()
        {
            if (!hasEndedTokens && isSimbol(INICIO))
            {
                updateToken();

                if (isSimbol(FIM))
                {
                    throwError(new CompiladorException(ERRO_SINTATICO), ERRO_CORPO);
                }

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
                        throwError(new CompiladorException(ERRO_SINTATICO), ERRO_PV);
                    }
                    
                }

                returnAlreadyMade = false;

                updateToken();
            }
            else
            {
                throwError(new CompiladorException(ERRO_SINTATICO), ERRO_BLOCO);
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
                    if (Semantico.pesquisaDeclVarFuncTabela(actualToken.lexem))
                    {
                        Struct identifierStruct = Semantico.pesquisaTabela(actualToken.lexem, 0);

                        if (identifierStruct.nome.Equals(NOME_FUNCAO))
                        {
                            CodeGenerator.gera(EMPTY_STRING, CALL, identifierStruct.rotulo.ToString(), EMPTY_STRING);
                            CodeGenerator.gera(EMPTY_STRING, LDV, FUNCTION_RETURN_LABEL, EMPTY_STRING);
                        }
                        else
                        {
                            CodeGenerator.gera(EMPTY_STRING, LDV, identifierStruct.rotulo.ToString(), EMPTY_STRING);
                        }
                        
                        CodeGenerator.gera(EMPTY_STRING, PRN, EMPTY_STRING, EMPTY_STRING);

                        updateToken();

                        if (!hasEndedTokens && isSimbol(FECHA_PARENTESES))
                        {
                            updateToken();
                        }
                        else
                        {
                            throwError(new CompiladorException(ERRO_SINTATICO), ERRO_PARENTESIS);
                        }
                    }
                    else
                    {
                        throwError(new CompiladorException(ERRO_SEMANTICO), DECL_VAR_FUNC_ERROR);
                    }
                }
                else
                {
                    throwError(new CompiladorException(ERRO_SINTATICO), ERRO_NOME);
                }
            }
            else
            {
                throwError(new CompiladorException(ERRO_SINTATICO), ERRO_PARENTESIS);
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
                    if (Semantico.pesquisaDeclVarTabela(actualToken.lexem))
                    {
                        CodeGenerator.gera(EMPTY_STRING, RD, EMPTY_STRING, EMPTY_STRING);
                        CodeGenerator.gera(EMPTY_STRING, STR, Semantico.pesquisaTabela(actualToken.lexem, 0).rotulo.ToString(), EMPTY_STRING);
                        updateToken();

                        if (!hasEndedTokens && isSimbol(FECHA_PARENTESES))
                        {
                            updateToken();
                        }
                        else
                        {
                            throwError(new CompiladorException(ERRO_SINTATICO), ERRO_PARENTESIS);
                        }
                    }
                    else
                    {
                        throwError(new CompiladorException(ERRO_SEMANTICO), DECL_VAR_ERROR);
                    }
                }
                else
                {
                    throwError(new CompiladorException(ERRO_SINTATICO), ERRO_NOME);
                }
            }
            else
            {
                throwError(new CompiladorException(ERRO_SINTATICO), ERRO_PARENTESIS);
            }
        }

        private void analisaEnquanto()
        {
            int auxrot1, auxrot2;

            auxrot1 = rotulo;
            CodeGenerator.gera(auxrot1.ToString(), NULL, EMPTY_STRING, EMPTY_STRING);
            rotulo++;

            updateToken();

            Semantico.cleanExpression();
            analisaExpressao();

            try
            {
                returnType = Semantico.analyzeExpression();
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
                CodeGenerator.geraCodeExpression(Semantico.getPosFixExpression());
            }

            if (!hasEndedTokens && isSimbol(FACA))
            {
                auxrot2 = rotulo;
                CodeGenerator.gera(EMPTY_STRING, JMPF, auxrot2.ToString(), EMPTY_STRING);
                rotulo++;

                updateToken();

                analisaComandoSimples();

                returnMade = false;

                CodeGenerator.gera(EMPTY_STRING, JMP, auxrot1.ToString(), EMPTY_STRING);
                CodeGenerator.gera(auxrot2.ToString(), NULL, EMPTY_STRING, EMPTY_STRING);
            }
            else
            {
                throwError(new CompiladorException(ERRO_SINTATICO), ERRO_FALTA);
            }
        }

        private void analisaSe()
        {
            int auxrot1=0, auxrot2=0;

            updateToken();

            Semantico.cleanExpression();
            analisaExpressao();

            try
            {
                returnType = Semantico.analyzeExpression();
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
                CodeGenerator.geraCodeExpression(Semantico.getPosFixExpression());

                auxrot1 = rotulo;
                CodeGenerator.gera(EMPTY_STRING, JMPF, auxrot1.ToString(), EMPTY_STRING);
                rotulo++;
            }

            if (!hasEndedTokens && isSimbol(ENTAO))
            {
                bool entaoReturnMade = false;
                bool senaoReturnMade = false;
                
                updateToken();

                analisaComandoSimples();

                if (functionReturnsExpected > 0)
                {
                    entaoReturnMade = returnMade;

                    if (!hasEndedTokens && isSimbol(SENAO))
                    {
                        auxrot2 = rotulo;
                        CodeGenerator.gera(EMPTY_STRING, JMP, auxrot2.ToString(), EMPTY_STRING);
                        CodeGenerator.gera(auxrot1.ToString(), NULL, EMPTY_STRING, EMPTY_STRING);
                        rotulo++;

                        returnAlreadyMade = false;
                        updateToken();

                        analisaComandoSimples();

                        CodeGenerator.gera(auxrot2.ToString(), NULL, EMPTY_STRING, EMPTY_STRING);
                        senaoReturnMade = returnMade;
                    }
                    else
                    {
                        CodeGenerator.gera(auxrot1.ToString(), NULL, EMPTY_STRING, EMPTY_STRING);
                    }

                    returnMade = entaoReturnMade && senaoReturnMade;
                    returnAlreadyMade = returnMade;
                }
                else
                {
                    if (!hasEndedTokens && isSimbol(SENAO))
                    {
                        auxrot2 = rotulo;
                        CodeGenerator.gera(EMPTY_STRING, JMP, auxrot2.ToString(), EMPTY_STRING);
                        CodeGenerator.gera(auxrot1.ToString(), NULL, EMPTY_STRING, EMPTY_STRING);
                        rotulo++;

                        updateToken();
                        analisaComandoSimples();

                        CodeGenerator.gera(auxrot2.ToString(), NULL, EMPTY_STRING, EMPTY_STRING);
                    }
                    else
                    {
                        CodeGenerator.gera(auxrot1.ToString(), NULL, EMPTY_STRING, EMPTY_STRING);
                    }
                }
            }
            else
            {
                throwError(new CompiladorException(ERRO_SINTATICO), ERRO_FALTA);
            }
        }

        private void analisaAtribChamadaProc()
        {
            tokenAtribuicao = actualToken;
            structReceivedForAssignment = Semantico.pesquisaTabela(tokenAtribuicao.lexem, 0);

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
                if (structReceivedForAssignment.nome.Equals(NOME_PROCEDIMENTO))
                {
                    analisaChamadaProcedimento(structReceivedForAssignment);
                }
                else
                {
                    throwError(new CompiladorException(ERRO_SEMANTICO), INVALID_PROC_CALL);
                }
            }
        }

        private void analisaSubRotinas()
        {
            int flag = 0;
            int auxrot = rotulo;

            if (!hasEndedTokens && (isSimbol(PROCEDIMENTO) || isSimbol(FUNCAO)))
            {
                CodeGenerator.gera(EMPTY_STRING, JMP, auxrot.ToString(), EMPTY_STRING);
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
                    CodeGenerator.gera(EMPTY_STRING, RETURN, EMPTY_STRING, EMPTY_STRING);
                    updateToken();
                }
                else
                {
                    throwError(new CompiladorException(ERRO_SINTATICO), ERRO_PV);
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
                if (!Semantico.pesquisaDeclProcTabela(actualToken.lexem))
                {
                    Semantico.insereTabela(actualToken.lexem, NOME_PROCEDIMENTO, rotulo);
                    CodeGenerator.gera(rotulo.ToString(), NULL, EMPTY_STRING, EMPTY_STRING);
                    rotulo++;

                    Semantico.increaseLevel();
                    updateToken();

                    if (!hasEndedTokens && isSimbol(PONTO_VIRGULA))
                    {
                        analisaBloco();
                    }
                    else
                    {
                        throwError(new CompiladorException(ERRO_SINTATICO), ERRO_PV);
                    }
                }
                else
                {
                    throwError(new CompiladorException(ERRO_SEMANTICO), DECL_PROC_ERROR);
                }
            }
            else
            {
                throwError(new CompiladorException(ERRO_SINTATICO), ERRO_FALTA);
            }

            Semantico.voltaNivel();
        }

        private void analisaDeclaracaoFuncao()
        {
            updateToken();

            if (!hasEndedTokens && isSimbol(IDENTIFICADOR))
            {
                actualFunctionName.Push(actualToken.lexem);

                if (!Semantico.pesquisaDeclFuncTabela(actualToken.lexem))
                {
                    Semantico.insereTabela(actualToken.lexem, NOME_FUNCAO, rotulo);
                    CodeGenerator.gera(rotulo.ToString(), NULL, EMPTY_STRING, EMPTY_STRING);
                    rotulo++;

                    Semantico.increaseLevel();
                    functionLine.Push(actualToken.line);
                    updateToken();

                    if (!hasEndedTokens && isSimbol(DOIS_PONTOS))
                    {
                        updateToken();

                        if (!hasEndedTokens && (isSimbol(INTEIRO) || isSimbol(BOOLEANO)))
                        {
                            string type = isSimbol(INTEIRO) ? TIPO_INTEIRO : TIPO_BOOLEANO;
                            Semantico.colocaTipoTabela(type);
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
                            throwError(new CompiladorException(ERRO_SINTATICO), ERRO_TIPO);
                        }
                    }
                    else
                    {
                        throwError(new CompiladorException(ERRO_SINTATICO), ERRO_FALTA);
                    }
                } 
                else
                {
                    throwError(new CompiladorException(ERRO_SEMANTICO), DECL_FUNC_ERROR);
                }
            }
            else
            {
                throwError(new CompiladorException(ERRO_SINTATICO), ERRO_NOME);
            }

            Semantico.voltaNivel();
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
                Semantico.addCharToExpression(actualToken);
                updateToken();

                analisaExpressaoSimples();
            }
        }

        private void analisaExpressaoSimples()
        {
            if (!hasEndedTokens && (isSimbol(MAIS) || isSimbol(MENOS)))
            {
                Semantico.addCharToExpression(new Token(actualToken.simbol, actualToken.lexem+"u", actualToken.line));
                updateToken();
            }

            analisaTermo();

            while (!hasEndedTokens && (isSimbol(MAIS) || isSimbol(MENOS) || isSimbol(OU)))
            {
                Semantico.addCharToExpression(actualToken);
                updateToken();

                analisaTermo();
            }
        }

        private void analisaTermo()
        {
            analisaFator();

            while (!hasEndedTokens && (isSimbol(MULTI) || isSimbol(DIV) || isSimbol(E)))
            {
                Semantico.addCharToExpression(actualToken);
                updateToken();

                analisaFator();
            }
        }

        private void analisaFator()
        {
            if (!hasEndedTokens && isSimbol(IDENTIFICADOR))
            {
                Struct actualItem = Semantico.pesquisaTabela(actualToken.lexem, 0);

                if (actualItem != null)
                {
                    Semantico.addCharToExpression(actualToken);

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
                Semantico.addCharToExpression(actualToken);
                updateToken();
            }
            else if (!hasEndedTokens && isSimbol(NAO))
            {
                Semantico.addCharToExpression(actualToken);
                updateToken();

                analisaFator();
            }
            else if (!hasEndedTokens && isSimbol(ABRE_PARENTESES))
            {
                parentesisCount++;
                Semantico.addCharToExpression(actualToken);
                updateToken();

                analisaExpressao();

                if (!hasEndedTokens && isSimbol(FECHA_PARENTESES))
                {
                    parentesisCount--;
                    Semantico.addCharToExpression(actualToken);
                    updateToken();
                }
                else 
                {
                    throwError(new CompiladorException(ERRO_SINTATICO), ERRO_PARENTESIS);
                }
            }
            else if (!hasEndedTokens && (isSimbol(VERDADEIRO) || isSimbol(FALSO)))
            {
                Semantico.addCharToExpression(actualToken);
                updateToken();
            }
            else
            {
                throwError(new CompiladorException(ERRO_SINTATICO), ERRO_CARACTER);
            }
        }

        private void analisaChamadaProcedimento(Struct structReceivedForAssignment)
        {
            CodeGenerator.gera(EMPTY_STRING, CALL, structReceivedForAssignment.rotulo.ToString(), EMPTY_STRING);
        }

        private void analisaChamadaFuncao()
        {
            updateToken();
        }

        private void analisaAtribuicao()
        {
            updateToken();

            Semantico.cleanExpression();
            analisaExpressao();

            try
            {
                returnType = Semantico.analyzeExpression();
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
                CodeGenerator.geraCodeExpression(Semantico.getPosFixExpression());
            }

            if (structReceivedForAssignment.nome.Equals(NOME_FUNCAO))
            {
                CodeGenerator.gera(EMPTY_STRING, STR, FUNCTION_RETURN_LABEL, EMPTY_STRING);
            }
            else
            {
                CodeGenerator.gera(EMPTY_STRING, STR, structReceivedForAssignment.rotulo.ToString(), EMPTY_STRING);
            }
        }
    }
}
