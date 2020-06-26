using System;
using System.Threading;
public class ConsoleIO
{
    public static void ExecutarConsoleIO()
    {
        Console.WriteLine("Entrei no console de pedidos antes do shell wait");

        if (FilaPedidosConsole.ContarProcessos() != 0)
        {
            PedidoConsoleIO pedido = FilaPedidosConsole.DequeuePedido();

            int endereco = pedido.Endereco;

            if (pedido.IOOperation == IOType.READ)
            {
                Console.WriteLine($"O processo [{pedido.ProcessID}] está solicitando leitura de um valor que será salvo na posição de memória [{endereco}]\n");
                Console.WriteLine("Digite o valor para ser salvo:");

                int value = Convert.ToInt32(Console.ReadLine());

                GerenteDeMemoria.Memoria[endereco] = new PosicaoDeMemoria
                {
                    OPCode = "DATA",
                    Parameter = value
                };

                Console.WriteLine("Valor salvo com sucesso na memória!");
            }
            else
            {
                int value = GerenteDeMemoria.Memoria[endereco].Parameter;
                Console.WriteLine($"O processo [{pedido.ProcessID}] está solicitando o " +
                                  $"valor no endereço de memória [{endereco}] e encontrou o valor [{value}]");
            }
        }

        Console.WriteLine("Não há pedidos de IO para serem processados");
        //Liberando shell para voltar a executar
        SistemaOperacional.semaforoShell.Release();

    }
}