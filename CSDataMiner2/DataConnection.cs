using System;
using System.Data.OleDb ;
using System.Data;
using System.IO ;

namespace CSDataMiner2
{
    class DataConnection
    {
        public DataTable RawData = new DataTable(Guid.NewGuid().ToString()); 
        public string ErrorString { get; set; }

        public DataConnection (string dbFilename)
        {
            try
            {
            OleDbDataAdapter _oleAdapter = new OleDbDataAdapter ("SELECT * FROM [Sheet1$]", "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" + dbFilename + "'; Extended Properties='Excel 12.0;IMEX=1;HDR=NO'");
            _oleAdapter.Fill(RawData);
            }
            catch (IOException e)
            {
                ErrorString = e.ToString ();
            }  
        }
    }
}
