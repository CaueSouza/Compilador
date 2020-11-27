namespace Compilador
{
    class Constantes
    {
        public const string PROGRAMA = "sprograma";
        public const string INICIO = "sinicio";
        public const string FIM = "sfim";
        public const string PROCEDIMENTO = "sprocedimento";
        public const string FUNCAO = "sfuncao";
        public const string SE = "sse";
        public const string ENTAO = "sentao";
        public const string SENAO = "ssenao";
        public const string ENQUANTO = "senquanto";
        public const string FACA = "sfaca";
        public const string ATRIBUICAO = "satribuicao";
        public const string ESCREVA = "sescreva";
        public const string LEIA = "sleia";
        public const string VAR = "svar";
        public const string INTEIRO = "sinteiro";
        public const string BOOLEANO = "sbooleano";
        public const string IDENTIFICADOR = "sidentificador";
        public const string NUMERO = "snumero";
        public const string PONTO = "sponto";
        public const string PONTO_VIRGULA = "sponto_virgula";
        public const string VIRGULA = "svirgula";
        public const string ABRE_PARENTESES = "sabre_parenteses";
        public const string FECHA_PARENTESES = "sfecha_parenteses";
        public const string MAIOR = "smaior";
        public const string MAIORIG = "smaiorig";
        public const string IGUAL = "sig";
        public const string MENOR = "smenor";
        public const string MENORIG = "smenorig";
        public const string DIF = "sdif";
        public const string MAIS = "smais";
        public const string MENOS = "smenos";
        public const string MULTI = "smult";
        public const string DIV = "sdiv";
        public const string E = "se";
        public const string OU = "sou";
        public const string NAO = "snao";
        public const string DOIS_PONTOS = "sdoispontos";
        public const string VERDADEIRO = "sverdadeiro";
        public const string FALSO = "sfalso";

        public const int COMENTARIO_ERROR = 1;
        public const int CARACTER_ERROR = 2;
        public const int IDENTIFICADOR_COM_UNDERLINE = 3;

        public const string ERRO_LEXICO = "erro_lexico";
        public const string ERRO_SINTATICO = "erro_sintatico";
        public const string ERRO_SEMANTICO = "erro_semantico";

        public const string NOME_PROGRAMA = "programa";
        public const string NOME_VARIAVEL = "variavel";
        public const string NOME_PROCEDIMENTO = "procedimento";
        public const string NOME_FUNCAO = "funcao";

        public const string TIPO_INTEIRO = "inteiro";
        public const string TIPO_BOOLEANO = "booleano";

        public const int ITEM_NOT_FOUND = 1;
        public const int DUPLIC_VAR_ERROR = 2;
        public const int DECL_VAR_ERROR = 3;
        public const int DECL_PROC_ERROR = 4;
        public const int DECL_FUNC_ERROR = 5;
        public const int INVALID_TYPES = 6;
        public const int DECL_VAR_FUNC_ERROR = 7;
        public const int EXPECTED_FUNCTION_RETURN = 8;
        public const int FUNCTION_LAST_LINE_NOT_RETURN = 9;
        public const int INVALID_FUNCTION_NAME = 10;
        public const int WHILE_WITH_RETURN = 11;
        public const int ASSIGNMENT_EXPRESSION_MUST_BE_BOOL = 12;
        public const int ASSIGNMENT_EXPRESSION_MUST_BE_INT = 13;
        public const int EXPRESSION_MUST_BE_BOOL = 14;
        public const int EXPRESSION_MUST_BE_INT = 15;
        public const int ANALYZING_EXPRESSION_ERROR = 16;
        public const int UNREACHABLE_CODE = 17;

        
        public const string ERROR_MESSAGE_INT_UNARY = "Operacao unaria +/- deve ser feita com tipo inteiro";
        public const string ERROR_MESSAGE_BOOL_UNARY = "Operacao unaria 'nao' deve ser feita com tipo booleano";
        public const string ERROR_MESSAGE_E_OU = "Operacao E/OU deve ser feita com dois booleanos";
        public const string ERROR_MESSAGE_EQUAL_DIF = "Tipos comparados incompativeis";
        public const string ERROR_MESSAGE_NORMAL_OP = "Tipos incompatives para operacao {0}";
        public const string ERROR_MESSAGE_IDENTIFIER_NOT_FOUND = "Identificador nao encontrado";

        public const string EMPTY_STRING = "";

        public const string LDC = "LDC";
        public const string LDV = "LDV";
        public const string ADD = "ADD";
        public const string SUB = "SUB";
        public const string MULT = "MULT";
        public const string DIVI = "DIVI";
        public const string INV = "INV";
        public const string AND = "AND";
        public const string OR = "OR";
        public const string NEG = "NEG";
        public const string CME = "CME";
        public const string CMA = "CMA";
        public const string CEQ = "CEQ";
        public const string CDIF = "CDIF";
        public const string CMEQ = "CMEQ";
        public const string CMAQ = "CMAQ";
        public const string START = "START";
        public const string HLT = "HLT";
        public const string STR = "STR";
        public const string JMP = "JMP";
        public const string JMPF = "JMPF";
        public const string NULL = "NULL";
        public const string RD = "RD";
        public const string PRN = "PRN";
        public const string ALLOC = "ALLOC";
        public const string DALLOC = "DALLOC";
        public const string CALL = "CALL";
        public const string RETURN = "RETURN";

        public const int ERRO_PV = 1;
        public const int ERRO_FALTA = 2;
        public const int ERRO_NOME = 3;
        public const int ERRO_TIPO = 4;
        public const int ERRO_INICIO = 5;
        public const int ERRO_PARENTESIS = 6;
        public const int ERRO_MAIS = 7;
        public const int ERRO_PONTO = 8;
        public const int ERRO_CARACTER = 9;
        public const int ERRO_CORPO = 10;
        public const int ERRO_VAR = 11;
        public const int ERRO_DOIS_PONTOS = 12;
        public const int ERRO_VAR_ONDE = 13;
        public const int ERRO_FALTA_DPS = 14;

    }
}
