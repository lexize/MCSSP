using MCSSP.Protocol;

namespace MCSSP.Data.Writers;

public interface DataWriter<T> {
    public void Write(PacketBuffer buffer, T data);
}