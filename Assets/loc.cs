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
    private float requestInterval = 5.0f; // 5��ШD�@��
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

            // �ϥ� Newtonsoft.Json �ѪR JSON
            JObject json = JObject.Parse(response);
            string loc = json["loc"].ToString();
            Debug.Log("Location: " + loc);

            // �ѪR�g�n��
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
            IPAddress ip = IPAddress.Parse("172.20.10.5"); // �ؼЦ��A����IP�a�}
            IPEndPoint iPEndPoint = new IPEndPoint(ip, 5678); // �ؼЦ��A�����ݤf��

            using Socket client = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            await client.ConnectAsync(iPEndPoint);

            string message = latitude + "," + longitude; // �n�o�e���T��
            Debug.Log(latitude + "," + longitude);
            byte[] buffer = Encoding.UTF8.GetBytes(message);

            // �Ы� SocketAsyncEventArgs �ó]�m�����ݩ�
            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
            socketEventArg.RemoteEndPoint = iPEndPoint;
            socketEventArg.SetBuffer(buffer, 0, buffer.Length);

            // �ϥ� TaskCompletionSource �ӵ��ݫD�P�B�ާ@����
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
                // �p�G SendAsync ��^ false�A��ܾާ@�w�P�B����
                if (socketEventArg.SocketError == SocketError.Success)
                {
                    tcs.SetResult(true);
                }
                else
                {
                    tcs.SetException(new SocketException((int)socketEventArg.SocketError));
                }
            }

            // ���ݫD�P�B�ާ@����
            await tcs.Task;

            Debug.Log("Message sent: " + message);
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }
}