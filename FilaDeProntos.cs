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
        Fila.Enqueue(pcb);

        Console.WriteLine($"\nAdicionou o processo {pcb.ProcessID} a fila de prontos com status {pcb.State}");

        //Console.WriteLine($"Process Id: {pcb.ProcessID} | State: {pcb.State} | Offset: {pcb.OffSet} | EndereçoLimite: {pcb.EnderecoLimite}");
    }

    public static ProcessControlBlock DequeueProcess()
    {
        return Fila.Dequeue();
    }

    public static int ContarProcessos()
    {
        return Fila.Count;
    }

    public static void PrintFilaDeProntos()
    {
        Console.WriteLine("Printando fila de processos prontos:\n");

        foreach (var pcb in Fila)
        {
            Console.WriteLine($"Process Id: {pcb.ProcessID} | State: {pcb.State} | Offset: {pcb.OffSet} | EndereçoLimite: {pcb.EnderecoLimite}");
        }
    }
}
