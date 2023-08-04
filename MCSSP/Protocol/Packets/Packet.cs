namespace MCSSP.Protocol.Packets;

public abstract class Packet {
    public abstract int PacketID {get;}
    public virtual void Read(PacketBuffer buffer) {}
    public virtual void Write(PacketBuffer buffer) {}

    public void Send(ProtocolClient client) {
        MemoryStream ms = new();
        PacketBuffer tempBuffer = new(ms);
        tempBuffer.WriteVarInt(PacketID);
        Write(tempBuffer);
        int len = (int) ms.Length;
        client.Buffer.WriteVarInt(len);
        ms.CopyTo(client.Buffer.SourceStream, len);
        ms.Close();
        client.Buffer.Send();
    }

    public enum Side : byte {
        /// <summary>
        /// Sent to Client
        /// </summary>
        CLIENT = 0x01,
        /// <summary>
        /// Sent to Server
        /// </summary>
        SERVER = 0x02
    }
}