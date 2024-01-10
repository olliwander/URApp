using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;

public class RobotConnectionManager
{
    private TcpClient commandClient; // Send kommandoer
    private NetworkStream commandStream; 

    private TcpClient dataClient; // Modtag data
    private NetworkStream dataStream; 

    // Tjekker kommando er forbundet
    public bool IsCommandConnected => commandClient?.Connected ?? false;

    // Tjekker Data er forbundet
    public bool IsDataConnected => dataClient?.Connected ?? false;

    // Port til modtag data
    private const int dataPort = 30004;

    public async Task ConnectAsync(string ipAddress, int commandPort)
    {
        if (!IPAddress.TryParse(ipAddress, out _))
        {
            throw new ArgumentException("Invalid IP address.");
        }

        if (commandPort < 1 || commandPort > 65535)
        {
            throw new ArgumentException("Invalid port number.");
        }

        // Connect the client for sending commands
        commandClient = new TcpClient();
        await commandClient.ConnectAsync(ipAddress, commandPort);
        commandStream = commandClient.GetStream();

        // Connect the client for receiving data
        dataClient = new TcpClient();
        await dataClient.ConnectAsync(ipAddress, dataPort);
        dataStream = dataClient.GetStream();
    }

    public async Task DisconnectAsync()
    {
        // Disconnect the command client
        if (commandStream != null)
        {
            await commandStream.FlushAsync();
            commandStream.Close();
            commandStream = null;
        }
        if (commandClient != null)
        {
            commandClient.Close();
            commandClient = null;
        }

        // Disconnect the data client
        if (dataStream != null)
        {
            await dataStream.FlushAsync();
            dataStream.Close();
            dataStream = null;
        }
        if (dataClient != null)
        {
            dataClient.Close();
            dataClient = null;
        }
    }

    public bool SendCommand(string command)
    {
        if (commandStream != null && IsCommandConnected)
        {
            byte[] data = Encoding.ASCII.GetBytes(command);
            commandStream.Write(data, 0, data.Length);
            return true;
        }
        return false;
    }

    // Method to start listening for data on the data stream
    public async Task<string> ReceiveDataAsync()
    {
        if (dataStream != null && IsDataConnected)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = await dataStream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }
        return null;
    }

    // Additional event or delegate for connection status updates can be added here.
}
