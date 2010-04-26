using System.Security;
using Chwthewke.PasswordManager.Editor;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Editor
{
    [ TestFixture ]
    public class PasswordEditorControllerTest
    {
        [ SetUp ]
        public void SetUpController( )
        {
            _controller = new PasswordEditorController( );
        }

        [ Test ]
        public void InitialStateIsVoid( )
        {
            // Setup
            // Exercise
            // Verify
            Assert.That( _controller.Key, Is.EqualTo( string.Empty ) );
            Assert.That( _controller.IsDirty, Is.False );
            Assert.That( _controller.Note, Is.EqualTo( string.Empty ) );
            Assert.That( _controller.IsPasswordLoaded, Is.False );
            Assert.That( _controller.MasterPassword.Length, Is.EqualTo( 0 ) );
            Assert.That( _controller.MasterPasswordId, Is.Null );
            Assert.That( _controller.GeneratedPassword, Is.EqualTo( string.Empty ) );
            Assert.That( _controller.Generator, Is.Null );
        }


        private IPasswordEditorController _controller;
    }
}