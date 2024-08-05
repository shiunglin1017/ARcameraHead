using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if !UNITY_EDITOR
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif

public class Oriantation : MonoBehaviour
{
    public static string messageString; // 全域變數來存儲接收到的訊息

    // Start is called before the first frame update
    async void Start()
    {
        Debug.Log("ServerReader Start");
        await UpdateLoopAsync();
    }

    async Task UpdateLoopAsync()
    {
        while (true)
        {
            await UpdateAsync();
            await Task.Delay(50); // 等待0.05秒後再重新執行
        }
    }

    async Task UpdateAsync()
    {
        try
        {
#if UNITY_EDITOR
            await ConnectUnityAsync("172.20.10.5", 5678);
#else
            await ConnectUWPAsync("172.20.10.5", 5678);
#endif
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }

#if UNITY_EDITOR
    async Task ConnectUnityAsync(string host, int port)
    {
        IPAddress ip = IPAddress.Parse(host);
        IPEndPoint iPEndPoint = new IPEndPoint(ip, port);

        using Socket client = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        await client.ConnectAsync(iPEndPoint);

        var buffer = new byte[1024];
        int received = await client.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);

        messageString = Encoding.UTF8.GetString(buffer, 0, received); // 存儲接收到的訊息到全域變數
        Debug.Log(messageString);
    }
#else
    async Task ConnectUWPAsync(string host, int port)
    {
        using StreamSocket socket = new StreamSocket();
        HostName serverHost = new HostName(host);

        await socket.ConnectAsync(serverHost, port.ToString());

        DataReader reader = new DataReader(socket.InputStream);
        reader.InputStreamOptions = InputStreamOptions.Partial;

        uint bufferLength = 1024;
        uint loadedBytes = await reader.LoadAsync(bufferLength);
        byte[] buffer = new byte[loadedBytes];
        reader.ReadBytes(buffer);

        messageString = Encoding.UTF8.GetString(buffer, 0, (int)loadedBytes); // 存儲接收到的訊息到全域變數
        Debug.Log(messageString);
    }
#endif
}
