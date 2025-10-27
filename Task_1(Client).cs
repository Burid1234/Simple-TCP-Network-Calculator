using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Task3
{
    public static async Task SimpleArithServer()
    {
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1"); 
        IPEndPoint ipEndPoint = new(ipAddress, 11_000);

        using Socket listener = new(
            ipEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

        listener.Bind(ipEndPoint);
        listener.Listen(100);

        DataTable d = new DataTable();
        Console.WriteLine("Server is listening on 127.0.0.1:11000...");
        
        var handler = await listener.AcceptAsync();
        Console.WriteLine("Client connected!");

        while (true)
        {
            var buffer = new byte[1_024];
            var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
            if (received == 0)
            {
                Console.WriteLine("Client disconnected.");
                break;
            }
            var response = Encoding.UTF8.GetString(buffer, 0, received);
            Console.WriteLine($"Server received: \"{response}\"");

            string ackMessage;
            try
            {
                object x = d.Compute(response, null); 
                ackMessage = x.ToString() ?? "Calculation Error"; 
                Console.WriteLine($"Server calculated answer: \"{x}\"");
            }
            catch { ackMessage = "Unsupported expression"; }

            ackMessage = ackMessage + "<ACK>"; 
            var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
            await handler.SendAsync(echoBytes, 0);
            Console.WriteLine($"Server sent: \"{ackMessage}\"");
        }
    }

    //  CLIENT CODE (สำหรับ PC)
    public static async Task SimpleArithClient()
    {
        IPAddress ipAddress = IPAddress.Parse("192.168.1.112"); // <-- ใส่ IP ของ Mac ที่จดไว้
        IPEndPoint ipEndPoint = new(ipAddress, 11_000); 

        using Socket client = new(
            ipEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);


        Console.WriteLine("Client is connecting to 192.168.1.112:11000..."); // <-- ใส่ IP ของ Mac
        await client.ConnectAsync(ipEndPoint);
        Console.WriteLine("Client connected!");
        Console.WriteLine("Please enter a mathematical expression (or 'quit' to exit)");

        while (true)
        {
            Console.Write(">> ");
            string? message = Console.ReadLine();
            if (string.IsNullOrEmpty(message)) { continue; }
            if (message.IndexOf("quit") >= 0) { break; }

            var messageBytes = Encoding.UTF8.GetBytes(message);
            _ = await client.SendAsync(messageBytes, SocketFlags.None);
            Console.WriteLine($"Client sent: \"{message}\"");

            var buffer = new byte[1_024];
            var received = await client.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, received);

            var i = response.IndexOf("<ACK>"); 
            if (i > -1)
            {
                response = response.Substring(0, i); 
            }
            
            Console.WriteLine($"Server answer: {response}"); 
        }
        client.Shutdown(SocketShutdown.Both);
        Console.WriteLine("Disconnected from server.");
    }
}
