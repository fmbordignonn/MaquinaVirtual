﻿using System;
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

            int[] memoria = new int[1024];

            int pc = 0;

            Console.WriteLine("Digite o caminho do arquivo\n");
            var filePath = @"C:\Users\Felipe\Desktop\texiste.txt";//Console.ReadLine();

            var fileContent = File.ReadAllLines(filePath);

            while (fileContent[pc] != "STOP")
            {
                var dataContent = fileContent[pc].Split(' ');

                string command = dataContent[0];

                string[] parameters = dataContent[1].Split(',');

                string value = "";

                switch (command)
                {
                    // faz PC pular direto pra uma linha k
                    //JMP 12
                    case "JMP":
                        pc = Convert.ToInt32(parameters[0]);
                        break;

                    // faz PC pular direto pra linha contida no registrador r
                    // JMPI r1
                    case "JMPI":
                        pc = Convert.ToInt32(registradores[parameters[0]]);
                        break;

                    // faz PC pular direto pra linha contida no registrador rx, caso ry seja maior que 0
                    // JMPIG rx,ry
                    case "JMPIG":

                        //melhorar esse if dps pq ta mt feio
                        if (Convert.ToInt32(registradores[parameters[1]]) > 0)
                        {
                            pc = Convert.ToInt32(registradores[parameters[0]]);
                        }
                        else
                        {
                            pc++;
                        }
                        break;

                    // faz PC pular direto pra linha contida no registrador rx, caso ry seja menor que 0
                    // JMPIL rx,ry
                    case "JMPIL":

                        //melhorar esse if dps pq ta mt feio
                        if (Convert.ToInt32(registradores[parameters[1]]) < 0)
                        {
                            pc = Convert.ToInt32(registradores[parameters[0]]);
                        }
                        else
                        {
                            pc++;
                        }
                        break;

                    // faz PC pular direto pra linha contida no registrador rx, caso ry igual a 0
                    // JMPIE rx,ry
                    case "JMPIE":
                        //melhorar esse if dps pq ta mt feio
                        if (Convert.ToInt32(registradores[parameters[1]]) == 0)
                        {
                            pc = Convert.ToInt32(registradores[parameters[0]]);
                        }
                        else
                        {
                            pc++;
                        }
                        break;

                    // realiza a soma imediata de um valor k no registrador r
                    //ADDI r1,1
                    case "ADDI":
                        registradores[parameters[0]] += Convert.ToInt32(parameters[1]);
                        break;

                    // realiza a subtração imediata de um valor k no registrador r
                    //SUBI r1,1
                    case "SUBI":
                        registradores[parameters[0]] -= Convert.ToInt32(parameters[1]);
                        break;

                    // carrega um valor k em um registrador
                    // LDI r1,10
                    case "LDI":
                        registradores[parameters[0]] = Convert.ToInt32(parameters[1]);
                        break;

                    // carrega um valor da memoria em um registrador
                    // LDD r1,[50]
                    case "LDD":
                        value = parameters[1].Trim(new char[] { '[', ']' });
                        int convertedValue = Convert.ToInt32(value);

                        registradores[parameters[0]] = memoria[convertedValue];
                        break;

                    // guarda na memoria um valor contido no registrador r
                    // STD 52,r1
                    case "STD":
                        memoria[Convert.ToInt32(parameters[0])] = registradores[parameters[1]];
                        break;

                    // faz a operaçao: rx = rx + ry
                    // ADD rx,ry
                    case "ADD":
                        registradores[parameters[0]] += registradores[parameters[1]];
                        break;

                    // faz a operaçao: rx = rx - ry
                    // ADD rx,ry
                    case "SUB":
                        registradores[parameters[0]] -= registradores[parameters[1]];
                        break;

                    // faz a operaçao: rx = rx * ry
                    // ADD rx,ry
                    case "MULT":
                        registradores[parameters[0]] *= registradores[parameters[1]];
                        break;

                    //AND nao precisa

                    //OR nao precisa

                    // carrega em rx o dado contido na posiçao de memoria indicada por ry
                    // LDX rx,[ry]
                    case "LDX":
                        value = parameters[1].Trim(new char[] { '[', ']' });

                        registradores[parameters[0]] = memoria[registradores[value]];
                        break;

                    // guarda na posição de memoria rx o dado contido em ry
                    // STX [rx],ry
                    // tem q conserta esse
                    case "STX":
                        registradores["r2"] = 12;
                        memoria[12] = 123;
                        value = parameters[0].Trim(new char[] { '[', ']' });

                        memoria[registradores[value]] = registradores[parameters[1]];
                        break;

                    // todos outros q tem tbm n precisa, sao bitwise operators, ainda n chegamo lá
                    default:
                        throw new ArgumentException($"Não foi possível encontrar o comando [{command}]");
                }

                pc++; // acho q isso nao deve ser aqui, mas ve isso dps

                // tem q add os trim dps pra evitar q de exception por causa de whitespace
            }
        }
    }
}
