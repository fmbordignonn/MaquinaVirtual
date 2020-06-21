using System;
using System.Collections.Generic;

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

        //Console.WriteLine($"Process Id: {pcb.ProcessID} | State: {pcb.State} | Offset: {pcb.OffSet} | EndereçoLimite: {pcb.EnderecoLimite}");
    }

    public static ProcessControlBlock DequeueProcess()
    {
        return FilaBloqueados.Dequeue();
    }

    public static int ContarProcessos()
    {
        return FilaBloqueados.Count;
    }

    public static void PrintFilaDeBloqueados()
    {
        Console.WriteLine("Printando fila de processos bloqueados:\n");

        foreach (var pcb in FilaBloqueados)
        {
            Console.WriteLine($"Process Id: {pcb.ProcessID} | State: {pcb.State} | Offset: {pcb.OffSet} | EndereçoLimite: {pcb.EnderecoLimite}");
        }
    }
}