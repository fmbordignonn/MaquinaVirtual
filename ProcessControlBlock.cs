using System;
using System.Collections.Generic;

// é um bloco de informações de controle a respeito de um processo,
// ele basicamente representa um processo;
public class ProcessControlBlock
{
    public Dictionary<string, int> Registradores { get; set; }

    public string ProcessId { get; }

    public State State { get; set; }

    public int Pc { get; set; }

    public int OffSet { get; set; }

    public int EnderecoLimite { get; set; }

    public ProcessControlBlock(string processId)
    {

        Registradores = new Dictionary<string, int>();

        Registradores.Add("r0", 0);
        Registradores.Add("r1", 0);
        Registradores.Add("r2", 0);
        Registradores.Add("r3", 0);
        Registradores.Add("r4", 0);
        Registradores.Add("r5", 0);
        Registradores.Add("r6", 0);
        Registradores.Add("r7", 0);

        this.ProcessId = processId;

        State = State.READY;
    }
}

public enum State
{
    READY,
    RUNNING,
    WAITING,
    HALTED
}