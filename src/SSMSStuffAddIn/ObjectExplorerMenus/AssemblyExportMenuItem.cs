using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedGate.SIPFrameworkShared;
using SSMSStuffAddIn.Handlers;

namespace SSMSStuffAddIn.ObjectExplorerMenus
{

    class AssemblyExportMenuItem : ActionSimpleOeMenuItemBase
    {

        private readonly ISsmsFunctionalityProvider6 m_Provider;

        public AssemblyExportMenuItem(ISsmsFunctionalityProvider6 provider)
        {
            m_Provider = provider;
        }

        public override string ItemText => "Export to file...";

        public override bool AppliesTo(ObjectExplorerNodeDescriptorBase oeNode)
        {
            return ((oeNode as IOeNode)?.Type ?? "") == "SqlAssembly";
        }

        public override void OnAction(ObjectExplorerNodeDescriptorBase node)
        {

            var oeNode = node as IOeNode;

            if (oeNode == null) return;

            IDatabaseObjectInfo db;
            IConnectionInfo cn;

            if(oeNode.TryGetDatabaseObject(out db) && oeNode.TryGetConnection(out cn))
            {

                //m_Provider.QueryWindow.OpenNew($"select * from sys.assemblies where [name] = '{oeNode.Name.Replace("'", "''")}'", oeNode.Name, cn.ConnectionString);

                //var w = new Windows.ExportAssemblyWindow();

                //var wind = m_Provider.ToolWindow.Create(w, "test", Guid.NewGuid(), false);
                //wind.Activate(true);

                var handler = new AssemblyExportHandler(cn.ConnectionString, oeNode.Name);
                handler.SaveIt();

            }            

        }

    }

}
