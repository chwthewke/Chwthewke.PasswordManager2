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

        [ Test ]
        public void LoadReadsMasterPasswordGuidFromElement( )
        {
            // Setup
            Guid guid = Guid.Parse( "34579b9f-8ac1-464a-805a-abe564da8848" );
            SaveXml( new XElement( PasswordStoreSerializer.PasswordStoreElement,
                                   ( XElement )
                                   new SerializedPassword( "aKey" ) { MasterPasswordId = guid } ) );
            // Exercise
            _serializer.Load( _passwordStore, _inputStream );
            // Verify
            PasswordInfo passwordInfo = _passwordStore.FindPasswordInfo( "aKey" );
            Assert.That( passwordInfo.MasterPasswordId, Is.EqualTo( guid ) );
        }

        [ Test ]
        public void LoadReadsPasswordSettingsGuidFromElement( )
        {
            // Setup
            Guid guid = Guid.Parse( "34579b9f-8ac1-464a-805a-abe564da8848" );
            SaveXml( new XElement( PasswordStoreSerializer.PasswordStoreElement,
                                   ( XElement )
                                   new SerializedPassword( "aKey" ) { PasswordSettingsId = guid } ) );
            // Exercise
            Console.WriteLine( new UTF8Encoding( false ).GetString( _inputStream.ToArray( ) ) );

            _serializer.Load( _passwordStore, _inputStream );
            // Verify
            PasswordInfo passwordInfo = _passwordStore.FindPasswordInfo( "aKey" );
            Assert.That( passwordInfo.PasswordSettingsId, Is.EqualTo( guid ) );
        }

        [ Test ]
        public void LoadReadsCreationDateFromElement( )
        {
            // Setup
            DateTime creationTime = new DateTime( 634022874410500302 );
            SaveXml( new XElement( PasswordStoreSerializer.PasswordStoreElement,
                                   ( XElement )
                                   new SerializedPassword( "aKey" ) { CreationTime = creationTime } ) );
            // Exercise
            _serializer.Load( _passwordStore, _inputStream );
            // Verify
            PasswordInfo passwordInfo = _passwordStore.FindPasswordInfo( "aKey" );
            Assert.That( passwordInfo.CreationTime, Is.EqualTo( creationTime ) );
        }


        [ Test ]
        public void LoadReadsNoteFromElement( )
        {
            // Setup
            string note = "a Note";
            SaveXml( new XElement( PasswordStoreSerializer.PasswordStoreElement,
                                   ( XElement ) new SerializedPassword( "aKey" ) { Note = note } ) );
            // Exercise
            _serializer.Load( _passwordStore, _inputStream );
            // Verify
            PasswordInfo passwordInfo = _passwordStore.FindPasswordInfo( "aKey" );
            Assert.That( passwordInfo.Note, Is.EqualTo( note ) );
        }

        [ Test ]
        public void LoadReadsNullNoteFromMissingNoteElement( )
        {
            // Setup
            SaveXml( new XElement( PasswordStoreSerializer.PasswordStoreElement,
                                   ( XElement ) new SerializedPassword( "aKey" ) ) );
            // Exercise
            _serializer.Load( _passwordStore, _inputStream );
            // Verify
            PasswordInfo passwordInfo = _passwordStore.FindPasswordInfo( "aKey" );
            Assert.That( passwordInfo.Note, Is.Null );
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