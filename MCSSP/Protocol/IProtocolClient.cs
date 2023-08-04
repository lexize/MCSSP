using System.Net.Sockets;
using MCSSP.Protocol.Packets;

namespace MCSSP.Protocol;

public interface IProtocolClient<T> where T: IProtocolClient<T>, new() {
    public void Setup(ProtocolListener<T> listener, TcpClient client);
    public ProtocolListener<T> ParentListener {get;}
    public TcpClient Client {get;}
    public PacketBuffer Buffer {get;}
    public PacketContainer.Stage Stage {get;}
}