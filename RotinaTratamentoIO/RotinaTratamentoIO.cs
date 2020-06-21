using System;

public class RotinaTratamentoIO
{
    public static void TratamentoInput(ProcessControlBlock pcb, string operation, int endereco)
    {

    }

    public static void TratamentoOutput(ProcessControlBlock pcb, string operation, int endereco)
    {
        pcb.State = State.BLOCKED_BY_IO_OPERATION;

        FilaBloqueadosIO.AddProcess(pcb);

        Escalonador.semaforoEscalonador.Release();
        //Segue o flow de volta no escalonador

        Console.WriteLine("liberei o escalonador");
    }
}