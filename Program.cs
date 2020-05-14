using System;

namespace ConsoleApp1
{
    class Program
    {
        public static void Main(string[] args)
        {

            Console.WriteLine("Digite o numero de partições desejado para a CPU (4 ou 8):\n");
            int numeroParticoes = Convert.ToInt32(Console.ReadLine());
            SistemaOperacional SO = new SistemaOperacional(numeroParticoes);

            SO.Start();


            int exit;
            do
            {
                Console.WriteLine("Digite 0 para sair");
                exit = Convert.ToInt32(Console.ReadLine());
            }
            while(exit != 0);
        }
    }
}