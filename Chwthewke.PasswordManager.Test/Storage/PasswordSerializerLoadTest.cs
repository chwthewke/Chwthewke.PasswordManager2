using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Chwthewke.PasswordManager.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Storage
{
    [ TestFixture ]
    public class PasswordSerializerLoadTest
    {
        [ SetUp ]
        public void SetUpSerializer( )
        {
            _serializer = new PasswordSerializer( );
            _passwordStore = new InMemoryPasswordStore( );
        }

        [ Test ]
        public void LoadEmptyPasswordStore( )
        {
            // Setup
            SaveXml( new XElement( PasswordSerializer.PasswordStoreElement ) );
            // Exercise
            IEnumerable<PasswordDigest> passwords = _serializer.Load( _passwordStore );
            // Verify
            Assert.That( passwords, Is.Empty );
        }


        [ Test ]
        public void LoadMultiplePasswords( )
        {
            // Setup
            SaveXml( new XElement( PasswordSerializer.PasswordStoreElement,
                                   ( XElement ) new SerializedPassword( "aKey" ),
                                   ( XElement ) new SerializedPassword( "anotherKey" ) ) );
            // Exercise
            IEnumerable<PasswordDigest> passwords = _serializer.Load( _passwordStore );
            // Verify
            Assert.That( passwords.Count( ), Is.EqualTo( 2 ) );
        }


        [ Test ]
        public void LoadReadsPasswordKeyFromElement( )
        {
            // Setup
            SaveXml( new XElement( PasswordSerializer.PasswordStoreElement,
                                   ( XElement ) new SerializedPassword( "aKey" ) ) );
            // Exercise
            IEnumerable<PasswordDigest> passwords = _serializer.Load( _passwordStore );
            // Verify
            PasswordDigest passwordDigest = passwords.First( );
            Assert.That( passwordDigest.Key, Is.EqualTo( "aKey" ) );
        }

        [ Test ]
        public void LoadReadsPasswordHashFromElement( )
        {
            // Setup
            SaveXml( new XElement( PasswordSerializer.PasswordStoreElement,
                                   ( XElement ) new SerializedPassword( "aKey" ) { Hash = new byte[ ] { 0x44, 0x66 } } ) );
            // Exercise
            IEnumerable<PasswordDigest> passwords = _serializer.Load( _passwordStore );
            // Verify
            PasswordDigest passwordDigest = passwords.First( );
            Assert.That( passwordDigest.Key, Is.EqualTo( "aKey" ) );
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
            IEnumerable<PasswordDigest> passwords = _serializer.Load( _passwordStore );
            // Verify
            PasswordDigest passwordDigest = passwords.First( );
            Assert.That( passwordDigest.Key, Is.EqualTo( "aKey" ) );
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
            IEnumerable<PasswordDigest> passwords = _serializer.Load( _passwordStore );
            // Verify
            PasswordDigest passwordDigest = passwords.First( );
            Assert.That( passwordDigest.Key, Is.EqualTo( "aKey" ) );
            Assert.That( passwordDigest.PasswordGeneratorId, Is.EqualTo( guid ) );
        }

        [Test]
        public void LoadReadsCreationDateFromElement( )
        {
            // Setup
            DateTime creationTime = new DateTime( 634022874410500302 );
            SaveXml( new XElement( PasswordSerializer.PasswordStoreElement,
                                   ( XElement )
                                   new SerializedPassword( "aKey" ) { CreationTime = creationTime } ) );
            // Exercise
            IEnumerable<PasswordDigest> passwords = _serializer.Load( _passwordStore );
            // Verify
            PasswordDigest passwordDigest = passwords.First( );
            Assert.That( passwordDigest.Key, Is.EqualTo( "aKey" ) );
            Assert.That( passwordDigest.CreationTime, Is.EqualTo( creationTime ) );
        }

        [Test]
        public void LoadReadsModificationDateFromElement( )
        {
            // Setup
            DateTime modificationTime = new DateTime( 634122874455500442 );
            SaveXml( new XElement( PasswordSerializer.PasswordStoreElement,
                                   ( XElement )
                                   new SerializedPassword( "aKey" ) { ModificationTime = modificationTime } ) );
            // Exercise
            IEnumerable<PasswordDigest> passwords = _serializer.Load( _passwordStore );
            // Verify
            PasswordDigest passwordDigest = passwords.First( );
            Assert.That( passwordDigest.Key, Is.EqualTo( "aKey" ) );
            Assert.That( passwordDigest.ModificationTime, Is.EqualTo( modificationTime ) );
        }


        [ Test ]
        public void LoadReadsNoteFromElement( )
        {
            // Setup
            const string note = "a Note";
            SaveXml( new XElement( PasswordSerializer.PasswordStoreElement,
                                   ( XElement ) new SerializedPassword( "aKey" ) { Note = note } ) );
            // Exercise
            IEnumerable<PasswordDigest> passwords = _serializer.Load( _passwordStore );
            // Verify
            PasswordDigest passwordDigest = passwords.First( );
            Assert.That( passwordDigest.Key, Is.EqualTo( "aKey" ) );
            Assert.That( passwordDigest.Note, Is.EqualTo( note ) );
        }

        [ Test ]
        public void LoadReadsNullNoteFromMissingNoteElement( )
        {
            // Setup
            SaveXml( new XElement( PasswordSerializer.PasswordStoreElement,
                                   ( XElement ) new SerializedPassword( "aKey" ) ) );
            // Exercise
            IEnumerable<PasswordDigest> passwords = _serializer.Load( _passwordStore );
            // Verify
            PasswordDigest passwordDigest = passwords.First( );
            Assert.That( passwordDigest.Key, Is.EqualTo( "aKey" ) );
            Assert.That( passwordDigest.Note, Is.Null );
        }


        private void SaveXml( XElement xElement )
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
                                                      {
                                                          Encoding = new UTF8Encoding( false ),
                                                          OmitXmlDeclaration = true
                                                      };
            TextWriter stringWriter = new StringWriter( );
            using ( XmlWriter xmlWriter = XmlWriter.Create( stringWriter, xmlWriterSettings ) )
                xElement.Save( xmlWriter );
            _passwordStore.Content = stringWriter.ToString( );
        }

        private IPasswordSerializer _serializer;

        private InMemoryPasswordStore _passwordStore;
    }
}