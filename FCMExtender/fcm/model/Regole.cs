using main;
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
        public Dictionary<string, Modulo> moduli = new Dictionary<string, Modulo>();
        public Regole(OdbcConnection conn, string compSel)
        {
            OdbcCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT puntipervittoria, fattorecampo, regolaportiere, regoladifesa, regolacentmedia, " +
                    "regolacentdiffe, regoladiff4, regoladiff10, regolamin60, usaspeciale1, " +
                    "usaspeciale2, usaspeciale3, regolaattacco, regoladelta3, regolamin59," +
                    "regolaDiff4Valore, regolaDiff10Valore, regolaMin60Valore, regolaMin60Delta, regolaDelta3Valore, " +
                    "regolaMin59Valore, regolaMin59Delta, regolaMin59Almeno, VUCentrocampista, regolaDifesaVU," +
                    "VUDifensore FROM competizione WHERE id = " + compSel;
            OdbcDataReader rea = cmd.ExecuteReader();
            while (rea.Read())
            {
                puntiPerVittoria = rea.GetInt32(1);
                fattoreCampo = rea.GetDouble(2);
                regolaPortiere = rea.GetBoolean(3);
                regolaDifesa = rea.GetBoolean(4);
                regolaCentMedia = rea.GetBoolean(5);
                regolaCentDiffe = rea.GetBoolean(6);
                regolaDiff4 = rea.GetBoolean(7);
                regolaDiff10 = rea.GetBoolean(8);
                regolaMin60 = rea.GetBoolean(9);
                usaSpeciale1 = rea.GetBoolean(10);
                usaSpeciale2 = rea.GetBoolean(11);
                usaSpeciale3 = rea.GetBoolean(12);
                regolaAttacco = rea.GetBoolean(13);
                regolaDelta3 = rea.GetBoolean(14);
                regolaMin59 = rea.GetBoolean(15);
                regolaDiff4Valore = rea.GetDouble(16);
                regolaDiff10Valore = rea.GetDouble(17);
                regolaMin60Valore = rea.GetDouble(18);
                regolaMin60Delta = rea.GetDouble(19);
                regolaDelta3Valore = rea.GetDouble(20);
                regolaMin59Valore = rea.GetDouble(21);
                regolaMin59Delta = rea.GetDouble(22);
                regolaMin59Almeno = rea.GetDouble(23);
                VUCentrocampista = rea.GetDouble(24);
                regolaDifesaVU = rea.GetBoolean(25);
                VUDifensore = rea.GetDouble(26);

                FCMExtender.log("puntiPerVittoria: " + puntiPerVittoria);
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
                FCMExtender.log("VUDifensore: " + VUDifensore);
            }

            cmd = conn.CreateCommand();
            cmd.CommandText = "select numDife, numCent, numAtta, Modif, ModifAvv " +
                "from Modulo, ModuloAmmesso " +
                "WHERE ModuloAmmesso.IDCompetizione = " + compSel +
                " and ModuloAmmesso.IDModulo=Modulo.id";


            rea = cmd.ExecuteReader();
            while (rea.Read())
            {
                string mod = rea.GetInt32(1) + "-" + rea.GetString(2) + "-" + rea.GetString(3);
                moduli.Add(mod, new Modulo(rea.GetDouble(4), rea.GetDouble(5)));
            }
            FCMExtender.log("Moduli: " + moduli);
        }
    }
}