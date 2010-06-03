using System.Collections.Generic;
using System.Windows.Controls;

namespace Chwthewke.PasswordManager.App.View
{
    public interface ITabbed
    {
        void AddItem( TabItem tabItem );
        void RemoveItem( TabItem tabItem );
        void Clear( );

        IEnumerable<TabItem> Items { get; }
        int ItemCount { get; }
        int? SelectedIndex { get; set; }
        TabItem SelectedItem { get; set; }
    }
}