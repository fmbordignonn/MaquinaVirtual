﻿using System;

namespace ConsoleApp1
{
    class Program
    {
        public static void Main(string[] args)
        {
            //Console.WriteLine("Digite o numero de partições desejado para a CPU (4 ou 8):\n");
            int numeroParticoes = 8;
            SistemaOperacional SO = new SistemaOperacional(numeroParticoes);

            SO.Start();

            
        }
    }
}