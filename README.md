##  วิธีการรันโปรแกรม (How to Run)

โปรเจกต์นี้จำเป็นต้องรันโปรแกรม 2 ครั้ง (บน Terminal 2 หน้าต่าง หรือบนคอมพิวเตอร์ 2 เครื่อง)

### A: การทดสอบบนเครื่องเดียว (Localhost)

1.  **แก้ไข `Program.cs`:**
    * ตรวจสอบให้แน่ใจว่าไฟล์ `Program.cs` มีโค้ดสำหรับรันทั้ง Server และ Client
        ```csharp
        var taskArithServer = Task3.SimpleArithServer();
        var taskArithClient = Task3.SimpleArithClient();
        taskArithClient.Wait();
        taskArithServer.Wait();
        ```
2.  **แก้ไข `Task3.cs`:**
    * ใน `SimpleArithServer()` และ `SimpleArithClient()` ต้องตั้งค่า IP เป็น `IPAddress.Parse("127.0.0.1")`
3.  **รันโปรแกรม:**
    * เปิด Terminal ที่โฟลเดอร์โปรเจกต์ แล้วสั่ง:
        ```bash
        dotnet run
        ```
    * โปรแกรมจะทำหน้าที่เป็นทั้ง Server และ Client ในหน้าต่างเดียว

### B: การทดสอบบนคอมพิวเตอร์ 2 เครื่อง (ในวง LAN)

#### บนเครื่อง Server (เช่น Mac)

1.  **แก้ไข `Task3.cs` (Server):**
    * ในเมธอด `SimpleArithServer()` เปลี่ยน IP เป็น `IPAddress.Any`
2.  **แก้ไข `Program.cs` (Server):**
    * คอมเมนต์ส่วน Client ออก ให้รันเฉพาะ Server:
        ```csharp
        var taskArithServer = Task3.SimpleArithServer();
        taskArithServer.Wait();
        // var taskArithClient = Task3.SimpleArithClient();
        // taskArithClient.Wait();
        ```
3.  **รัน Server:**
    * `dotnet run`
    * (อย่าลืมตรวจสอบ IP Address ของเครื่องนี้ และตั้งค่า Firewall ให้อนุญาต Port 11000)

#### บนเครื่อง Client (เช่น PC)

1.  **แก้ไข `Task3.cs` (Client):**
    * ในเมธอด `SimpleArithClient()` เปลี่ยน IP เป็น IP Address จริงของเครื่อง Server (เช่น `IPAddress.Parse("192.168.1.10")`)
2.  **แก้ไข `Program.cs` (Client):**
    * คอมเมนต์ส่วน Server ออก ให้รันเฉพาะ Client:
        ```csharp
        // var taskArithServer = Task3.SimpleArithServer();
        // taskArithServer.Wait();
        var taskArithClient = Task3.SimpleArithClient();
        taskArithClient.Wait();
        ```
3.  **รัน Client:**
    * `dotnet run`
    * คุณจะสามารถป้อนสมการบน Client (PC) และเห็นคำตอบที่ถูกส่งมาจาก Server (Mac) ได้
