using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;

public class CPU
{
    private const int FATIA_DE_TEMPO = 10;

    public GerenteDeMemoria GerenteMemoria { get; set; }

    public ProcessControlBlock [] ProcessControlBlocks { get; set; }

    public Queue <ProcessControlBlock> fila;

    public CPU(int numeroParticoes)
    {
        GerenteMemoria = new GerenteDeMemoria(numeroParticoes);
        ProcessControlBlocks = new ProcessControlBlock[numeroParticoes]; 

        LoadProgramsAndCreatePCBs();
    }

    public void LoadProgramsAndCreatePCBs()
    {
        string filePath;
        Random r = new Random();

        int offSet;
        int enderecoMax;
        int particaoAleatoria;

        // de 1 a 5 pq to usando o i pra pegar todos os 4 txts
        for (int i = 1; i < 5; i++)
        {
            filePath = Environment.CurrentDirectory + @"\programs\P";

            particaoAleatoria = r.Next(0, GerenteMemoria.Particoes.Length);

            // enquanto a partição aleatoria nao estiver livre, procurar uma próxima aleatoria
            while (!GerenteMemoria.ParticaoEstaLivre(particaoAleatoria))
            {
                particaoAleatoria = r.Next(0, GerenteMemoria.Particoes.Length);
            }

            filePath = filePath + i + ".txt";

            GerenteMemoria.ReadFile(filePath, particaoAleatoria);

            // dps de carregar na memoria, cria PCB do processo na mesma posiçao da partição
            // calcula o offset e o endereço maximo da partição e salva no PCB
            ProcessControlBlocks[particaoAleatoria] = new ProcessControlBlock("Programa " + i);

            offSet = GerenteMemoria.CalculaOffset(particaoAleatoria);
            ProcessControlBlocks[particaoAleatoria].Pc = offSet;

            ProcessControlBlocks[particaoAleatoria].OffSet = offSet;

            enderecoMax = GerenteMemoria.CalculaEnderecoMax(particaoAleatoria);
            ProcessControlBlocks[particaoAleatoria].EnderecoLimite = enderecoMax;
        }

        CreateProcessQueue();
    }

    public void CreateProcessQueue()
    {
        fila = new Queue <ProcessControlBlock>();
        ProcessControlBlock pcb;

        Console.WriteLine("Fila de execução dos processos");
        Console.WriteLine("---------------------------------");

        // poe na fila todos os PCBs e printa suas informações
        for (int i = 0; i < ProcessControlBlocks.Length; i++)
        {
            pcb = ProcessControlBlocks[i];

            if (pcb != null)
            {
                fila.Enqueue(pcb);
                Console.WriteLine($"Process Id: {pcb.ProcessId} | State: {pcb.State} | Offset: {pcb.OffSet} | EndereçoLimite: {pcb.EnderecoLimite}");
            }
        }

        Console.WriteLine("\nInicio do execução de time-slice");
        Console.WriteLine("---------------------------------");

        // retira um de cada vez da fila e roda na CPU de acordo com a fatia de tempo
        while (fila.Count != 0)
        {
            pcb = fila.Dequeue();
            CPUWithProcessControlBlock(pcb);

            if (pcb.State == State.WAITING)
            {
                fila.Enqueue(pcb);
            }

            Console.WriteLine($"Process Id: {pcb.ProcessId} ; State: {pcb.State}\n");
        }

        // no final de tudo é printado o valor dos registradores dos PCBs e
        // as devidas posições de memórias ocupadas
         for (int i = 0; i < ProcessControlBlocks.Length; i++)
        {
            pcb = ProcessControlBlocks[i];

            if (pcb != null)
            {
                PrintRegistradoresEMemoria(pcb);
            }
        }
    }

    public void CPUWithProcessControlBlock(ProcessControlBlock pcb)
    {
        pcb.State = State.RUNNING;
        int ComandsCount = 0;

        // só pra mostrar que ta RUNNING
        Console.WriteLine($"Process Id: {pcb.ProcessId} ; State: {pcb.State}");
        
        string value = string.Empty;
        bool logicalResult = false;
        int memoryPosition = 0;

        PosicaoDeMemoria currentLine = GerenteMemoria.Memoria[pcb.Pc];


        while (currentLine.OPCode != "STOP" && ComandsCount != FATIA_DE_TEMPO)
        {
            currentLine = GerenteMemoria.Memoria[pcb.Pc];
            ComandsCount++;

            switch (currentLine.OPCode)
            {
                // faz PC pular direto pra uma linha k
                // JMP 12
                case "JMP":
                    pcb.Pc = GerenteMemoria.CalculaEnderecoMemoria(pcb, Convert.ToInt32(currentLine.Parameter));
                    break;

                // faz PC pular direto pra linha contida no registrador r
                // JMPI r1
                case "JMPI":
                    pcb.Pc = GerenteMemoria.CalculaEnderecoMemoria(pcb, Convert.ToInt32(pcb.Registradores[currentLine.Reg1]));
                    break;

                // faz PC pular direto pra linha contida no registrador rx, caso ry seja maior que 0
                // JMPIG rx,ry
                case "JMPIG":
                    if (Convert.ToInt32(pcb.Registradores[currentLine.Reg2]) > 0)
                    {
                        pcb.Pc = GerenteMemoria.CalculaEnderecoMemoria(pcb, Convert.ToInt32(pcb.Registradores[currentLine.Reg1]));
                        currentLine = GerenteMemoria.Memoria[pcb.Pc];
                    }
                    else
                    {
                        pcb.Pc++;
                        currentLine = GerenteMemoria.Memoria[pcb.Pc];
                    }
                    break;

                // faz PC pular direto pra linha contida no registrador rx, caso ry seja menor que 0
                // JMPIL rx,ry
                case "JMPIL":
                    if (Convert.ToInt32(pcb.Registradores[currentLine.Reg2]) < 0)
                    {
                        pcb.Pc = GerenteMemoria.CalculaEnderecoMemoria(pcb, Convert.ToInt32(pcb.Registradores[currentLine.Reg1]));
                        currentLine = GerenteMemoria.Memoria[pcb.Pc];
                    }
                    else
                    {
                        pcb.Pc++;
                        currentLine = GerenteMemoria.Memoria[pcb.Pc];
                    }
                    break;

                // faz PC pular direto pra linha contida no registrador rx, caso ry igual a 0
                // JMPIE rx,ry
                case "JMPIE":
                    if (Convert.ToInt32(pcb.Registradores[currentLine.Reg2]) == 0)
                    {
                        pcb.Pc = GerenteMemoria.CalculaEnderecoMemoria(pcb, Convert.ToInt32(pcb.Registradores[currentLine.Reg1]));
                        currentLine = GerenteMemoria.Memoria[pcb.Pc];
                    }
                    else
                    {
                        pcb.Pc++;
                        currentLine = GerenteMemoria.Memoria[pcb.Pc];
                    }
                    break;

                // realiza a soma imediata de um valor k no registrador r
                //ADDI r1,1
                case "ADDI":
                    pcb.Registradores[currentLine.Reg1] += currentLine.Parameter;

                    pcb.Pc++;
                    break;

                // realiza a subtração imediata de um valor k no registrador r
                //SUBI r1,1
                case "SUBI":
                    pcb.Registradores[currentLine.Reg1] -= currentLine.Parameter;

                    pcb.Pc++;
                    currentLine = GerenteMemoria.Memoria[pcb.Pc];
                    break;

                // carrega um valor k em um registrador
                // LDI r1,10
                case "LDI":
                    pcb.Registradores[currentLine.Reg1] = currentLine.Parameter;

                    pcb.Pc++;
                    currentLine = GerenteMemoria.Memoria[pcb.Pc];
                    break;

                // carrega um valor da memoria em um registrador
                // LDD r1,[50]
                case "LDD":
                    value = currentLine.Reg2.Trim(new char[] { '[', ']' });
                    memoryPosition = GerenteMemoria.CalculaEnderecoMemoria(pcb, Convert.ToInt32(value));

                    if (GerenteMemoria.Memoria[memoryPosition].OPCode == "DATA")
                    {
                        pcb.Registradores[currentLine.Reg1] = GerenteMemoria.Memoria[memoryPosition].Parameter;
                    }
                    else
                    {
                        throw new InvalidOperationException("Não é possivel ler dados de uma posição de memória com OPCode diferente de [DATA]. Encerrando execução");
                    }

                    pcb.Pc++;
                    currentLine = GerenteMemoria.Memoria[pcb.Pc];
                    break;

                // guarda na memoria um valor contido no registrador r
                // STD [52],r1
                case "STD":
                    value = currentLine.Reg1.Trim(new char[] { '[', ']' });
                    memoryPosition = GerenteMemoria.CalculaEnderecoMemoria(pcb, Convert.ToInt32(value));

                    GerenteMemoria.Memoria[memoryPosition] = new PosicaoDeMemoria
                    {
                        OPCode = "DATA",
                        Parameter = pcb.Registradores[currentLine.Reg2]
                    };

                    pcb.Pc++;
                    currentLine = GerenteMemoria.Memoria[pcb.Pc];
                    break;

                // faz a operaçao: rx = rx + ry
                // ADD rx,ry
                case "ADD":
                    pcb.Registradores[currentLine.Reg1] += pcb.Registradores[currentLine.Reg2];

                    pcb.Pc++;
                    break;

                // faz a operaçao: rx = rx - ry
                // SUB rx,ry
                case "SUB":
                    pcb.Registradores[currentLine.Reg1] -= pcb.Registradores[currentLine.Reg2];

                    pcb.Pc++;
                    break;

                // faz a operaçao: rx = rx * ry
                // MULT rx,ry
                case "MULT":
                    pcb.Registradores[currentLine.Reg1] *= pcb.Registradores[currentLine.Reg2];

                    pcb.Pc++;
                    currentLine = GerenteMemoria.Memoria[pcb.Pc];
                    break;

                // faz a operaçao: rx = rx AND k
                // AND rx,k
                case "ANDI":
                    if (new int[] { 0, 1 }.Contains(pcb.Registradores[currentLine.Reg1]))
                    {
                        logicalResult = Convert.ToBoolean(currentLine.Parameter) &&
                                        Convert.ToBoolean(pcb.Registradores[currentLine.Reg1]);

                        pcb.Registradores[currentLine.Reg1] = Convert.ToInt32(logicalResult);
                    }
                    else
                    {
                        throw new ArgumentException("O registrador não contém um valor lógico válido");
                    }

                    pcb.Pc++;
                    break;

                // faz a operaçao: rx = rx OR k
                // OR rx,k
                case "ORI":
                    if (new int[] { 0, 1 }.Contains(pcb.Registradores[currentLine.Reg1]))
                    {
                        logicalResult = Convert.ToBoolean(currentLine.Parameter) ||
                                        Convert.ToBoolean(pcb.Registradores[currentLine.Reg1]);

                        pcb.Registradores[currentLine.Reg1] = Convert.ToInt32(logicalResult);
                    }
                    else
                    {
                        throw new ArgumentException("O registrador não contém um valor lógico válido");
                    }

                    pcb.Pc++;
                    break;

                // carrega em rx o dado contido na posiçao de memoria indicada por ry
                // LDX rx,[ry]
                case "LDX":
                    value = currentLine.Reg2.Trim(new char[] { '[', ']' });
                    memoryPosition = GerenteMemoria.CalculaEnderecoMemoria(pcb, pcb.Registradores[value]);

                    pcb.Registradores[currentLine.Reg1] = GerenteMemoria.Memoria[memoryPosition].Parameter;

                    pcb.Pc++;
                    break;

                // guarda na posição de memoria rx o dado contido em ry
                // STX [rx],ry
                case "STX":
                    value = currentLine.Reg1.Trim(new char[] { '[', ']' });
                    memoryPosition = GerenteMemoria.CalculaEnderecoMemoria(pcb, pcb.Registradores[value]);

                    GerenteMemoria.Memoria[memoryPosition] = new PosicaoDeMemoria
                    {
                        OPCode = "DATA",
                        Parameter = pcb.Registradores[currentLine.Reg2]
                    };
                       
                    pcb.Pc++;
                    break;

                // troca os valores dos registradores; r7←r3, r6←r2, r5←r1, r4←r0
                // SWAP  
                case "SWAP":
                    pcb.Registradores["r4"] = pcb.Registradores["r0"];

                    pcb.Registradores["r5"] = pcb.Registradores["r1"];

                    pcb.Registradores["r6"] = pcb.Registradores["r2"];

                    pcb.Registradores["r7"] = pcb.Registradores["r3"];
                        
                    pcb.Pc++;
                    break;

                // todos outros q tem tbm n precisa, sao bitwise operators, ainda n chegamo lá
                default:
                    throw new ArgumentException($"Não foi possível encontrar o comando [{currentLine.OPCode}]");
            }
        }

        if (currentLine.OPCode == "STOP")
        {
            pcb.State = State.FINISHED;
        }
        else
        {
            pcb.State = State.WAITING;
        }
    }

    public void PrintRegistradoresEMemoria(ProcessControlBlock pcb)
    {
        Console.WriteLine("--------------------------------------------");
        Console.WriteLine($"DADOS DO PROCESSO: {pcb.ProcessId}\n");
        
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
            if (GerenteMemoria.Memoria[i] == null)
            {
                continue;
            }
            
            Console.WriteLine($"Posição de memória [{i}]:");
            Console.WriteLine(GerenteMemoria.Memoria[i].ToString() + "\n");
        }
    }
}