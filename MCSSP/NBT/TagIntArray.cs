namespace MCSSP.NBT;

public class TagIntArray : Tag
{
    public int[] Array {get; private set;}

    public TagIntArray(int capacity) : this(new int[capacity]) {}

    public TagIntArray(int[] array) {
        Array = array;
    }

    public override TagType GetTagType()
    {
        return TagType.INT_ARRAY;
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

    public static TagIntArray ReadFromStream(Stream stream) {
        byte[] lenBytes = new byte[4];
        stream.Read(lenBytes);
        System.Array.Reverse(lenBytes);
        int len = BitConverter.ToInt32(lenBytes);
        int[] arr = new int[len];

        for (int i = 0; i < len; i++) {
            byte[] valBytes = new byte[4];
            stream.Read(valBytes);
            System.Array.Reverse(valBytes);
            int val = BitConverter.ToInt32(valBytes);
            arr[i] = val;
        }

        return new(arr);
    }
}