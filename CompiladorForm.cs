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
            //richTextBox1.BindScroll(LineNumberTextBox);
        }

        public int getWidth()
        {
            int w = 25;
            // get total lines of richTextBox1    
            int line = richTextBox1.Lines.Length;

            if (line <= 99)
            {
                w = 20 + (int)richTextBox1.Font.Size;
            }
            else if (line <= 999)
            {
                w = 30 + (int)richTextBox1.Font.Size;
            }
            else
            {
                w = 50 + (int)richTextBox1.Font.Size;
            }
         
            return w;
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

            Console.WriteLine(pt.X + "///" + pt.Y);
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
            Console.WriteLine(richTextBox1.SelectionStart);
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
            /*if (KeyCodes.Contains(e.KeyValue) && (e.KeyCode == Keys.V && e.Control))
            {
                richTextBox1.SelectedText = "";
                richTextBox1.ScrollToCaret();
                AddLineNumbers();
            }*/

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
            bool lexicoExecutado = executarLexico(lexico);

            if (lexicoExecutado)
            {
                bool sintaticoExecutado = executarSintatico(sintatico);

                if (sintaticoExecutado)
                {
                    richTextBox2.Text += "Compilado com sucesso!\n";
                }
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
                if (exception.Message.Equals(ERRO_SINTATICO))
                {
                    paintErrorLine(sintatico.errorToken.getLine());
                    richTextBox2.Text += "Erro-> '" + sintatico.errorToken.getLexem() + "' na linha " + sintatico.errorToken.getLine() + "\n";
                }

                return false;
            }
        }

        private bool executarLexico(Lexico lexico)
        {
            try
            {
                lexico.executeLexico(richTextBox1.Text.Replace("\t", "").Replace("\n", " \n"));

                if (lexico.errorToken != null)
                {
                    throw new Exception(ERRO_LEXICO);
                }
                else
                {
                    return true;
                }
            }
            catch (Exception exception)
            {
                if (exception.Message.Equals(ERRO_LEXICO))
                {
                    int errorLine = lexico.errorToken.getLine();
                    paintErrorLine(errorLine);

                    switch (lexico.errorToken.getErrorType())
                    {
                        case COMENTARIO_ERROR:
                            richTextBox2.Text = richTextBox2.Text + "Comentário aberto mas não fechado na linha " + errorLine.ToString() + "\n";
                            break;
                        case CARACTER_ERROR:
                            richTextBox2.Text = richTextBox2.Text + "Caracter '" + lexico.errorToken.getLexem() + "' não reconhecido na linha " + errorLine.ToString() + "\n";
                            break;
                    }
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
