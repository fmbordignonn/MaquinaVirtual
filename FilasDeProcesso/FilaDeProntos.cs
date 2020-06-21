using System;
using System.Collections.Generic;
public static class FilaDeProntos
{
    private static Queue<ProcessControlBlock> FilaProntos { get; set; }

    static FilaDeProntos()
    {
        FilaProntos = new Queue<ProcessControlBlock>();
    }

    public static void AddProcess(ProcessControlBlock pcb)
    {
        FilaProntos.Enqueue(pcb);

        Console.WriteLine($"\nAdicionou o processo {pcb.ProcessID} a fila de prontos com status {pcb.State}");

        //Console.WriteLine($"Process Id: {pcb.ProcessID} | State: {pcb.State} | Offset: {pcb.OffSet} | EndereçoLimite: {pcb.EnderecoLimite}");
    }

    public static ProcessControlBlock DequeueProcess()
    {
        return FilaProntos.Dequeue();
    }

    public static int ContarProcessos()
    {
        return FilaProntos.Count;
    }

    public static void PrintFilaDeProntos()
    {
        Console.WriteLine("Printando fila de processos prontos:\n");

        foreach (var pcb in FilaProntos)
        {
            Console.WriteLine($"Process Id: {pcb.ProcessID} | State: {pcb.State} | Offset: {pcb.OffSet} | EndereçoLimite: {pcb.EnderecoLimite}");
        }
    }
}
