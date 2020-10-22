using System;
using ComponentesSO;
using Modelos;
using FilasProcesso;

namespace RotinaTratamento
{
    public class RotinaTratamentoFinalizacao
    {
        public static void TratamentoAcessoIndevido(ProcessControlBlock pcb)
        {
            Console.WriteLine($"Encaminhando para rotina de tratamento finalização por acesso indevido");
            Console.WriteLine($"O processo '[{pcb.ProcessID}]' está sendo encerrado");

            pcb.State = State.ABORTED_DUE_TO_EXCEPTION;

            FilaDeFinalizados.AddProcess(pcb);

            GerenteDeMemoria.DesalocarParticao(pcb.ParticaoAtual, pcb.OffSet, pcb.EnderecoLimite);

            Escalonador.semaforoEscalonador.Release();
            //Segue o flow de volta no escalonador

            //Console.WriteLine("liberei o escalonador");
        }

        public static void TratamentoDivisaoPorZero(ProcessControlBlock pcb)
        {
            Console.WriteLine($"Encaminhando para rotina de tratamento finalização de divisão por zero");
            Console.WriteLine($"O processo '[{pcb.ProcessID}]' está sendo encerrado");

            pcb.State = State.ABORTED_DUE_TO_EXCEPTION;

            FilaDeFinalizados.AddProcess(pcb);

            GerenteDeMemoria.DesalocarParticao(pcb.ParticaoAtual, pcb.OffSet, pcb.EnderecoLimite);

            Escalonador.semaforoEscalonador.Release();
            //Segue o flow de volta no escalonador

            //Console.WriteLine("liberei o escalonador");
        }

        public static void FinalizarProcesso(ProcessControlBlock pcb)
        {
            pcb.State = State.FINISHED;
            Console.WriteLine($"---------Terminou de rodar processo {pcb.ProcessID}---------");

            //PrintRegistradoresEMemoria(pcb);

            GerenteDeMemoria.DesalocarParticao(pcb.ParticaoAtual, pcb.OffSet, pcb.EnderecoLimite);

            FilaDeFinalizados.AddProcess(pcb);

            Escalonador.semaforoEscalonador.Release();
            //Segue o flow de volta no escalonador

            //Console.WriteLine("liberei o escalonador");
        }

        public static void PrintRegistradoresEMemoria(ProcessControlBlock pcb)
        {
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine($"DADOS DO PROCESSO: {pcb.ProcessID}\n\n");

            Console.WriteLine("Valores finais dos registradores:\n");

            foreach (var item in pcb.Registradores)
            {
                Console.WriteLine("--------------------------------------------\n");

                Console.WriteLine($"Registrador [{item.Key}] - valor final [{item.Value}]\n");
            }

            Console.WriteLine("--------------------------------------------\n");

            Console.WriteLine("Status finais das posições de memória (não nulas) da particao\n");

            for (int i = pcb.OffSet; i < pcb.EnderecoLimite; i++)
            {
                if (GerenteDeMemoria.Memoria[i] == null)
                {
                    continue;
                }

                Console.WriteLine($"Posição de memória [{i}]:");
                Console.WriteLine(GerenteDeMemoria.Memoria[i].ToString() + "\n");
            }
        }
    }
}