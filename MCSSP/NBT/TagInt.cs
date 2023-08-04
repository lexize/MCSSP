namespace MCSSP.NBT;

public class TagInt : Tag
{

    public int Value {get;set;}

    public TagInt(int v) {
        Value = v;
    }

    public override TagType GetTagType() {
        return TagType.INT;
    }

    public override void WriteToStream(Stream stream) {
        byte[] intBytes = BitConverter.GetBytes(Value);
        Array.Reverse(intBytes);
        stream.Write(intBytes);
    }

    public override string ToString(string name)
    {
        return $"{base.ToString(name)}: {Value}";
    }

    public static TagInt ReadFromStream(Stream stream) {
        byte[] intBytes = new byte[4];
        stream.Read(intBytes, 0, 4);
        Array.Reverse(intBytes);
        return new(BitConverter.ToInt32(intBytes));
    }
}