using plus.enhancer;
using System;
using System.Collections.Generic;
using System.Data.Odbc;

namespace fcm.model
{

    public class Regole
    {

        public int puntiPerVittoria = 3;
        public double fattoreCampo = 0;
        public Boolean regolaPortiere = false;
        public Boolean regolaDifesa = false;
        public Boolean regolaCentMedia = false;
        public Boolean regolaCentDiffe = false;
        public Boolean regolaDiff4 = false;
        public Boolean regolaDiff10 = false;
        public Boolean regolaMin60 = false;
        public Boolean usaSpeciale1 = false;
        public Boolean usaSpeciale2 = false;
        public Boolean usaSpeciale3 = false;
        public Boolean regolaAttacco = false;
        public Boolean regolaDelta3 = false;
        public Boolean regolaMin59 = false;
        public double regolaDiff4Valore = 0.0;
        public double regolaDiff10Valore = 0.0;
        public double regolaMin60Valore = 0.0;
        public double regolaMin60Delta = 0.0;
        public double regolaDelta3Valore = 0.0;
        public double regolaMin59Valore = 0.0;
        public double regolaMin59Delta = 0.0;
        public double regolaMin59Almeno = 0.0;
        public double VUCentrocampista = 0.0;
        public Boolean regolaDifesaVU = false;
        public double VUDifensore = 0.0;
        public int usaTabellino = 0;
        public string nomeSpeciale1;
        public string nomeSpeciale2;
        public string nomeSpeciale3;
        public Dictionary<string, Modulo> moduli = new Dictionary<string, Modulo>();
        public Regole(OdbcConnection conn, string compSel)
        {
            using (OdbcCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT puntipervittoria, fattorecampo, regolaportiere, regoladifesa, regolacentmedia, " +
                    "regolacentdiffe, regoladiff4, regoladiff10, regolamin60, usaspeciale1, " +
                    "usaspeciale2, usaspeciale3, regolaattacco, regoladelta3, regolamin59," +
                    "regolaDiff4Valore, regolaDiff10Valore, regolaMin60Valore, regolaMin60Delta, regolaDelta3Valore, " +
                    "regolaMin59Valore, regolaMin59Delta, regolaMin59Almeno, VUCentrocampista, regolaDifesaVU," +
                    "VUDifensore, usatabellino, nomeSpeciale1, nomeSpeciale2, nomeSpeciale3 FROM competizione WHERE id = " + compSel;
                using (OdbcDataReader rea = cmd.ExecuteReader())
                {
                    while (rea.Read())
                    {
                        puntiPerVittoria = rea.GetInt32(0);
                        fattoreCampo = rea.GetDouble(1);
                        regolaPortiere = rea.GetBoolean(2);
                        regolaDifesa = rea.GetBoolean(3);
                        regolaCentMedia = rea.GetBoolean(4);
                        regolaCentDiffe = rea.GetBoolean(5);
                        regolaDiff4 = rea.GetBoolean(6);
                        regolaDiff10 = rea.GetBoolean(7);
                        regolaMin60 = rea.GetBoolean(8);
                        usaSpeciale1 = rea.GetBoolean(9);
                        usaSpeciale2 = rea.GetBoolean(10);
                        usaSpeciale3 = rea.GetBoolean(11);
                        regolaAttacco = rea.GetBoolean(12);
                        regolaDelta3 = rea.GetBoolean(13);
                        regolaMin59 = rea.GetBoolean(14);
                        regolaDiff4Valore = rea.GetDouble(15);
                        regolaDiff10Valore = rea.GetDouble(16);
                        regolaMin60Valore = rea.GetDouble(17);
                        regolaMin60Delta = rea.GetDouble(18);
                        regolaDelta3Valore = rea.GetDouble(19);
                        regolaMin59Valore = rea.GetDouble(20);
                        regolaMin59Delta = rea.GetDouble(21);
                        regolaMin59Almeno = rea.GetDouble(22);
                        VUCentrocampista = rea.GetDouble(23);
                        regolaDifesaVU = rea.GetBoolean(24);
                        VUDifensore = rea.GetDouble(25);
                        usaTabellino = rea.GetInt32(26);
                        try {nomeSpeciale1 = rea.GetString(27);}
                        catch {nomeSpeciale1 = "ModPers1"; }
                        try { nomeSpeciale2 = rea.GetString(28); }
                        catch { nomeSpeciale2 = "ModPers2"; }
                        try { nomeSpeciale3 = rea.GetString(29); }
                        catch { nomeSpeciale3 = "ModPers3"; }

                        /*FCMExtender.log("puntiPerVittoria: " + puntiPerVittoria);
                        FCMExtender.log("fattoreCampo: " + fattoreCampo);
                        FCMExtender.log("regolaPortiere: " + regolaPortiere);
                        FCMExtender.log("regolaDifesa: " + regolaDifesa);
                        FCMExtender.log("regolaCentMedia: " + regolaCentMedia);
                        FCMExtender.log("regolaCentDiffe: " + regolaCentDiffe);
                        FCMExtender.log("regolaDiff4: " + regolaDiff4);
                        FCMExtender.log("regolaDiff10: " + regolaDiff10);
                        FCMExtender.log("regolaMin60: " + regolaMin60);
                        FCMExtender.log("usaSpeciale1: " + usaSpeciale1);
                        FCMExtender.log("usaSpeciale2: " + usaSpeciale2);
                        FCMExtender.log("usaSpeciale3: " + usaSpeciale3);
                        FCMExtender.log("regolaAttacco: " + regolaAttacco);
                        FCMExtender.log("regolaDelta3: " + regolaDelta3);
                        FCMExtender.log("regolaMin59: " + regolaMin59);
                        FCMExtender.log("regolaDiff4Valore: " + regolaDiff4Valore);
                        FCMExtender.log("regolaDiff10Valore: " + regolaDiff10Valore);
                        FCMExtender.log("regolaMin60Valore: " + regolaMin60Valore);
                        FCMExtender.log("regolaMin60Delta: " + regolaMin60Delta);
                        FCMExtender.log("regolaDelta3Valore: " + regolaDelta3Valore);
                        FCMExtender.log("regolaMin59Valore: " + regolaMin59Valore);
                        FCMExtender.log("regolaMin59Delta: " + regolaMin59Delta);
                        FCMExtender.log("regolaMin59Almeno: " + regolaMin59Almeno);
                        FCMExtender.log("VUCentrocampista: " + VUCentrocampista);
                        FCMExtender.log("regolaDifesaVU: " + regolaDifesaVU);
                        FCMExtender.log("VUDifensore: " + VUDifensore);*/
                    }
                }
            }

            using (OdbcCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "select numDife, numCent, numAtta, Modif, ModifAvv " +
                "from Modulo, ModuloAmmesso " +
                "WHERE ModuloAmmesso.IDCompetizione = " + compSel +
                " and ModuloAmmesso.IDModulo=Modulo.id";
                using (OdbcDataReader rea = cmd.ExecuteReader())
                {
                    while (rea.Read())
                    {
                        string mod = rea.GetInt32(0) + "-" + rea.GetString(1) + "-" + rea.GetString(2);
                        moduli.Add(mod, new Modulo(rea.GetDouble(3), rea.GetDouble(4)));
                    }
                    //FCMExtender.log("Moduli: " + moduli);
                }
            }
        }
    }
}