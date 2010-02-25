using System;
using System.IO;
using System.Text;
using System.Xml;
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
        private XmlWriter _xmlWriter;

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

        [ Test ]
        public void LoadMultiplePasswords( )
        {
            // Setup
            SaveXml( new XElement( PasswordStoreSerializer.PasswordStoreElement,
                                   MakePasswordElement( "aKey" ),
                                   MakePasswordElement( "anotherKey" ) ) );
            // Exercise
            _serializer.Load( _passwordStore, _inputStream );
            // Verify
            Assert.That( _passwordStore.Passwords, Has.Count.EqualTo( 2 ) );
        }

        private static XElement MakePasswordElement( string key )
        {
            return new XElement( PasswordStoreSerializer.PasswordElement,
                                 new XElement( PasswordStoreSerializer.KeyElement, key ) );
        }
    }
}