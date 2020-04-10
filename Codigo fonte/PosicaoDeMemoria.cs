using System;

public class PosicaoDeMemoria
{
    public string OPCode { get; set; }

    public string Reg1 { get; set; }

    public string Reg2 { get; set; }

    public int Parameter { get; set; }

    public override string ToString()
    {
        return $"OPCode: {OPCode}\nRegistrador 1: {Reg1}\nRegistrador 2: {Reg2}\nParameter: {Parameter}";
    }

}