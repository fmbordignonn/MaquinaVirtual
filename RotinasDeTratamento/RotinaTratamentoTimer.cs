using System;

public class RotinaTratamentoTimer
{
    public static void TratarInterrupcaoTimer(ProcessControlBlock pcb)
    {
        //Decidindo se o processo volta pra fila
        if (pcb.State == State.WAITING)
        {
            FilaDeProntos.AddProcess(pcb);
            Console.WriteLine("voltou pra fila de prontos");
        }
        //quando o processo estiver FINISHED
        //possivel q saia daqui pois a rotina de stop é diferente de timer
        else if (pcb.State == State.FINISHED)
        {
            //PrintRegistradoresEMemoria(pcb);
        }

        //Liberado o escalonador para puxar outro processo da fila de prontos, já
        //que o que estava rodando foi interrompido por timer
        Escalonador.semaforoEscalonador.Release();
        //Segue o flow de volta no escalonador
        
        Console.WriteLine("liberei o escalonador");
    }

}