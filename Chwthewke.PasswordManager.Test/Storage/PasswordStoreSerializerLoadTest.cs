using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Chwthewke.PasswordManager.Engine;
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
            _serializer = new PasswordSerializer( );
            _passwordRepository = new PasswordRepository( PasswordGenerators.All, new Sha512Factory( ) );
        }

        [ TearDown ]
        public void TearDownMemoryStream( )
        {
            _inputReader.Dispose( );
        }

        [ Test ]
        public void LoadEmptyPasswordStore( )
        {
            // Setup
            SaveXml( new XElement( PasswordSerializer.PasswordStoreElement ) );
            // Exercise
            _serializer.Load( _passwordRepository, _inputReader );
            // Verify
            Assert.That( _passwordRepository.Passwords, Is.Empty );
        }


        [ Test ]
        public void LoadMultiplePasswords( )
        {
            // Setup
            SaveXml( new XElement( PasswordSerializer.PasswordStoreElement,
                                   ( XElement ) new SerializedPassword( "aKey" ),
                                   ( XElement ) new SerializedPassword( "anotherKey" ) ) );
            // Exercise
            _serializer.Load( _passwordRepository, _inputReader );
            // Verify
            Assert.That( _passwordRepository.Passwords, Has.Count.EqualTo( 2 ) );
        }


        [ Test ]
        public void LoadReadsPasswordKeyFromElement( )
        {
            // Setup
            SaveXml( new XElement( PasswordSerializer.PasswordStoreElement,
                                   ( XElement ) new SerializedPassword( "aKey" ) ) );
            // Exercise
            _serializer.Load( _passwordRepository, _inputReader );
            // Verify
            PasswordDigest passwordDigest = _passwordRepository.Passwords.First( );
            Assert.That( passwordDigest.Key, Is.EqualTo( "aKey" ) );
        }

        [ Test ]
        public void LoadReadsPasswordHashFromElement( )
        {
            // Setup
            SaveXml( new XElement( PasswordSerializer.PasswordStoreElement,
                                   ( XElement ) new SerializedPassword( "aKey" ) { Hash = new byte[ ] { 0x44, 0x66 } } ) );
            // Exercise
            _serializer.Load( _passwordRepository, _inputReader );
            // Verify
            PasswordDigest passwordDigest = _passwordRepository.FindPasswordInfo( "aKey" );
            Assert.That( passwordDigest.Hash.SequenceEqual( new byte[ ] { 0x44, 0x66 } ) );
        }

        [ Test ]
        public void LoadReadsMasterPasswordGuidFromElement( )
        {
            // Setup
            Guid guid = Guid.Parse( "34579b9f-8ac1-464a-805a-abe564da8848" );
            SaveXml( new XElement( PasswordSerializer.PasswordStoreElement,
                                   ( XElement )
                                   new SerializedPassword( "aKey" ) { MasterPasswordId = guid } ) );
            // Exercise
            _serializer.Load( _passwordRepository, _inputReader );
            // Verify
            PasswordDigest passwordDigest = _passwordRepository.FindPasswordInfo( "aKey" );
            Assert.That( passwordDigest.MasterPasswordId, Is.EqualTo( guid ) );
        }

        [ Test ]
        public void LoadReadsPasswordSettingsGuidFromElement( )
        {
            // Setup
            Guid guid = Guid.Parse( "34579b9f-8ac1-464a-805a-abe564da8848" );
            SaveXml( new XElement( PasswordSerializer.PasswordStoreElement,
                                   ( XElement )
                                   new SerializedPassword( "aKey" ) { PasswordSettingsId = guid } ) );
            // Exercise
            _serializer.Load( _passwordRepository, _inputReader );
            // Verify
            PasswordDigest passwordDigest = _passwordRepository.FindPasswordInfo( "aKey" );
            Assert.That( passwordDigest.PasswordGeneratorId, Is.EqualTo( guid ) );
        }

        [ Test ]
        public void LoadReadsCreationDateFromElement( )
        {
            // Setup
            DateTime creationTime = new DateTime( 634022874410500302 );
            SaveXml( new XElement( PasswordSerializer.PasswordStoreElement,
                                   ( XElement )
                                   new SerializedPassword( "aKey" ) { CreationTime = creationTime } ) );
            // Exercise
            _serializer.Load( _passwordRepository, _inputReader );
            // Verify
            PasswordDigest passwordDigest = _passwordRepository.FindPasswordInfo( "aKey" );
            Assert.That( passwordDigest.CreationTime, Is.EqualTo( creationTime ) );
        }


        [ Test ]
        public void LoadReadsNoteFromElement( )
        {
            // Setup
            string note = "a Note";
            SaveXml( new XElement( PasswordSerializer.PasswordStoreElement,
                                   ( XElement ) new SerializedPassword( "aKey" ) { Note = note } ) );
            // Exercise
            _serializer.Load( _passwordRepository, _inputReader );
            // Verify
            PasswordDigest passwordDigest = _passwordRepository.FindPasswordInfo( "aKey" );
            Assert.That( passwordDigest.Note, Is.EqualTo( note ) );
        }

        [ Test ]
        public void LoadReadsNullNoteFromMissingNoteElement( )
        {
            // Setup
            SaveXml( new XElement( PasswordSerializer.PasswordStoreElement,
                                   ( XElement ) new SerializedPassword( "aKey" ) ) );
            // Exercise
            _serializer.Load( _passwordRepository, _inputReader );
            // Verify
            PasswordDigest passwordDigest = _passwordRepository.FindPasswordInfo( "aKey" );
            Assert.That( passwordDigest.Note, Is.Null );
        }


        private void SaveXml( XElement xElement )
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
                                                      {
                                                          Encoding = new UTF8Encoding( false ),
                                                          OmitXmlDeclaration = true
                                                      };
            using ( TextWriter stringWriter = new StringWriter( ) )
            {
                using ( XmlWriter xmlWriter = XmlWriter.Create( stringWriter, xmlWriterSettings ) )
                    if ( xmlWriter != null ) xElement.Save( xmlWriter );
                _input = stringWriter.ToString( );
                _inputReader = new StringReader( _input );
            }
        }

        private IPasswordSerializer _serializer;
        private StringReader _inputReader;
        private IPasswordRepository _passwordRepository;
        private string _input;
    }
}