internal class Program
{
    private static void Main(string[] args)
    {
        var taskArithClient = Task3.SimpleArithClient(); 
        taskArithClient.Wait();   
    }
}

#สำหรับ Client
