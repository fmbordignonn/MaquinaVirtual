using System;
using System.Collections.Generic;
using Modelos;

namespace FilasProcesso
{
    public static class FilaPedidosConsole
    {
        private static Queue<PedidoConsoleIO> FilaPedidos { get; set; }

        static FilaPedidosConsole()
        {
            FilaPedidos = new Queue<PedidoConsoleIO>();
        }

        public static void AddPedidoIO(PedidoConsoleIO pedido)
        {
            FilaPedidos.Enqueue(pedido);

            Console.WriteLine($"\nAdicionou o processo {pedido.ProcessID} a fila de pedidos do console requisitando IO tipo [{pedido.IOOperation.ToString()}]");
        }

        public static PedidoConsoleIO DequeuePedido()
        {
            return FilaPedidos.Dequeue();
        }

        public static int ContarProcessos()
        {
            return FilaPedidos.Count;
        }

        public static void PrintFilaPedidosConsole()
        {
            Console.WriteLine("Printando fila de pedidos do console:\n");

            foreach (var pedido in FilaPedidos)
            {
                Console.WriteLine($"Process Id: {pedido.ProcessID} | Operation: {pedido.IOOperation.ToString()} | Endereço solicitado: {pedido.Endereco}");
            }
        }
    }
}