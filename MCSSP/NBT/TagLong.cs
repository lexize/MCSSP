namespace MCSSP.NBT;

public class TagLong : Tag
{

    public long Value {get;set;}

    public TagLong(long v) {
        Value = v;
    }

    public override TagType GetTagType() {
        return TagType.LONG;
    }

    public override void WriteToStream(Stream stream) {
        byte[] longBytes = BitConverter.GetBytes(Value);
        Array.Reverse(longBytes);
        stream.Write(longBytes);
    }

    public override string ToString(string name)
    {
        return $"{base.ToString(name)}: {Value}";
    }

    public static TagLong ReadFromStream(Stream stream) {
        byte[] longBytes = new byte[8];
        stream.Read(longBytes, 0, 8);
        Array.Reverse(longBytes);
        return new(BitConverter.ToInt64(longBytes));
    }
}