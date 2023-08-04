using System.Collections;
using System.Text;

namespace MCSSP.NBT;
public class TagList : Tag, IList<Tag>
{
    private List<Tag> _Tags {get;} = new List<Tag>();
    public Tag this[int index] { get => _Tags[index]; set => _Tags[index] = value; }
    public int Count => _Tags.Count;
    public bool IsReadOnly => false;
    public TagType ContainedType { get => Count == 0 ? TagType.END : _Tags[0].GetTagType(); }
    public void Add(Tag item) {
        if (ContainedType == TagType.END || item.GetTagType() == ContainedType) _Tags.Add(item);
        else throw new ArgumentException("List tag can't contain tags with different type");
    }

    public void Clear() {
        _Tags.Clear();
    }

    public bool Contains(Tag item) {
        return _Tags.Contains(item);
    }

    public void CopyTo(Tag[] array, int arrayIndex) {
        _Tags.CopyTo(array, arrayIndex);
    }

    public IEnumerator<Tag> GetEnumerator() {
        return _Tags.GetEnumerator();
    }

    public override TagType GetTagType() {
        return TagType.LIST;
    }

    public int IndexOf(Tag item) {
        return _Tags.IndexOf(item);
    }

    public void Insert(int index, Tag item) {
        if (ContainedType == TagType.END || item.GetTagType() == ContainedType) _Tags.Insert(index, item);
        else throw new ArgumentException("List tag can't contain tags with different type");
    }

    public bool Remove(Tag item) {
        return _Tags.Remove(item);
    }

    public void RemoveAt(int index) {
        _Tags.RemoveAt(index);
    }

    public override void WriteToStream(Stream stream) {
        stream.WriteByte((byte) ContainedType);

        byte[] lenBytes = BitConverter.GetBytes(Count);
        System.Array.Reverse(lenBytes);
        stream.Write(lenBytes);

        foreach (Tag item in _Tags)
        {
            item.WriteToStream(stream);
        }
    }

    public override string ToString(string name)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append($"{base.ToString(name)}({ContainedType}, {Count} entries) " + "{");
        if (Count > 0) builder.AppendLine();
        foreach (Tag tag in _Tags)
        {
            string[] s = tag.ToString().Split('\n');
            foreach (string str in s)
            {
                builder.Append("\t");
                builder.Append(str);
                builder.AppendLine();
            }
        }
        builder.Append("}");
        return builder.ToString();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return _Tags.GetEnumerator();
    }

    public static TagList ReadFromStream(Stream stream)
    {
        TagList list = new();
        TagType type = (TagType) stream.ReadByte();

        byte[] lenBytes = new byte[4];
        stream.Read(lenBytes);
        System.Array.Reverse(lenBytes);
        int len = BitConverter.ToInt32(lenBytes);

        if (len <= 0) return list;

        for (int i = 0; i < len; i++)
        {
            list.Add(Tag.ReadFromStream(type, stream));
        }

        return list;
    }
}