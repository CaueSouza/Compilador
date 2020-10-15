using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    class FileReader
    {

        public FileReader()
        {

        }

        public string readFile(String filePath)
        {
            //string filePath, fileName;
            //Console.Write("Informe o caminho do arquivo: ");
            //filePath = Console.ReadLine();
            //Console.Write("Informe o nome do arquivo: ");
            // fileName = Console.ReadLine();

            //string fullPath = filePath + '\\' + fileName;
            //Console.WriteLine("Full Path: {0}\n", fullPath);

            return File.ReadAllText(filePath).Replace("\t", "").Replace("\r\n", " \n");
        }

        public string parseFileCommands(String filePath)
        {
            return readFile(filePath);
        }
    }
}
