using System;

namespace Modelos
{
    public class PedidoConsoleIO
    {
        public string ProcessID { get; set; }

        public IOType IOOperation { get; set; }

        public int Endereco { get; set; }

        public string Value { get; set; }

        public PedidoConsoleIO(string processID, IOType operation, int endereco)
        {
            this.ProcessID = processID;
            this.IOOperation = operation;
            this.Endereco = endereco;
        }
    }

    public enum IOType
    {
        READ,
        WRITE
    }
}