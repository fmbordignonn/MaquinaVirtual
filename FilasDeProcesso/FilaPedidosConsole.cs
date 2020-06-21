using System;
using System.Collections.Generic;

public static class FilaPedidosConsole
{
    private static Queue<ProcessControlBlock> FilaPedidos{ get; set; }

    static FilaPedidosConsole()
    {
        FilaPedidos = new Queue<ProcessControlBlock>();
    }

    public static void AddProcess(ProcessControlBlock pcb)
    {
        FilaPedidos.Enqueue(pcb);

        Console.WriteLine($"\nAdicionou o processo {pcb.ProcessID} a fila de pedidos do console com status {pcb.State}");

        //Console.WriteLine($"Process Id: {pcb.ProcessID} | State: {pcb.State} | Offset: {pcb.OffSet} | EndereçoLimite: {pcb.EnderecoLimite}");
    }

    public static ProcessControlBlock DequeueProcess()
    {
        return FilaPedidos.Dequeue();
    }

    public static int ContarProcessos()
    {
        return FilaPedidos.Count;
    }

    public static void PrintFilaPedidosConsole()
    {
        Console.WriteLine("Printando fila de pedidos do console:\n");

        foreach (var pcb in FilaPedidos)
        {
            Console.WriteLine($"Process Id: {pcb.ProcessID} | State: {pcb.State} | Offset: {pcb.OffSet} | EndereçoLimite: {pcb.EnderecoLimite}");
        }
    }
}