using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Chwthewke.PasswordManager.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Storage
{
    [ TestFixture ]
    public class PasswordStoreSerializerLoadTest
    {
        [ SetUp ]
        public void SetUpSerializer( )
        {
            _serializer = new PasswordStoreSerializer( new UTF8Encoding( false ) );
            _inputStream = new MemoryStream( );
            _passwordStore = new PasswordStore( );
        }

        [ TearDown ]
        public void TearDownMemoryStream( )
        {
            _inputStream.Dispose( );
        }

        private IPasswordStoreSerializer _serializer;
        private MemoryStream _inputStream;
        private IPasswordStore _passwordStore;

        [ Test ]
        public void LoadEmptyPasswordStore( )
        {
            // Setup
            TextWriter tw = new StreamWriter( _inputStream, new UTF8Encoding( false ) );
            new XElement( PasswordStoreSerializer.PasswordStoreElement ).Save( tw );
            _inputStream.Seek( 0, SeekOrigin.Begin );

            // Exercise
            _serializer.Load( _passwordStore, _inputStream );
            // Verify
            Assert.That( _passwordStore.Passwords, Is.Empty );
        }
    }
}