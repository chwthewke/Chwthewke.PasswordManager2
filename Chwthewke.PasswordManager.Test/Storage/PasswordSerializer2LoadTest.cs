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
    public class PasswordSerializer2LoadTest
    {
        [ SetUp ]
        public void SetUpSerializer( )
        {
            _serializer = new PasswordSerializer2( );
            _passwordStore = new InMemoryPasswordStore( );
        }

        [ Test ]
        public void LoadEmptyPasswordStore( )
        {
            // Setup
            SaveXml( new XElement( PasswordSerializer2.PasswordStoreElement ) );
            // Exercise
            IEnumerable<PasswordDigestDocument> passwords = _serializer.Load( _passwordStore );
            // Verify
            Assert.That( passwords, Is.Empty );
        }


        [ Test ]
        public void LoadMultiplePasswords( )
        {
            // Setup
            XElement element1 = new PasswordDigestDocumentBuilder { Key = "aKey" };
            XElement element2 = new PasswordDigestDocumentBuilder { Key = "anotherKey" };
            SaveXml( new XElement( PasswordSerializer2.PasswordStoreElement,
                                   element1,
                                   element2 ) );
            // Exercise
            IEnumerable<PasswordDigestDocument> passwords = _serializer.Load( _passwordStore );
            // Verify
            Assert.That( passwords.Count( ), Is.EqualTo( 2 ) );
        }


        [ Test ]
        public void LoadReadsPasswordKeyFromElement( )
        {
            // Setup
            XElement element = new PasswordDigestDocumentBuilder { Key = "aKey" };
            SaveXml( new XElement( PasswordSerializer2.PasswordStoreElement,
                                   element ) );
            // Exercise
            IEnumerable<PasswordDigestDocument> passwords = _serializer.Load( _passwordStore );
            // Verify
            PasswordDigestDocument passwordDigest = passwords.First( );
            Assert.That( passwordDigest.Key, Is.EqualTo( "aKey" ) );
        }

        [ Test ]
        public void LoadReadsPasswordHashFromElement( )
        {
            // Setup
            XElement element = new PasswordDigestDocumentBuilder { Key = "aKey", Hash = new byte[ ] { 0x44, 0x66 } };
            SaveXml( new XElement( PasswordSerializer2.PasswordStoreElement,
                                   element ) );
            // Exercise
            IEnumerable<PasswordDigestDocument> passwords = _serializer.Load( _passwordStore );
            // Verify
            PasswordDigestDocument passwordDigest = passwords.First( );
            Assert.That( passwordDigest.Key, Is.EqualTo( "aKey" ) );
            Assert.That( passwordDigest.Hash.SequenceEqual( new byte[ ] { 0x44, 0x66 } ) );
        }

        [ Test ]
        public void LoadReadsMasterPasswordGuidFromElement( )
        {
            // Setup
            Guid guid = Guid.Parse( "34579b9f-8ac1-464a-805a-abe564da8848" );
            XElement element = new PasswordDigestDocumentBuilder { Key = "aKey", MasterPasswordId = guid };
            SaveXml( new XElement( PasswordSerializer2.PasswordStoreElement,
                                   element ) );
            // Exercise
            IEnumerable<PasswordDigestDocument> passwords = _serializer.Load( _passwordStore );
            // Verify
            PasswordDigestDocument passwordDigest = passwords.First( );
            Assert.That( passwordDigest.Key, Is.EqualTo( "aKey" ) );
            Assert.That( passwordDigest.MasterPasswordId, Is.EqualTo( guid ) );
        }

        [ Test ]
        public void LoadReadsPasswordSettingsGuidFromElement( )
        {
            // Setup
            Guid guid = Guid.Parse( "34579b9f-8ac1-464a-805a-abe564da8848" );
            XElement element = new PasswordDigestDocumentBuilder { Key = "aKey", PasswordGenerator = guid };
            SaveXml( new XElement( PasswordSerializer2.PasswordStoreElement,
                                   element ) );
            // Exercise
            IEnumerable<PasswordDigestDocument> passwords = _serializer.Load( _passwordStore );
            // Verify
            PasswordDigestDocument passwordDigest = passwords.First( );
            Assert.That( passwordDigest.Key, Is.EqualTo( "aKey" ) );
            Assert.That( passwordDigest.PasswordGenerator, Is.EqualTo( guid ) );
        }

        [ Test ]
        public void LoadReadsCreationDateFromElement( )
        {
            // Setup
            DateTime creationTime = new DateTime( 634022874410500302 );
            XElement element = new PasswordDigestDocumentBuilder { Key = "aKey", CreatedOn = creationTime };
            SaveXml( new XElement( PasswordSerializer2.PasswordStoreElement,
                                   element ) );
            // Exercise
            IEnumerable<PasswordDigestDocument> passwords = _serializer.Load( _passwordStore );
            // Verify
            PasswordDigestDocument passwordDigest = passwords.First( );
            Assert.That( passwordDigest.Key, Is.EqualTo( "aKey" ) );
            Assert.That( passwordDigest.CreatedOn, Is.EqualTo( creationTime ) );
        }

        [ Test ]
        public void LoadReadsModificationDateFromElement( )
        {
            // Setup
            DateTime modificationTime = new DateTime( 634122874455500442 );
            XElement element = new PasswordDigestDocumentBuilder { Key = "aKey", ModifiedOn = modificationTime };
            SaveXml( new XElement( PasswordSerializer2.PasswordStoreElement,
                                   element ) );
            // Exercise
            IEnumerable<PasswordDigestDocument> passwords = _serializer.Load( _passwordStore );
            // Verify
            PasswordDigestDocument passwordDigest = passwords.First( );
            Assert.That( passwordDigest.Key, Is.EqualTo( "aKey" ) );
            Assert.That( passwordDigest.ModifiedOn, Is.EqualTo( modificationTime ) );
        }


        [ Test ]
        public void LoadReadsNoteFromElement( )
        {
            // Setup
            const string note = "a Note";
            XElement element = new PasswordDigestDocumentBuilder { Key = "aKey", Note = note };
            SaveXml( new XElement( PasswordSerializer2.PasswordStoreElement,
                                   element ) );
            // Exercise
            IEnumerable<PasswordDigestDocument> passwords = _serializer.Load( _passwordStore );
            // Verify
            PasswordDigestDocument passwordDigest = passwords.First( );
            Assert.That( passwordDigest.Key, Is.EqualTo( "aKey" ) );
            Assert.That( passwordDigest.Note, Is.EqualTo( note ) );
        }

        [ Test ]
        public void LoadReadsEmptyNoteFromMissingNoteElement( )
        {
            // Setup
            XElement element = new PasswordDigestDocumentBuilder { Key = "aKey" };
            SaveXml( new XElement( PasswordSerializer2.PasswordStoreElement,
                                   element ) );
            // Exercise
            IEnumerable<PasswordDigestDocument> passwords = _serializer.Load( _passwordStore );
            // Verify
            PasswordDigestDocument passwordDigest = passwords.First( );
            Assert.That( passwordDigest.Key, Is.EqualTo( "aKey" ) );
            Assert.That( passwordDigest.Note, Is.Empty );
        }

        [ Test ]
        public void LoadSetsDefaultModificationTimeInVersion0File( )
        {
            // Set up
            XElement element = new PasswordDigestDocumentBuilder { Key = "aKey", CreatedOn = new DateTime( 123456789123L ) };
            SaveXml( new XElement( PasswordSerializer2.PasswordStoreElement,
                                   new XAttribute( PasswordSerializer2.VersionAttribute, "0" ),
                                   element ) );

            // Exercise
            IEnumerable<PasswordDigestDocument> passwords = _serializer.Load( _passwordStore );
            // Verify
            PasswordDigestDocument passwordDigest = passwords.First( );
            Assert.That( passwordDigest.ModifiedOn, Is.EqualTo( passwordDigest.CreatedOn ) );
        }

        [ Test ]
        public void LoadDropsPasswordWithoutModificationTimeInVersion1File( )
        {
            // Set up
            XElement element = new PasswordDigestDocumentBuilder { Key = "aKey", CreatedOn = new DateTime( 123456789123L ) };

            element.Elements( PasswordSerializer2.ModifiedElement ).Remove( );

            SaveXml( new XElement( PasswordSerializer2.PasswordStoreElement,
                                   new XAttribute( PasswordSerializer2.VersionAttribute, "1" ),
                                   element ) );

            // Exercise
            IEnumerable<PasswordDigestDocument> passwords = _serializer.Load( _passwordStore );
            // Verify
            Assert.That( passwords, Is.Empty );
        }

        [ Test ]
        public void LoadFixesInconsistentModificationTimeInVersion1File( )
        {
            // Set up
            XElement element = new PasswordDigestDocumentBuilder
                                   {
                                       Key = "aKey",
                                       CreatedOn = new DateTime( 123456789123L ),
                                       ModifiedOn = new DateTime( 111111111111L ),
                                   };
            SaveXml( new XElement( PasswordSerializer2.PasswordStoreElement,
                                   new XAttribute( PasswordSerializer2.VersionAttribute, "1" ),
                                   element ) );

            // Exercise
            IEnumerable<PasswordDigestDocument> passwords = _serializer.Load( _passwordStore );
            // Verify
            PasswordDigestDocument passwordDigest = passwords.First( );
            Assert.That( passwordDigest.ModifiedOn, Is.EqualTo( passwordDigest.CreatedOn ) );
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

        private PasswordSerializer2 _serializer;

        private InMemoryPasswordStore _passwordStore;
    }
}