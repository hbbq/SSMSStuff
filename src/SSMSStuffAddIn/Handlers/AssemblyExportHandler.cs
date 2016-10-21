using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SSMSStuffAddIn.Handlers
{

    class AssemblyExportHandler
    {

        public string ConnectionString { get; private set; }
        public string AssemblyName { get; private set; }
        public int AssemblyId { get; private set; }
        public string AssemblyFileName { get; private set; }

        public AssemblyExportHandler(string connectionString, string assemblyName)
        {
            ConnectionString = connectionString;
            AssemblyName = assemblyName;
            ReadFileName();
        }

        private void ReadFileName()
        {
            using (var cn = new SqlConnection(ConnectionString))
            using (var cm = new SqlCommand("select a.assembly_id, af.name from sys.assemblies a inner join sys.assembly_files af on af.assembly_id = a.assembly_id where a.name = @name", cn))
            {
                cn.Open();
                cm.Parameters.AddWithValue("@name", AssemblyName);
                using(var rdr = cm.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        AssemblyId = rdr.GetInt32(0);
                        AssemblyFileName = System.IO.Path.GetFileName(rdr.GetString(1));
                    }
                }
            }
        }

        public void SaveIt()
        {

            var dlg = new System.Windows.Forms.SaveFileDialog();
            dlg.Filter = "dll files (*.dll)|*.dll|All files (*.*)|*,*";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;
            dlg.FileName = AssemblyFileName;

            if(dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                using (var cn = new SqlConnection(ConnectionString))
                using (var cm = new SqlCommand("select af.content from sys.assembly_files af where af.assembly_id = @id", cn))
                {
                    cn.Open();
                    cm.Parameters.AddWithValue("@id", AssemblyId);
                    using (var rdr = cm.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            var dta = rdr.GetSqlBinary(0).Value;

                            using (var str = dlg.OpenFile())
                            {
                                if (str != null)
                                {
                                    using(var sw = new System.IO.BinaryWriter(str))
                                    {
                                        sw.Write(dta);
                                    }
                                }

                                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{dlg.FileName}\"");

                            }

                        }
                    }
                }

            }

        }

    }

}
