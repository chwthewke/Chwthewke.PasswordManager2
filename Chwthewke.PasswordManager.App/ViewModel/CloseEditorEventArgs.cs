using System;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class CloseEditorEventArgs : EventArgs
    {
        public CloseEditorEventArgs( CloseEditorEventType type )
        {
            _type = type;
        }

        public CloseEditorEventType Type
        {
            get { return _type; }
        }

        private readonly CloseEditorEventType _type;
    }

    public enum CloseEditorEventType
    {
        Self,
        All,
        AllButSelf,
        RightOfSelf,
        Insecure
    }
}