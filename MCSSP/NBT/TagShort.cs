namespace MCSSP.NBT;

public class TagShort : Tag
{

    public short Value {get;set;}

    public TagShort(short v) {
        Value = v;
    }

    public override TagType GetTagType() {
        return TagType.SHORT;
    }

    public override void WriteToStream(Stream stream) {
        stream.WriteByte((byte)((Value >> 8) & 0xFF));
        stream.WriteByte((byte)(Value & 0xFF));
    }

    public override string ToString(string name)
    {
        return $"{base.ToString(name)}: {Value}";
    }

    public static TagShort ReadFromStream(Stream stream) {
        short val = (short)(((stream.ReadByte() & 0xFF) << 8) + (stream.ReadByte() & 0xFF));
        return new(val);
    }
}