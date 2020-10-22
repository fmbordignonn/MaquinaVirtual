using System;
using Modelos;
using FilasProcesso;

namespace ComponentesSO
{
    public class GerenteDeProcesso
    {
        private const int TAMANHO_MINIMO_PROCESSOS_PERMITIDO = 4;

        private const int TAMANHO_MAXIMO_PROCESSOS_PERMITIDO = 8;
    
        public GerenteDeMemoria GerenteMemoria { get; set; }

        private int count = 0; 

        public GerenteDeProcesso(int numeroParticoes)
        {
            if (numeroParticoes > TAMANHO_MAXIMO_PROCESSOS_PERMITIDO)
            {
                throw new ArgumentException($"O tamanho máximo de partições permitido para uma CPU é de [{TAMANHO_MAXIMO_PROCESSOS_PERMITIDO}]. Encerrando execução.");
            }

            if (numeroParticoes < TAMANHO_MINIMO_PROCESSOS_PERMITIDO)
            {
                throw new ArgumentException($"O tamanho mínimo de partições permitido para uma CPU é de [{TAMANHO_MINIMO_PROCESSOS_PERMITIDO}]. Encerrando execução.");
            }

            GerenteMemoria = new GerenteDeMemoria(numeroParticoes);
        }

        public void LoadProgram(int programNumber)
        {
            string filePath = Environment.CurrentDirectory + @"\assemblyPrograms\P" + programNumber + ".txt";

            int particao = ParticaoAleatoria();

            GerenteMemoria.ReadFile(filePath, particao);

            count++;
            string processID = "P" + count;

            // dps de carregar na memoria, cria PCB do processo na mesma posiçao da partição
            CreatePCB(processID, particao);            
        }

        public int ParticaoAleatoria()
        {
            if(GerenteMemoria.MemoriaCheia())
            {
                throw new StackOverflowException("Impossível alocar mais processos na memória pois todas partições já estão alocadas. Encerrando execução da VM");
            }

            Random r = new Random();

            int particaoAleatoria = r.Next(0, GerenteDeMemoria.NumeroParticoes);

            // enquanto a partição aleatoria estiver ocupada, procurar uma próxima aleatoria
            while (!GerenteMemoria.ParticaoEstaLivre(particaoAleatoria))
            {
                particaoAleatoria = r.Next(0, GerenteDeMemoria.NumeroParticoes);
            }

            return particaoAleatoria;
        }

        public void CreatePCB(string processID, int particao)
        {
            ProcessControlBlock pcb = new ProcessControlBlock(processID, particao);

            int offSet;
            int enderecoMax;

            offSet = GerenteDeMemoria.CalculaOffset(particao);
            pcb.Pc = offSet;
            pcb.OffSet = offSet;

            enderecoMax = GerenteDeMemoria.CalculaEnderecoMax(particao);
            pcb.EnderecoLimite = enderecoMax;

            FilaDeProntos.AddProcess(pcb);
        }
    }
}