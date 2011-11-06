using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Chwthewke.PasswordManager.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Storage
{
    [ TestFixture ]
    public class PasswordSerializer2SaveTest
    {
        [ SetUp ]
        public void SetUpSerializer( )
        {
            _serializer = new PasswordSerializer2( );
            _passwordStore = new InMemoryPasswordStore( );
            _passwords = new List<PasswordDigestDocument>( );
        }


        [ Test ]
        public void DocumentHasNoXmlDeclaration( )
        {
            // Setup
            // Exercise
            _serializer.Save( _passwords, _passwordStore );
            // Verify
            XDocument xDocument = XDocument.Parse( _passwordStore.Content );
            Assert.That( xDocument.Declaration, Is.Null );
        }

        [ Test ]
        public void SerializeEmptyPasswordStore( )
        {
            // Setup
            // Exercise
            _serializer.Save( _passwords, _passwordStore );
            // Verify
            XElement xElement = ReadSerializedXml( );
            Assert.That( xElement.IsEmpty );
            Assert.That( xElement.Name, Is.EqualTo( (XName) PasswordSerializer2.PasswordStoreElement ) );

            XAttribute versionAttribute = xElement.Attribute( PasswordSerializer2.VersionAttribute );
            Assert.That( versionAttribute, Is.Not.Null );
// ReSharper disable PossibleNullReferenceException
            Assert.That( versionAttribute.Value, Is.EqualTo( "2" ) );
// ReSharper restore PossibleNullReferenceException
        }

        [ Test ]
        public void SerializeMultiplePasswords( )
        {
            // Setup
            _passwords.Add( new PasswordDigestDocumentBuilder { Key = "key", Hash = new byte[ ] { 0x55, 0xda } } );
            _passwords.Add( new PasswordDigestDocumentBuilder { Key = "otherKey", Hash = new byte[ ] { 0x55, 0xef } } );
            // Exercise
            _serializer.Save( _passwords, _passwordStore );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            Assert.That( rootElement.Elements( ).Count( ), Is.EqualTo( 2 ) );
            Assert.That( rootElement.Elements( PasswordSerializer2.PasswordElement ).Count( ), Is.EqualTo( 2 ) );
        }

        [ Test ]
        public void SerializePasswordKeyToElement( )
        {
            // Setup
            _passwords.Add( new PasswordDigestDocumentBuilder { Key = "key", Hash = new byte[ ] { 0x55, 0xda } } );
            // Exercise
            _serializer.Save( _passwords, _passwordStore );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            XElement passwordElement = SingleChild( rootElement, PasswordSerializer2.PasswordElement );

            XElement keyElement = SingleChild( passwordElement, PasswordSerializer2.KeyElement );
            Assert.That( keyElement.Value, Is.EqualTo( "key" ) );
        }


        [ Test ]
        public void SerializePasswordHashToElement( )
        {
            // Setup
            _passwords.Add( new PasswordDigestDocumentBuilder { Key = "key", Hash = new byte[ ] { 0x55, 0xda } } );

            // Exercise
            _serializer.Save( _passwords, _passwordStore );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            XElement passwordElement = SingleChild( rootElement, PasswordSerializer2.PasswordElement );

            XElement hashElement = SingleChild( passwordElement, PasswordSerializer2.HashElement );
            Assert.That( hashElement.Value, Is.EqualTo( Convert.ToBase64String( new byte[ ] { 0x55, 0xda } ) ) );
        }

        [ Test ]
        public void SerializeMasterPasswordGuidToElement( )
        {
            // Setup
            _passwords.Add( new PasswordDigestDocumentBuilder
                                {
                                    Key = "key",
                                    Hash = new byte[ ] { 0x55, 0xda },
                                    MasterPasswordId = new Guid( "34579b9f-8ac1-464a-805a-abe564da8848" )
                                } );
            // Exercise
            _serializer.Save( _passwords, _passwordStore );
            // Verify
            XElement rootElement = ReadSerializedXml( );

            XElement passwordElement = SingleChild( rootElement, PasswordSerializer2.PasswordElement );

            XElement masterPasswordIdElement = SingleChild( passwordElement, PasswordSerializer2.MasterPasswordIdElement );
            Assert.That( masterPasswordIdElement.Value, Is.EqualTo( "34579b9f-8ac1-464a-805a-abe564da8848" ).IgnoreCase );
        }

        [ Test ]
        public void SerializePasswordSettingsGuidToElement( )
        {
            // Setup
            _passwords.Add( new PasswordDigestDocumentBuilder
                                {
                                    Key = "key",
                                    Hash = new byte[ ] { 0x55, 0xda },
                                    PasswordGenerator = new Guid( "34579b9f-8ac1-464a-805a-abe564da8848" )
                                } );
            // Exercise
            _serializer.Save( _passwords, _passwordStore );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            XElement passwordElement = SingleChild( rootElement, PasswordSerializer2.PasswordElement );


            XElement passwordSettingsIdElement = SingleChild( passwordElement, PasswordSerializer2.PasswordSettingsIdElement );
            Assert.That( passwordSettingsIdElement.Value, Is.EqualTo( "34579b9f-8ac1-464a-805a-abe564da8848" ).IgnoreCase );
        }

        [ Test ]
        public void SerializeIterationToElement( )
        {
            // Set up
            _passwords.Add( new PasswordDigestDocumentBuilder
                                {
                                    Key = "key",
                                    Hash = new byte[ ] { 0x55, 0xda },
                                    PasswordGenerator = new Guid( "34579b9f-8ac1-464a-805a-abe564da8848" ),
                                    Iteration = 2
                                } );
            // Exercise
            _serializer.Save( _passwords, _passwordStore );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            XElement passwordElement = SingleChild( rootElement, PasswordSerializer2.PasswordElement );

            XElement iterationElement = SingleChild( passwordElement, PasswordSerializer2.IterationElement );
            Assert.That( iterationElement.Value, Is.EqualTo( "2" ) );
        }

        [ Test ]
        public void SerializePasswordCreationTimestampToElement( )
        {
            // Setup
            _passwords.Add( new PasswordDigestDocumentBuilder
                                {
                                    Key = "key",
                                    Hash = new byte[ ] { 0x55, 0xda },
                                    CreatedOn = new DateTime( 634022874410500302 )
                                } );
            // Exercise
            _serializer.Save( _passwords, _passwordStore );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            XElement passwordElement = SingleChild( rootElement, PasswordSerializer2.PasswordElement );

            XElement creationDateElement = SingleChild( passwordElement, PasswordSerializer2.TimestampElement );
            Assert.That( creationDateElement.Value, Is.EqualTo( "634022874410500302" ) );
        }

        [ Test ]
        public void SerializePasswordModificationTimestampToElement( )
        {
            // Setup
            _passwords.Add( new PasswordDigestDocumentBuilder
                                {
                                    Key = "key",
                                    Hash = new byte[ ] { 0x55, 0xda },
                                    ModifiedOn = new DateTime( 634022374410599302 )
                                } );
            // Exercise
            _serializer.Save( _passwords, _passwordStore );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            XElement passwordElement = SingleChild( rootElement, PasswordSerializer2.PasswordElement );

            XElement modificationDateElement = SingleChild( passwordElement, PasswordSerializer2.ModifiedElement );
            Assert.That( modificationDateElement.Value, Is.EqualTo( "634022374410599302" ) );
        }

        [ Test ]
        public void SerializePasswordNoteToElement( )
        {
            // Setup
            _passwords.Add( new PasswordDigestDocumentBuilder
                                {
                                    Key = "key",
                                    Hash = new byte[ ] { 0x55, 0xda },
                                    Note = "No note"
                                } );
            // Exercise
            _serializer.Save( _passwords, _passwordStore );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            XElement passwordElement = SingleChild( rootElement, PasswordSerializer2.PasswordElement );

            XElement noteElement = SingleChild( passwordElement, PasswordSerializer2.NoteElement );
            Assert.That( noteElement.Value, Is.EqualTo( "No note" ) );
        }

        [ Test ]
        public void SerializeNullPasswordNoteToEmptyElement( )
        {
            // Setup
            _passwords.Add( new PasswordDigestDocumentBuilder { Key = "key", Hash = new byte[ ] { 0x55, 0xda }, } );
            // Exercise
            _serializer.Save( _passwords, _passwordStore );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            XElement passwordElement = SingleChild( rootElement, PasswordSerializer2.PasswordElement );

            XElement noteElement = SingleChild( passwordElement, PasswordSerializer2.NoteElement );
            Assert.That( noteElement.Value, Is.EqualTo( string.Empty ) );
        }


        private XElement SingleChild( XElement parent, XName name )
        {
            IEnumerable<XElement> elements = parent.Elements( name ).ToList( );
            Assert.That( elements.Count( ), Is.EqualTo( 1 ) );
            return elements.First( );
        }

        private XElement ReadSerializedXml( )
        {
            return XElement.Parse( _passwordStore.Content );
        }

        private PasswordSerializer2 _serializer;

        private IList<PasswordDigestDocument> _passwords;

        private InMemoryPasswordStore _passwordStore;
    }
}