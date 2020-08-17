using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using fcm.dao;
using fcm.calcolatore;
using System.Collections.Generic;
using fcm.model;

namespace FCMExtenderUnitTest
{
    [TestClass]
    public class CalcoliHelperTest
    {
        [TestMethod]
        public void TestCalcolo()
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
                        Match match = helper.calcolaMatch(tabCasa, tabTrasferta, true, true);
                        Assert.AreEqual(inc.parzcasa, match.squadra1.parziale);
                        Assert.AreEqual(inc.parzfuori, match.squadra2.parziale);
                        Assert.AreEqual(inc.totcasa, match.squadra1.getTotale());
                        Assert.AreEqual(inc.totfuori, match.squadra2.getTotale());
                        Assert.AreEqual(inc.golcasa, match.squadra1.numeroGol);
                        Assert.AreEqual(inc.golfuori, match.squadra2.numeroGol);
                    }
                }
            }
        }
    }
}
