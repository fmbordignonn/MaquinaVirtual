using System;
using Modelos;
using FilasProcesso;
using ComponentesSO;

namespace RotinaTratamento
{
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

            //Liberado o escalonador para puxar outro processo da fila de prontos, já
            //que o que estava rodando foi interrompido por timer
            Escalonador.semaforoEscalonador.Release();
            //Segue o flow de volta no escalonador

            //Console.WriteLine("liberei o escalonador");
        }

    }
}