using System;
using Modelos;
using FilasProcesso;

namespace RotinaTratamento
{
    public class RotinaTratamentoRetornoIO
    {
        public static void TratarRetornoIO(string pcbID)
        {
            ProcessControlBlock pcb = FilaBloqueadosIO.DequeueProcess(pcbID);

            pcb.State = State.WAITING;

            Console.WriteLine($"Desbloqueou o processo [{pcb.ProcessID}]");

            FilaDeProntos.AddProcess(pcb);
        }
    }
}