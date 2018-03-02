using System.IO;

public class ISerial
{
    public virtual bool serial(BinaryWriter write) { return true; }
    public virtual bool unserial(BinaryReader read) { return true; }
}
