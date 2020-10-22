using System;
using System.Threading;

namespace ComponentesSO
{
    public class TimerCPU
    {
        private const int LIMITE_DE_COMANDOS = 30;

        public static bool VerificaFatiaDeTempo(int contadorComandos)
        {
            return contadorComandos == LIMITE_DE_COMANDOS;
        }
    }
}