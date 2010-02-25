using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
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
            _serializer = new PasswordStoreSerializer( new UTF8Encoding( false ) );
            _outputStream = new MemoryStream( );
            _passwordStore = new PasswordStore( );
        }

        [TearDown]
        public void TearDownMemoryStream( )
        {
            _outputStream.Dispose( );
        }

        private IPasswordStoreSerializer _serializer;
        private MemoryStream _outputStream;
        private IPasswordStore _passwordStore;

        private XElement ReadSerializedXml( )
        {
            string text = new UTF8Encoding( false ).GetString( _outputStream.ToArray( ) );
            Console.WriteLine( "Text: [{0}]", text );
            return XElement.Parse( text );
        }

        [ Test ]
        public void SerializeEmptyPasswordStore( )
        {
            // Setup
            // Exercise
            _serializer.Save( _passwordStore, _outputStream );
            // Verify
            XElement xElement = ReadSerializedXml( );
            Assert.That( xElement.IsEmpty );
            Assert.That( xElement.Name, Is.EqualTo( ( XName ) PasswordStoreSerializer.PasswordStoreElement ) );
        }

        [ Test ]
        public void SerializeMasterPasswordGuidToElement( )
        {
            // Setup
            _passwordStore.AddOrUpdate( new PasswordInfo( "key", new byte[ ] { 0x55, 0xda },
                                                          new Guid( "34579b9f-8ac1-464a-805a-abe564da8848" ),
                                                          new DateTime( ), "No note" ) );
            // Exercise
            _serializer.Save( _passwordStore, _outputStream );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            XElement passwordElement = rootElement.Elements( PasswordStoreSerializer.PasswordElement ).First( );
            IEnumerable<XElement> guidElements = passwordElement.Elements( PasswordStoreSerializer.GuidElement );
            Assert.That( guidElements.Count( ), Is.EqualTo( 1 ) );
            Assert.That( guidElements.First( ).Value, Is.EqualTo( "34579b9f-8ac1-464a-805a-abe564da8848" ).IgnoreCase );
        }


        [ Test ]
        public void SerializeMultiplePasswords( )
        {
            // Setup
            _passwordStore.AddOrUpdate( new PasswordInfo( "key", new byte[ ] { 0x55, 0xda }, new Guid( ),
                                                          new DateTime( ), "No note" ) );
            _passwordStore.AddOrUpdate( new PasswordInfo( "otherKey", new byte[ ] { 0x55, 0xef }, new Guid( ),
                                                          new DateTime( ), "Still no note" ) );
            // Exercise
            _serializer.Save( _passwordStore, _outputStream );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            Assert.That( rootElement.Elements( ).Count( ), Is.EqualTo( 2 ) );
            Assert.That( rootElement.Elements( PasswordStoreSerializer.PasswordElement ).Count( ), Is.EqualTo( 2 ) );
        }

        [ Test ]
        public void SerializePasswordHashToElement( )
        {
            // Setup
            _passwordStore.AddOrUpdate( new PasswordInfo( "key", new byte[ ] { 0x55, 0xda }, new Guid( ),
                                                          new DateTime( ), "No note" ) );

            // Exercise
            _serializer.Save( _passwordStore, _outputStream );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            XElement passwordElement = rootElement.Elements( PasswordStoreSerializer.PasswordElement ).First( );
            IEnumerable<XElement> hashElements = passwordElement.Elements( PasswordStoreSerializer.HashElement );
            Assert.That( hashElements.Count( ), Is.EqualTo( 1 ) );
            Assert.That( hashElements.First( ).Value, Is.EqualTo( Convert.ToBase64String( new byte[ ] { 0x55, 0xda } ) ) );
        }

        [ Test ]
        public void SerializePasswordKeyToElement( )
        {
            // Setup
            _passwordStore.AddOrUpdate( new PasswordInfo( "key", new byte[ ] { 0x55, 0xda }, new Guid( ),
                                                          new DateTime( ), "No note" ) );
            // Exercise
            _serializer.Save( _passwordStore, _outputStream );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            XElement passwordElement = rootElement.Elements( PasswordStoreSerializer.PasswordElement ).First( );
            IEnumerable<XElement> keyElements = passwordElement.Elements( PasswordStoreSerializer.KeyElement );
            Assert.That( keyElements.Count( ), Is.EqualTo( 1 ) );
            Assert.That( keyElements.First( ).Value, Is.EqualTo( "key" ) );
        }

        [ Test ]
        public void SerializePasswordNoteToElement( )
        {
            // Setup
            _passwordStore.AddOrUpdate( new PasswordInfo( "key", new byte[ ] { 0x55, 0xda },
                                                          new Guid( ),
                                                          new DateTime( 634022874410500302 ),
                                                          "No note" ) );
            // Exercise
            _serializer.Save( _passwordStore, _outputStream );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            XElement passwordElement = rootElement.Elements( PasswordStoreSerializer.PasswordElement ).First( );
            IEnumerable<XElement> noteElements = passwordElement.Elements( PasswordStoreSerializer.NoteElement );
            Assert.That( noteElements.Count( ), Is.EqualTo( 1 ) );
            Assert.That( noteElements.First( ).Value, Is.EqualTo( "No note" ) );
        }

        [ Test ]
        public void SerializePasswordTimestampToElement( )
        {
            // Setup
            _passwordStore.AddOrUpdate( new PasswordInfo( "key", new byte[ ] { 0x55, 0xda },
                                                          new Guid( ),
                                                          new DateTime( 634022874410500302 ),
                                                          "No note" ) );
            // Exercise
            _serializer.Save( _passwordStore, _outputStream );
            // Verify
            XElement rootElement = ReadSerializedXml( );
            XElement passwordElement = rootElement.Elements( PasswordStoreSerializer.PasswordElement ).First( );
            IEnumerable<XElement> timestampElements = passwordElement.Elements( PasswordStoreSerializer.TimestampElement );
            Assert.That( timestampElements.Count( ), Is.EqualTo( 1 ) );
            Assert.That( timestampElements.First( ).Value, Is.EqualTo( "634022874410500302" ) );
        }
    }
}