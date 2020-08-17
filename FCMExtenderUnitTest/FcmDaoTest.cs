using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using fcm.dao;
using System.Collections.Generic;
using fcm.model;
using fcm.calcolatore;
using plus.enhancer;

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

                Dictionary<int, FCMData> data = dao.getGiocaIn(1, idGiocatori, 34);
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

        [TestMethod]
        public void TestUpdate()
        {
            string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filename = basePath + @"\Materdei League 2016-1-2016.fcm";
            using (FcmDao dao = new FcmDao(filename))
            {
                Regole regole = dao.getRegoleCompetizione("1");
                List<Fascia> fasceModDifesa = dao.getFasceModificatoreDifesa("1");
                List<Fascia> fasceNumDifensori = dao.getContributoNumeroDifensoriModificatoreDifesa("1");
                List<Fascia> fasceModCentrocampo = dao.getFasceModificatoreCentrocampo("1");
                List<Fascia> fasceGol = dao.getFasceConversioneGol("1");
                for (int gior = 2; gior < 36; gior++)
                {
                    List<Incontro> incontri = dao.getIncontri("1", gior);

                    foreach (var inc in incontri)
                    {
                        List<int> idIncontri = new List<int>();
                        idIncontri.Add(inc.idIncontro);
                        Dictionary<int, Tabellino> tabellini = dao.getTabellini(idIncontri);
                        CalcoliHelper helper = new CalcoliHelper(
                            regole,
                            fasceModDifesa,
                            fasceNumDifensori,
                            fasceModCentrocampo,
                            fasceGol);
                        Tabellino tabCasa = tabellini[inc.casa];
                        Tabellino tabTrasferta = tabellini[inc.trasferta];
                        Enhancer.enhanceIncontro(inc, tabCasa, tabTrasferta, null);
                        Match match = helper.calcolaMatch(tabCasa, tabTrasferta, true, true);
                        dao.setDatiIncontro(inc.idIncontro, inc.casa, inc.trasferta, match, new bool[] { true, false, false });
                    }
                }
            }

            using (FcmDao dao = new FcmDao(filename))
            {

                for (int gior = 2; gior < 36; gior++)
                {
                    List<Incontro> incontri = dao.getIncontri("1", gior);

                    foreach (var inc in incontri)
                    {
                        List<int> idIncontri = new List<int>();
                        idIncontri.Add(inc.idIncontro);
                        Dictionary<int, Tabellino> tabellini = dao.getTabellini(idIncontri);
                        Assert.AreEqual(tabellini[inc.casa].modDifesa, tabellini[inc.casa].modPers1);
                        Assert.AreEqual(tabellini[inc.trasferta].modDifesa, tabellini[inc.trasferta].modPers1);
                    }
                }
            }
        }
    }
}
