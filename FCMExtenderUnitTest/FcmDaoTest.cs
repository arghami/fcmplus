using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using fcm.dao;
using System.Collections.Generic;
using fcm.model;

namespace FCMExtenderUnitTest
{
    [TestClass]
    public class FcmDaoTest
    {
        [TestMethod]
        public void TestGetCompetizioni()
        {
            string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filename = basePath + @"\Materdei League 2016-1-2016.fcm";
            using (FcmDao dao = new FcmDao(filename))
            {
                string[] comps = dao.getListaCompetizioni();
                Assert.IsTrue(comps.Length > 0);
                Assert.AreEqual(comps[0], "1 - Campionato");
            }
        }

        [TestMethod]
        public void TestGiocaIn()
        {
            string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filename = basePath + @"\Materdei League 2016-1-2016.fcm";
            using (FcmDao dao = new FcmDao(filename))
            {
                List<int> idGiocatori = new List<int>();
                idGiocatori.Add(59);

                Dictionary<int, FCMData> data = dao.getGiocaIn(idGiocatori, 34);
                Assert.IsTrue(data.Count==1);
                FCMData dat = data[59];
                Assert.AreEqual(7, dat.voto1);
                Assert.AreEqual(7,5, dat.voto2);
                Assert.AreEqual(7, dat.voto3);
                Assert.AreEqual(true, dat.golvittoria);
                Assert.AreEqual(false, dat.golpareggio);
                Assert.AreEqual(1, dat.golfatti1);
                Assert.AreEqual(1, dat.golfatti2);
                Assert.AreEqual(1, dat.golfatti3);
            }
        }
    }
}
