using System;
using System.Threading;

public class Escalonador
{
    public void Escalona()
    {
        
        ProcessControlBlock pcb = FilaDeProntos.DequeueProcess();

    }
}