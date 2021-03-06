﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static Compilador.Constantes;

namespace Compilador
{
    public partial class CompiladorForm : Form
    {
        private static FileManager fileReader = new FileManager();
        private bool pintado = false;
        private Lexico lexico = new Lexico();
        private Sintatico sintatico = new Sintatico();
        private int lastIndex = 0;
        private int lastLength = 0;

        public CompiladorForm()
        {
            InitializeComponent();
            addLineNumbers();
        }

        private void addLineNumbers()
        {
            Point pt = new Point(0, 0);

            int First_Index = richTextBox1.GetCharIndexFromPosition(pt);
            int First_Line = richTextBox1.GetLineFromCharIndex(First_Index);

            pt.X = ClientRectangle.Width;
            pt.Y = ClientRectangle.Height;

            int Last_Index = richTextBox1.GetCharIndexFromPosition(pt);
            int Last_Line = richTextBox1.GetLineFromCharIndex(Last_Index);
            LineNumberTextBox.SelectionAlignment = HorizontalAlignment.Right;
            LineNumberTextBox.Text = "";

            for (int i = First_Line; i < Last_Line + 1; i++)
            {
                LineNumberTextBox.Text += i + 1 + "\n";
            }
            LineNumberTextBox.Invalidate();
        }

        private void CompiladorForm_Load(object sender, EventArgs e)
        {
            LineNumberTextBox.Font = richTextBox1.Font;
            richTextBox1.Select();
            addLineNumbers();
        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            Point pt = richTextBox1.GetPositionFromCharIndex(richTextBox1.SelectionStart);
            if (pt.X == 1)
            {
                addLineNumbers();
            }
        }

        private void richTextBox1_VScroll(object sender, EventArgs e)
        {
            LineNumberTextBox.Text = "";
            addLineNumbers();
            LineNumberTextBox.Invalidate();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (richTextBox1.Text == "")
            {
                addLineNumbers();
            }
            richTextBox1.Font = new Font("Microsoft Sans Serif", 9);
        }

        private void richTextBox1_FontChanged(object sender, EventArgs e)
        {
            LineNumberTextBox.Font = richTextBox1.Font;
            richTextBox1.Select();
            addLineNumbers();
        }

        private void RichTextBox1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void CompiladorForm_Resize(object sender, EventArgs e)
        {
            addLineNumbers();
        }

        private void compilarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lexico.executeLexico(richTextBox1.Text.Replace("\t", "").Replace("\n", " \n"));

            if (executarSintatico(sintatico))
            {
                richTextBox2.Text += "Compilado com sucesso!\n";
            }
        }

        private bool executarSintatico(Sintatico sintatico)
        {
            try
            {
                sintatico.executeSintatico(lexico.getTokens());
                fileReader.saveCompilerResponse(CodeGenerator.getVMCommands());

                return true;
            }
            catch (CompiladorException exception)
            {
                Token errorToken = sintatico.errorToken;

                switch (exception.Message)
                {
                    case ERRO_LEXICO:
                        paintErrorLine(errorToken.line);

                        switch (errorToken.errorType)
                        {
                            case COMENTARIO_ERROR:
                                richTextBox2.Text += "Comentário aberto mas não fechado na linha " + errorToken.line + "\n";
                                break;
                            case CARACTER_ERROR:
                                richTextBox2.Text += "Caracter '" + errorToken.lexem + "' não reconhecido na linha " + errorToken.line + "\n";
                                break;
                            case IDENTIFICADOR_COM_UNDERLINE:
                                richTextBox2.Text += "Identificador comecando com underline na linha " + errorToken.line + "\n";
                                break;
                        }

                        break;

                    case ERRO_SINTATICO:
                        paintErrorLine(errorToken.line);

                        switch (errorToken.errorType)
                        {
                            case ERRO_PV:                             
                                richTextBox2.Text += "Falta ponto e virgula na linha " + errorToken.line + "\n";
                                break;

                            case ERRO_NOME:                        
                                richTextBox2.Text += "Não pode usar '" + errorToken.lexem + "' palavra pré determinada na linha " + errorToken.line + "\n";
                                break;

                            case ERRO_FALTA:                      
                                richTextBox2.Text += "Falta caracter ou comando na linha " + errorToken.line + "\n";
                                break;

                            case ERRO_INICIO:                   
                                richTextBox2.Text += "Falta INICIO na linha " + errorToken.line + "\n";
                                break;

                            case ERRO_PARENTESIS:                 
                                richTextBox2.Text += "Falta parentesis na linha " + errorToken.line + "\n";
                                break;

                            case ERRO_TIPO:            
                                richTextBox2.Text += "São aceitos apenas INTEIRO ou BOOLEANO na linha " + errorToken.line + "\n";
                                break;

                            case ERRO_MAIS:        
                                richTextBox2.Text += "Linha com caracter a mais na linha " + errorToken.line + "\n";
                                break;

                            case ERRO_PONTO_MEIO:        
                                richTextBox2.Text += "Ponto final apenas na última linha e não na linha " + errorToken.line + "\n";
                                break;

                            case ERRO_CARACTER:
                                richTextBox2.Text += "'" + errorToken.lexem + "' na linha " + errorToken.line + "\n";
                                break;

                            case ERRO_CORPO:
                                richTextBox2.Text += "Corpo vazio na linha " + errorToken.line + "\n";
                                break;
                            case ERRO_VAR:
                                richTextBox2.Text += "Apenas variável após a virgula na linha " + errorToken.line + "\n";
                                break;
                            case ERRO_DOIS_PONTOS:
                                richTextBox2.Text += "Espera-se ' : ' após a última variável e não ' " + errorToken.lexem + 
                                    " ' na linha " + errorToken.line + "\n";
                                break;
                            case ERRO_ATRIBUICAO:
                                richTextBox2.Text += "Espera-se ' := ' e não ' " + errorToken.lexem + " ' após a variável na linha " + errorToken.line + "\n";
                                break;
                            case ERRO_BLOCO:
                                richTextBox2.Text += "Caracter encontrado ' " + errorToken.lexem + " '. Não é possível continuar sem pelo menos 'var', 'procedimento', 'função' ou 'inicio' após o ponto e vírgula da linha " + errorToken.line + "\n";
                                break;
                            case ERRO_PONTO_FALTA:
                                richTextBox2.Text += "Falta ponto final no fim da linha " + errorToken.line + "\n";
                                break;
                        }
                        break;

                    case ERRO_SEMANTICO:
                        paintErrorLine(errorToken.line);

                        switch (errorToken.errorType)
                        {
                            case DUPLIC_VAR_ERROR:
                                richTextBox2.Text += "Variavel '" + errorToken.lexem + "' nao declarada ou duplicada na linha " + errorToken.line + "\n";
                                break;
                            case DECL_VAR_ERROR:
                                richTextBox2.Text += "Variavel '" + errorToken.lexem + "' nao declarada ou duplicada na linha " + errorToken.line + "\n";
                                break;
                            case DECL_PROC_ERROR:
                                richTextBox2.Text += "Procedimento '" + errorToken.lexem + "' nao declarado ou duplicado na linha " + errorToken.line + "\n";
                                break;
                            case DECL_FUNC_ERROR:
                                richTextBox2.Text += "Funcao '" + errorToken.lexem + "' nao declarada ou duplicada na linha " + errorToken.line + "\n";
                                break;
                            case ITEM_NOT_FOUND:
                                richTextBox2.Text += "Identificador '" + errorToken.lexem + "' não encontrado\n";
                                break;
                            case INVALID_TYPES:
                                richTextBox2.Text += "Expressao da linha " + errorToken.line + " com tipos incoerentes\n";
                                break;
                            case DECL_VAR_FUNC_ERROR:
                                richTextBox2.Text += "Variavel ou funcao nao encontrada na linha " + errorToken.line + "\n";
                                break;
                            case EXPECTED_FUNCTION_RETURN:
                                richTextBox2.Text += "Faltando retornos da funcao na linha " + errorToken.line + "\n";
                                break;
                            case FUNCTION_LAST_LINE_NOT_RETURN:
                                richTextBox2.Text += "Nem todos os caminhos de codigo da funcao " + exception.InnerException.Message + " retornam um valor\n";
                                break;
                            case INVALID_FUNCTION_NAME:
                                richTextBox2.Text += "Atribuicao da linha " + errorToken.line + " nao referencia a funcao atual\n";
                                break;
                            case WHILE_WITH_RETURN:
                                richTextBox2.Text += "Enquanto da linha " + errorToken.line + " nao deve conter retorno da funcao\n";
                                break;
                            case ASSIGNMENT_EXPRESSION_MUST_BE_BOOL:
                                richTextBox2.Text += "Tipo atribuido na linha " + errorToken.line + " deve ser booleano\n";
                                break;
                            case ASSIGNMENT_EXPRESSION_MUST_BE_INT:
                                richTextBox2.Text += "Tipo atribuido na linha " + errorToken.line + " deve ser inteiro\n";
                                break;
                            case EXPRESSION_MUST_BE_BOOL:
                                richTextBox2.Text += "Expressao da linha " + errorToken.line + " deve ter resultado booleano\n";
                                break;
                            case EXPRESSION_MUST_BE_INT:
                                richTextBox2.Text += "Expressao da linha " + errorToken.line + " deve ter resultado inteiro\n";
                                break;
                            case ANALYZING_EXPRESSION_ERROR:
                                richTextBox2.Text += "Erro na linha " + errorToken.line + ": " + exception.InnerException.Message + "\n";
                                break;
                            case UNREACHABLE_CODE:
                                richTextBox2.Text += "Funcao da linha " + errorToken.line + " possui codigo inalcançavel\n";
                                break;
                            case INVALID_PROC_CALL:
                                richTextBox2.Text += "Somente pode ser chamado procedimentos na linha " + errorToken.line + "\n";
                                break;
                        }
                        break;
                }

                return false;
            }
        }

        private void paintErrorLine(int errorLine)
        {
            int index = richTextBox1.GetFirstCharIndexFromLine(errorLine - 1);
            int length = richTextBox1.Lines[errorLine - 1].Length;
            richTextBox1.Select(index, length);
            richTextBox1.SelectionColor = Color.Red;

            lastIndex = index;
            lastLength = length;
            pintado = true;

            richTextBox1.SelectionStart = richTextBox1.Find(richTextBox1.Lines[errorLine - 1]);
            richTextBox1.ScrollToCaret();
        }

        private void richTextBox1_Click(object sender, EventArgs e)
        {
            int clickLocationIndex = richTextBox1.SelectionStart;
            int clickLocationLength = richTextBox1.SelectionLength;

            if (pintado)
            {
                richTextBox1.Select(lastIndex, lastLength);
                richTextBox1.SelectionColor = Color.Black;
                richTextBox1.Select(clickLocationIndex, clickLocationLength);
                pintado = false;
            }
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fullstring = fileReader.readFile();
            richTextBox1.Text = fullstring.Equals("") ? richTextBox1.Text : fullstring;
            addLineNumbers();
        }

        private void salvarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileReader.saveFile(richTextBox1);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void LineNumberTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = "";
        }

    }
}
