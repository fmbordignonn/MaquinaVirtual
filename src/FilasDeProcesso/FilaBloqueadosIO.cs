using System;
using System.Linq;
using System.Collections.Generic;
using Modelos;

namespace FilasProcesso
{
    public class FilaBloqueadosIO
    {
        private static Queue<ProcessControlBlock> FilaBloqueados { get; set; }

        static FilaBloqueadosIO()
        {
            FilaBloqueados = new Queue<ProcessControlBlock>();
        }

        public static void AddProcess(ProcessControlBlock pcb)
        {
            FilaBloqueados.Enqueue(pcb);

            Console.WriteLine($"\nAdicionou o processo {pcb.ProcessID} a fila de bloqueados por IO");
        }

        public static ProcessControlBlock DequeueProcess(string pcbID)
        {
            foreach (var item in FilaBloqueados)
            {
                if (item.ProcessID == pcbID)
                {
                    return FilaBloqueados.Dequeue();
                }
            }
            throw new ArgumentException($"O process id numero [{pcbID}] não existe na fila de processos bloqueados");
        }

        public static int ContarProcessos()
        {
            return FilaBloqueados.Count;
        }

        public static void PrintFilaDeBloqueados()
        {
            if (FilaBloqueados.Count == 0)
            {
                Console.WriteLine("Não há processos bloqueados por IO no momento");
            }
            else
            {
                Console.WriteLine("Printando fila de processos bloqueados:\n");

                foreach (var pcb in FilaBloqueados)
                {
                    Console.WriteLine($"Process Id: {pcb.ProcessID} | State: {pcb.State} | Offset: {pcb.OffSet} | EndereçoLimite: {pcb.EnderecoLimite}");
                }
            }
        }
    }
}