using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;

public class loc : MonoBehaviour
{
    private string apiUrl = "https://ipinfo.io/json";
    private float requestInterval = 5.0f; // 5秒請求一次
    private float nextRequestTime = 0f;
    private float latitude;
    private float longitude;

    void Start()
    {
        Debug.Log("Start");
        StartCoroutine(RequestAndSendLocation());
    }

    void Update()
    {
        if (Time.time >= nextRequestTime)
        {
            StartCoroutine(RequestAndSendLocation());
            nextRequestTime = Time.time + requestInterval;
        }
    }

    IEnumerator RequestAndSendLocation()
    {
        yield return StartCoroutine(GetLocation());
        yield return SendMessageAsync();
    }

    IEnumerator GetLocation()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string response = request.downloadHandler.text;
            Debug.Log(response);

            // 使用 Newtonsoft.Json 解析 JSON
            JObject json = JObject.Parse(response);
            string loc = json["loc"].ToString();
            Debug.Log("Location: " + loc);

            // 解析經緯度
            string[] latLong = loc.Split(',');
            latitude = float.Parse(latLong[0]);
            longitude = float.Parse(latLong[1]);

            Debug.Log("Latitude: " + latitude + ", Longitude: " + longitude);
        }
    }

    async Task SendMessageAsync()
    {
        try
        {
            IPAddress ip = IPAddress.Parse("172.20.10.5"); // 目標伺服器的IP地址
            IPEndPoint iPEndPoint = new IPEndPoint(ip, 5678); // 目標伺服器的端口號

            using Socket client = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            await client.ConnectAsync(iPEndPoint);

            string message = "Latitude: " + latitude + ", Longitude: " + longitude; // 要發送的訊息
            Debug.Log("傳送:Latitude: " + latitude + ", Longitude: " + longitude);
            byte[] buffer = Encoding.UTF8.GetBytes(message);

            // 創建 SocketAsyncEventArgs 並設置相關屬性
            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
            socketEventArg.RemoteEndPoint = iPEndPoint;
            socketEventArg.SetBuffer(buffer, 0, buffer.Length);

            // 使用 TaskCompletionSource 來等待非同步操作完成
            var tcs = new TaskCompletionSource<bool>();

            socketEventArg.Completed += (s, e) => {
                if (e.SocketError == SocketError.Success)
                {
                    tcs.SetResult(true);
                }
                else
                {
                    tcs.SetException(new SocketException((int)e.SocketError));
                }
            };

            if (!client.SendAsync(socketEventArg))
            {
                // 如果 SendAsync 返回 false，表示操作已同步完成
                if (socketEventArg.SocketError == SocketError.Success)
                {
                    tcs.SetResult(true);
                }
                else
                {
                    tcs.SetException(new SocketException((int)socketEventArg.SocketError));
                }
            }

            // 等待非同步操作完成
            await tcs.Task;

            Debug.Log("Message sent: " + message);
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }
}
