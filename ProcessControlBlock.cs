using System;
using System.Collections.Generic;

// é um bloco de informações de controle a respeito de um processo,
// ele basicamente representa um processo;
public class ProcessControlBLock
{
    public Dictionary<string, int> registradores { get; set; }

    public string processId { get;}

    public State State { get; set; }

    public int pc { get; set; }

    public int offSet { get; set; }

    public int enderecoLimite { get; set; }

    public ProcessControlBLock(string processId)
    {

        registradores = new Dictionary<string, int>();

        registradores.Add("r0", 0);
        registradores.Add("r1", 0);
        registradores.Add("r2", 0);
        registradores.Add("r3", 0);
        registradores.Add("r4", 0);
        registradores.Add("r5", 0);
        registradores.Add("r6", 0);
        registradores.Add("r7", 0);

        this.processId = processId;

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