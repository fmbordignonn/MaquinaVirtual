using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class CPU
{
    public GerenteDeMemoria GerenteMemoria { get; set; }

    public ProcessControlBLock [] ProcessControlBLocks { get; set; }

    public CPU(int numeroParticoes)
    {
        GerenteMemoria = new GerenteDeMemoria(numeroParticoes);
        ProcessControlBLocks = new ProcessControlBLock[numeroParticoes]; 
    }

    public void loadProgramsAndCreateProcess()
    {
        string filePath;

        // de 1 a 5 pq to usando o i pra pegar todos os 4 txt
        for (int i = 1; i < 5; i++)
        {
            filePath = Environment.CurrentDirectory + @"\programs\P";

            Random r = new Random();
            int particaoAleatoria = 1;//r.Next(0, GerenteMemoria.Particoes.Length);

            // enquanto a partição aleatoria nao estiver livre, procurar uma próxima aleatoria
            while (!GerenteMemoria.ParticaoEstaLivre(particaoAleatoria))
            {
                particaoAleatoria = r.Next(0, GerenteMemoria.Particoes.Length);
            }

            filePath = filePath + i + ".txt";

            GerenteMemoria.ReadFile(filePath, particaoAleatoria);

            // dps de carregar na memoria, cria Pcb do processo na mesma posiçao da partição com id P1,P2,etc
            ProcessControlBLocks [particaoAleatoria] = new ProcessControlBLock("P" + i);
        }
    }
    public void CPUWithProcessControlBlock(ProcessControlBLock pcb)
    {

        GerenteMemoria.LoadProgramsInDifferentPartitions();

        string value = string.Empty;
        bool logicalResult = false;
        int memoryPosition = 0;
        

        //int offsetParticao = GerenteMemoria.CalculaOffset(particaoAleatoria);

        //PC precisa ser a primeira posição da partição em questão que está sendo usada (confirmar depois) 
        //int pc = offsetParticao;
        

        PosicaoDeMemoria currentLine = GerenteMemoria.Memoria[pc];

        while (currentLine.OPCode != "STOP")
        {
            currentLine = GerenteMemoria.Memoria[pc];

            switch (currentLine.OPCode)
            {
                // faz PC pular direto pra uma linha k
                // JMP 12
                case "JMP":
                    pc = GerenteMemoria.CalculaEnderecoMemoria(particaoAleatoria, Convert.ToInt32(currentLine.Parameter));
                    break;

                // faz PC pular direto pra linha contida no registrador r
                // JMPI r1
                case "JMPI":
                    pc = GerenteMemoria.CalculaEnderecoMemoria(particaoAleatoria, Convert.ToInt32(registradores[currentLine.Reg1]));
                    break;

                // faz PC pular direto pra linha contida no registrador rx, caso ry seja maior que 0
                // JMPIG rx,ry
                case "JMPIG":
                    if (Convert.ToInt32(registradores[currentLine.Reg2]) > 0)
                    {
                        pc = GerenteMemoria.CalculaEnderecoMemoria(particaoAleatoria, Convert.ToInt32(registradores[currentLine.Reg1]));
                        currentLine = GerenteMemoria.Memoria[pc];
                    }
                    else
                    {
                        pc++;
                        currentLine = GerenteMemoria.Memoria[pc];
                    }
                    break;

                // faz PC pular direto pra linha contida no registrador rx, caso ry seja menor que 0
                // JMPIL rx,ry
                case "JMPIL":
                    if (Convert.ToInt32(registradores[currentLine.Reg2]) < 0)
                    {
                        pc = GerenteMemoria.CalculaEnderecoMemoria(particaoAleatoria, Convert.ToInt32(registradores[currentLine.Reg1]));
                        currentLine = GerenteMemoria.Memoria[pc];
                    }
                    else
                    {
                        pc++;
                        currentLine = GerenteMemoria.Memoria[pc];
                    }
                    break;

                // faz PC pular direto pra linha contida no registrador rx, caso ry igual a 0
                // JMPIE rx,ry
                case "JMPIE":
                    if (Convert.ToInt32(registradores[currentLine.Reg2]) == 0)
                    {
                        pc = GerenteMemoria.CalculaEnderecoMemoria(particaoAleatoria, Convert.ToInt32(registradores[currentLine.Reg1]));
                        currentLine = GerenteMemoria.Memoria[pc];
                    }
                    else
                    {
                        pc++;
                        currentLine = GerenteMemoria.Memoria[pc];
                    }
                    break;

                // realiza a soma imediata de um valor k no registrador r
                //ADDI r1,1
                case "ADDI":
                    registradores[currentLine.Reg1] += currentLine.Parameter;

                    pc++;
                    break;

                // realiza a subtração imediata de um valor k no registrador r
                //SUBI r1,1
                case "SUBI":
                    registradores[currentLine.Reg1] -= currentLine.Parameter;

                    pc++;
                    currentLine = GerenteMemoria.Memoria[pc];
                    break;

                // carrega um valor k em um registrador
                // LDI r1,10
                case "LDI":
                    registradores[currentLine.Reg1] = currentLine.Parameter;

                    pc++;
                    currentLine = GerenteMemoria.Memoria[pc];
                    break;

                // carrega um valor da memoria em um registrador
                // LDD r1,[50]
                case "LDD":
                    value = currentLine.Reg2.Trim(new char[] { '[', ']' });
                    memoryPosition = GerenteMemoria.CalculaEnderecoMemoria(particaoAleatoria, Convert.ToInt32(value));

                    if (GerenteMemoria.Memoria[memoryPosition].OPCode == "DATA")
                    {
                        registradores[currentLine.Reg1] = GerenteMemoria.Memoria[memoryPosition].Parameter;
                    }
                    else
                    {
                        throw new InvalidOperationException("Não é possivel ler dados de uma posição de memória com OPCode diferente de [DATA]. Encerrando execução");
                    }

                    pc++;
                    currentLine = GerenteMemoria.Memoria[pc];
                    break;

                //asdiapiofjasifaf-0--------------------------------------------

                // guarda na memoria um valor contido no registrador r
                // STD [52],r1
                case "STD":
                    value = currentLine.Reg1.Trim(new char[] { '[', ']' });
                    memoryPosition = GerenteMemoria.CalculaEnderecoMemoria(particaoAleatoria, Convert.ToInt32(value));

                    GerenteMemoria.Memoria[memoryPosition] = new PosicaoDeMemoria
                    {
                        OPCode = "DATA",
                        Parameter = registradores[currentLine.Reg2]
                    };

                    pc++;
                    currentLine = GerenteMemoria.Memoria[pc];
                    break;

                // faz a operaçao: rx = rx + ry
                // ADD rx,ry
                case "ADD":
                    registradores[currentLine.Reg1] += registradores[currentLine.Reg2];

                    pc++;
                    break;

                // faz a operaçao: rx = rx - ry
                // SUB rx,ry
                case "SUB":
                    registradores[currentLine.Reg1] -= registradores[currentLine.Reg2];

                    pc++;
                    break;

                // faz a operaçao: rx = rx * ry
                // MULT rx,ry
                case "MULT":
                    registradores[currentLine.Reg1] *= registradores[currentLine.Reg2];

                    pc++;
                    currentLine = GerenteMemoria.Memoria[pc];
                    break;

                // faz a operaçao: rx = rx AND k
                // AND rx,k
                case "ANDI":
                    if (new int[] { 0, 1 }.Contains(registradores[currentLine.Reg1]))
                    {
                        logicalResult = Convert.ToBoolean(currentLine.Parameter) &&
                                        Convert.ToBoolean(registradores[currentLine.Reg1]);

                        registradores[currentLine.Reg1] = Convert.ToInt32(logicalResult);
                    }
                    else
                    {
                        throw new ArgumentException("O registrador não contém um valor lógico válido");
                    }

                    pc++;
                    break;

                // faz a operaçao: rx = rx OR k
                // OR rx,k
                case "ORI":
                    if (new int[] { 0, 1 }.Contains(registradores[currentLine.Reg1]))
                    {
                        logicalResult = Convert.ToBoolean(currentLine.Parameter) ||
                                        Convert.ToBoolean(registradores[currentLine.Reg1]);

                        registradores[currentLine.Reg1] = Convert.ToInt32(logicalResult);
                    }
                    else
                    {
                        throw new ArgumentException("O registrador não contém um valor lógico válido");
                    }

                    pc++;
                    break;

                // carrega em rx o dado contido na posiçao de memoria indicada por ry
                // LDX rx,[ry]
                case "LDX":
                    value = currentLine.Reg2.Trim(new char[] { '[', ']' });
                    memoryPosition = GerenteMemoria.CalculaEnderecoMemoria(particaoAleatoria, registradores[value]);

                    registradores[currentLine.Reg1] = GerenteMemoria.Memoria[memoryPosition].Parameter;

                    pc++;
                    break;

                // guarda na posição de memoria rx o dado contido em ry
                // STX [rx],ry
                case "STX":
                    value = currentLine.Reg1.Trim(new char[] { '[', ']' });
                    memoryPosition = GerenteMemoria.CalculaEnderecoMemoria(particaoAleatoria, registradores[value]);

                    // pelo oq eu entendi esse if checa se a posicao é destinada para guardar dados e garante que nao é codigo
                    // porém tem uma linha no P1 assim:
                    // STX [r2],r5         r2=52; r5=1
                    // a posição 52 nao foi inicializada ainda então aponta pra null.
                    // esse Convert.ToInt32(value) ta dando o mesmo problema que deu no LDD

                    //if (memoria[Convert.ToInt32(value)].OPCode == "DATA")
                    //{
                    GerenteMemoria.Memoria[memoryPosition] = new PosicaoDeMemoria
                    {
                        OPCode = "DATA",
                        Parameter = registradores[currentLine.Reg2]
                    };

                    /* }
                       else
                       {
                           throw new InvalidOperationException("Não é possivel ler dados de uma posição de memória com OPCode diferente de [DATA]. Encerrando execução");
                       }
                       */
                    pc++;
                    break;

                // troca os valores dos registradores; r7←r3, r6←r2, r5←r1, r4←r0
                // até agora acreditamos que só da pra usar como parametro o r4, r5, r6 e r7
                // SWAP rx  
                case "SWAP":
                    switch (currentLine.Reg1)
                    {
                        case "r4":
                            registradores[currentLine.Reg1] = registradores["r0"];
                            break;

                        case "r5":
                            registradores[currentLine.Reg1] = registradores["r1"];
                            break;

                        case "r6":
                            registradores[currentLine.Reg1] = registradores["r2"];
                            break;

                        case "r7":
                            registradores[currentLine.Reg1] = registradores["r3"];
                            break;

                        default:
                            throw new ArgumentException($"Não é possível fazer SWAP conm o registrador [{currentLine.Reg1}]");
                    }

                    pc++;
                    break;

                // todos outros q tem tbm n precisa, sao bitwise operators, ainda n chegamo lá
                default:
                    throw new ArgumentException($"Não foi possível encontrar o comando [{currentLine.OPCode}]");

            }
        }

        Console.WriteLine("Status final dos registradores:\n");

        foreach (var item in registradores)
        {
            Console.WriteLine("--------------------------------------------\n");

            Console.WriteLine($"Registrador [{item.Key}] - valor final [{item.Value}]\n");
        }

        Console.WriteLine("--------------------------------------------\n");

        Console.WriteLine("Status final das posições de memória (não nulas)\n");

        for (int i = 0; i < GerenteMemoria.Memoria.Length; i++)
        {
            if (GerenteMemoria.Memoria[i] == null)
            {
                continue;
            }
            
            Console.WriteLine($"Posição de memória [{i}]:");
            Console.WriteLine(GerenteMemoria.Memoria[i].ToString() + "\n");
        }

    }
    public void NewCPUVirtualMachine(string filePath)
    {
        Dictionary<string, int> registradores = new Dictionary<string, int>();

        registradores.Add("r0", 0);
        registradores.Add("r1", 0);
        registradores.Add("r2", 0);
        registradores.Add("r3", 0);
        registradores.Add("r4", 0);
        registradores.Add("r5", 0);
        registradores.Add("r6", 0);
        registradores.Add("r7", 0);

        string value = string.Empty;
        bool logicalResult = false;
        int memoryPosition = 0;

        Random r = new Random();
        int particaoAleatoria = 1;//r.Next(0, GerenteMemoria.Particoes.Length);

        // enquanto a partição aleatoria nao estiver livre, procurar uma próxima aleatoria
        while (!GerenteMemoria.ParticaoEstaLivre(particaoAleatoria))
        {
            particaoAleatoria = r.Next(0, GerenteMemoria.Particoes.Length);
        }

        GerenteMemoria.ReadFile(filePath, particaoAleatoria);

        int offsetParticao = GerenteMemoria.CalculaOffset(particaoAleatoria);

        //PC precisa ser a primeira posição da partição em questão que está sendo usada (confirmar depois) 
        int pc = offsetParticao;

        PosicaoDeMemoria currentLine = GerenteMemoria.Memoria[pc];

        while (currentLine.OPCode != "STOP")
        {
            currentLine = GerenteMemoria.Memoria[pc];

            switch (currentLine.OPCode)
            {
                // faz PC pular direto pra uma linha k
                // JMP 12
                case "JMP":
                    pc = GerenteMemoria.CalculaEnderecoMemoria(particaoAleatoria, Convert.ToInt32(currentLine.Parameter));
                    break;

                // faz PC pular direto pra linha contida no registrador r
                // JMPI r1
                case "JMPI":
                    pc = GerenteMemoria.CalculaEnderecoMemoria(particaoAleatoria, Convert.ToInt32(registradores[currentLine.Reg1]));
                    break;

                // faz PC pular direto pra linha contida no registrador rx, caso ry seja maior que 0
                // JMPIG rx,ry
                case "JMPIG":
                    if (Convert.ToInt32(registradores[currentLine.Reg2]) > 0)
                    {
                        pc = GerenteMemoria.CalculaEnderecoMemoria(particaoAleatoria, Convert.ToInt32(registradores[currentLine.Reg1]));
                        currentLine = GerenteMemoria.Memoria[pc];
                    }
                    else
                    {
                        pc++;
                        currentLine = GerenteMemoria.Memoria[pc];
                    }
                    break;

                // faz PC pular direto pra linha contida no registrador rx, caso ry seja menor que 0
                // JMPIL rx,ry
                case "JMPIL":
                    if (Convert.ToInt32(registradores[currentLine.Reg2]) < 0)
                    {
                        pc = GerenteMemoria.CalculaEnderecoMemoria(particaoAleatoria, Convert.ToInt32(registradores[currentLine.Reg1]));
                        currentLine = GerenteMemoria.Memoria[pc];
                    }
                    else
                    {
                        pc++;
                        currentLine = GerenteMemoria.Memoria[pc];
                    }
                    break;

                // faz PC pular direto pra linha contida no registrador rx, caso ry igual a 0
                // JMPIE rx,ry
                case "JMPIE":
                    if (Convert.ToInt32(registradores[currentLine.Reg2]) == 0)
                    {
                        pc = GerenteMemoria.CalculaEnderecoMemoria(particaoAleatoria, Convert.ToInt32(registradores[currentLine.Reg1]));
                        currentLine = GerenteMemoria.Memoria[pc];
                    }
                    else
                    {
                        pc++;
                        currentLine = GerenteMemoria.Memoria[pc];
                    }
                    break;

                // realiza a soma imediata de um valor k no registrador r
                //ADDI r1,1
                case "ADDI":
                    registradores[currentLine.Reg1] += currentLine.Parameter;

                    pc++;
                    break;

                // realiza a subtração imediata de um valor k no registrador r
                //SUBI r1,1
                case "SUBI":
                    registradores[currentLine.Reg1] -= currentLine.Parameter;

                    pc++;
                    currentLine = GerenteMemoria.Memoria[pc];
                    break;

                // carrega um valor k em um registrador
                // LDI r1,10
                case "LDI":
                    registradores[currentLine.Reg1] = currentLine.Parameter;

                    pc++;
                    currentLine = GerenteMemoria.Memoria[pc];
                    break;

                // carrega um valor da memoria em um registrador
                // LDD r1,[50]
                case "LDD":
                    value = currentLine.Reg2.Trim(new char[] { '[', ']' });
                    memoryPosition = GerenteMemoria.CalculaEnderecoMemoria(particaoAleatoria, Convert.ToInt32(value));

                    if (GerenteMemoria.Memoria[memoryPosition].OPCode == "DATA")
                    {
                        registradores[currentLine.Reg1] = GerenteMemoria.Memoria[memoryPosition].Parameter;
                    }
                    else
                    {
                        throw new InvalidOperationException("Não é possivel ler dados de uma posição de memória com OPCode diferente de [DATA]. Encerrando execução");
                    }

                    pc++;
                    currentLine = GerenteMemoria.Memoria[pc];
                    break;

                //asdiapiofjasifaf-0--------------------------------------------

                // guarda na memoria um valor contido no registrador r
                // STD [52],r1
                case "STD":
                    value = currentLine.Reg1.Trim(new char[] { '[', ']' });
                    memoryPosition = GerenteMemoria.CalculaEnderecoMemoria(particaoAleatoria, Convert.ToInt32(value));

                    GerenteMemoria.Memoria[memoryPosition] = new PosicaoDeMemoria
                    {
                        OPCode = "DATA",
                        Parameter = registradores[currentLine.Reg2]
                    };

                    pc++;
                    currentLine = GerenteMemoria.Memoria[pc];
                    break;

                // faz a operaçao: rx = rx + ry
                // ADD rx,ry
                case "ADD":
                    registradores[currentLine.Reg1] += registradores[currentLine.Reg2];

                    pc++;
                    break;

                // faz a operaçao: rx = rx - ry
                // SUB rx,ry
                case "SUB":
                    registradores[currentLine.Reg1] -= registradores[currentLine.Reg2];

                    pc++;
                    break;

                // faz a operaçao: rx = rx * ry
                // MULT rx,ry
                case "MULT":
                    registradores[currentLine.Reg1] *= registradores[currentLine.Reg2];

                    pc++;
                    currentLine = GerenteMemoria.Memoria[pc];
                    break;

                // faz a operaçao: rx = rx AND k
                // AND rx,k
                case "ANDI":
                    if (new int[] { 0, 1 }.Contains(registradores[currentLine.Reg1]))
                    {
                        logicalResult = Convert.ToBoolean(currentLine.Parameter) &&
                                        Convert.ToBoolean(registradores[currentLine.Reg1]);

                        registradores[currentLine.Reg1] = Convert.ToInt32(logicalResult);
                    }
                    else
                    {
                        throw new ArgumentException("O registrador não contém um valor lógico válido");
                    }

                    pc++;
                    break;

                // faz a operaçao: rx = rx OR k
                // OR rx,k
                case "ORI":
                    if (new int[] { 0, 1 }.Contains(registradores[currentLine.Reg1]))
                    {
                        logicalResult = Convert.ToBoolean(currentLine.Parameter) ||
                                        Convert.ToBoolean(registradores[currentLine.Reg1]);

                        registradores[currentLine.Reg1] = Convert.ToInt32(logicalResult);
                    }
                    else
                    {
                        throw new ArgumentException("O registrador não contém um valor lógico válido");
                    }

                    pc++;
                    break;

                // carrega em rx o dado contido na posiçao de memoria indicada por ry
                // LDX rx,[ry]
                case "LDX":
                    value = currentLine.Reg2.Trim(new char[] { '[', ']' });
                    memoryPosition = GerenteMemoria.CalculaEnderecoMemoria(particaoAleatoria, registradores[value]);

                    registradores[currentLine.Reg1] = GerenteMemoria.Memoria[memoryPosition].Parameter;

                    pc++;
                    break;

                // guarda na posição de memoria rx o dado contido em ry
                // STX [rx],ry
                case "STX":
                    value = currentLine.Reg1.Trim(new char[] { '[', ']' });
                    memoryPosition = GerenteMemoria.CalculaEnderecoMemoria(particaoAleatoria, registradores[value]);

                    // pelo oq eu entendi esse if checa se a posicao é destinada para guardar dados e garante que nao é codigo
                    // porém tem uma linha no P1 assim:
                    // STX [r2],r5         r2=52; r5=1
                    // a posição 52 nao foi inicializada ainda então aponta pra null.
                    // esse Convert.ToInt32(value) ta dando o mesmo problema que deu no LDD

                    //if (memoria[Convert.ToInt32(value)].OPCode == "DATA")
                    //{
                    GerenteMemoria.Memoria[memoryPosition] = new PosicaoDeMemoria
                    {
                        OPCode = "DATA",
                        Parameter = registradores[currentLine.Reg2]
                    };

                    /* }
                       else
                       {
                           throw new InvalidOperationException("Não é possivel ler dados de uma posição de memória com OPCode diferente de [DATA]. Encerrando execução");
                       }
                       */
                    pc++;
                    break;

                // troca os valores dos registradores; r7←r3, r6←r2, r5←r1, r4←r0
                // até agora acreditamos que só da pra usar como parametro o r4, r5, r6 e r7
                // SWAP rx  
                case "SWAP":
                    switch (currentLine.Reg1)
                    {
                        case "r4":
                            registradores[currentLine.Reg1] = registradores["r0"];
                            break;

                        case "r5":
                            registradores[currentLine.Reg1] = registradores["r1"];
                            break;

                        case "r6":
                            registradores[currentLine.Reg1] = registradores["r2"];
                            break;

                        case "r7":
                            registradores[currentLine.Reg1] = registradores["r3"];
                            break;

                        default:
                            throw new ArgumentException($"Não é possível fazer SWAP conm o registrador [{currentLine.Reg1}]");
                    }

                    pc++;
                    break;

                // todos outros q tem tbm n precisa, sao bitwise operators, ainda n chegamo lá
                default:
                    throw new ArgumentException($"Não foi possível encontrar o comando [{currentLine.OPCode}]");

            }
        }

        Console.WriteLine("Status final dos registradores:\n");

        foreach (var item in registradores)
        {
            Console.WriteLine("--------------------------------------------\n");

            Console.WriteLine($"Registrador [{item.Key}] - valor final [{item.Value}]\n");
        }

        Console.WriteLine("--------------------------------------------\n");

        Console.WriteLine("Status final das posições de memória (não nulas)\n");

        for (int i = 0; i < GerenteMemoria.Memoria.Length; i++)
        {
            if (GerenteMemoria.Memoria[i] == null)
            {
                continue;
            }
            
            Console.WriteLine($"Posição de memória [{i}]:");
            Console.WriteLine(GerenteMemoria.Memoria[i].ToString() + "\n");
        }
    }





    //----------------------------------------------------------------------------------------------
    // codjego do T1
    //----------------------------------------------------------------------------------------------

    public void VirtualMachine(string filePath)
    {
        Dictionary<string, int> registradores = new Dictionary<string, int>();

        registradores.Add("r0", 0);
        registradores.Add("r1", 0);
        registradores.Add("r2", 0);
        registradores.Add("r3", 0);
        registradores.Add("r4", 0);
        registradores.Add("r5", 0);
        registradores.Add("r6", 0);
        registradores.Add("r7", 0);

        PosicaoDeMemoria[] memoria = new PosicaoDeMemoria[1024];

        int pc = 0;

        string value = string.Empty;
        bool logicalResult = false;

        //ReadFile(filePath, memoria);

        PosicaoDeMemoria currentLine = memoria[pc];

        while (currentLine.OPCode != "STOP")
        {
            currentLine = memoria[pc];

            switch (currentLine.OPCode)
            {
                // faz PC pular direto pra uma linha k
                // JMP 12
                case "JMP":
                    pc = Convert.ToInt32(currentLine.Parameter);
                    break;

                // faz PC pular direto pra linha contida no registrador r
                // JMPI r1
                case "JMPI":
                    pc = Convert.ToInt32(registradores[currentLine.Reg1]);
                    break;

                // faz PC pular direto pra linha contida no registrador rx, caso ry seja maior que 0
                // JMPIG rx,ry
                case "JMPIG":
                    if (Convert.ToInt32(registradores[currentLine.Reg2]) > 0)
                    {
                        pc = Convert.ToInt32(registradores[currentLine.Reg1]);
                        currentLine = memoria[pc];
                    }
                    else
                    {
                        pc++;
                        currentLine = memoria[pc];
                    }
                    break;

                // faz PC pular direto pra linha contida no registrador rx, caso ry seja menor que 0
                // JMPIL rx,ry
                case "JMPIL":
                    if (Convert.ToInt32(registradores[currentLine.Reg2]) < 0)
                    {
                        pc = Convert.ToInt32(registradores[currentLine.Reg1]);
                        currentLine = memoria[pc];
                    }
                    else
                    {
                        pc++;
                        currentLine = memoria[pc];
                    }
                    break;

                // faz PC pular direto pra linha contida no registrador rx, caso ry igual a 0
                // JMPIE rx,ry
                case "JMPIE":
                    if (Convert.ToInt32(registradores[currentLine.Reg2]) == 0)
                    {
                        pc = Convert.ToInt32(registradores[currentLine.Reg1]);
                        currentLine = memoria[pc];
                    }
                    else
                    {
                        pc++;
                        currentLine = memoria[pc];
                    }
                    break;

                // realiza a soma imediata de um valor k no registrador r
                //ADDI r1,1
                case "ADDI":
                    registradores[currentLine.Reg1] += currentLine.Parameter;

                    pc++;
                    break;

                // realiza a subtração imediata de um valor k no registrador r
                //SUBI r1,1
                case "SUBI":
                    registradores[currentLine.Reg1] -= currentLine.Parameter;

                    pc++;
                    currentLine = memoria[pc];
                    break;

                // carrega um valor k em um registrador
                // LDI r1,10
                case "LDI":
                    registradores[currentLine.Reg1] = currentLine.Parameter;

                    pc++;
                    currentLine = memoria[pc];
                    break;

                // carrega um valor da memoria em um registrador
                // LDD r1,[50]
                case "LDD":
                    value = currentLine.Reg2.Trim(new char[] { '[', ']' });
                    int memoryPosition = Convert.ToInt32(value);

                    if (memoria[memoryPosition].OPCode == "DATA")
                    {
                        registradores[currentLine.Reg1] = memoria[memoryPosition].Parameter;
                    }
                    else
                    {
                        throw new InvalidOperationException("Não é possivel ler dados de uma posição de memória com OPCode diferente de [DATA]. Encerrando execução");
                    }

                    pc++;
                    currentLine = memoria[pc];
                    break;

                // guarda na memoria um valor contido no registrador r
                // STD [52],r1
                case "STD":
                    value = currentLine.Reg1.Trim(new char[] { '[', ']' });

                    memoria[Convert.ToInt32(value)] = new PosicaoDeMemoria
                    {
                        OPCode = "DATA",
                        Parameter = registradores[currentLine.Reg2]
                    };

                    pc++;
                    currentLine = memoria[pc];
                    break;

                // faz a operaçao: rx = rx + ry
                // ADD rx,ry
                case "ADD":
                    registradores[currentLine.Reg1] += registradores[currentLine.Reg2];

                    pc++;
                    break;

                // faz a operaçao: rx = rx - ry
                // SUB rx,ry
                case "SUB":
                    registradores[currentLine.Reg1] -= registradores[currentLine.Reg2];

                    pc++;
                    break;

                // faz a operaçao: rx = rx * ry
                // MULT rx,ry
                case "MULT":
                    registradores[currentLine.Reg1] *= registradores[currentLine.Reg2];

                    pc++;
                    currentLine = memoria[pc];
                    break;

                // faz a operaçao: rx = rx AND k
                // AND rx,k
                case "ANDI":
                    if (new int[] { 0, 1 }.Contains(registradores[currentLine.Reg1]))
                    {
                        logicalResult = Convert.ToBoolean(currentLine.Parameter) &&
                                        Convert.ToBoolean(registradores[currentLine.Reg1]);

                        registradores[currentLine.Reg1] = Convert.ToInt32(logicalResult);
                    }
                    else
                    {
                        throw new ArgumentException("O registrador não contém um valor lógico válido");
                    }

                    pc++;
                    break;

                // faz a operaçao: rx = rx OR k
                // OR rx,k
                case "ORI":
                    if (new int[] { 0, 1 }.Contains(registradores[currentLine.Reg1]))
                    {
                        logicalResult = Convert.ToBoolean(currentLine.Parameter) ||
                                        Convert.ToBoolean(registradores[currentLine.Reg1]);

                        registradores[currentLine.Reg1] = Convert.ToInt32(logicalResult);
                    }
                    else
                    {
                        throw new ArgumentException("O registrador não contém um valor lógico válido");
                    }

                    pc++;
                    break;

                // carrega em rx o dado contido na posiçao de memoria indicada por ry
                // LDX rx,[ry]
                case "LDX":
                    value = currentLine.Reg2.Trim(new char[] { '[', ']' });

                    registradores[currentLine.Reg1] = memoria[registradores[value]].Parameter;

                    pc++;
                    break;

                // guarda na posição de memoria rx o dado contido em ry
                // STX [rx],ry
                case "STX":
                    value = currentLine.Reg1.Trim(new char[] { '[', ']' });


                    // pelo oq eu entendi esse if checa se a posicao é destinada para guardar dados e garante que nao é codigo
                    // porém tem uma linha no P1 assim:
                    // STX [r2],r5         r2=52; r5=1
                    // a posição 52 nao foi inicializada ainda então aponta pra null.
                    // esse Convert.ToInt32(value) ta dando o mesmo problema que deu no LDD

                    //if (memoria[Convert.ToInt32(value)].OPCode == "DATA")
                    //{
                    memoria[registradores[value]] = new PosicaoDeMemoria
                    {
                        OPCode = "DATA",
                        Parameter = registradores[currentLine.Reg2]
                    };

                    /* }
                       else
                       {
                           throw new InvalidOperationException("Não é possivel ler dados de uma posição de memória com OPCode diferente de [DATA]. Encerrando execução");
                       }
                       */
                    pc++;
                    break;

                // troca os valores dos registradores; r7←r3, r6←r2, r5←r1, r4←r0
                // até agora acreditamos que só da pra usar como parametro o r4, r5, r6 e r7
                // SWAP rx  
                case "SWAP":
                    switch (currentLine.Reg1)
                    {
                        case "r4":
                            registradores[currentLine.Reg1] = registradores["r0"];
                            break;

                        case "r5":
                            registradores[currentLine.Reg1] = registradores["r1"];
                            break;

                        case "r6":
                            registradores[currentLine.Reg1] = registradores["r2"];
                            break;

                        case "r7":
                            registradores[currentLine.Reg1] = registradores["r3"];
                            break;

                        default:
                            throw new ArgumentException($"Não é possível fazer SWAP conm o registrador [{currentLine.Reg1}]");
                    }

                    pc++;
                    break;

                // todos outros q tem tbm n precisa, sao bitwise operators, ainda n chegamo lá
                default:
                    throw new ArgumentException($"Não foi possível encontrar o comando [{currentLine.OPCode}]");
            }
        }

        Console.WriteLine("Status final dos registradores:\n");

        foreach (var item in registradores)
        {
            Console.WriteLine("--------------------------------------------\n");

            Console.WriteLine($"Registrador [{item.Key}] - valor final [{item.Value}]\n");
        }

        Console.WriteLine("--------------------------------------------\n");

        Console.WriteLine("Status final das posições de memória (não nulas)\n");

        for (int i = 0; i < memoria.Length; i++)
        {
            if (memoria[i] == null)
            {
                continue;
            }
            Console.WriteLine($"Posição de memória [{i}]:");
            Console.WriteLine(memoria[i].ToString() + "\n");
        }
    }

}