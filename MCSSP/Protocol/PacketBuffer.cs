using MCSSP.Data;
using MCSSP.Data.Readers;
using MCSSP.Data.Writers;

using System.Text;

namespace MCSSP.Protocol;

public class PacketBuffer {

    private const int VAR_SEGMENT_BITS = 0x7F;
    private const int VAR_CONTINUE_BIT = 0x80;

    public Stream SourceStream {get; private set;}
    public PacketBuffer(Stream sourceStream) {
        SourceStream = sourceStream;
    }
    
    public bool ReadBool() {
        return ReadByte() > 0;
    }

    public byte ReadByte()
    {
        int i = SourceStream.ReadByte();
        if (i < 0) throw new EndOfStreamException();
        return (byte) i;
    }

    public byte[] ReadByteArray() {
        int len = ReadVarInt();
        return ReadNBytes(len);
    }

    public byte[] ReadNBytes(int bytesAmount) {
        byte[] bytes = new byte[bytesAmount];
        for (int i = 0; i < bytesAmount; i++)
        {
            bytes[i] = ReadByte();
        }
        return bytes;
    }

    public int ReadVarInt() {
        int value = 0;
        int position = 0;
        byte currentByte;

        while (true) {
            currentByte = ReadByte();
            value |= (currentByte & VAR_SEGMENT_BITS) << position;

            if ((currentByte & VAR_CONTINUE_BIT) == 0) break;

            position += 7;

            if (position >= 32) throw new InvalidDataException("VarInt is too big");
        }

        return value;
    }

    public long ReadVarLong() {
        long value = 0;
        int position = 0;
        byte currentByte;

        while (true) {
            currentByte = ReadByte();
            value |= (long) (currentByte & VAR_SEGMENT_BITS) << position;

            if ((currentByte & VAR_CONTINUE_BIT) == 0) break;

            position += 7;

            if (position >= 32) throw new InvalidDataException("VarLong is too big");
        }

        return value;
    }

    public sbyte ReadSignedByte() {
        return unchecked((sbyte)ReadSignedByte());
    }

    public short ReadShort() {
        byte[] shortBytes = ReadNBytes(2);
        Array.Reverse(shortBytes);
        return BitConverter.ToInt16(shortBytes);
    }

    public ushort ReadUnsignedShort() {
        byte[] shortBytes = ReadNBytes(2);
        Array.Reverse(shortBytes);
        return BitConverter.ToUInt16(shortBytes);
    }

    public int ReadInt() {
        byte[] intBytes = ReadNBytes(4);
        Array.Reverse(intBytes);
        return BitConverter.ToInt32(intBytes);
    }

    public long ReadLong() {
        byte[] intBytes = ReadNBytes(8);
        Array.Reverse(intBytes);
        return BitConverter.ToInt64(intBytes);
    }

    public string ReadString() {
        int strBytesCount = ReadVarInt();
        byte[] strBytes = ReadNBytes(strBytesCount);
        string str = Encoding.UTF8.GetString(strBytes);
        return str;
    }
    
    public T Read<T>(DataReader<T> reader) {
        return reader.Read(this);
    }

    public T? ReadOptional<T>(DataReader<T> reader) {
        if (ReadBool()) return reader.Read(this);
        return default(T);
    }
    
    public T[] ReadArray<T>(DataReader<T> reader) {
        int arrayLen = ReadVarInt();
        T[] arr = new T[arrayLen];
        for (int i = 0; i < arrayLen; i++)
        {
            arr[i] = reader.Read(this);
        }
        return arr;
    }

    public List<T> ReadList<T>(DataReader<T> reader) {
        int listLen = ReadVarInt();
        List<T> list = new();
        for (int i = 0; i < listLen; i++)
        {
            list.Add(reader.Read(this));
        }
        return list;
    }

    public Dictionary<K, V> ReadDictionary<K,V>(DataReader<K> keyReader, DataReader<V> dataReader) where K : notnull {
        int dicitonarySize = ReadVarInt();
        Dictionary<K, V> dict = new();
        for (int i = 0; i < dicitonarySize; i++)
        {
            dict.Add(keyReader.Read(this), dataReader.Read(this));
        }
        return dict;
    }
    
    public void WriteBool(bool val) {
        SourceStream.WriteByte((byte)(val ? 1 : 0));
    }

    public void WriteByte(byte val) {
        SourceStream.WriteByte(val);
    }
    
    public void WriteByteArray(byte[] bytes) {
        WriteVarInt(bytes.Length);
        foreach (byte b in bytes)
        {
            WriteByte(b);
        }
    }
    
    public void WriteVarInt(int val) {
        while(true) {
            if ((val & ~VAR_SEGMENT_BITS) == 0) {
                WriteByte((byte) val);
                return;
            }

            WriteByte((byte)((val & VAR_SEGMENT_BITS) | VAR_CONTINUE_BIT));

            val >>>= 7;
        }
    }

    public void WriteVarLong(long val) {
        while(true) {
            if ((val & ~VAR_SEGMENT_BITS) == 0) {
                WriteByte((byte) val);
                return;
            }

            WriteByte((byte)((val & VAR_SEGMENT_BITS) | VAR_CONTINUE_BIT));

            val >>>= 7;
        }
    }

    public void WriteSignedByte(sbyte val) {
        SourceStream.WriteByte(unchecked((byte)val));
    }

    public void WriteShort(short val) {
        byte[] data = BitConverter.GetBytes(val);
        Array.Reverse(data);
        SourceStream.Write(data);
    }

    public void WriteUnsignedShort(ushort val) {
        byte[] data = BitConverter.GetBytes(val);
        Array.Reverse(data);
        SourceStream.Write(data);
    }

    public void WriteInt(int val) {
        byte[] data = BitConverter.GetBytes(val);
        Array.Reverse(data);
        SourceStream.Write(data);
    }

    public void WriteLong(long val) {
        byte[] data = BitConverter.GetBytes(val);
        Array.Reverse(data);
        SourceStream.Write(data);
    }

    public void WriteString(string str, int maxLength) {
        if (str.Length > maxLength) {
            throw new ArgumentException($"String length ({str.Length}) is too big, max length is {maxLength}");
        }
        else {
            int maxBytes = maxLength*3;
            byte[] strBytes = Encoding.UTF8.GetBytes(str);
            if (strBytes.Length > maxBytes) throw new ArgumentException($"Encoded data length ({strBytes.Length}) is too big, max length is {maxBytes}");
            WriteVarInt(strBytes.Length);
            WriteByteArray(strBytes);
        }
    }

    public void Write<T>(T serializable) where T: PacketSerializable {
        serializable.Write(this);
    }

    public void Write<T>(T data, DataWriter<T> writer) {
        writer.Write(this, data);
    }

    public void WriteOptional<T>(T? serializable) where T: PacketSerializable {
        if (serializable != null) {
            WriteBool(true);
            serializable.Write(this);
        }
        else WriteBool(false);
    }

    public void WriteOptional<T>(T? data, DataWriter<T> writer) {
        if (data != null) {
            WriteBool(true);
            writer.Write(this, data);
        }
        else WriteBool(false);
    }

    public void WriteArray<T>(T[] array) where T: PacketSerializable {
        WriteVarInt(array.Length);
        foreach (PacketSerializable serializable in array)
        {
            serializable.Write(this);
        }
    }

    public void WriteArray<T>(T[] array, DataWriter<T> writer) {
        WriteVarInt(array.Length);
        foreach (T item in array)
        {
            writer.Write(this, item);
        }
    }

    public void WriteCollection<T>(ICollection<T> list) where T: PacketSerializable {
        WriteVarInt(list.Count);
        foreach (T item in list)
        {
            item.Write(this);
        }
    }

    public void WriteCollection<T>(ICollection<T> list, DataWriter<T> writer) {
        foreach (T item in list)
        {
            writer.Write(this, item);
        }
    }

    public void WriteDictionary<K, V>(IDictionary<K, V> dict, DataWriter<K> keyWriter) where V : PacketSerializable {
        WriteVarInt(dict.Count);
        foreach (KeyValuePair<K, V> kv in dict)
        {
            keyWriter.Write(this, kv.Key);
            kv.Value.Write(this);
        }
    }

    public void WriteDictionary<K, V>(IDictionary<K, V> dict, DataWriter<K> keyWriter, DataWriter<V> valueWriter) {
        WriteVarInt(dict.Count);
        foreach (KeyValuePair<K, V> kv in dict)
        {
            keyWriter.Write(this, kv.Key);
            valueWriter.Write(this, kv.Value);
        }
    }

    public void Send() {
        SourceStream.Flush();
    }
}