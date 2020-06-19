using System;
using System.Collections.Generic;
using System.Threading;

public class SistemaOperacional
{
    public CPU cpu;

    public static GerenteDeProcesso GerenteProcesso;

    //Thread Shell
    static Thread shell = new Thread(new ThreadStart(RodarShell));

    //Thread Escalonador
    static Thread escalonador = new Thread(new ThreadStart(Escalonador.Escalona));

    public SistemaOperacional(int numeroParticoes)
    {
        //tem o gerente de memoria dentro
        GerenteProcesso = new GerenteDeProcesso(numeroParticoes);
    }

    public void IniciarExecucao()
    {
        shell.Start();
        escalonador.Start();
    }

    public static void EncerrarExecucao()
    {
        shell.Interrupt();
        escalonador.Interrupt();
    }

    public static void RodarShell()
    {
        int program;
        while(true)
        {
            Console.WriteLine("                LISTA DE PROGRAMAS                ");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("1- 10 primeiros números da sequência de Fibonacci");
            Console.WriteLine("2- Quantidade de números da sequência de Fibonacci que deseja calcular");
            Console.WriteLine("3- Calcular fatorial");
            Console.WriteLine("4- Bubble sort para ordenar 5 valores");
            Console.WriteLine("--------------------------------------------------");

            Console.WriteLine("5 - Acessar fila de IO");
            Console.WriteLine("6 - Printar fila prontos");
            Console.WriteLine("0 - Shutdown\n");


            Console.WriteLine("Digite o número do programa que deseja executar:");
            program = Convert.ToInt32(Console.ReadLine());

            switch(program)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    GerenteProcesso.LoadProgram(program);
                    Thread.Sleep(100);
                    break;

                case 5:
                    // Acessa a fila de IO
                    break;

                case 6:
                    FilaDeProntos.PrintFilaDeProntos();
                    break;

                case 0:
                    EncerrarExecucao();
                    return;

                default:
                    Console.WriteLine("Programa não existe.");
                    break;
            }
        }
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