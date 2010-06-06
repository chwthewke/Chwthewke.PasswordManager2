using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Storage
{
    [ TestFixture ]
    public class PasswordStoreSerializerSaveTest
    {
        [ SetUp ]
        public void SetUpSerializer( )
        {
            _serializer = new PasswordSerializer( );
            _writer = new StringWriter( );
            _passwordRepository = new PasswordRepository( PasswordGenerators.All, new Sha512Factory( ) );
        }

        [ TearDown ]
        public void TearDownMemoryStream( )
        {
            _writer.Dispose( );
        }


        [ Test ]
        public void DocumentHasNoXmlDeclaration( )
        {
            // Setup
            // Exercise
            _serializer.Save( _passwordRepository, _writer );
            // Verify
            XDocument xDocument = XDocument.Parse( SerializedXml );
            Assert.That( xDocument.Declaration, Is.Null );
        }

        [ Test ]
        public void SerializeEmptyPasswordStore( )
        {
            // Setup
            // Exercise
            _serializer.Save( _passwordRepository, _writer );
            // Verify
            XElement xElement = ReadSerializedXml( );
            Assert.That( xElement.IsEmpty );
            Assert.That( xElement.Name, Is.EqualTo( ( XName ) PasswordSerializer.PasswordStoreElement ) );
        }

        [ Test ]
        public void SerializeMultiplePasswords( )
        {
            // Setup
            _passwordRepository.AddOrUpdate( new PasswordDigest( "key", new byte[ ] { 0x55, 0xda }, default( Guid ),
                                                            default( Guid ), default( DateTime ), "No note" ) );
            _passwordRepository.AddOrUpdate( new PasswordDigest( "otherKey", new byte[ ] { 0x55, 0xef }, default( Guid ),
                                                            default( Guid ), default( DateTime ), "Still no note" ) );
            // Exercise
            _serializer.Save( _passwordRepository, _writer );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            Assert.That( rootElement.Elements( ).Count( ), Is.EqualTo( 2 ) );
            Assert.That( rootElement.Elements( PasswordSerializer.PasswordElement ).Count( ), Is.EqualTo( 2 ) );
        }

        [ Test ]
        public void SerializePasswordKeyToElement( )
        {
            // Setup
            _passwordRepository.AddOrUpdate( new PasswordDigest( "key", new byte[ ] { 0x55, 0xda }, default( Guid ),
                                                            default( Guid ), default( DateTime ), "No note" ) );
            // Exercise
            _serializer.Save( _passwordRepository, _writer );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            XElement passwordElement = rootElement.Elements( PasswordSerializer.PasswordElement ).First( );
            IEnumerable<XElement> keyElements = passwordElement.Elements( PasswordSerializer.KeyElement );
            Assert.That( keyElements.Count( ), Is.EqualTo( 1 ) );
            Assert.That( keyElements.First( ).Value, Is.EqualTo( "key" ) );
        }


        [ Test ]
        public void SerializePasswordHashToElement( )
        {
            // Setup
            _passwordRepository.AddOrUpdate( new PasswordDigest( "key", new byte[ ] { 0x55, 0xda }, default( Guid ),
                                                            default( Guid ), default( DateTime ), "No note" ) );

            // Exercise
            _serializer.Save( _passwordRepository, _writer );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            XElement passwordElement = rootElement.Elements( PasswordSerializer.PasswordElement ).First( );
            IEnumerable<XElement> hashElements = passwordElement.Elements( PasswordSerializer.HashElement );
            Assert.That( hashElements.Count( ), Is.EqualTo( 1 ) );
            Assert.That( hashElements.First( ).Value, Is.EqualTo( Convert.ToBase64String( new byte[ ] { 0x55, 0xda } ) ) );
        }

        [ Test ]
        public void SerializeMasterPasswordGuidToElement( )
        {
            // Setup
            _passwordRepository.AddOrUpdate( new PasswordDigest( "key", new byte[ ] { 0x55, 0xda },
                                                            new Guid( "34579b9f-8ac1-464a-805a-abe564da8848" ),
                                                            default( Guid ), default( DateTime ), "No note" ) );
            // Exercise
            _serializer.Save( _passwordRepository, _writer );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            XElement passwordElement = rootElement.Elements( PasswordSerializer.PasswordElement ).First( );
            IEnumerable<XElement> guidElements =
                passwordElement.Elements( PasswordSerializer.MasterPasswordIdElement );
            Assert.That( guidElements.Count( ), Is.EqualTo( 1 ) );
            Assert.That( guidElements.First( ).Value, Is.EqualTo( "34579b9f-8ac1-464a-805a-abe564da8848" ).IgnoreCase );
        }

        [ Test ]
        public void SerializePasswordSettingsGuidToElement( )
        {
            // Setup
            _passwordRepository.AddOrUpdate( new PasswordDigest( "key", new byte[ ] { 0x55, 0xda }, default( Guid ),
                                                            new Guid( "34579b9f-8ac1-464a-805a-abe564da8848" ),
                                                            default( DateTime ), "No note" ) );
            // Exercise
            _serializer.Save( _passwordRepository, _writer );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            XElement passwordElement = rootElement.Elements( PasswordSerializer.PasswordElement ).First( );
            IEnumerable<XElement> guidElements =
                passwordElement.Elements( PasswordSerializer.PasswordSettingsIdElement );
            Assert.That( guidElements.Count( ), Is.EqualTo( 1 ) );
            Assert.That( guidElements.First( ).Value, Is.EqualTo( "34579b9f-8ac1-464a-805a-abe564da8848" ).IgnoreCase );
        }


        [ Test ]
        public void SerializePasswordTimestampToElement( )
        {
            // Setup
            _passwordRepository.AddOrUpdate( new PasswordDigest( "key", new byte[ ] { 0x55, 0xda },
                                                            default( Guid ), default( Guid ),
                                                            new DateTime( 634022874410500302 ),
                                                            "No note" ) );
            // Exercise
            _serializer.Save( _passwordRepository, _writer );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            XElement passwordElement = rootElement.Elements( PasswordSerializer.PasswordElement ).First( );
            IEnumerable<XElement> timestampElements = passwordElement.Elements( PasswordSerializer.TimestampElement );
            Assert.That( timestampElements.Count( ), Is.EqualTo( 1 ) );
            Assert.That( timestampElements.First( ).Value, Is.EqualTo( "634022874410500302" ) );
        }

        [ Test ]
        public void SerializePasswordNoteToElement( )
        {
            // Setup
            _passwordRepository.AddOrUpdate( new PasswordDigest( "key", new byte[ ] { 0x55, 0xda },
                                                            default( Guid ), default( Guid ),
                                                            new DateTime( 634022874410500302 ),
                                                            "No note" ) );
            // Exercise
            _serializer.Save( _passwordRepository, _writer );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            XElement passwordElement = rootElement.Elements( PasswordSerializer.PasswordElement ).First( );
            IEnumerable<XElement> noteElements = passwordElement.Elements( PasswordSerializer.NoteElement );
            Assert.That( noteElements.Count( ), Is.EqualTo( 1 ) );
            Assert.That( noteElements.First( ).Value, Is.EqualTo( "No note" ) );
        }

        [ Test ]
        public void SerializeNullPasswordNoteToEmptyElement( )
        {
            // Setup
            _passwordRepository.AddOrUpdate( new PasswordDigest( "key", new byte[ ] { 0x55, 0xda },
                                                            default( Guid ), default( Guid ),
                                                            new DateTime( 634022874410500302 ),
                                                            null ) );
            // Exercise
            _serializer.Save( _passwordRepository, _writer );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            XElement passwordElement = rootElement.Elements( PasswordSerializer.PasswordElement ).First( );
            IEnumerable<XElement> noteElements = passwordElement.Elements( PasswordSerializer.NoteElement );
            Assert.That( noteElements.Count( ), Is.EqualTo( 0 ) );
        }


        private XElement ReadSerializedXml( )
        {
            return XElement.Parse( SerializedXml );
        }

        private string SerializedXml
        {
            get { return _writer.ToString( ); }
        }

        private IPasswordSerializer _serializer;
        private IPasswordRepository _passwordRepository;
        private StringWriter _writer;
    }
}