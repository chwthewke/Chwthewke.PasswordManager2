using System;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordSlotViewModel: ObservableObject {
        public PasswordSlotViewModel( IPasswordGenerator generator )
        {
            _generator = generator;
        }

        public IPasswordGenerator Generator
        {
            get { return _generator; }
        }

        private IPasswordGenerator _generator;
    }
}