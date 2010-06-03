using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Chwthewke.PasswordManager.App.View
{
    public class TabControlWrapper : ITabbed
    {

        public TabControlWrapper( TabControl tabControl )
        {
            _tabControl = tabControl;
        }

        void ITabbed.AddItem( TabItem tabItem )
        {
            _tabControl.Items.Add( tabItem );
        }

        void ITabbed.RemoveItem( TabItem tabItem )
        {
            _tabControl.Items.Remove( tabItem );
        }

        void ITabbed.Clear( )
        {
            _tabControl.Items.Clear( );
        }

        IEnumerable<TabItem> ITabbed.Items
        {
            get { return _tabControl.Items.Cast<TabItem>( ); }
        }

        int ITabbed.ItemCount
        {
            get { return _tabControl.Items.Count; }
        }

        int? ITabbed.SelectedIndex
        {
            get { return _tabControl.SelectedIndex == -1 ? (int?)null : _tabControl.SelectedIndex; }
            set { _tabControl.SelectedIndex = value ?? -1; }
        }

        TabItem ITabbed.SelectedItem
        {
            get { return _tabControl.SelectedItem as TabItem; }
            set { _tabControl.SelectedItem = value; }
        }

        private readonly TabControl _tabControl;
    }
}