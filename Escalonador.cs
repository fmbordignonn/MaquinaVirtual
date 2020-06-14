using System;
using System.Threading;

public class Escalonador
{
    Thread escalonador = new Thread(new ThreadStart(Escalona));

    static Semaphore semaforo = new Semaphore(0, 1);

    public Escalonador()
    {
        escalonador.Start();
    }

    public static void Escalona()
    {
        Console.WriteLine("Entrei no escalona cusao");

        if (FilaDeProntos.ContarProcessos() != 0)
        {
            Console.WriteLine("to escalonando");

            semaforo.Release();

            ProcessControlBlock pcb = FilaDeProntos.DequeueProcess();

            Thread cpuProcess = new Thread(() => CPU.ExecutarCPU(pcb));

            if(pcb.State == State.WAITING){
                FilaDeProntos.AddProcess(pcb);
            }

            //ver dps oq faz com o processo qnd ele estiver finished
        }
        Console.WriteLine("nao escalonei nada");
        semaforo.WaitOne();
    }
}