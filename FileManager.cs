using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Windows.Forms;

namespace Compilador
{
    class FileManager
    {
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;

        public FileManager()
        {
            openFileDialog = new OpenFileDialog()
            {
                FileName = "Select a text file",
                Filter = "Text files (*.txt)|*.txt",
                Title = "Open text file"
            };

            saveFileDialog = new SaveFileDialog()
            {
                FileName = "Compilador text file",
                Filter = "Text files (*.txt)|*.txt",
                Title = "Save text file"
            };
        }

        public string readFile()
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var filePath = openFileDialog.FileName;
                    using (Stream str = openFileDialog.OpenFile())
                    {
                        return File.ReadAllText(filePath).Replace("\t", "").Replace("\r\n", " \n");
                    }
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                    return "";
                }
            }

            return "";
        }

        public void saveFile(RichTextBox richTextBox)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK && saveFileDialog.FileName.Length > 0)
            {
                richTextBox.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.PlainText);
            }
        }

        public void saveCompilerResponse(List<string> VMCommands)
        {
            string finalText = "";

            foreach (string command in VMCommands)
            {
                finalText += command + "\n";
            }

            if (saveFileDialog.ShowDialog() == DialogResult.OK && saveFileDialog.FileName.Length > 0)
            {
                using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                {
                    writer.Write(finalText);
                    writer.Close();
                }
            }
        }
    }
}
