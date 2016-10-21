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

        private readonly ISsmsFunctionalityProvider6 _provider;

        public AssemblyExportMenuItem(ISsmsFunctionalityProvider6 provider)
        {
            _provider = provider;
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

                var handler = new AssemblyExportHandler(cn.ConnectionString, oeNode.Name);
                handler.SaveIt();

            }            

        }

    }

}
