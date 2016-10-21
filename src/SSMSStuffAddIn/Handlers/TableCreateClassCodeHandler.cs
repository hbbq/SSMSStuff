using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SSMSStuffAddIn.Handlers
{

    class TableCreateClassCodeHandler
    {
        
        public string ConnectionString { get; private set; }
        public string TableName { get; private set; }

        public TableCreateClassCodeHandler(string connectionString, string tableName)
        {
            ConnectionString = connectionString;
            TableName = tableName;
        }

        private class ColumnMapping
        {
            public string SqlType { get; private set; }
            public string CSType { get; private set; }
            public bool CSReferenceType { get; private set; }

            public ColumnMapping(string sqlType, string csType, bool csReferenceType)
            {
                SqlType = sqlType;
                CSType = csType;
                CSReferenceType = csReferenceType;
            }
        }

        private List<ColumnMapping> ColumnMappings = new List<ColumnMapping>
        {
            new ColumnMapping("bigint", "long", false),
            new ColumnMapping("bit", "bool", false),
            new ColumnMapping("varchar", "string", true),
            new ColumnMapping("nvarchar", "string", true),
            new ColumnMapping("char", "string", true),
            new ColumnMapping("nchar", "string", true),
            new ColumnMapping("int", "int", false),
            new ColumnMapping("datetime", "DateTime", true),
            new ColumnMapping("money", "decimal", false),
            new ColumnMapping("float", "decimal", false),
            new ColumnMapping("varbinary", "byte[]", true),
        };
        
        public string GetCode()
        {

            var props = "";

            using (var cn = new SqlConnection(ConnectionString))
            using (var cm = new SqlCommand("", cn))
            {
                cm.CommandText = @"
select c.name, t.name, c.is_nullable
from sys.columns c
	inner join sys.types t
		on t.user_type_id = c.user_type_id
where c.object_id = object_id(@table)
order by c.column_id
";
                cm.Parameters.AddWithValue("@table", TableName);
                cn.Open();

                using(var rdr = cm.ExecuteReader())
                {
                    while (rdr.Read())
                    {

                        var nme = rdr.GetString(0);
                        var tpe = rdr.GetString(1);
                        var nul = rdr.GetBoolean(2);

                        var clm = ColumnMappings.FirstOrDefault(e => e.SqlType.ToLowerInvariant() == tpe.ToLowerInvariant());
                        
                        if(clm == null)
                        {
                            props +=
                                $"    //type {tpe} not handled...\n" +
                                $"    public object {nme} {{ get; set; }}\n" +
                                $"    //...\n";
                        }else
                        {
                            props +=
                                $"    public {clm.CSType}{((nul && !clm.CSReferenceType) ? "?" : "")} {nme} {{ get; set; }}\n";
                        }

                    }
                }

            }

            var strCode =
                $"public class {TableName}\n" +
                $"{{\n" +
                $"\n" +
                $"{props}" +
                $"\n" +
                $"    public {TableName}()\n" +
                $"    {{\n" +
                $"\n" +
                $"    }}\n" +
                $"\n" +
                $"}}";
            
            return strCode;

        }

    }

}
