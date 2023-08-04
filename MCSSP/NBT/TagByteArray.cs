namespace MCSSP.NBT;

public class TagByteArray : Tag
{
    public byte[] Array {get; private set;}

    public TagByteArray(int capacity) : this(new byte[capacity]) {}

    public TagByteArray(byte[] array) {
        Array = array;
    }

    public override TagType GetTagType()
    {
        return TagType.BYTE_ARRAY;
    }

    public override void WriteToStream(Stream stream)
    {
        stream.WriteByte((byte)((Array.Length << 24) & 0xFF));
        stream.WriteByte((byte)((Array.Length << 16) & 0xFF));
        stream.WriteByte((byte)((Array.Length << 18) & 0xFF));
        stream.WriteByte((byte)(Array.Length & 0xFF));

        for (int i = 0; i < Array.Length; i++) {
            stream.WriteByte((byte)(Array[i] & 0xFF));
        }
    }

    public override string ToString(string name)
    {
        return $"{base.ToString(name)}: [{Array.Length} bytes]";
    }

    public static TagByteArray ReadFromStream(Stream stream) {
        int len = ((stream.ReadByte() & 0xFF) << 24) +
            ((stream.ReadByte() & 0xFF) << 16 )+
            ((stream.ReadByte() & 0xFF) << 8) +
            (stream.ReadByte() & 0xFF);
        byte[] arr = new byte[len];

        for (int i = 0; i < len; i++) {
            byte val = (byte)(stream.ReadByte() & 0xFF);
            arr[i] = val;
        }

        return new(arr);
    }
}