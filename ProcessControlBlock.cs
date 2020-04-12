
// é um bloco de informações de controle a respeito de um processo,
// ele basicamente representa um processo;
public class ProcessControlBLock
{
    public string processId;

    public State State { get; set; }

    public int pc { get; set; }

    public int enderecoLimite { get; set; }
}

public enum State
{
    READY,
    RUNNING
}