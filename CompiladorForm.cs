using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compilador
{
    public partial class CompiladorForm : Form
    {
        public CompiladorForm()
        {
            InitializeComponent();
        }

        private void CompiladorForm_Load(object sender, EventArgs e)
        {

        }


        private void compilarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Lexico lexico = new Lexico();
            lexico.executeLexico(richTextBox1.Text.Replace("\t", "").Replace("\n", " \n"));
        }
    }
}
