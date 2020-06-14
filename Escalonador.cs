using System;
using System.Threading;

public class Escalonador
{
    static Semaphore semaforo = new Semaphore(0, 1);

    public static void Escalona()
    {
        Console.WriteLine("to antes do while true no escalona");

        while (true)
        {
            semaforo.Release();

            //Console.WriteLine("Entrei no escalona cusao");

            if (FilaDeProntos.ContarProcessos() != 0)
            {
                Console.WriteLine("to escalonando");

                ProcessControlBlock pcb = FilaDeProntos.DequeueProcess();

                Thread cpuProcess = new Thread(() => CPU.ExecutarCPU(pcb));

                cpuProcess.Start();

                if (pcb.State == State.WAITING)
                {
                    FilaDeProntos.AddProcess(pcb);
                }

                //ver dps oq faz com o processo qnd ele estiver finished
            }
            //Console.WriteLine("nao escalonei nada");
            semaforo.WaitOne();
        }
    }
}