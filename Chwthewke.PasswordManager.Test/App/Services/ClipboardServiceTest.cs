using System.Windows;
using Chwthewke.PasswordManager.App.Services;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.Services
{
    [ TestFixture ]
    [ RequiresSTA ]
    public class ClipboardServiceTest
    {
        [ SetUp ]
        public void SetUpService( )
        {
            _clipboardService = new ClipboardService( );
        }

        [ Test ]
        public void TextIsCopiedToClipboard( )
        {
            // Setup

            // Exercise
            _clipboardService.CopyToClipboard( "some text." );
            // Verify
            Assert.That( Clipboard.ContainsData( DataFormats.UnicodeText ) );
            Assert.That( Clipboard.GetText( ), Is.EqualTo( "some text." ) );
        }

        private IClipboardService _clipboardService;
    }
}