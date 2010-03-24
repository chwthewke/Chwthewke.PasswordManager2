using Chwthewke.MvvmUtils;

namespace Chwthewke.PasswordManager.Migration
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel( ImporterViewModel importer )
        {
            Importer = importer;
        }

        public ImporterViewModel Importer { get; private set; }
    }
}