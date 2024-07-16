using System;
using System.Net;
using System.Net.NetworkInformation;

namespace WpfApp1.ViewModel
{ 

    public class NetworkInfoRetriever
    {
        public string GetPing(string hostName)
        {
            // Get ping information
            Ping ping = new Ping();
            PingReply reply = ping.Send(hostName);
            if (reply != null)
            {
                return reply.RoundtripTime.ToString() + " MS";
            }
            return "Error: Unable to retrieve ping information";
        }

        public int GetMaxDownloadSpeed()
        {
            
            WebClient client = new WebClient();
            client.Credentials = CredentialCache.DefaultCredentials;
            DateTime startTime = DateTime.Now;
            client.DownloadData("https://www.speedtest.net/");
            DateTime endTime = DateTime.Now;
            TimeSpan timeTaken = endTime - startTime;
            long fileSize = 100 * 1024 * 1024; // 100MB file size
            return (int)((fileSize / 1000000) / (timeTaken.TotalSeconds)); // MB/s
        }

        public int GetMaxUploadSpeed()
        {
            // Get maximum upload speed
            WebClient client = new WebClient();
            client.Credentials = CredentialCache.DefaultCredentials;
            byte[] data = new byte[1024 * 1024]; // 1MB data
            DateTime startTime = DateTime.Now;
            client.UploadData("http://httpbin.org/post", "POST", data);
            DateTime endTime = DateTime.Now;
            TimeSpan timeTaken = endTime - startTime;
            return (int)((data.Length / 1000000) / (timeTaken.TotalSeconds / 1000)); // MB/s
        }

    }
}
    
