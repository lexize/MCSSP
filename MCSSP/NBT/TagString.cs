using System.Text;

namespace MCSSP.NBT;

public class TagString : Tag
{
    private string _value;

    public TagString(string val) {
        _value = val;
    }

    public string Value {get => _value; set {
        if (value.Length > ushort.MaxValue) throw new ArgumentException($"Lenght of string cant be more than {ushort.MaxValue}");
        _value = value;
    }}
    public override TagType GetTagType()
    {
        return TagType.STRING;
    }

    public override void WriteToStream(Stream stream)
    {
        stream.WriteByte((byte) ((_value.Length >> 8) & 0xFF));
        stream.WriteByte((byte) (_value.Length & 0xFF));

        stream.Write(Encoding.UTF8.GetBytes(_value));
    }

    public override string ToString(string name)
    {
        return $"{base.ToString(name)}: \"{_value}\"";
    }

    public static TagString ReadFromStream(Stream stream) {
        int stringLength = ((stream.ReadByte() << 8) + (stream.ReadByte()));
        byte[] stringBytes = new byte[stringLength];
        stream.Read(stringBytes, 0, stringLength);
        string s = Encoding.UTF8.GetString(stringBytes, 0, stringLength);
        return new(s);
    }
}