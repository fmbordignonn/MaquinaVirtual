using System;
using System.Threading;
using Modelos;
using FilasProcesso;

namespace ComponentesSO
{
    public class Escalonador
    {
        public static Semaphore semaforoEscalonador = new Semaphore(0, 1);

        public static void Escalona()
        {  
            //Console.WriteLine("to antes do while true no escalona");

            while (true)
            {
                if (FilaDeProntos.ContarProcessos() != 0)
                {
                    //Console.WriteLine("\nto escalonando");

                    ProcessControlBlock pcb = FilaDeProntos.DequeueProcess();

                    Thread cpuProcess = new Thread(() => CPU.ExecutarCPU(pcb));

                    //Apenas pra debuggar e ver os processo rodando
                    Thread.Sleep(2000);

                    //Iniciando execução do processo na CPU
                    cpuProcess.Start();

                    //Bloqueia a execução do escalonador até que o processo que está
                    //executando atualmente na CPU tenha estourado o timer, ou ocorrido
                    //outro tipo de interrupção
                    semaforoEscalonador.WaitOne();
                    //Segue o flow de semaforos lá na CPU

                
                    //Soltando a CPU que havia sido bloqueada após a interrupção, para que
                    //ela esteja disponível para rodar um novo processo 'cpuProcess.Start()'
                    //assim que ele for escalonado
                    CPU.semCPU.Release();
                    //Console.WriteLine("liberei a cpu");

                }
            }
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