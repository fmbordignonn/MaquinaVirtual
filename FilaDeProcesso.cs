using System;
using System.Collections.Generic;
public class FilaDeProcesso
{

  private Queue<ProcessControlBlock> Fila { get; set; }

  public FilaDeProcesso()
  {
    Fila = new Queue<ProcessControlBlock>();
  }

  public void AddProcess(ProcessControlBlock pcb)
  {

    Console.WriteLine($"Adicionou o processo {pcb.ProcessID} a fila de prontos");
    Console.WriteLine("---------------------------------");

    pcb.State = State.READY;
    Fila.Enqueue(pcb);
    
    Console.WriteLine($"Process Id: {pcb.ProcessID} | State: {pcb.State} | Offset: {pcb.OffSet} | EndereçoLimite: {pcb.EnderecoLimite}");
  }
}
