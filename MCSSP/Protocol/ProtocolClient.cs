using System.Net.Sockets;
using MCSSP.Protocol.Packets;

namespace MCSSP.Protocol;

public class ProtocolClient : IProtocolClient<ProtocolClient> {
    public delegate void PacketReceived(ProtocolClient client, int packetLength, int packetId);
    private TcpClient _client;
    public ProtocolListener<ProtocolClient> ParentListener {get; private set;}
    public TcpClient Client => _client;
    private PacketBuffer _buffer;
    public PacketBuffer Buffer => _buffer;

    public PacketContainer.Stage Stage {get; set;} = PacketContainer.Stage.HANDSHAKE;

    public PacketReceived OnPacketReceived;

    #pragma warning disable CS8618
    public ProtocolClient() {

    }

    public void Setup(ProtocolListener<ProtocolClient> listener, TcpClient client)
    {
        _client = client;
        _buffer = new(client.GetStream());
        ParentListener = listener;
    }
}