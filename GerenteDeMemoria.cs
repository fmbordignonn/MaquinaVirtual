using System;
using System.IO;

public class GerenteDeMemoria
{
    private const int TAMANHO_MAXIMO_POSICOES_DE_MEMORIA = 1024;

    private const int TAMANHO_MAXIMO_PARTICOES_PERMITIDO = 8;

    public ParticaoMemoria[] Particoes { get; set; }

    public PosicaoDeMemoria[] Memoria { get; set; }

    public GerenteDeMemoria(int numeroParticoes)
    {
        if (numeroParticoes % 2 != 0) //talllvez isso nao esteja tao certo, ver dps
        {
            throw new ArgumentException("Não é possivel criar um número impar de partições, use apenas números 2^n. Encerrando execução");
        }

        if (numeroParticoes > TAMANHO_MAXIMO_PARTICOES_PERMITIDO)
        {
            throw new ArgumentException($"O tamanho máximo de partições permitido apra uma CPU é de [{TAMANHO_MAXIMO_PARTICOES_PERMITIDO}]. Encerrando execução.");
        }

        Particoes = new ParticaoMemoria[numeroParticoes];
        Memoria = new PosicaoDeMemoria[TAMANHO_MAXIMO_POSICOES_DE_MEMORIA];

        for (int i = 0; i < numeroParticoes; i++)
        {
            Particoes[i] = new ParticaoMemoria();
        }

        // // LDI 50
        // var ss = CalculaEnderecoMemoria(2, 50);

        // var PP = CalculaEnderecoMax(1);
    }



    public int CalculaOffset(int particao)
    {
        return particao * (Memoria.Length / Particoes.Length);
    }

    //LDI 50
    public int CalculaEnderecoMemoria(int particao, int endereco)
    {
        int offset = CalculaOffset(particao);

        // ver se ta certo pois array inicia em 0
        int enderecoCorrigido = offset + endereco;

        int maximumBound = CalculaEnderecoMax(particao);

        if (enderecoCorrigido > maximumBound)
        {
            throw new IndexOutOfRangeException($"SEGMENTATION FAULT, o endereço fornecido está fora do limite da partição {particao}");
        }

        // avaliar se precisa ver endereço de memoria negativo, LDI 129 - nao precisa

        return enderecoCorrigido;
    }

    private int CalculaEnderecoMax(int particao)
    {
        // Como o array de partiçoes inicia em 0, se nao realizarmos a operação ++ o calculo de boundsRegister ia pegar o endereço mínimo ao invés do máximo,
        // Exemplo, caso nao tivesse esse comando abaixo, na partição 1 (segunda do array) o calculo iria retornar 127, ao invés de 255, que é realmente o endereço maximo dessa
        // partição em específico
        particao++;

        // ve se precisa do -1
        int boundsRegister = particao * (Memoria.Length / Particoes.Length) - 1;
        return boundsRegister;
    }

    public bool ParticaoEstaLivre(int particao)
    {
        return Particoes[particao].Status == Status.DESALOCADO;
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

            string[] parameters = dataContent[1].Replace(" ", "").Split(',');

            //avaliar swap


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
    }
}