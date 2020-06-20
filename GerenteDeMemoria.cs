using System;
using System.IO;
using System.Linq;

public class GerenteDeMemoria
{
    private const int TAMANHO_MAXIMO_POSICOES_DE_MEMORIA = 1024;

    private const int TAMANHO_MINIMO_PARTICOES_PERMITIDO = 4;

    private const int TAMANHO_MAXIMO_PARTICOES_PERMITIDO = 8;

    public static ParticaoMemoria[] Particoes { get; set; }

    public static int NumeroParticoes { get; set; }

    private int ParticoesAlocadas { get; set; }

    public static PosicaoDeMemoria[] Memoria { get; set; }

    public GerenteDeMemoria(int numeroParticoes)
    {
        if (numeroParticoes % 2 != 0)
        {
            throw new ArgumentException("Não é possivel criar um número impar de partições, use apenas números 2^n. Encerrando execução");
        }

        if (numeroParticoes > TAMANHO_MAXIMO_PARTICOES_PERMITIDO)
        {
            throw new ArgumentException($"O tamanho máximo de partições permitido para uma CPU é de [{TAMANHO_MAXIMO_PARTICOES_PERMITIDO}]. Encerrando execução.");
        }

        if (numeroParticoes < TAMANHO_MINIMO_PARTICOES_PERMITIDO)
        {
            throw new ArgumentException($"O tamanho mínimo de partições permitido para uma CPU é de [{TAMANHO_MINIMO_PARTICOES_PERMITIDO}]. Encerrando execução.");
        }

        NumeroParticoes = numeroParticoes;
        ParticoesAlocadas = 0;

        Particoes = new ParticaoMemoria[numeroParticoes];
        Memoria = new PosicaoDeMemoria[TAMANHO_MAXIMO_POSICOES_DE_MEMORIA];

        for (int i = 0; i < numeroParticoes; i++)
        {
            Particoes[i] = new ParticaoMemoria();
        }
    }

    public bool ParticaoEstaLivre(int particao)
    {
        return Particoes[particao].Status == Status.DESALOCADO;
    }

    public bool MemoriaCheia()
    {
        return Particoes.All(x => x.Status == Status.ALOCADO);
    }

    public static int CalculaOffset(int particao)
    {
        return particao * (Memoria.Length / NumeroParticoes);
    }

    public static int CalculaEnderecoMax(int particao)
    {
        // Como o array de partiçoes inicia em 0, se nao realizarmos a operação ++ o calculo de boundsRegister ia pegar o endereço mínimo ao invés do máximo,
        // Exemplo, caso nao tivesse esse comando abaixo, na partição 1 (segunda do array) o calculo iria retornar 127, ao invés de 255, que é realmente o endereço maximo dessa
        // partição em específico
        particao++;

        int boundsRegister = particao * (Memoria.Length / NumeroParticoes) - 1;
        return boundsRegister;
    }

    public static int CalculaEnderecoMemoria(ProcessControlBlock pcb, int endereco)
    {
        int offset = pcb.OffSet;

        int enderecoCorrigido = offset + endereco;

        int maximumBound = pcb.EnderecoLimite;

        if (enderecoCorrigido > maximumBound)
        {
            throw new AcessoIndevidoException($"SEGMENTATION FAULT, o endereço fornecido {enderecoCorrigido} está fora do limite da partição que é {maximumBound}");
        }

        return enderecoCorrigido;
    }

    public void ReadFile(string filePath, int particao)
    {
        string[] fileContent = File.ReadAllLines(filePath);

        int offsetParticao = CalculaOffset(particao);

        for (int i = 0; i < fileContent.Length; i++)
        {
            string[] dataContent = fileContent[i].Split(' ');

            string command = dataContent[0];

            if (command == "STOP")
            {
                Memoria[i + offsetParticao] = new PosicaoDeMemoria
                {
                    OPCode = "STOP"
                };

                continue;
            }

            if (command == "SWAP")
            {
                Memoria[i + offsetParticao] = new PosicaoDeMemoria
                {
                    OPCode = "SWAP"
                };

                continue;
            }

            string[] parameters = dataContent[1].Replace(" ", "").Split(',');

            if (command == "JMP" || command == "JMPI")
            {
                Memoria[i + offsetParticao] = new PosicaoDeMemoria
                {
                    OPCode = command,
                    Reg1 = parameters[0].Contains("r") ? parameters[0] : null,
                    Parameter = Int32.TryParse(parameters[0], out int value) ? value : 0
                };
            }

            else
            {
                Memoria[i + offsetParticao] = new PosicaoDeMemoria
                {
                    OPCode = command,
                    Reg1 = parameters[0].Contains("r") || parameters[0].Contains("[") ? parameters[0] : null,
                    Reg2 = parameters[1].Contains("r") || parameters[1].Contains("[") ? parameters[1] : null,
                    Parameter = Int32.TryParse(parameters[1], out int value) ? value : 0
                };
            }
        }
        // indicando que a partição já está alocada
        Particoes[particao].Status = Status.ALOCADO;
        ParticoesAlocadas++;
    }

    public static void DesalocarParticao(int particao, int offset, int enderecoLimite)
    {
        Particoes[particao].Status = Status.DESALOCADO;
        
        for (int i = offset; i <= enderecoLimite; i++)
        {
            Memoria[offset] = new PosicaoDeMemoria();
        }
    }
}