using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using fcm.dao;
using fcm.calcolatore;
using System.Collections.Generic;
using fcm.model;
using plus.enhancer;

namespace FCMExtenderUnitTest
{
    [TestClass]
    public class EnhancerHelperTest
    {
        [TestMethod]
        public void TestEnhancer()
        {
            //devio la console su file
            FileStream fs = new FileStream("Test.txt", FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            Console.SetOut(sw);

            string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filename = basePath + @"\Materdei League 2016-1-2016.fcm";
            using (FcmDao dao = new FcmDao(filename))
            {
                Regole regole = dao.getRegoleCompetizione("1");
                List<Fascia> fasceModDifesa = dao.getFasceModificatoreDifesa("1");
                List<Fascia> fasceNumDifensori = dao.getContributoNumeroDifensoriModificatoreDifesa("1");
                List<Fascia> fasceModCentrocampo = dao.getFasceModificatoreCentrocampo("1");
                List<Fascia> fasceGol = dao.getFasceConversioneGol("1");
                for (int gior = 2; gior < 38; gior++)
                {
                    Console.WriteLine("gior: " + gior);
                    List<Incontro> incontri = dao.getIncontri("1", gior);

                    foreach (var inc in incontri)
                    {
                        Console.WriteLine("inc: " + inc.idIncontro);
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
                        Assert.AreEqual(tabCasa.modDifesa, match.squadra1.modSpeciale1);
                        Assert.AreEqual(tabTrasferta.modDifesa, match.squadra2.modSpeciale1);
                    }
                }
            }
        }
    }
}
