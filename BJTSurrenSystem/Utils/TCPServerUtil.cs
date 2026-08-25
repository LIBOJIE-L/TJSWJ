using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BJTSurrenSystem.Utils
{
    public class TCPServerUtil
    {
        Main main;
        public string tempIp, tempPort, tempValue = "";
        // 保存客户端发送过来的数据
        public string data = "";

        public TCPServerUtil(Main main, string ip, int port) {
            this.main = main;

            tempIp = ip;
            tempPort = port + "";

            // 创建一个线程用于启动TCP服务器
            Thread serverThread = new Thread(() => StartServer(IPAddress.Parse(ip), port));
            serverThread.Start();

        }

        public void StartServer(IPAddress ipAddress, int port)
        {
            // 创建TcpListener实例
            TcpListener listener = new TcpListener(ipAddress, port);

            try
            {
                // 启动服务器
                listener.Start();
                main.outDiary($"TCP服务器已启动，IP:{tempIp}:{tempPort}，等待连接...", "信息");

                while (true)
                {
                    // 接受客户端连接
                    TcpClient client = listener.AcceptTcpClient();
                    main.outDiary($"TCP客户端已连接，IP:{client.Client.RemoteEndPoint.ToString()}", "信息");

                    // 创建一个线程来处理连接
                    Thread clientThread = new Thread(() => HandleClient(client));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                main.outDiary($"TCP服务器启动失败，IP:{tempIp}，端口:{tempPort}，{ex.Message}", "错误");
            }
            finally
            {
                // 停止服务器
                listener.Stop();
            }
        }

        public void HandleClient(TcpClient client)
        {
            try
            {
                // 获取客户端的网络流
                NetworkStream stream = client.GetStream();

                // 阻塞等待客户端发送的消息
                byte[] buffer = new byte[1024];
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    main.outDiary($"接收到TCP客户端消息，消息内容:{message}", "信息");
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        data = message;
                    }
                    // 在此处处理消息，可以进行回复等操作

                    // 清空缓冲区
                    Array.Clear(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                main.outDiary($"监听TCP客户端消息异常，{ex.Message}", "错误");
            }
            finally
            {
                main.outDiary($"断开TCP客户端连接，IP:{client.Client.RemoteEndPoint.ToString()}", "信息");
                // 关闭连接
                if (client != null)
                {
                    client.Close();
                }
            }
        }

        // 使用方法：需要再异步方法或者线程中调用
        /*string data = "";
        TCPServerUtil tcpServer = new TCPServerUtil(main, "127.0.0.1", 5002);
        using (var cts = new CancellationTokenSource(10000))
        {
            while (true)
            {
                data = tcpServer.data;
                if (!string.IsNullOrWhiteSpace(data) || cts.IsCancellationRequested)
                {
                    break;
                }
                Thread.Sleep(100);
            }
        }
        Console.WriteLine(data);*/

    }
}
