using fcm.model;
using math.utils;
using System;
using System.Collections.Generic;
using static fcm.model.Match;

namespace fcm.calcolatore
{

    public class CalcoliHelper
    {

        private Regole regole;
        private List<Fascia> fasceModDifesa;
        private List<Fascia> fasceNumeroDifensori;
        private List<Fascia> fasceModCentrocampo;
        private List<Fascia> fasceGol;

        public CalcoliHelper(Regole regole, List<Fascia> fasceModDifesa, List<Fascia> fasceNumeroDifensori,
                List<Fascia> fasceModCentrocampo, List<Fascia> fasceGol)
        {
            this.regole = regole;
            this.fasceModDifesa = fasceModDifesa;
            this.fasceNumeroDifensori = fasceNumeroDifensori;
            this.fasceModCentrocampo = fasceModCentrocampo;
            this.fasceGol = fasceGol;
        }

        public Match calcolaMatch(Tabellino tabellinoJ, Tabellino tabellinoK, Boolean squadraJInCasa, Boolean esisteFattoreCampo, Boolean gp)
        {

            Match match = new Match();
            Team squadraJ;
            Team squadraK;
            if (squadraJInCasa)
            {
                squadraJ = match.squadra1;
                squadraK = match.squadra2;
            }
            else
            {
                squadraJ = match.squadra2;
                squadraK = match.squadra1;
            }
            match.squadra1.fattoreCampo = esisteFattoreCampo ? regole.fattoreCampo : 0;

            gestisciInferiorita(tabellinoJ.giocatori);
            if (!gp)
                gestisciInferiorita(tabellinoK.giocatori);

            squadraJ.parziale = sum11(tabellinoJ.giocatori);
            if (!gp)
                squadraK.parziale = sum11(tabellinoK.giocatori);

            //i modificatori portiere rimangono inalterati. Li prelevo dal relativo tabellino
            if (regole.regolaPortiere)
            {
                squadraJ.modPortiere = tabellinoJ.modPortiere;
                if (!gp)
                    squadraK.modPortiere = tabellinoK.modPortiere;
            }

            //il mod difesa si calcola con i dati dell'avversario
            if (regole.regolaDifesa && !gp)
            {
                squadraJ.modDifesa = calcolaModDifesa(tabellinoK);
                squadraK.modDifesa = calcolaModDifesa(tabellinoJ);
            }

            if (regole.regolaCentDiffe && !gp)
            {
                double modCentrocampo = calcolaModCentrocampoDifferenza(tabellinoJ, tabellinoK);
                squadraJ.modCentrocampo = modCentrocampo;
                squadraK.modCentrocampo = modCentrocampo * -1;
            }

            //i modificatori attacco rimangono inalterati. Li prelevo dal relativo tabellino
            if (regole.regolaAttacco)
            {
                squadraJ.modAttacco = tabellinoJ.modAttacco;
                if (!gp)
                    squadraK.modAttacco = tabellinoK.modAttacco;
            }

            //anche i modificatori speciali rimangono inalterati
            if (regole.usaSpeciale1)
            {
                squadraJ.modSpeciale1 = tabellinoJ.modPers1;
                if (!gp)
                    squadraK.modSpeciale1 = tabellinoK.modPers1;
            }

            if (regole.usaSpeciale2)
            {
                squadraJ.modSpeciale2 = tabellinoJ.modPers2;
                if (!gp)
                    squadraK.modSpeciale2 = tabellinoK.modPers2;
            }

            if (regole.usaSpeciale3)
            {
                squadraJ.modSpeciale3 = tabellinoJ.modPers3;
                if (!gp)
                    squadraK.modSpeciale3 = tabellinoK.modPers3;
            }


            //bonus moduli
            String moduloJ = calcModulo(tabellinoJ.giocatori);
            if (regole.moduli.ContainsKey(moduloJ) && regole.moduli[moduloJ] != null)
            {
                squadraJ.modModulo += regole.moduli[moduloJ].modif;
                if (!gp)
                    squadraK.modModulo += regole.moduli[moduloJ].modifAvv;
            }

            if (!gp)
            {
                String moduloK = calcModulo(tabellinoK.giocatori);
                if (regole.moduli.ContainsKey(moduloK) && regole.moduli[moduloK] != null)
                {
                    squadraK.modModulo += regole.moduli[moduloK].modif;
                    squadraJ.modModulo += regole.moduli[moduloK].modifAvv;
                }
            }

            if (!gp)
            {
                squadraJ.numeroGol = (int)getFascia(fasceGol, squadraJ.getTotale()).valore;
                squadraK.numeroGol = (int)getFascia(fasceGol, squadraK.getTotale()).valore;
                affinaNumeroGol(squadraJ, squadraK);
            }

            return match;
        }

        private double sum11(Giocatore[] giocatori)
        {
            double sum = 0;
            for (int i = 0; i < 11; i++)
            {
                sum += NumParser.parseDouble(giocatori[i].voto.Replace(',', '.'));
            }
            return sum;
        }

        private double calcolaModDifesa(Tabellino tab)
        {
            double totDif = 0;
            int numDif = 0;
            for (int x = 0; x < 11; x++)
            {
                Giocatore gio = tab.giocatori[x];
                if (gio.ruolo.Equals("2") || gio.ruolo.Equals("6"))
                {
                    totDif += NumParser.parseDouble(gio.votoPuro.Replace(',', '.'));
                    numDif++;
                    if (NumParser.parseDouble(gio.votoPuro.Replace(',', '.')) == 0)
                    {
                        if (regole.regolaDifesaVU)
                        {
                            totDif += regole.VUDifensore;
                        }
                        else
                        {
                            numDif--;
                        }
                    }
                }
            }
            double medDif1 = totDif / numDif;
            double mod = getFascia(fasceModDifesa, medDif1).valore;
            mod += getFascia(fasceNumeroDifensori, numDif).valore;
            return mod;
        }

        private double calcolaModCentrocampoDifferenza(Tabellino tabJ, Tabellino tabK)
        {
            //mod centrocampo per differenza
            double totCentJ = calcolaCC(tabJ);
            double totCentK = calcolaCC(tabK);
            double diff = totCentJ - totCentK;
            double modValue = getFascia(fasceModCentrocampo, Math.Abs(diff)).valore;
            return modValue * Math.Sign(diff);
        }

        private double calcolaCC(Tabellino tabJ)
        {
            double totCent = 0;
            int numcent = 0;
            for (int x = 0; x < 11; x++)
            {
                Giocatore gio = tabJ.giocatori[x];
                if (gio.ruolo.Equals("3") || gio.ruolo.Equals("7"))
                {
                    if (NumParser.parseDouble(gio.votoPuro.Replace(',', '.')) > 0)
                    {
                        totCent += NumParser.parseDouble(gio.votoPuro.Replace(',', '.'));
                        numcent++;
                    }
                }
            }
            totCent += (5 - numcent) * regole.VUCentrocampista;
            return totCent;
        }

        private Fascia getFascia(List<Fascia> lista, double value)
        {
            lista.Sort((o1, o2) =>
            {
                if (o1.min > o2.min) return -1;
                if (o1.min < o2.min) return 1;
                return 0;
            });
            foreach (var f in lista)
            {
                if (value >= f.min)
                {
                    return f;
                }
            }
            return new Fascia();
        }

        private static void gestisciInferiorita(Giocatore[] giocatori)
        {
            string prevRuolo = "2";
            for (int x = 1; x < 11; x++)
            {
                if (giocatori[x].ruolo.Equals("0"))
                {
                    giocatori[x].ruolo = prevRuolo;
                }
                prevRuolo = giocatori[x].ruolo;
            }
        }

        private static String calcModulo(Giocatore[] giocatori)
        {
            int dif = 0;
            int cen = 0;
            int att = 0;
            for (int x = 0; x < 11; x++)
            {
                string ruolo = giocatori[x].ruolo;
                if (ruolo.Equals("2") || ruolo.Equals("6"))
                {
                    dif++;
                }
                if (ruolo.Equals("3") || ruolo.Equals("7"))
                {
                    cen++;
                }
                if (ruolo.Equals("4") || ruolo.Equals("8"))
                {
                    att++;
                }
            }
            return dif + "-" + cen + "-" + att;
        }

        private void affinaNumeroGol(Team squadraJ, Team squadraK)
        {
            //regola diff 4 (o valore esatto)
            if (regole.regolaDiff4 && squadraJ.numeroGol == squadraK.numeroGol && squadraJ.numeroGol >= 1)
            {
                if (squadraJ.getTotale() >= squadraK.getTotale() + regole.regolaDiff4Valore)
                {
                    squadraJ.numeroGol++;
                }
                if (squadraK.getTotale() >= squadraJ.getTotale() + regole.regolaDiff4Valore)
                {
                    squadraK.numeroGol++;
                }
            }

            //regola diff 10 (o valore esatto)
            if (regole.regolaDiff10)
            {
                if (squadraJ.getTotale() >= squadraK.getTotale() + regole.regolaDiff10Valore)
                {
                    squadraJ.numeroGol++;
                }
                if (squadraK.getTotale() >= squadraJ.getTotale() + regole.regolaDiff10Valore)
                {
                    squadraK.numeroGol++;
                }
            }

            //regola min 60
            if (regole.regolaMin60)
            {
                if (squadraK.getTotale() < regole.regolaMin60Valore && squadraJ.getTotale() >= squadraK.getTotale() + regole.regolaMin60Delta)
                {
                    squadraJ.numeroGol++;
                }
                if (squadraJ.getTotale() < regole.regolaMin60Valore && squadraK.getTotale() >= squadraJ.getTotale() + regole.regolaMin60Delta)
                {
                    squadraK.numeroGol++;
                }
            }

            //regola diff 3
            if (regole.regolaDelta3)
            {
                if (squadraJ.numeroGol > squadraK.numeroGol && squadraJ.getTotale() < squadraK.getTotale() + regole.regolaDelta3Valore)
                {
                    squadraK.numeroGol++;
                    if (squadraJ.numeroGol == 1)
                    {
                        squadraJ.numeroGol--;
                        squadraK.numeroGol--;
                    }
                }
                if (squadraK.numeroGol > squadraJ.numeroGol && squadraK.getTotale() < squadraJ.getTotale() + regole.regolaDelta3Valore)
                {
                    squadraJ.numeroGol++;
                    if (squadraJ.numeroGol == 1)
                    {
                        squadraJ.numeroGol--;
                        squadraK.numeroGol--;
                    }
                }
            }

            //regola min 59
            if (regole.regolaMin59)
            {
                if (squadraJ.getTotale() < regole.regolaMin59Valore && squadraK.getTotale() >= regole.regolaMin59Almeno && squadraK.getTotale() >= squadraJ.getTotale() + regole.regolaMin59Delta)
                {
                    squadraK.numeroGol++;
                }
                if (squadraK.getTotale() < regole.regolaMin59Valore && squadraJ.getTotale() >= regole.regolaMin59Almeno && squadraJ.getTotale() >= squadraK.getTotale() + regole.regolaMin59Delta)
                {
                    squadraJ.numeroGol++;
                }
            }
        }

    }
}