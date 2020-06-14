using System;
using System.Collections.Generic;
public static class FilaDeProntos
{
    private static Queue<ProcessControlBlock> Fila { get; set; }

    static FilaDeProntos()
    {
        Fila = new Queue<ProcessControlBlock>();
    }

    public static void AddProcess(ProcessControlBlock pcb)
    {

        Console.WriteLine($"Adicionou o processo {pcb.ProcessID} a fila de prontos");
        Console.WriteLine("---------------------------------");

        pcb.State = State.READY;
        Fila.Enqueue(pcb);

        Console.WriteLine($"Process Id: {pcb.ProcessID} | State: {pcb.State} | Offset: {pcb.OffSet} | EndereçoLimite: {pcb.EnderecoLimite}");
    }

    public static ProcessControlBlock DequeueProcess()
    {
        return Fila.Dequeue();
    }

    public static int ContarProcessos()
    {
        return Fila.Count;
    }
}
