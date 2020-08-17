using fcm.entity;
using System.Data.Odbc;

namespace fcm.dao
{
    class TabellinoDAO
    {
        OdbcConnection conn;

        public TabellinoDAO (OdbcConnection conn)
        {
            this.conn = conn;
        }

        public TabellinoWrapper getTabellinoByIdGiornata(int idGiornata)
        {
            return null;
        }
    }
}
