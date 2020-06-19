using System;

public class GerenteDeProcesso
{
    private const int TAMANHO_MINIMO_PROCESSOS_PERMITIDO = 4;

    private const int TAMANHO_MAXIMO_PROCESSOS_PERMITIDO = 8;
    
    public GerenteDeMemoria GerenteMemoria { get; set; }

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
        string filePath = Environment.CurrentDirectory + @"\programs\P" + programNumber + ".txt";

        int particao = ParticaoAleatoria();

        GerenteMemoria.ReadFile(filePath, particao);

        string processID = "Programa " + programNumber;

        // dps de carregar na memoria, cria PCB do processo na mesma posiçao da partição
        CreatePCB(processID, particao);            
    }

      public int ParticaoAleatoria()
    {
        if(GerenteMemoria.MemoriaCheia())
        {
            throw new StackOverflowException("Todas as partições na memória estão alocadas.");
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

    public void CreatePCB(string ProcessID, int particao)
    {
        ProcessControlBlock pcb = new ProcessControlBlock(ProcessID);

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