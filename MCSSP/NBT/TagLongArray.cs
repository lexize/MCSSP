namespace MCSSP.NBT;

public class TagLongArray : Tag
{
    public long[] Array {get; private set;}

    public TagLongArray(int capacity) : this(new long[capacity]) {}

    public TagLongArray(long[] array) {
        Array = array;
    }

    public override TagType GetTagType()
    {
        return TagType.LONG_ARRAY;
    }

    public override void WriteToStream(Stream stream)
    {
        byte[] lenBytes = BitConverter.GetBytes(Array.Length);
        System.Array.Reverse(lenBytes);
        stream.Write(lenBytes);

        for (int i = 0; i < Array.Length; i++) {
            byte[] valBytes = BitConverter.GetBytes(Array[i]);
            System.Array.Reverse(valBytes);
            stream.Write(valBytes);
        }
    }

    public override string ToString(string name)
    {
        return $"{base.ToString(name)}: {Array.Length} entries";
    }

    public static TagLongArray ReadFromStream(Stream stream) {
        byte[] lenBytes = new byte[4];
        stream.Read(lenBytes);
        System.Array.Reverse(lenBytes);
        int len = BitConverter.ToInt32(lenBytes);
        long[] arr = new long[len];

        for (int i = 0; i < len; i++) {
            byte[] valBytes = new byte[8];
            stream.Read(valBytes);
            System.Array.Reverse(valBytes);
            long val = BitConverter.ToInt64(valBytes);
            arr[i] = val;
        }

        return new(arr);
    }
}