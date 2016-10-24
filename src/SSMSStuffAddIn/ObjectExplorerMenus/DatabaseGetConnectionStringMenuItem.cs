using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedGate.SIPFrameworkShared;
using SSMSStuffAddIn.Handlers;

namespace SSMSStuffAddIn.ObjectExplorerMenus
{

    class DatabaseGetConnectionStringMenuItem : ActionSimpleOeMenuItemBase
    {

        private readonly ISsmsFunctionalityProvider6 _provider;

        public DatabaseGetConnectionStringMenuItem(ISsmsFunctionalityProvider6 provider)
        {
            _provider = provider;
        }

        public override string ItemText => "Get connection string";

        public override bool AppliesTo(ObjectExplorerNodeDescriptorBase oeNode)
        {
            return ((oeNode as IOeNode)?.Type ?? "") == "Database";
        }

        public override void OnAction(ObjectExplorerNodeDescriptorBase node)
        {

            var oeNode = node as IOeNode;

            if (oeNode == null) return;
            
            IConnectionInfo cn;

            if(oeNode.TryGetConnection(out cn))
            {
                var cs = cn.ConnectionString;
                cs = String.Join(";", cs.Split(';').Where(e => !e.StartsWith("Application Name=")).ToArray());
                _provider.QueryWindow.OpenNew(cs);
            }            

        }

    }

}

