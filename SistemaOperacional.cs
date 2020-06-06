using System;
using System.Collections.Generic;
public class SistemaOperacional
{
  public CPU cpu;

  public GerenteDeProcesso GerenteProcesso;

  public FilaDeProcesso FilaProntos;
  public SistemaOperacional(int numeroParticoes)
  {
    cpu = new CPU();
    FilaProntos = new FilaDeProcesso();
    //tem o gerente de memoria dentro
    GerenteProcesso = new GerenteDeProcesso(numeroParticoes, FilaProntos);

    int exit;
    do
    {
      Console.WriteLine("Digite 0 para sair");
      exit = Convert.ToInt32(Console.ReadLine());
    }
    while (exit != 0);

  }

  public void Start()
  {
    GerenteProcesso.LoadPrograms();

    TimeSliceExecution();
  }



//   public void TimeSliceExecution()
//   {
//     //Queue <ProcessControlBlock> fila = new Queue <ProcessControlBlock>();
//     Queue<ProcessControlBlock> fila = GerenteProcesso.Fila;
//     ProcessControlBlock pcb;

//     Console.WriteLine("\nInicio da execução em time-slice");
//     Console.WriteLine("---------------------------------");

//     // retira um de cada vez da fila e roda na CPU de acordo com a fatia de tempo
//     while (fila.Count != 0)
//     {
//       pcb = fila.Dequeue();
//       cpu.NewCPU(pcb);

//       if (pcb.State == State.WAITING)
//       {
//         fila.Enqueue(pcb);
//       }

//       Console.WriteLine($"Process Id: {pcb.ProcessID} ; State: {pcb.State}\n");
//     }

//     // no final de tudo é printado o valor dos registradores dos PCBs e
//     // as devidas posições de memórias ocupadas
//     for (int i = 0; i < GerenteProcesso.PCBs.Length; i++)
//     {
//       pcb = GerenteProcesso.PCBs[i];

//       if (pcb != null)
//       {
//         PrintRegistradoresEMemoria(pcb);
//       }
//     }
//   }

  // ve se da pra trocar o length por NumeroParticoes (static)


  public void PrintRegistradoresEMemoria(ProcessControlBlock pcb)
  {
    Console.WriteLine("--------------------------------------------");
    Console.WriteLine("--------------------------------------------");
    Console.WriteLine($"DADOS DO PROCESSO: {pcb.ProcessID}\n\n");

    Console.WriteLine("Valores finais dos registradores:\n");

    foreach (var item in pcb.Registradores)
    {
      Console.WriteLine("--------------------------------------------\n");

      Console.WriteLine($"Registrador [{item.Key}] - valor final [{item.Value}]\n");
    }

    Console.WriteLine("--------------------------------------------\n");

    Console.WriteLine("Status finais das posições de memória (não nulas) da particao\n");

    for (int i = pcb.OffSet; i < pcb.EnderecoLimite; i++)
    {
      if (GerenteDeMemoria.Memoria[i] == null)
      {
        continue;
      }

      Console.WriteLine($"Posição de memória [{i}]:");
      Console.WriteLine(GerenteDeMemoria.Memoria[i].ToString() + "\n");
    }
  }
}