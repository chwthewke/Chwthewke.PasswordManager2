using System;
using System.Windows.Media;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public interface IGuidToColorConverter
    {
        Color Convert( Guid guid );
    }
}