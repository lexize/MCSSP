namespace MCSSP.NBT;

public class TagFloat : Tag
{

    public float Value {get;set;}

    public TagFloat(float v) {
        Value = v;
    }

    public override TagType GetTagType() {
        return TagType.FLOAT;
    }

    public override void WriteToStream(Stream stream) {
        byte[] floatBytes = BitConverter.GetBytes(Value);
        Array.Reverse(floatBytes);
        stream.Write(floatBytes);
    }

    public override string ToString(string name)
    {
        return $"{base.ToString(name)}: {Value}";
    }

    public static TagFloat ReadFromStream(Stream stream) {
        byte[] floatBytes = new byte[4];
        stream.Read(floatBytes, 0, 4);
        Array.Reverse(floatBytes);
        return new(BitConverter.ToSingle(floatBytes));
    }
}