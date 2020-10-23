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
        public bool pintado = false;
        Lexico lexico = new Lexico();
        Sintatico sintatico = new Sintatico();
        private int lastIndex = 0;
        private int lastLength = 0;
        public List<int> KeyCodes = new List<int>() { 17, 86 }; //ctrl + v
        public int storeLastLine = -1;

        public CompiladorForm()
        {
            InitializeComponent();
            AddLineNumbers();
        }

        public void AddLineNumbers()
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
            AddLineNumbers();
        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            Point pt = richTextBox1.GetPositionFromCharIndex(richTextBox1.SelectionStart);
            if (pt.X == 1)
            {
                AddLineNumbers();
            }
        }

        private void richTextBox1_VScroll(object sender, EventArgs e)
        {
            LineNumberTextBox.Text = "";
            AddLineNumbers();
            LineNumberTextBox.Invalidate();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (richTextBox1.Text == "")
            {
                AddLineNumbers();
            }
            richTextBox1.Font = new Font("Microsoft Sans Serif", 9);
        }

        private void richTextBox1_FontChanged(object sender, EventArgs e)
        {
            LineNumberTextBox.Font = richTextBox1.Font;
            richTextBox1.Select();
            AddLineNumbers();
        }

        private void RichTextBox1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        public void MarkSingleLine()
        {
            int firstCharOfLineIndex = richTextBox1.GetFirstCharIndexOfCurrentLine();
            int currentLine = richTextBox1.GetLineFromCharIndex(firstCharOfLineIndex);
            richTextBox1.SelectionStart = currentLine;
            richTextBox1.SelectionBackColor = Color.Aqua;
        }

        private void LineNumberTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            richTextBox1.Select();
            LineNumberTextBox.DeselectAll();
        }

        private void CompiladorForm_Resize(object sender, EventArgs e)
        {
            AddLineNumbers();
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
                return true;
            }
            catch (Exception exception)
            {
                Token errorToken = sintatico.errorToken;

                switch (exception.Message)
                {
                    case ERRO_LEXICO:
                        paintErrorLine(errorToken.getLine());

                        switch (errorToken.getErrorType())
                        {
                            case COMENTARIO_ERROR:
                                richTextBox2.Text = richTextBox2.Text + "Comentário aberto mas não fechado na linha " + errorToken.getLine() + "\n";
                                break;
                            case CARACTER_ERROR:
                                richTextBox2.Text = richTextBox2.Text + "Caracter '" + errorToken.getLexem() + "' não reconhecido na linha " + errorToken.getLine() + "\n";
                                break;
                        }

                        break;

                    case ERRO_SINTATICO:
                        paintErrorLine(errorToken.getLine());
                        richTextBox2.Text += "Erro-> '" + errorToken.getLexem() + "' na linha " + errorToken.getLine() + "\n";
                        break;

                    case ERRO_PV:
                        paintErrorLine(errorToken.getLine());
                        richTextBox2.Text += "Erro-> " + ERRO_PV + " na linha " + errorToken.getLine() + "\n";
                        break;

                    case ERRO_NOME:
                        paintErrorLine(errorToken.getLine());
                        richTextBox2.Text += "Erro-> " + ERRO_NOME + " na linha " + errorToken.getLine() + "\n";
                        break;

                    case ERRO_FALTA:
                        paintErrorLine(errorToken.getLine());
                        richTextBox2.Text += "Erro-> " + ERRO_FALTA + " na linha " + errorToken.getLine() + "\n";
                        break;

                    case ERRO_INICIO:
                        paintErrorLine(errorToken.getLine());
                        richTextBox2.Text += "Erro-> " + ERRO_INICIO + " na linha " + errorToken.getLine() + "\n";
                        break;

                    case ERRO_PARENTESIS:
                        paintErrorLine(errorToken.getLine());
                        richTextBox2.Text += "Erro-> " + ERRO_PARENTESIS + " na linha " + errorToken.getLine() + "\n";
                        break;

                    case ERRO_TIPO:
                        paintErrorLine(errorToken.getLine());
                        richTextBox2.Text += "Erro-> " + ERRO_TIPO + " na linha " + errorToken.getLine() + "\n";
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
            AddLineNumbers();
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
