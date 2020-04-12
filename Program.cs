using System;

namespace ConsoleApp1
{
    class Program
    {
        public static void Main(string[] args)
        {
            string input;
            string filePath = Environment.CurrentDirectory + @"\programs\";

            Console.WriteLine("Digite o numero de partições desejado para a CPU:");
            //int numeroParticoes = Convert.ToInt32(Console.ReadLine());
            CPU cpu = new CPU(8);

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
                input = "3";//Console.ReadLine();

                switch (input)
                {
                    case "1":
                        cpu.NewCPUVirtualMachine(filePath + "P1.txt");
                        break;

                    case "2":
                        cpu.NewCPUVirtualMachine(filePath + "P2.txt");
                        break;

                    case "3":
                        cpu.NewCPUVirtualMachine(filePath + "P3.txt");
                        break;

                    case "4":
                        cpu.NewCPUVirtualMachine(filePath + "P4.txt");
                        break;

                    default:
                        break;    
                }
            } while (input != "0");
        }
    }
}