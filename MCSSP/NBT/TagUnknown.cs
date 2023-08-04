namespace MCSSP.NBT;

public class TagUnknown : Tag
{
    public override TagType GetTagType()
    {
        return TagType.END;
    }

    public override void WriteToStream(Stream stream)
    {
        throw new NotSupportedException();
    }
}