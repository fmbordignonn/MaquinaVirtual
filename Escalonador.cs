using System;
using System.Threading;

public class Escalonador
{
    public static Semaphore semaforoEscalonador = new Semaphore(0, 1);

    public static void Escalona()
    {
        //Console.WriteLine("to antes do while true no escalona");

        while (true)
        {

            //Console.WriteLine("Entrei no escalona cusao");

            if (FilaDeProntos.ContarProcessos() != 0)
            {
                Console.WriteLine("\nto escalonando");

                ProcessControlBlock pcb = FilaDeProntos.DequeueProcess();

                Thread cpuProcess = new Thread(() => CPU.ExecutarCPU(pcb));

                //Iniciando execução do processo na CPU
                cpuProcess.Start();

                //Bloqueando execução do escalonador até que o processo que está
                //executando atualmente na CPU tenha estourado o timer/time slice
                semaforoEscalonador.WaitOne();

                if (pcb.State == State.WAITING)
                {
                    FilaDeProntos.AddProcess(pcb);
                }
                //quando o processo estiver FINISHED
                else if (pcb.State == State.FINISHED)
                {
                    //PrintRegistradoresEMemoria(pcb);
                }

            }
            
            //Console.WriteLine("nao escalonei nada");
            
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