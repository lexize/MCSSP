using MCSSP.Protocol.Packets;

namespace MCSSP.Protocol;

public class PacketContainer {
    private static Type[] EMPTY_TYPE_ARRAY {get;} = new Type[0];
    private Dictionary<PacketReference, Type> _packetConstructors = new();
    public Packet? TryReadPacket(PacketBuffer buffer, Stage packetStage, Packet.Side side, int packetId) {
        foreach (var kv in _packetConstructors)
        {
            PacketReference reference = kv.Key;
            if (reference.packetStage == packetStage && reference.side == side) {
                Packet? p = Activator.CreateInstance(kv.Value) as Packet;
                if (p != null) {
                    p.Read(buffer);
                    return p;
                }
            }
        }
        return null;
    }

    public void AddPacket<T>(Stage packetStage, Packet.Side packetSide, int packetId) where T: Packet, new() {
        AddPacket(typeof(T), packetStage, packetSide, packetId);
    }

    public void AddPacket(Type packetType, Stage packetStage, Packet.Side packetSide, int packetId) {
        AddPacket(packetType, new(packetStage, packetSide, packetId));
    }

    public void AddPacket<T>(PacketReference reference) where T: Packet, new() {
        AddPacket(typeof(T), reference);
    }

    public void AddPacket(Type packetType, PacketReference reference) {
        if (packetType.IsAssignableTo(typeof(Packet)) && packetType.GetConstructor(EMPTY_TYPE_ARRAY) != null) {
            _packetConstructors.Add(reference, packetType);
        }
    }

    public enum Stage: byte {
        HANDSHAKE = 0x00,
        STATUS = 0x01,
        LOGIN = 0x02,
        PLAY = 0x04
    }

    public struct PacketReference {
        public Stage packetStage;
        public Packet.Side side;
        public int packetId;
        public PacketReference(Stage packetStage, Packet.Side side, int packetId) {
            this.packetStage = packetStage;
            this.side = side;
            this.packetId = packetId;
        }
    }
}