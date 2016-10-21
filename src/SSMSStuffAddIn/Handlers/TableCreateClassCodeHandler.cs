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
        public string SchemaName { get; private set; }
        public string TableName { get; private set; }

        public TableCreateClassCodeHandler(string connectionString, string schemaName, string tableName)
        {
            ConnectionString = connectionString;
            SchemaName = schemaName;
            TableName = tableName;
        }

        private class ColumnMapping
        {
            public string SqlType { get; private set; }
            public string CsType { get; private set; }
            public bool CsReferenceType { get; private set; }

            public ColumnMapping(string sqlType, string csType, bool csReferenceType)
            {
                SqlType = sqlType;
                CsType = csType;
                CsReferenceType = csReferenceType;
            }
        }
        
        private readonly List<ColumnMapping> ColumnMappings = new List<ColumnMapping>
        {

            new ColumnMapping("bigint", "long", false),
            new ColumnMapping("binary", "byte[]", true),
            new ColumnMapping("bit", "bool", false),
            new ColumnMapping("char", "string", true),
            new ColumnMapping("date", "DateTime", false),
            new ColumnMapping("datetime", "DateTime", false),
            new ColumnMapping("datetime2", "DateTime", false),
            new ColumnMapping("datetimeoffset", "DateTimeOffset", false),
            new ColumnMapping("decimal", "decimal", false),
            new ColumnMapping("float", "double", false),
            new ColumnMapping("int", "int", false),
            new ColumnMapping("money", "decimal", false),
            new ColumnMapping("nchar", "string", true),
            new ColumnMapping("ntext", "string", true),
            new ColumnMapping("numeric", "decimal", false),
            new ColumnMapping("nvarchar", "string", true),
            new ColumnMapping("real", "float", false),
            new ColumnMapping("smalldatetime", "DateTime", false),
            new ColumnMapping("smallint", "short", false),
            new ColumnMapping("smallmoney", "decimal", false),
            new ColumnMapping("text", "string", true),
            new ColumnMapping("time", "TimeSpan", false),
            new ColumnMapping("tinyint", "byte", false),
            new ColumnMapping("uniqueidentifier", "Guid", false),
            new ColumnMapping("varbinary", "byte[]", true),
            new ColumnMapping("varchar", "string", true),

            new ColumnMapping("geography", null, false),
            new ColumnMapping("geometry", null, false),
            new ColumnMapping("hierarchyid", null, false),
            new ColumnMapping("image", null, false),
            new ColumnMapping("sql_variant", null, false),
            new ColumnMapping("sysname", null, false),
            new ColumnMapping("timestamp", null, false),
            new ColumnMapping("xml", null, false),

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
                cm.Parameters.AddWithValue("@table", SchemaName + "." + TableName);
                cn.Open();

                using(var rdr = cm.ExecuteReader())
                {
                    while (rdr.Read())
                    {

                        var nme = rdr.GetString(0);
                        var tpe = rdr.GetString(1);
                        var nul = rdr.GetBoolean(2);

                        var clm = ColumnMappings.FirstOrDefault(e => e.SqlType.ToLowerInvariant() == tpe.ToLowerInvariant());
                        
                        if(clm?.CsType == null)
                        {
                            props +=
                                $"    //SQL type {tpe} not handled, mapping to object...\n" +
                                $"    public object {nme} {{ get; set; }}\n" +
                                $"    //...\n";
                        }else
                        {
                            props +=
                                $"    public {clm.CsType}{((nul && !clm.CsReferenceType) ? "?" : "")} {nme} {{ get; set; }}\n";
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
