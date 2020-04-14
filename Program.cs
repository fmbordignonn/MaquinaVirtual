using System;

namespace ConsoleApp1
{
    class Program
    {
        public static void Main(string[] args)
        {

            Console.WriteLine("Digite o numero de partições desejado para a CPU (4, 6 ou 8):");
            //int numeroParticoes = Convert.ToInt32(Console.ReadLine());
            SistemaOperacional SO = new SistemaOperacional(8);

            SO.Start();
        }
    }
}