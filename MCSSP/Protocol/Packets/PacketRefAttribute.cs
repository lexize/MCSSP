namespace MCSSP.Protocol.Packets;

[AttributeUsage(AttributeTargets.Class)]
public class PacketRefAttribute : Attribute {
    public PacketContainer.PacketReference PacketReference {get; private set;}
    public PacketRefAttribute(int id, PacketContainer.Stage stage, Packet.Side packetSide) {
        PacketReference = new(stage, packetSide, id);
    }

    public PacketRefAttribute(PacketContainer.PacketReference reference) {
        PacketReference = reference;
    }
}