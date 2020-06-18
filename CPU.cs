using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class CPU
{
    private const int FATIA_DE_TEMPO = 10;

    public static void ExecutarCPU(ProcessControlBlock pcb)
    {
        pcb.State = State.RUNNING;
        int ComandsCount = 0;

        // só pra mostrar que o processo ta RUNNING
        Console.WriteLine($"Process Id: {pcb.ProcessID} ; State: {pcb.State}");
        
        Console.WriteLine("As thread tao bombando - to na cpu");
        string value = string.Empty;
        bool logicalResult = false;
        int memoryPosition = 0;

        PosicaoDeMemoria currentLine = GerenteDeMemoria.Memoria[pcb.Pc];


        while (currentLine.OPCode != "STOP" && ComandsCount != FATIA_DE_TEMPO)
        {
            currentLine = GerenteDeMemoria.Memoria[pcb.Pc];
            ComandsCount++;

            switch (currentLine.OPCode)
            {
                // faz PC pular direto pra uma linha k
                // JMP 12
                case "JMP":
                    pcb.Pc = GerenteDeMemoria.CalculaEnderecoMemoria(pcb, Convert.ToInt32(currentLine.Parameter));
                    break;

                // faz PC pular direto pra linha contida no registrador r
                // JMPI r1
                case "JMPI":
                    pcb.Pc = GerenteDeMemoria.CalculaEnderecoMemoria(pcb, Convert.ToInt32(pcb.Registradores[currentLine.Reg1]));
                    break;

                // faz PC pular direto pra linha contida no registrador rx, caso ry seja maior que 0
                // JMPIG rx,ry
                case "JMPIG":
                    if (Convert.ToInt32(pcb.Registradores[currentLine.Reg2]) > 0)
                    {
                        pcb.Pc = GerenteDeMemoria.CalculaEnderecoMemoria(pcb, Convert.ToInt32(pcb.Registradores[currentLine.Reg1]));
                        currentLine = GerenteDeMemoria.Memoria[pcb.Pc];
                    }
                    else
                    {
                        pcb.Pc++;
                        currentLine = GerenteDeMemoria.Memoria[pcb.Pc];
                    }
                    break;

                // faz PC pular direto pra linha contida no registrador rx, caso ry seja menor que 0
                // JMPIL rx,ry
                case "JMPIL":
                    if (Convert.ToInt32(pcb.Registradores[currentLine.Reg2]) < 0)
                    {
                        pcb.Pc = GerenteDeMemoria.CalculaEnderecoMemoria(pcb, Convert.ToInt32(pcb.Registradores[currentLine.Reg1]));
                        currentLine = GerenteDeMemoria.Memoria[pcb.Pc];
                    }
                    else
                    {
                        pcb.Pc++;
                        currentLine = GerenteDeMemoria.Memoria[pcb.Pc];
                    }
                    break;

                // faz PC pular direto pra linha contida no registrador rx, caso ry igual a 0
                // JMPIE rx,ry
                case "JMPIE":
                    if (Convert.ToInt32(pcb.Registradores[currentLine.Reg2]) == 0)
                    {
                        pcb.Pc = GerenteDeMemoria.CalculaEnderecoMemoria(pcb, Convert.ToInt32(pcb.Registradores[currentLine.Reg1]));
                        currentLine = GerenteDeMemoria.Memoria[pcb.Pc];
                    }
                    else
                    {
                        pcb.Pc++;
                        currentLine = GerenteDeMemoria.Memoria[pcb.Pc];
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
                    currentLine = GerenteDeMemoria.Memoria[pcb.Pc];
                    break;

                // carrega um valor k em um registrador
                // LDI r1,10
                case "LDI":
                    pcb.Registradores[currentLine.Reg1] = currentLine.Parameter;

                    pcb.Pc++;
                    currentLine = GerenteDeMemoria.Memoria[pcb.Pc];
                    break;

                // carrega um valor da memoria em um registrador
                // LDD r1,[50]
                case "LDD":
                    value = currentLine.Reg2.Trim(new char[] { '[', ']' });
                    memoryPosition = GerenteDeMemoria.CalculaEnderecoMemoria(pcb, Convert.ToInt32(value));

                    if (GerenteDeMemoria.Memoria[memoryPosition].OPCode == "DATA")
                    {
                        pcb.Registradores[currentLine.Reg1] = GerenteDeMemoria.Memoria[memoryPosition].Parameter;
                    }
                    else
                    {
                        throw new InvalidOperationException("Não é possivel ler dados de uma posição de memória com OPCode diferente de [DATA]. Encerrando execução");
                    }

                    pcb.Pc++;
                    currentLine = GerenteDeMemoria.Memoria[pcb.Pc];
                    break;

                // guarda na memoria um valor contido no registrador r
                // STD [52],r1
                case "STD":
                    value = currentLine.Reg1.Trim(new char[] { '[', ']' });
                    memoryPosition = GerenteDeMemoria.CalculaEnderecoMemoria(pcb, Convert.ToInt32(value));

                    GerenteDeMemoria.Memoria[memoryPosition] = new PosicaoDeMemoria
                    {
                        OPCode = "DATA",
                        Parameter = pcb.Registradores[currentLine.Reg2]
                    };

                    pcb.Pc++;
                    currentLine = GerenteDeMemoria.Memoria[pcb.Pc];
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
                    currentLine = GerenteDeMemoria.Memoria[pcb.Pc];
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
                    memoryPosition = GerenteDeMemoria.CalculaEnderecoMemoria(pcb, pcb.Registradores[value]);

                    pcb.Registradores[currentLine.Reg1] = GerenteDeMemoria.Memoria[memoryPosition].Parameter;

                    pcb.Pc++;
                    break;

                // guarda na posição de memoria rx o dado contido em ry
                // STX [rx],ry
                case "STX":
                    value = currentLine.Reg1.Trim(new char[] { '[', ']' });
                    memoryPosition = GerenteDeMemoria.CalculaEnderecoMemoria(pcb, pcb.Registradores[value]);

                    GerenteDeMemoria.Memoria[memoryPosition] = new PosicaoDeMemoria
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

                case "TRAP":
                    value = currentLine.Reg1;
                    memoryPosition = GerenteDeMemoria.CalculaEnderecoMemoria(pcb, pcb.Registradores[value]);

                    if (value != "1" || value != "2")
                    {
                        throw new ArgumentException($"O valor [{value}] é inválido para operação de IO. Somente é aceito '1' ou '2' como argumento.");
                    }
                    else if (value == "1")
                    {
                        //READ
                    }
                    else
                    {
                        //WRITE
                    }
                        
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
}