using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        public static void Main(string[] args)
        {
            Dictionary<string, int> registradores = new Dictionary<string, int>();

            registradores.Add("r1", 0);
            registradores.Add("r2", 0);
            registradores.Add("r3", 0);
            registradores.Add("r4", 0);
            registradores.Add("r5", 0);
            registradores.Add("r6", 0);
            registradores.Add("r7", 0);
            registradores.Add("r8", 0);

            string[] memoria = new string[1024];

            int pc = 0;

            Console.WriteLine("Digite o caminho do arquivo\n");
            var filePath = @"C:\Users\Felipe\Desktop\texiste.txt";//Console.ReadLine();

            var fileContent = File.ReadAllLines(filePath);

            while (fileContent[pc] != "STOP")
            {
                ParseLineContent(fileContent[pc], registradores);
            }
        }

        public static string ParseLineContent(string line, Dictionary<string, int> registradores)
        {
            var dataContent = line.Split(' ');

            string command = dataContent[0];
            string[] parameters = dataContent[1].Split(',');
            
            switch (command)
            {
                case "LDI":
                registradores[parameters[0]] = Convert.ToInt32(parameters[1]);
                break;
            }

            return null;
        }
    }
}
