using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebServerProgram;

public static class StreamIO
{
    public static async Task<string> read(NetworkStream stream)
    {
        // 2kb 단위로 읽기
        byte[] buffer = new byte[2048];
        StringBuilder message = new StringBuilder();
        if (stream.CanRead)
        {
            while (stream.DataAvailable)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                message.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            }
        }
        return message.ToString();
    }

    public static async void write(NetworkStream stream, string content)
    {
        byte[] writeBuffer = Encoding.UTF8.GetBytes(content);
        await stream.WriteAsync(writeBuffer, 0, writeBuffer.Length);
    }
}