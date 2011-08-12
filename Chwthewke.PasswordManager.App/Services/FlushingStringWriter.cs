using System;
using System.IO;

namespace Chwthewke.PasswordManager.App.Services
{
    public class FlushingStringWriter : StringWriter
    {
        public FlushingStringWriter( Action<string> saveAction )
        {
            _saveAction = saveAction;
        }

        public override void Flush( )
        {
            _saveAction( ToString( ) );
        }

        private readonly Action<string> _saveAction;
    }
}