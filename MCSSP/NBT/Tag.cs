using System.IO;
using System.IO.Compression;
using System.Text;

namespace MCSSP.NBT;
public abstract class Tag {
    private static Dictionary<TagType, Func<Stream, Tag>> _tagReaders {get;} = new();

    static Tag() {
        _tagReaders[TagType.BYTE] = TagByte.ReadFromStream;
        _tagReaders[TagType.SHORT] = TagShort.ReadFromStream;
        _tagReaders[TagType.INT] = TagInt.ReadFromStream;
        _tagReaders[TagType.LONG] = TagLong.ReadFromStream;
        _tagReaders[TagType.FLOAT] = TagFloat.ReadFromStream;
        _tagReaders[TagType.DOUBLE] = TagDouble.ReadFromStream;
        _tagReaders[TagType.BYTE_ARRAY] = TagByteArray.ReadFromStream;
        _tagReaders[TagType.STRING] = TagString.ReadFromStream;
        _tagReaders[TagType.LIST] = TagList.ReadFromStream;
        _tagReaders[TagType.COMPOUND] = TagCompound.ReadFromStream;
        _tagReaders[TagType.INT_ARRAY] = TagIntArray.ReadFromStream;
        _tagReaders[TagType.LONG_ARRAY] = TagLongArray.ReadFromStream;
    }
    
    public abstract void WriteToStream(Stream stream);
    public abstract TagType GetTagType();

    public override string ToString()
    {
        return ToString("");
    }

    public virtual string ToString(string name) {
        return $"TAG_{GetTagType()}('{name}')";
    }

    public void WriteWithPrefix(Stream stream, string name) {
        stream.WriteByte((byte) GetTagType());
        stream.WriteByte((byte) ((name.Length << 8) & 0xFF));
        stream.WriteByte((byte) (name.Length & 0xFF));
        stream.Write(Encoding.UTF8.GetBytes(name));

        WriteToStream(stream);
    }

    public byte[] GetBytes() {
        MemoryStream ms = new MemoryStream();
        WriteToStream(ms);
        byte[] data = ms.GetBuffer();
        ms.Close();
        return data;
    }

    public static Tag ReadFromStream(TagType type, Stream s) {
        return _tagReaders[type](s);
    }

    public static KeyValuePair<string, Tag> ReadByPrefix(Stream stream) {
        TagType t = (TagType)stream.ReadByte();
        if (t == TagType.END) return new("", new TagUnknown());
        int tagNameLength = ((stream.ReadByte() << 8) + (stream.ReadByte()));
        byte[] tagNameBytes = new byte[tagNameLength];
        stream.Read(tagNameBytes, 0, tagNameLength);
        string tagName = Encoding.UTF8.GetString(tagNameBytes, 0, tagNameLength);
        Tag tag = ReadFromStream(t, stream);
        return new(tagName, tag);
    }
}