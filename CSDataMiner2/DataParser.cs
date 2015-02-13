using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace CSDataMiner2
{
    class DataParser
    {
        public string[,] RawData { get; set; }
        public string[,] BinaryData {get; set;}
        public string[] Standards { get; set; }
        public string TestName {get; set;}
        public string NullValue = "NaN";
        public int NumStudentsDroopped { get; set; }

        public ViewSettings EdViewLocations = new ViewSettings(new Corner(5, 6), new Corner(5, 5));

        public DataParser (DataTable RawData)
        {
        try
            {
                TestName = RawData.Rows[0].ItemArray[6].ToString ();
       
                int _x = 0;
                for (int i = 0; i < RawData.Columns.Count - 1;++i )
                {
                    for (int j = 0; j<10; ++j)
                    {
                        switch (RawData.Rows[j].ItemArray[i].ToString ())
                        {
                            case "Ethnicity":
                                EdViewLocations.UpperLeft.x = i + 1;
                                EdViewLocations.UpperLeft.y = j + 2;
                                break;

                            case "Raw Score":
                                EdViewLocations.UpperRight.x = i - 1;
                                EdViewLocations.UpperRight.y = j + 2;
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                
            }        
        
            BinaryData = new string[RawData.Rows.Count, RawData.Columns.Count];
            for (int i = RawData.Rows.Count - 1; i > -1; i--)
            {
                for (int j = RawData.Columns.Count - 1; j > -1; j--)
                {
                    try
                    {
                        if (DBNull.Value.Equals (RawData.Rows[i].ItemArray[j]) || RawData.Rows[i].ItemArray[j].ToString() == "")
                        {
                            BinaryData[i, j] = NullValue; 
                        }
                        else
                        {
                            BinaryData[i, j] = RawData.Rows[i].ItemArray[j].ToString();
                        }

                    }
                    catch
                    {
                        BinaryData[i, j] = NullValue;
                    }
                }

            }
        }
        }
    }
