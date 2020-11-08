using System;
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
        private Semantico semantico = new Semantico();
        private int lastIndex = 0;
        private int lastLength = 0;

        public CompiladorForm()
        {
            InitializeComponent();
            addLineNumbers();
        }

        private void addLineNumbers()
        {
            // create & set Point pt to (0,0)    
            Point pt = new Point(0, 0);
            // get First Index & First Line from richTextBox1    
            int First_Index = richTextBox1.GetCharIndexFromPosition(pt);
            int First_Line = richTextBox1.GetLineFromCharIndex(First_Index);
            // set X & Y coordinates of Point pt to ClientRectangle Width & Height respectively    
            pt.X = ClientRectangle.Width;
            pt.Y = ClientRectangle.Height;

            // get Last Index & Last Line from richTextBox1    
            int Last_Index = richTextBox1.GetCharIndexFromPosition(pt);
            int Last_Line = richTextBox1.GetLineFromCharIndex(Last_Index);
            // set Center alignment to LineNumberTextBox    
            LineNumberTextBox.SelectionAlignment = HorizontalAlignment.Right;
            // set LineNumberTextBox text to null & width to getWidth() function value    
            LineNumberTextBox.Text = "";

            //LineNumberTextBox.Width = getWidth();
            // now add each line number to LineNumberTextBox upto last line   
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
                sintatico.executeSintatico(lexico.getTokens(), semantico);
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
                        }

                        break;

                    case ERRO_SINTATICO:
                        paintErrorLine(errorToken.line);
                        richTextBox2.Text += "Erro-> '" + errorToken.lexem + "' na linha " + errorToken.line + "\n";
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
                                richTextBox2.Text += "Procedimento '" + errorToken.lexem + "' nao declarada ou duplicado na linha " + errorToken.line + "\n";
                                break;
                            case DECL_FUNC_ERROR:
                                richTextBox2.Text += "Funcao '" + errorToken.lexem + "' nao declarada ou duplicada na linha " + errorToken.line + "\n";
                                break;
                            case ITEM_NOT_FOUND:
                                richTextBox2.Text += "Item '" + errorToken.lexem + "' não encontrado\n";
                                break;
                            case INVALID_TYPES:
                                richTextBox2.Text += "Expressao da linha " + errorToken.line + " com tipos incoerentes\n";
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
