using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    class Program
    {
        public static void Main(string[] args)
        {
            Dictionary<string, int> registradores = new Dictionary<string, int>();

            registradores.Add("r1", 0);
            registradores.Add("r2", 0);
            registradores.Add("r3", 0);
            registradores.Add("r4", 0);
            registradores.Add("r5", 0);
            registradores.Add("r6", 0);
            registradores.Add("r7", 0);
            registradores.Add("r8", 0);

            PosicaoDeMemoria[] memoria = new PosicaoDeMemoria[1024];

            int pc = 0;

            string value = string.Empty;
            bool logicalResult = false;

            Console.WriteLine("Digite o caminho do arquivo\n");
            string filePath = Environment.CurrentDirectory + @"\programs\P3.txt";//Console.ReadLine();

            ReadFile(filePath, memoria);

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

                        if (memoria[Convert.ToInt32(value)].OPCode == "DATA")
                        {
                            memoria[registradores[value]] = new PosicaoDeMemoria
                            {
                                OPCode = "DATA",
                                Parameter = registradores[currentLine.Reg2]
                            };

                        }
                        else
                        {
                            throw new InvalidOperationException("Não é possivel ler dados de uma posição de memória com OPCode diferente de [DATA]. Encerrando execução");
                        }

                        pc++;
                        break;

                    // todos outros q tem tbm n precisa, sao bitwise operators, ainda n chegamo lá
                    default:
                        throw new ArgumentException($"Não foi possível encontrar o comando [{command}]");
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

        public static void ReadFile(string filePath, PosicaoDeMemoria[] memoria)
        {
            string[] fileContent = File.ReadAllLines(filePath);

            for (int i = 0; i < fileContent.Length; i++)
            {

                string[] dataContent = fileContent[i].Split(' ');

                string command = dataContent[0];

                if (command == "STOP")
                {
                    memoria[i] = new PosicaoDeMemoria
                    {
                        OPCode = "STOP"
                    };
                    continue;
                }

                string[] parameters = dataContent[1].Replace(" ", "").Split(',');

                memoria[i] = new PosicaoDeMemoria
                {
                    OPCode = command,
                    Reg1 = parameters[0].Contains("r") || parameters[0].Contains("[") ? parameters[0] : null,
                    Reg2 = parameters[1].Contains("r") || parameters[1].Contains("[") ? parameters[1] : null,
                    Parameter = Int32.TryParse(parameters[1], out int value) ? value : 0
                };
            }
        }
    }
}