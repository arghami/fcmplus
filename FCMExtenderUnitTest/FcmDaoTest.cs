using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using fcm.dao;

namespace FCMExtenderUnitTest
{
    [TestClass]
    public class FcmDaoTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filename = basePath + @"\Materdei League 2016-1-2016 - Copia.fcm";
            using (FcmDao dao = new FcmDao(filename))
            {
                string[] comps = dao.getListaCompetizioni();
                Assert.IsTrue(comps.Length > 0);
                Assert.AreEqual(comps[0], "1 - Campionato");
            }
        }
    }
}
