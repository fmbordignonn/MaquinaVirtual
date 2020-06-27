using System;

public class RotinaTratamentoIO
{
    public static void TratarPedidoIO(ProcessControlBlock pcb, IOType operation, int endereco)
    {
        pcb.State = State.BLOCKED_BY_IO_OPERATION;

        FilaBloqueadosIO.AddProcess(pcb);

        PedidoConsoleIO pedido = new PedidoConsoleIO(pcb.ProcessID, operation, endereco);

        FilaPedidosConsole.AddPedidoIO(pedido);

        Escalonador.semaforoEscalonador.Release();
        //Segue o flow de volta no escalonador

        Console.WriteLine("liberei o escalonador");
    }
}