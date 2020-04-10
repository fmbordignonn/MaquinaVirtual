using System;

public class ParticaoMemoria
{
    public ParticaoMemoria(int numeroParticao, int tamanhoParticao)
    {
        NumeroParticao = numeroParticao;
        TamanhoParticao = tamanhoParticao;
        MemoriaParticao = new PosicaoDeMemoria[tamanhoParticao];

        // avaliar dps se precisa disso mesmo
        for (int i = 0; i < tamanhoParticao; i++)
        {
            MemoriaParticao[i] = new PosicaoDeMemoria();
        }

    }

    public int NumeroParticao { get; set; }

    public int TamanhoParticao { get; set; }

    public PosicaoDeMemoria[] MemoriaParticao { get; set; }

}