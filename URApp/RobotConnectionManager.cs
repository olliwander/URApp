using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;

public class RobotConnectionManager
{
    private TcpClient client;
    private NetworkStream stream;

    public bool IsConnected => client?.Connected ?? false;

    public async Task ConnectAsync(string ipAddress, int port)
    {
        if (!IPAddress.TryParse(ipAddress, out _))
        {
            throw new ArgumentException("Invalid IP address.");
        }

        if (port < 1 || port > 65535)
        {
            throw new ArgumentException("Invalid port number.");
        }

        if (client != null && IsConnected)
        {
            await DisconnectAsync(); // Ensure we are disconnected before trying to connect again
        }

        client = new TcpClient();
        await client.ConnectAsync(ipAddress, port);
        stream = client.GetStream();
    }

    public async Task DisconnectAsync()
    {
        if (stream != null)
        {
            await stream.FlushAsync();
            stream.Close();
            stream = null;
        }
        if (client != null)
        {
            client.Close();
            client = null;
        }
    }

    public bool SendCommand(string command)
    {
        if (stream != null && IsConnected)
        {
            byte[] data = Encoding.ASCII.GetBytes(command);
            stream.Write(data, 0, data.Length);
            return true;
        }
        return false;
    }

    // Additional event or delegate for connection status updates can be added here.
}
