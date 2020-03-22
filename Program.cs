using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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

            int[] memoria = new int[1024];

            int pc = 0;

            Console.WriteLine("Digite o caminho do arquivo\n");
            var filePath = @"C:\Users\Felipe\Desktop\texiste.txt";//Console.ReadLine();

            var fileContent = File.ReadAllLines(filePath);

            while (fileContent[pc] != "STOP")
            {
                var dataContent = fileContent[pc].Split(' ');

                string command = dataContent[0];

                string[] parameters = dataContent[1].Split(',');

                switch (command)
                {
                    // faz PC pular direto pra uma linha k
                    //JMP 12
                    case "JMP":
                        pc = Convert.ToInt32(parameters[0]);
                    break;

                    // faz PC pular direto pra linha contida no registrador r
                    // JMPI r1
                    case "JMPI":
                        pc = Convert.ToInt32(registradores[parameters[0]]);
                    break;

                    // carrega um valor k em um registrador
                    // LDI r1, 10
                    case "LDI":
                        registradores[parameters[0]] = Convert.ToInt32(parameters[1].Trim());
                    break;

                    // carrega um valor da memoria em um registrador
                    // LDD r1,[50]
                    case "LDD":
                        var value = parameters[1].Trim(new char [] {'[', ']'});
                        int convertedValue = Convert.ToInt32(value);

                        //memoria[51] = 12313;
                        registradores[parameters[0]] = memoria[convertedValue];
                    break;

                    default:
                        throw new ArgumentException($"Não foi possível encontrar o comando [{command}]");
                }

                pc++; // acho q isso nao deve ser aqui, mas ve isso dps
            }
        }
    }
}
