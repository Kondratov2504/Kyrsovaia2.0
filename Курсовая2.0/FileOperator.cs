using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTools
{
    internal class FileOperator
    {
        // readPath = dataPath; или вместо dataPath любая другая строковая переменная "с:\text.txt"
        public void CloneFile(string readPath, string writePath)
      
        {
            using (StreamReader reader = new StreamReader(readPath))
            {
                using (StreamWriter writer = new StreamWriter(writePath))
                {
                    //string[] lines = File.ReadAllLines(dataPath);
                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                    {

                        

                        // Пишем измененную строку в выходной файл
                        writer.WriteLine(line);

                    }
                    // Читаем и записываем содержимое файла




                }
            }
        }
        public void TryCreateFile(string path)
        {
            if (false == File.Exists(path)) // если файла не существует , то создаем этот файл в папке temp
            {
                // Create a file to write to.
                using (StreamWriter file = File.CreateText(path))
                {

                }


            }
        }
        public string ReadFile(string path)
        {
            using (StreamReader file = File.OpenText(path)) // открываем файл для чтения 
            {
                string text = "";
                string line = "";
                while ((line = file.ReadLine()) != null) // если строка не пустая в файле
                {

                    if (text == "")
                    {

                        text = line;
                    }
                    else
                    {
                        text += "\r\n" + line; // то переносим все строки в таком расположении в каком они были и в файле (с отступами)

                    }
                }
                return text;
            }
        }
        public static void SaveFile(string savePath, string text)
        {
            using (StreamWriter file = File.CreateText(savePath)) // создаем или перезаписываем с именем path и записывваем в него строку из textBox1
            {
                file.WriteLine(text);
            }
        }

        public void AppendLine(string path, string line)
        {
            using (StreamWriter writer = File.AppendText(path))
            {
                writer.WriteLine(line);
            }
        }
    }
}
