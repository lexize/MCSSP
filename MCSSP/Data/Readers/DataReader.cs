using MCSSP.Protocol;

namespace MCSSP.Data.Readers;

public interface DataReader<T> {
    public T Read(PacketBuffer buffer);
}