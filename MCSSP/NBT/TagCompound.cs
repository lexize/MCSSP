using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MCSSP.NBT;
public class TagCompound : Tag, IDictionary<string, Tag>
{
    private Dictionary<string, Tag> _compoundData;
    public TagCompound(Dictionary<string, Tag> tagDictionary) {
        _compoundData = tagDictionary;

    }
    public TagCompound() : this(new()) {}
    public ICollection<string> Keys => _compoundData.Keys;
    public ICollection<Tag> Values => _compoundData.Values;
    public int Count => _compoundData.Count;
    public bool IsReadOnly => false;

    public Tag this[string s] {
        get => _compoundData[s];
        set {
            if (s.Length > ushort.MaxValue) throw new ArgumentOutOfRangeException($"Lenght of tag name cant be more than {ushort.MaxValue}");
            _compoundData[s] = value;
        }
    }

    public override TagType GetTagType() {
        return TagType.COMPOUND;
    }

    public override void WriteToStream(Stream stream) {

        foreach(var kv in _compoundData) {
            Tag tag = kv.Value;
            string key = kv.Key;
            tag.WriteWithPrefix(stream, key);
        }

        stream.WriteByte((byte) TagType.END);
    }

    public void Add(string key, Tag value) {
        this[key] = value;
    }

    public bool ContainsKey(string key) {
        return _compoundData.ContainsKey(key);
    }

    public bool Remove(string key) {
        return _compoundData.Remove(key);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out Tag value) {
        Tag? t = _compoundData[key];
        if (t == null) {
            value = null;
            return false;
        }
        value = t;
        return true;
    }

    public void Add(KeyValuePair<string, Tag> item) {
        Add(item.Key, item.Value);
    }

    public void Clear() {
        _compoundData.Clear();
    }

    public bool Contains(KeyValuePair<string, Tag> item) {
        return _compoundData[item.Key] == item.Value;
    }

    public void CopyTo(KeyValuePair<string, Tag>[] array, int arrayIndex) {
        var col = ((ICollection<KeyValuePair<string, Tag>>)_compoundData);
        col.CopyTo(array, arrayIndex); 
    }

    public bool Remove(KeyValuePair<string, Tag> item) {
        return _compoundData.Remove(item.Key);
    }

    public IEnumerator<KeyValuePair<string, Tag>> GetEnumerator() {
        return _compoundData.GetEnumerator();
    }

    public override string ToString(string name)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append($"{base.ToString(name)}" + "{");
        if (Count > 0) builder.AppendLine();
        foreach (KeyValuePair<string, Tag> kv in _compoundData)
        {
            string[] s = kv.Value.ToString(kv.Key).Split('\n');
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
        return _compoundData.GetEnumerator();
    }

    public static TagCompound ReadFromStream(Stream stream) {
        Dictionary<string, Tag> tags = new();
        while(true) {
            KeyValuePair<string, Tag>? t = Tag.ReadByPrefix(stream);
            if (t == null) break;
            tags[t.Value.Key] = t.Value.Value;
        }
        return new TagCompound(tags);
    }
}