using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using utils.logging;

namespace net.downloader
{
    public class Downloader
    {
        private List<MyUrl> urlsParametrici = new List<MyUrl>();
        private List<MyUrl> urlsStatici = new List<MyUrl>();
        private bool staticiScaricati = false;
        private string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public void getUrls()
        {
            string[] filePaths = Directory.GetFiles(basePath + @"\script", "*.remote", SearchOption.TopDirectoryOnly);
            Logger.log("Trovati "+filePaths.Length+" script remote");
            foreach (var fName in filePaths)
            {
                using (FileStream fs = new FileStream(fName, FileMode.Open))
                {
                    StreamReader reader = new StreamReader(fs);
                    string row;
                    while ((row = reader.ReadLine()) != null)
                    {
                        if (row.Contains("[GG]"))
                        {
                            urlsParametrici.Add(new MyUrl(row, fName));
                        }
                        else
                        {
                            urlsStatici.Add(new MyUrl(row, fName));
                        }
                    }
                }
            }
        }

        public void download(int giornata)
        {
            using (var client = new WebClient()) {
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                foreach (var url in urlsParametrici)
                {
                    string endpoint = url.url.Replace("[GG]", ""+giornata);
                    Logger.log("Downloading " + endpoint);
                    string filename = url.localName+endpoint.Split('/')[endpoint.Split('/').Length-1];
                    client.DownloadFile(endpoint, basePath+@"\data\" + filename);
                }
                if (!staticiScaricati)
                {
                    foreach (var url in urlsStatici)
                    {
                        Logger.log("Downloading " + url.url);
                        string filename = url.localName + url.url.Split('/')[url.url.Split('/').Length - 1];
                        client.DownloadFile(url.url, basePath + @"\data\" + filename);
                    }
                    staticiScaricati = true;
                }
            }
        }

        public void cleanDataDir()
        {
            DirectoryInfo di = new DirectoryInfo(basePath + @"\data");
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }
    }

    class MyUrl
    {
        public MyUrl(string url, string localName)
        {
            this.url = url;
            this.localName = localName;
        }
        public string url;
        public string localName;
    }
}
