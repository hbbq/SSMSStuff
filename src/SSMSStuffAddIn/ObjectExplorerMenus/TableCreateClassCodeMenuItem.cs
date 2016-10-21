using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedGate.SIPFrameworkShared;
using SSMSStuffAddIn.Handlers;

namespace SSMSStuffAddIn.ObjectExplorerMenus
{

    class TableCreateClassCodeMenuItem : ActionSimpleOeMenuItemBase
    {

        private readonly ISsmsFunctionalityProvider6 m_Provider;

        public TableCreateClassCodeMenuItem(ISsmsFunctionalityProvider6 provider)
        {
            m_Provider = provider;
        }

        public override string ItemText => "Generate C# class";

        public override bool AppliesTo(ObjectExplorerNodeDescriptorBase oeNode)
        {
            return ((oeNode as IOeNode)?.Type ?? "") == "Table";
        }

        public override void OnAction(ObjectExplorerNodeDescriptorBase node)
        {

            var oeNode = node as IOeNode;

            if (oeNode == null) return;

            IDatabaseObjectInfo db;
            IConnectionInfo cn;

            if(oeNode.TryGetDatabaseObject(out db) && oeNode.TryGetConnection(out cn))
            {

                var handler = new TableCreateClassCodeHandler(cn.ConnectionString, oeNode.Name);
                var str = handler.GetCode();

                m_Provider.QueryWindow.OpenNew(str);

            }            

        }

    }

}

