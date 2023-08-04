namespace MCSSP.NBT;

public class TagByte : Tag
{

    public byte Value {get;set;}

    public TagByte(byte v) {
        Value = v;
    }

    public override TagType GetTagType() {
        return TagType.BYTE;
    }

    public override void WriteToStream(Stream stream) {
        stream.WriteByte(Value);
    }

    public override string ToString(string name)
    {
        return $"{base.ToString(name)}: {Value}";
    }

    public static TagByte ReadFromStream(Stream stream) {
        return new TagByte((byte)stream.ReadByte());
    }
}