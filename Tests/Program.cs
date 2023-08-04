using System.IO.Compression;
using MCSSP.Protocol;
using MCSSP.Protocol.Packets;
using MCSSP.Protocol.Packets.Handshake;

public class Program {
    public static void Main(string[] args) {
        ProtocolListener<ProtocolClient> listener = new(25565);
        listener.OnAnyPacketReceived += OnPacketReceived;
        listener.Container.AddPacket<HandshakePacket>(PacketContainer.Stage.HANDSHAKE, Packet.Side.SERVER, 0x00);
        listener.Start();
        Console.WriteLine("Server started");
        while (listener.Running)
        {
            
        }
    }

    public static void OnPacketReceived(ProtocolClient client, Packet packet) {
        Console.WriteLine($"Received packet: {packet}");
        if (packet is HandshakePacket hsp) {
            client.Stage = hsp.NextStage;
        }
    }
}