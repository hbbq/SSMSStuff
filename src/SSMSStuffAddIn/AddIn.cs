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
        
        private ISsmsFunctionalityProvider6 _provider;

        public void OnLoad(ISsmsExtendedFunctionalityProvider provider)
        {
            _provider = (ISsmsFunctionalityProvider6)provider;
            AddObjectExplorerContextMenu();
            AddObjectExplorerListener();
        }

        private void AddObjectExplorerListener()
        {
            _provider.ObjectExplorerWatcher.ConnectionsChanged += (args) => { OnConnectionsChanged(args); };
            _provider.ObjectExplorerWatcher.SelectionChanged += (args) => { OnSelectionChanged(args); };
        }

        private void AddObjectExplorerContextMenu()
        {
            _provider.AddTopLevelMenuItem(new AssemblyExportMenuItem(_provider));
            _provider.AddTopLevelMenuItem(new DatabaseGetConnectionStringMenuItem(_provider));
            _provider.AddTopLevelMenuItem(new TableCreateClassCodeMenuItem(_provider));
        }

        private void OnSelectionChanged(ISelectionChangedEventArgs args)
        {
        }

        private void OnConnectionsChanged(IConnectionsChangedEventArgs args)
        {
        }

        public void OnNodeChanged(ObjectExplorerNodeDescriptorBase node)
        {
        }

        public void OnShutdown()
        {
        }

    }

}
