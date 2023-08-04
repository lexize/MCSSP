namespace MCSSP.NBT;

public class TagDouble : Tag
{

    public double Value {get;set;}

    public TagDouble(double v) {
        Value = v;
    }

    public override TagType GetTagType() {
        return TagType.DOUBLE;
    }

    public override void WriteToStream(Stream stream) {
        byte[] doubleBytes = BitConverter.GetBytes(Value);
        Array.Reverse(doubleBytes);
        stream.Write(doubleBytes);
    }

    public override string ToString(string name)
    {
        return $"{base.ToString(name)}: {Value}";
    }

    public static TagDouble ReadFromStream(Stream stream) {
        byte[] doubleBytes = new byte[8];
        stream.Read(doubleBytes, 0, 8);
        Array.Reverse(doubleBytes);
        return new(BitConverter.ToDouble(doubleBytes));
    }
}