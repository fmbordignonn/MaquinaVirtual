using System;

public class RotinaTratamentoFinalizacao
{
    public static void TratamentoAcessoIndevido(ProcessControlBlock pcb)
    {
        Console.WriteLine($"Encaminhando para rotina de tratamento finalização");
        Console.WriteLine($"O processo '[{pcb.ProcessID}]' está sendo encerrado");

        // DESALOCAR MEMORIA

        Escalonador.semaforoEscalonador.Release();
        //Segue o flow de volta no escalonador

        Console.WriteLine("liberei o escalonador");
    }

    public static void FinalizarProcesso(ProcessControlBlock pcb)
    {
        pcb.State = State.FINISHED;
        Console.WriteLine($"Terminou de rodar processo {pcb.ProcessID}");

        
        GerenteDeMemoria.DesalocarParticao(pcb.ParticaoAtual, pcb.OffSet, pcb.EnderecoLimite);

        Escalonador.semaforoEscalonador.Release();
        //Segue o flow de volta no escalonador

        Console.WriteLine("liberei o escalonador");
    }
}