using System;
using System.Collections.Generic;
using System.Threading;

public class SistemaOperacional
{
    public CPU cpu;

    public static GerenteDeProcesso GerenteProcesso;

    public FilaDeProntos FilaProntos;

    Semaphore sem = new Semaphore(0, 1);

    Thread shell = new Thread(new ThreadStart(RodarShell));



    public SistemaOperacional(int numeroParticoes)
    {
        cpu = new CPU();

        FilaProntos = new FilaDeProntos();

        //tem o gerente de memoria dentro
        GerenteProcesso = new GerenteDeProcesso(numeroParticoes, FilaProntos);

    }

    public void Start()
    {
        shell.Start();
    }

    public static void RodarShell()
    {
        int program;

        Console.WriteLine("------- LISTA DE PROGRAMAS -------\n");
        Console.WriteLine("1- 10 primeiros números da sequência de Fibonacci");
        Console.WriteLine("2- Quantidade de números da sequência de Fibonacci que deseja calcular");
        Console.WriteLine("3- Calcular fatorial");
        Console.WriteLine("4- Bubble sort para ordenar 5 valores\n");
        
        Console.WriteLine("5 - Acessar fila de IO");

        do
        {
            Console.WriteLine("Digite o número do programa que deseja executar:");
            program = Convert.ToInt32(Console.ReadLine());

            if (program == 1 || program == 2 || program == 3 || program == 4)
            {
                GerenteProcesso.LoadProgram(program);
            }
            else
            {
                Console.WriteLine("Programa não existe.");
            }
        }
        while (program != 0);

        //TimeSliceExecution();
    }



    //   public void TimeSliceExecution()
    //   {
    //     //Queue <ProcessControlBlock> fila = new Queue <ProcessControlBlock>();
    //     Queue<ProcessControlBlock> fila = GerenteProcesso.Fila;
    //     ProcessControlBlock pcb;

    //     Console.WriteLine("\nInicio da execução em time-slice");
    //     Console.WriteLine("---------------------------------");

    //     // retira um de cada vez da fila e roda na CPU de acordo com a fatia de tempo
    //     while (fila.Count != 0)
    //     {
    //       pcb = fila.Dequeue();
    //       cpu.NewCPU(pcb);

    //       if (pcb.State == State.WAITING)
    //       {
    //         fila.Enqueue(pcb);
    //       }

    //       Console.WriteLine($"Process Id: {pcb.ProcessID} ; State: {pcb.State}\n");
    //     }

    //     // no final de tudo é printado o valor dos registradores dos PCBs e
    //     // as devidas posições de memórias ocupadas
    //     for (int i = 0; i < GerenteProcesso.PCBs.Length; i++)
    //     {
    //       pcb = GerenteProcesso.PCBs[i];

    //       if (pcb != null)
    //       {
    //         PrintRegistradoresEMemoria(pcb);
    //       }
    //     }
    //   }

    // ve se da pra trocar o length por NumeroParticoes (static)


    public void PrintRegistradoresEMemoria(ProcessControlBlock pcb)
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