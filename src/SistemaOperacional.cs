using System;
using System.Threading;
using ComponentesSO;
using FilasProcesso;

public class SistemaOperacional
{
    public static GerenteDeProcesso GerenteProcesso;

    //Thread Shell
    public static Thread shell = new Thread(new ThreadStart(RodarShell));

    //Thread Escalonador
    public static Thread escalonador = new Thread(new ThreadStart(Escalonador.Escalona));

    public static Thread consoleIO;

    //Semáforo Shell
    public static Semaphore semaforoShell = new Semaphore(0, 1);

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

    public static void RodarShell()
    {
        int program;
        while (true)
        {
            Console.WriteLine("                LISTA DE PROGRAMAS                ");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("1- 10 primeiros números da sequência de Fibonacci");
            Console.WriteLine("2- Quantidade de números da sequência de Fibonacci que deseja calcular");
            Console.WriteLine("3- Calcular fatorial");
            Console.WriteLine("4- Bubble sort para ordenar 5 valores");
            Console.WriteLine("5- Programa para testar acesso indevido");
            Console.WriteLine("--------------------------------------------------");

            Console.WriteLine("6 - Acessar fila de IO");
            Console.WriteLine("7 - Printar fila prontos");
            Console.WriteLine("8 - Printar fila bloqueados por IO");
            Console.WriteLine("9 - Printar fila de processos finalizados");
            Console.WriteLine("0 - Shutdown\n");


            Console.WriteLine("Digite o número do programa que deseja executar:");

            try
            {
                program = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Digite uma opção válida");
                continue;
            }

            Console.WriteLine("--------------------------------------------");

            switch (program)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    GerenteProcesso.LoadProgram(program);
                    Thread.Sleep(100);
                    break;

                case 6:
                    consoleIO = new Thread(new ThreadStart(ConsoleIO.ExecutarConsoleIO));
                    consoleIO.Start();

                    semaforoShell.WaitOne();

                    SistemaOperacional.consoleIO.Interrupt();
                    break;

                case 7:
                    FilaDeProntos.PrintFilaDeProntos();
                    break;

                case 8:
                    FilaBloqueadosIO.PrintFilaDeBloqueados();
                    break;

                case 9:
                    FilaDeFinalizados.PrintFilaDeFinalizados();
                    break;

                case 0:
                    System.Environment.Exit(0);
                    return;

                default:
                    Console.WriteLine("Opção não existe");
                    break;
            }
        }
    }
}