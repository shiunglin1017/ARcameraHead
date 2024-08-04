using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;

public class oriantation : MonoBehaviour
{
    private string messageString; // 私有成員變數來存儲接收到的訊息

    // 公有只讀屬性，用於讀取 messageString 的值
    public string ReadMes
    {
        get { return messageString; }
    }

    // Start is called before的第一個偵更新
    async void Start()
    {
        Debug.Log("Start");
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
            IPEndPoint iPEndPoint = new IPEndPoint(ip, 5678);

            using Socket client = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            await client.ConnectAsync(iPEndPoint);

            var buffer = new byte[1024];
            int received = await client.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);

            messageString = Encoding.UTF8.GetString(buffer, 0, received); // 存儲接收到的訊息到成員變數

            Debug.Log(messageString);
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }
}
