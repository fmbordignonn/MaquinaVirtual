using System;
using System.Collections.Generic;

public class GerenteDeProcesso
{
    private const int TAMANHO_MINIMO_PROCESSOS_PERMITIDO = 4;

    private const int TAMANHO_MAXIMO_PROCESSOS_PERMITIDO = 8;

    public ProcessControlBlock [] PCBs { get; set; }

    public Queue <ProcessControlBlock> fila { get; set; }

    public GerenteDeProcesso(int numeroProcessos)
    {
        if (numeroProcessos > TAMANHO_MAXIMO_PROCESSOS_PERMITIDO)
        {
            throw new ArgumentException($"O tamanho máximo de partições permitido para uma CPU é de [{TAMANHO_MAXIMO_PROCESSOS_PERMITIDO}]. Encerrando execução.");
        }

        if (numeroProcessos < TAMANHO_MINIMO_PROCESSOS_PERMITIDO)
        {
            throw new ArgumentException($"O tamanho mínimo de partições permitido para uma CPU é de [{TAMANHO_MINIMO_PROCESSOS_PERMITIDO}]. Encerrando execução.");
        }

        PCBs = new ProcessControlBlock[numeroProcessos];
    }

    public void CreatePCB(string ProcessID, int particao)
    {
        PCBs[particao] = new ProcessControlBlock(ProcessID);

        int offSet;
        int enderecoMax;

        offSet = GerenteDeMemoria.CalculaOffset(particao);
        PCBs[particao].Pc = offSet;
        PCBs[particao].OffSet = offSet;

        enderecoMax = GerenteDeMemoria.CalculaEnderecoMax(particao);
        PCBs[particao].EnderecoLimite = enderecoMax;
    }

    public void CreateProcessQueue()
    {   
        fila = new Queue <ProcessControlBlock>();
        ProcessControlBlock pcb;

        Console.WriteLine("Fila de execução dos processos");
        Console.WriteLine("---------------------------------");

        // poe na fila todos os PCBs e printa suas informações
        for (int i = 0; i < PCBs.Length; i++)
        {
            pcb = PCBs[i];

            if (pcb != null)
            {
                fila.Enqueue(pcb);
                Console.WriteLine($"Process Id: {pcb.ProcessID} | State: {pcb.State} | Offset: {pcb.OffSet} | EndereçoLimite: {pcb.EnderecoLimite}");
            }
        }    
    }
}