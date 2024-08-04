using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Mes : MonoBehaviour
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
            IPAddress[] ips = await Dns.GetHostAddressesAsync(Dns.GetHostName());
            IPAddress ip = IPAddress.Parse("172.20.10.5");
            IPEndPoint iPEndPoint = new IPEndPoint(ip, 1234);

            using Socket client = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            await client.ConnectAsync(iPEndPoint);

            var buffer = new byte[1024];
            int received = await client.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);

            messageString = Encoding.UTF8.GetString(buffer, 0, received); // 存儲接收到的訊息到全域變數
            Debug.Log(messageString);
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }
}
