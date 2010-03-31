using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.App.ViewModel;

namespace Chwthewke.PasswordManager.DesignApp
{
    public class DesignViewModel : ObservableObject
    {
        public PasswordGroupingViewModel PasswordGrouping
        {
            get { return _passwordGrouping; }
            set
            {
                if ( _passwordGrouping == value )
                    return;
                _passwordGrouping = value;
                RaisePropertyChanged( ( ) => PasswordGrouping );
            }
        }

        private PasswordGroupingViewModel _passwordGrouping;

    }
}