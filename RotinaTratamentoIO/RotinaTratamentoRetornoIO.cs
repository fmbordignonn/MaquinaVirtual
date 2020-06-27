using System;

public class RotinaTratamentoRetornoIO
{
    public static void TratarRetornoIO(string pcbID)
    {
        ProcessControlBlock pcb = FilaBloqueadosIO.DequeueProcess(pcbID);

        pcb.State = State.READY;

        Console.WriteLine($"Desbloqueou o processo [{pcb.ProcessID}]");

        FilaDeProntos.AddProcess(pcb);

    }
}