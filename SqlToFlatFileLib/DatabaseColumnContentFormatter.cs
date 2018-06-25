using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlToFlatFileLib
{
    public static class DatabaseColumnContentFormatter
    {
        public static string ReadColumnData(IDataReader reader, int zeroBasedColumnNumber, string textEnclosure)
        {
            string contents = "";
            if (!reader.IsDBNull(zeroBasedColumnNumber))
            {
                var dataTypeName = reader.GetDataTypeName(zeroBasedColumnNumber).ToLower();
                switch (dataTypeName)
                {
                    case "dbtype_dbtimestamp":
                    case "datetime":
                        contents = reader.GetDateTime(zeroBasedColumnNumber).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        break;
                    case "dbtype_dbdate":
                    case "date":
                        contents = reader.GetDateTime(zeroBasedColumnNumber).ToString("yyyy-MM-dd");
                        break;
                    case "dbtype_i4":
                    case "int":
                        contents = reader.GetInt32(zeroBasedColumnNumber).ToString();
                        break;
                    case "dbtype_numeric":
                    case "dbtype_decimal":
                    case "decimal":
                    case "numeric":
                        contents = reader.GetDecimal(zeroBasedColumnNumber).ToString(CultureInfo.InvariantCulture);
                        break;
                    case "dbtype_bool":
                    case "bit":
                        contents = reader.GetBoolean(zeroBasedColumnNumber) ? "True" : "False";
                        break;
                    case "tinyint":
                        contents = reader.GetByte(zeroBasedColumnNumber).ToString();
                        break;
                    case "bigint":
                    case "dbtype_i8":
                        contents = reader.GetInt64(zeroBasedColumnNumber).ToString();
                        break;
                    case "dbtype_i2":
                    case "smallint":
                        contents = reader.GetInt16(zeroBasedColumnNumber).ToString();
                        break;
                    case "dbtype_r8":
                        contents = reader.GetDouble(zeroBasedColumnNumber).ToString(CultureInfo.InvariantCulture);
                        break;
                    case "dbtype_varbinary":
                    case "dbtype_binary":
                    case "varbinary":
                    case "binary":
                        contents = "0x" + BitConverter.ToString((byte[])reader[zeroBasedColumnNumber]).Replace("-", "");
                        break;
                    default:
                        if (string.IsNullOrEmpty(textEnclosure))
                            contents = reader[zeroBasedColumnNumber].ToString();
                        else
                            contents =
                                $"{textEnclosure}{reader[zeroBasedColumnNumber].ToString().Replace(textEnclosure, textEnclosure + textEnclosure)}{textEnclosure}";

                        break;
                }
            }
            return contents;
        }
    }
}
