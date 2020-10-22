using System;

namespace Modelos
{
    public class ParticaoMemoria
    {
        public ParticaoMemoria()
        {
            Status = Status.DESALOCADO;
        }

        public Status Status { get; set; }
    }

    public enum Status
    {
        ALOCADO,
        DESALOCADO
    }
}