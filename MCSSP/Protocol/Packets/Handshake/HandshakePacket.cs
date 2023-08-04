namespace MCSSP.Protocol.Packets.Handshake;

[PacketRef(0x00, PacketContainer.Stage.HANDSHAKE, Side.SERVER)]
public class HandshakePacket : Packet {
    public int ProtocolVersion {get; private set;}
    #pragma warning disable CS8618
    public string ServerAddress {get; private set;}
    public ushort ServerPort {get; private set;}
    public PacketContainer.Stage NextStage {get; private set;}

    public override int PacketID => 0x00;

    public override void Read(PacketBuffer buffer)
    {
        ProtocolVersion = buffer.ReadVarInt();
        ServerAddress = buffer.ReadString();
        ServerPort = buffer.ReadUnsignedShort();
        NextStage = (PacketContainer.Stage) buffer.ReadVarInt();
    }

    public override string ToString()
    {
        return $"Handshake: PROTOCOL={ProtocolVersion}; SERVER_ADDRESS={ServerAddress}; SERVER_PORT={ServerPort}; NEXT_STAGE={NextStage}";
    }
}