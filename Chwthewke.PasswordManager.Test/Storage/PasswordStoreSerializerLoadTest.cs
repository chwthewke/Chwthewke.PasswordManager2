using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Chwthewke.PasswordManager.Storage;
using NUnit.Framework;
using System.Linq;

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
            SaveXml( new XElement( PasswordStoreSerializer.PasswordStoreElement ) );
            // Exercise
            _serializer.Load( _passwordStore, _inputStream );
            // Verify
            Assert.That( _passwordStore.Passwords, Is.Empty );
        }


        [ Test ]
        public void LoadMultiplePasswords( )
        {
            // Setup
            SaveXml( new XElement( PasswordStoreSerializer.PasswordStoreElement,
                                   ( XElement ) new SerializedPassword( "aKey" ),
                                   ( XElement ) new SerializedPassword( "anotherKey" ) ) );
            // Exercise
            _serializer.Load( _passwordStore, _inputStream );
            // Verify
            Assert.That( _passwordStore.Passwords, Has.Count.EqualTo( 2 ) );
        }


        [ Test ]
        public void LoadReadsPasswordKeyFromElement( )
        {
            // Setup
            SaveXml( new XElement( PasswordStoreSerializer.PasswordStoreElement,
                                   ( XElement ) new SerializedPassword( "aKey" ) ) );
            // Exercise
            _serializer.Load( _passwordStore, _inputStream );
            // Verify
            PasswordInfo passwordInfo = _passwordStore.Passwords.First( );
            Assert.That( passwordInfo.Key, Is.EqualTo( "aKey" ) );
        }

        [ Test ]
        public void LoadReadsPasswordHashFromElement( )
        {
            // Setup
            SaveXml( new XElement( PasswordStoreSerializer.PasswordStoreElement,
                                   ( XElement ) new SerializedPassword( "aKey" ) { Hash = new byte[ ] { 0x44, 0x66 } } ) );
            // Exercise
            _serializer.Load( _passwordStore, _inputStream );
            // Verify
            PasswordInfo passwordInfo = _passwordStore.FindPasswordInfo( "aKey" );
            Assert.That( passwordInfo.Hash.SequenceEqual( new byte[ ] { 0x44, 0x66 } ) );
        }

        private void SaveXml( XElement xElement )
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
                                                      {
                                                          Encoding = new UTF8Encoding( false ),
                                                          OmitXmlDeclaration = true
                                                      };
            using ( XmlWriter xmlWriter = XmlWriter.Create( _inputStream, xmlWriterSettings ) )
                xElement.Save( xmlWriter );

            _inputStream.Seek( 0, SeekOrigin.Begin );
        }
    }
}