using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCSSP.Protocol.Packets.Status;

[PacketRef(0x00, PacketContainer.Stage.STATUS, Side.CLIENT)]
public class StatusResponsePacket : Packet
{
    public override int PacketID => 0x00;

    public override void Write(PacketBuffer buffer)
    {
        
    }
}