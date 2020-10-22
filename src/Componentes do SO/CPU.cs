using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class CPU
{

    public static Semaphore semCPU = new Semaphore(0, 1);

    public static void ExecutarCPU(ProcessControlBlock pcb)
    {
        pcb.State = State.RUNNING;
        int CommandsCount = 0;

        // só pra mostrar que o processo ta RUNNING
        Console.WriteLine($"\nProcess Id: {pcb.ProcessID} ; State: {pcb.State}\n");

        //Console.WriteLine("\nTo na cpu");
        string value = string.Empty;
        bool logicalResult = false;
        int memoryPosition = 0;

        PosicaoDeMemoria currentLine = GerenteDeMemoria.Memoria[pcb.Pc];

        try
        {
            while (true)
            {
                //Console.WriteLine($"Current OPCode: {currentLine.OPCode}");

                //Verificando interrupção por fatia de tempo
                if (TimerCPU.VerificaFatiaDeTempo(CommandsCount))
                {
                    //Console.WriteLine($"atingi a fatia de tempo {CommandsCount}");

                    pcb.State = State.WAITING;
                    //Console.WriteLine($"Status da pcb {pcb.State}");

                    //Trata interrupção ocorrida por TIMER
                    RotinaTratamentoTimer.TratarInterrupcaoTimer(pcb);
                    //Break para sair do while(true) e travar o semáforo da CPU lá embaixo
                    break;
                }

                if (currentLine.OPCode == "STOP")
                {
                    Console.WriteLine("entrando na rotina de finalização");
                    RotinaTratamentoFinalizacao.FinalizarProcesso(pcb);
                    break;
                }

                //IO
                if (currentLine.OPCode == "TRAP")
                {
                    Console.WriteLine("Entrando na rotina de tratamento de IO");

                    string operation = currentLine.Reg1.Trim(new char[] { '[', ']' });

                    //Console.WriteLine(currentLine.ToString());
                    //Console.WriteLine($"valor em operation: {operation}");

                    value = currentLine.Reg2.Trim(new char[] { '[', ']' });
                    memoryPosition = GerenteDeMemoria.CalculaEnderecoMemoria(pcb, pcb.Registradores[value]);

                    //TRAP 1 [r1]
                    //TRAP 2 [50]
                    if (!new string[] {"1", "2"}.Contains(operation))
                    {
                        throw new ArgumentException($"O valor [{operation}] é inválido para operação de IO. Somente é aceito '1' ou '2' como argumento.");
                    }

                    // incrementa o PC pq senao nunca sai da linha TRAP
                    pcb.Pc++;

                    //Read
                    if (operation == "1")
                    {
                        RotinaTratamentoIO.TratarPedidoIO(pcb, IOType.READ, memoryPosition);
                        break;
                    }
                    //Write
                    else
                    {
                        RotinaTratamentoIO.TratarPedidoIO(pcb, IOType.WRITE, memoryPosition);
                        break;
                    }
                }

                else
                {
                    //Console.WriteLine($"entrei no swtich com {CommandsCount} comandos");

                    currentLine = GerenteDeMemoria.Memoria[pcb.Pc];
                    CommandsCount++;

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

                        default:
                            throw new ArgumentException($"Não foi possível encontrar o comando [{currentLine.OPCode}]");

                    }
                }
            }
        }
        catch (AcessoIndevidoException)
        {
            RotinaTratamentoFinalizacao.TratamentoAcessoIndevido(pcb);
        }
        catch (DivideByZeroException)
        {
            RotinaTratamentoFinalizacao.TratamentoDivisaoPorZero(pcb);
        }
        catch (Exception ex)
        {
            //Caso erro genérico
            throw new Exception($"Ocorreu um erro ao executar o comando na VM: [{ex.Message}]");
        }

        //Console.WriteLine("indo interromper a CPU");
        
        //Bloqueando a execução da CPU até que o escalonador libere a CPU 
        //para estar pronta a executar um novo processo
        CPU.semCPU.WaitOne();
    }
}