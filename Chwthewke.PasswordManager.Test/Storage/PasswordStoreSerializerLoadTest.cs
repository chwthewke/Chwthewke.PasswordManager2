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
        private IPasswordStoreSerializer _serializer;
        private MemoryStream _inputStream;
        private IPasswordStore _passwordStore;

        [ SetUp ]
        public void SetUpSerializer( )
        {
            _serializer = new PasswordStoreSerializer( );
            _inputStream = new MemoryStream( );
            _passwordStore = new PasswordStore( );
        }

        [ Test ]
        public void LoadEmptyPasswordStore( )
        {
            // Setup
            new XElement( PasswordStoreSerializer.PasswordStoreElement ).Save( _inputStream );
            _inputStream.Flush(  );
            var tr = new StreamReader( _inputStream );
            Console.WriteLine( "We read [" + tr.ReadToEnd( ) + "]" );
            Console.WriteLine( "Content is [" + Encoding.Default.GetString( _inputStream.ToArray( ) ) + "]" );
            // Exercise
            _serializer.Load( _passwordStore, _inputStream );
            // Verify
            Assert.That( _passwordStore.Passwords, Is.Empty );
        }
    }
}