using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Linq;

namespace FCMExtender.gui
{
    public class ElabResult
    {
        public string competizione { get; set; }
        public string giornata { get; set; }
        public string incontro { get; set; }
        public string vecchioRisultato { get; set; }
        public string nuovoRisultato { get; set; }
    }
}
