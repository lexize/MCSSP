using MCSSP.Protocol;

namespace MCSSP.Data;

public interface PacketSerializable {
    public void Write(PacketBuffer buffer);
}