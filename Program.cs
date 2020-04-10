using System;

namespace ConsoleApp1
{
    class Program
    {
        public static void Main(string[] args)
        {
            string input;
            string filePath = Environment.CurrentDirectory + @"\programs\";

            CPU cpu = new CPU();

            do
            {
                Console.WriteLine("---------------------------------");
                Console.WriteLine("1 - P1");
                Console.WriteLine("2 - P2");
                Console.WriteLine("3 - P3");
                Console.WriteLine("4 - P4");
                Console.WriteLine("0 - exit");
                Console.WriteLine("---------------------------------\n");
                Console.WriteLine("Digite o programa que deseja executar");
                input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        cpu.VirtualMachine(filePath + "P1.txt");
                        break;

                    case "2":
                        cpu.VirtualMachine(filePath + "P2.txt");
                        break;

                    case "3":
                        cpu.VirtualMachine(filePath + "P3.txt");
                        break;

                    case "4":
                        cpu.VirtualMachine(filePath + "P4.txt");
                        break;

                    default:
                        break;    
                }
            } while (input != "0");
        }
    }
}