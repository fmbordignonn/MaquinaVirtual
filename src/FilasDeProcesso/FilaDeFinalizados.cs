using System;
using System.Collections.Generic;
using Modelos;

namespace FilasProcesso
{
    public static class FilaDeFinalizados
    {
        private static Queue<ProcessControlBlock> FilaFinalizados { get; set; }

        static FilaDeFinalizados()
        {
            FilaFinalizados = new Queue<ProcessControlBlock>();
        }

        public static void AddProcess(ProcessControlBlock pcb)
        {
            FilaFinalizados.Enqueue(pcb);

            Console.WriteLine($"Adicionou o processo {pcb.ProcessID} a fila de processos finalizados");
        }

        public static ProcessControlBlock DequeueProcess()
        {
            return FilaFinalizados.Dequeue();
        }

        public static int ContarProcessos()
        {
            return FilaFinalizados.Count;
        }

        public static void PrintFilaDeFinalizados()
        {
            if (FilaFinalizados.Count == 0)
            {
                Console.WriteLine("Não há processos finalizados no momento");
            }
            else
            {
                Console.WriteLine("Printando fila de processos finalizados:\n");

                foreach (var pcb in FilaFinalizados)
                {
                    Console.WriteLine($"Process Id: {pcb.ProcessID} | State: {pcb.State} | Offset: {pcb.OffSet} | EndereçoLimite: {pcb.EnderecoLimite}");
                }
            }
        }
    }

}