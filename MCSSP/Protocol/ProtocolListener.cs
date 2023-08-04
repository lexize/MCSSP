using System.Net;
using System.Net.Sockets;
using System.Reflection;
using MCSSP.Protocol.Packets;

namespace MCSSP.Protocol;

public class ProtocolListener<T> where T: IProtocolClient<T>, new() {
    public delegate void AnyPacketReceived(T client, Packet packet);
    public delegate void PacketReceived<P>(T client, P packet) where P : Packet;
    private TcpListener _listener;
    public List<T> ConnectedClients {get;} = new();
    public PacketContainer Container {get;} = new();
    public event AnyPacketReceived OnAnyPacketReceived;
    public int NetTicksPerSecond {get; set;} = 100;
    private bool _running;
    public bool Running => _running;
    #pragma warning disable CS8618
    public ProtocolListener(int port) {
        _listener = new(IPAddress.Any, port);
    }


    public void Start() {
        _listener.Start();
        _running = true;
        Thread mainLoopThread = new Thread(MainLoop);
    }

    private void MainLoop() {
        while (_running)
        {
            DateTime dtStart = DateTime.Now;
            while (_listener.Pending()) {
                T client = new();
                client.Setup(this, _listener.AcceptTcpClient());
                ConnectedClients.Add(client);
            }
            foreach (T client in ConnectedClients)
            {
                if (client.Client.Available > 0) {
                    int len = client.Buffer.ReadVarInt();
                    int packetId = client.Buffer.ReadVarInt();
                    Packet? p = Container.TryReadPacket(client.Buffer, client.Stage, Packet.Side.SERVER, packetId);
                    if (p != null && OnAnyPacketReceived != null) {
                        OnAnyPacketReceived(client, p);
                    }
                }
            }
            TimeSpan ts = DateTime.Now - dtStart;
            double operationTime = (1d / NetTicksPerSecond);
            if (ts.TotalMilliseconds < operationTime) Thread.Sleep((int)(operationTime - ts.TotalMilliseconds));
        }
        _listener.Stop();
    }

    public void Stop() {
        _running = false;
    }
}