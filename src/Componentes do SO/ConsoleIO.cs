using System;
using Modelos;
using FilasProcesso;
using RotinaTratamento;

namespace ComponentesSO
{
    public class ConsoleIO
    {
        public static void ExecutarConsoleIO()
        {
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
                    Console.WriteLine($"O processo [{pedido.ProcessID}] solicitou o " +
                                      $"valor no endereço de memória [{endereco}] e encontrou o valor [{value}]");
                }

                RotinaTratamentoRetornoIO.TratarRetornoIO(pedido.ProcessID);
            }

            Console.WriteLine("Não há pedidos de IO para serem processados");
            //Liberando shell para voltar a executar
            SistemaOperacional.semaforoShell.Release();

        }
    }
}