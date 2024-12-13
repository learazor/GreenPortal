using System.Net.Sockets;
using System.Text;

namespace GreenPortal.util;

public class DistanceCalc
{
    internal static async Task<double> CalculateDistance(string clientPostalCode, string clientCountry,
        string companyPostalCode, string companyCountry)
    {
        using (var client = new TcpClient())
        {
            Console.WriteLine("Connecting to server...");
            await client.ConnectAsync("127.0.0.1", 5001);
            Console.WriteLine("Connected to server.");

            using (var networkStream = client.GetStream())
            {
                //format: "Country1|PostalCode1|Country2|PostalCode2"
                string message = $"{clientCountry}|{clientPostalCode}|{companyCountry}|{companyPostalCode}";
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);

                await networkStream.WriteAsync(messageBytes, 0, messageBytes.Length);
                Console.WriteLine("Message sent.");

                byte[] buffer = new byte[1024];
                int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                Console.WriteLine("Response from server:");
                Console.WriteLine(response);
                return Double.Parse(response.Substring(0, 7));
            }
        }
    }
}