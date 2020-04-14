using System;
using System.Collections.Generic;
public class SistemaOperacional
{
    public CPU cpu;

    public GerenteDeMemoria GerenteMemoria;

    public GerenteDeProcesso GerenteProcesso;

    public SistemaOperacional(int numeroParticoes)
    {
        cpu = new CPU();

        GerenteMemoria = new GerenteDeMemoria(numeroParticoes);

        GerenteProcesso = new GerenteDeProcesso(numeroParticoes);
    }

    public void Start()
    {
        LoadPrograms();
        GerenteProcesso.CreateProcessQueue();
        TimeSliceExecution();
    }

    public void LoadPrograms()
    {
        string filePath;
        string processID;
        int particao;

        // de 1 a 5 pq to usando o i pra pegar todos os 4 txts
        for (int i = 1; i < 5; i++)
        {
            filePath = Environment.CurrentDirectory + @"\programs\P" + i + ".txt";

            particao = ParticaoAleatoria();

            GerenteMemoria.ReadFile(filePath, particao);

            processID = "Programa " + i;

            // dps de carregar na memoria, cria PCB do processo na mesma posiçao da partição
            GerenteProcesso.CreatePCB(processID, particao);            
        }
    }

    public void TimeSliceExecution()
    {
        //Queue <ProcessControlBlock> fila = new Queue <ProcessControlBlock>();
        Queue <ProcessControlBlock> fila = GerenteProcesso.fila;
        ProcessControlBlock pcb;

        Console.WriteLine("\nInicio do execução de time-slice");
        Console.WriteLine("---------------------------------");

        // retira um de cada vez da fila e roda na CPU de acordo com a fatia de tempo
        while (fila.Count != 0)
        {
            pcb = fila.Dequeue();
            cpu.NewCPU(pcb);

            if (pcb.State == State.WAITING)
            {
                fila.Enqueue(pcb);
            }

            Console.WriteLine($"Process Id: {pcb.ProcessID} ; State: {pcb.State}\n");
        }

        // no final de tudo é printado o valor dos registradores dos PCBs e
        // as devidas posições de memórias ocupadas
         for (int i = 0; i < GerenteProcesso.PCBs.Length; i++)
        {
            pcb = GerenteProcesso.PCBs[i];

            if (pcb != null)
            {
                PrintRegistradoresEMemoria(pcb);
            }
        }
    }

    // ve se da pra trocar o length por NumeroParticoes (static)
    public int ParticaoAleatoria()
    {
        Random r = new Random();

        int particaoAleatoria = r.Next(0, GerenteDeMemoria.NumeroParticoes);

            // enquanto a partição aleatoria estiver ocupada, procurar uma próxima aleatoria
            while (!GerenteMemoria.ParticaoEstaLivre(particaoAleatoria))
            {
                particaoAleatoria = r.Next(0, GerenteDeMemoria.NumeroParticoes);
            }

        return particaoAleatoria;
    }

    public void PrintRegistradoresEMemoria(ProcessControlBlock pcb)
    {
        Console.WriteLine("--------------------------------------------");
        Console.WriteLine($"DADOS DO PROCESSO: {pcb.ProcessID}\n");
        
        Console.WriteLine("Valores finais dos registradores:");

        foreach (var item in pcb.Registradores)
        {
            Console.WriteLine("--------------------------------------------\n");

            Console.WriteLine($"Registrador [{item.Key}] - valor final [{item.Value}]\n");
        }

        Console.WriteLine("--------------------------------------------\n");

        Console.WriteLine("Status finais das posições de memória (não nulas) da particao\n");

        for (int i = pcb.OffSet; i < pcb.EnderecoLimite; i++)
        {
            if (GerenteDeMemoria.Memoria[i] == null)
            {
                continue;
            }
            
            Console.WriteLine($"Posição de memória [{i}]:");
            Console.WriteLine(GerenteDeMemoria.Memoria[i].ToString() + "\n");
        }
    }
}