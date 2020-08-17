using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.downloader;

namespace DownloaderTest
{
    [TestClass]
    public class DownloaderTest
    {
        [TestMethod]
        public void TestDownloader()
        {
            Downloader d = new Downloader();
            d.getUrls();
            for (int i=1; i<39; i++)
            {
                d.download(i);
            }
            d.cleanDataDir();
        }
        
    }
}
