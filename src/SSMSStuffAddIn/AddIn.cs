using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedGate.SIPFrameworkShared;
using RedGate.SIPFrameworkShared.ObjectExplorer;
using SSMSStuffAddIn.ObjectExplorerMenus;

namespace SSMSStuffAddIn
{

    public class AddIn : ISsmsAddin4
    {

        public string Author => @"Henrik Bergqvist";

        public string Description => Name;

        public string Name => @"SSMS Stuff Add In";

        public string Url => @"https://github.com/hbbq/SSMSStuff";

        public string Version => @"1.0.0.0";
        
        private ISsmsFunctionalityProvider6 m_Provider;

        public void OnLoad(ISsmsExtendedFunctionalityProvider provider)
        {
            m_Provider = (ISsmsFunctionalityProvider6)provider;
            AddObjectExplorerContextMenu();
            AddObjectExplorerListener();
        }

        private void AddObjectExplorerListener()
        {
            m_Provider.ObjectExplorerWatcher.ConnectionsChanged += (args) => { OnConnectionsChanged(args); };
            m_Provider.ObjectExplorerWatcher.SelectionChanged += (args) => { OnSelectionChanged(args); };
        }

        private void AddObjectExplorerContextMenu()
        {
            m_Provider.AddTopLevelMenuItem(new AssemblyExportMenuItem(m_Provider));
            m_Provider.AddTopLevelMenuItem(new TableCreateClassCodeMenuItem(m_Provider));
        }

        private void OnSelectionChanged(ISelectionChangedEventArgs args)
        {
            //m_MessageLog.AddMessage(string.Format("Object explorer selection: {0}", args.Selection.Path));
        }

        private void OnConnectionsChanged(IConnectionsChangedEventArgs args)
        {
            //m_MessageLog.AddMessage("Object explorer connections:");
            int count = 1;
            foreach (var connection in args.Connections)
            {
                //m_MessageLog.AddMessage(string.Format("\t{0}: {1}", count, connection.Server));
                count++;
            }
        }

        public void OnNodeChanged(ObjectExplorerNodeDescriptorBase node)
        {
        }

        public void OnShutdown()
        {
        }

    }

}
