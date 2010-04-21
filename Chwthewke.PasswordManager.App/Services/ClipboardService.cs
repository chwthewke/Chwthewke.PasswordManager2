using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Chwthewke.PasswordManager.App.Services
{
    internal class ClipboardService : IClipboardService
    {
        public void CopyToClipboard( string value )
        {
            try
            {
                Clipboard.SetData( DataFormats.UnicodeText, value );
            }
            catch ( COMException )
            {
            }
        }
    }
}