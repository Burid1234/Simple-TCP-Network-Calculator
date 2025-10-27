using System.Data; 
using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Task3
{
        public static async Task SimpleArithServer()
    {
        IPAddress ipAddress = IPAddress.Any;// local host
        IPEndPoint ipEndPoint = new(ipAddress, 11_000); // 11_000 = 11000

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
                // 4. พยายามคำนวณสมการที่ได้รับ 
                object x = d.Compute(response, null); 
                ackMessage = x.ToString() ?? "Calculation Error"; 
                Console.WriteLine($"Server calculated answer: \"{x}\"");
            }
            catch // 5. ถ้าคำนวณไม่ได้ (เช่น พิมพ์ "abc")
            {
                ackMessage = "Unsupported expression"; 
            }

            // 6. "ผนึก" คำตอบด้วย <ACK> เพื่อบอกว่าส่งจบแล้ว 
            ackMessage = ackMessage + "<ACK>"; 

            // 7. ส่ง "คำตอบ+<ACK>" กลับไปให้ Client
            var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
            await handler.SendAsync(echoBytes, 0);
            Console.WriteLine($"Server sent: \"{ackMessage}\"");
        }
    }

    
    public static async Task SimpleArithClient()
    {
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1"); // local host
        IPEndPoint ipEndPoint = new(ipAddress, 11_000); 

        using Socket client = new(
            ipEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

        // 1. พยายามเชื่อมต่อ
        Console.WriteLine("Client is connecting to 127.0.0.1:11000...");
        await client.ConnectAsync(ipEndPoint);
        Console.WriteLine("Client connected!");
        Console.WriteLine("Please enter a mathematical expression (or 'quit' to exit)");

        while (true)
        {
            // 2. รับโจทย์จากคีย์บอร์ด
            Console.Write(">> ");
            string? message = Console.ReadLine();

            // 3. ตรวจสอบว่าต้องส่งข้อมูลหรือไม่
            if (string.IsNullOrEmpty(message))
            {
                continue; // ถ้าไม่ได้พิมพ์อะไร ให้วนกลับไปรับใหม่
            }

            // 4. ถ้าพิมพ์ "quit" ให้ออกจากลูป
            if (message.IndexOf("quit") >= 0)
            {
                break;
            }

            // 5. ส่งโจทย์ (สมการ) ไปให้ Server
            var messageBytes = Encoding.UTF8.GetBytes(message);
            _ = await client.SendAsync(messageBytes, SocketFlags.None);
            Console.WriteLine($"Client sent: \"{message}\"");

            // 6. รอรับ "คำตอบ" กลับมาจาก Server
            var buffer = new byte[1_024];
            var received = await client.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, received);

            // 7. *** จุดแก้ไข BUG ***
            //    เราต้อง "แกะ" เอาเฉพาะคำตอบที่อยู่ก่อน <ACK>
            var i = response.IndexOf("<ACK>"); 
            if (i > -1)
            {
                // แก้ไขจาก .Substring(0, 1) เป็น .Substring(0, i)
                response = response.Substring(0, i); 
            }
            
            // 8. แสดงผลคำตอบ
            Console.WriteLine($"Server answer: {response}"); 
        }

        // 9. ปิดการเชื่อมต่อ
        client.Shutdown(SocketShutdown.Both);
        Console.WriteLine("Disconnected from server.");
    }
}

#สำหรับSERVER
