using System;
using System.Collections.Generic;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Storage
{
    [ TestFixture ]
    public class XmlPasswordDataTest
    {
        private XmlPasswordData _passwordData;
        private InMemoryPasswordStore _inMemoryPasswordStore;
        private List<PasswordDigestDocument> _passwords;
        private PasswordSerializer2 _serializer;

        [ SetUp ]
        public void SetupPasswordData( )
        {
            _inMemoryPasswordStore = new InMemoryPasswordStore( );
            _serializer = new PasswordSerializer2( );
            _passwordData = new XmlPasswordData( _serializer, _inMemoryPasswordStore );

            _passwords = new List<PasswordDigestDocument>
                             {
                                 new PasswordDigestDocumentBuilder
                                     {
                                         Key = "abcd",
                                         Iteration = 1,
                                         Hash = new byte[ ] { 0xAA, 0xBB },
                                         PasswordGenerator = PasswordGenerators2.AlphaNumeric,
                                         CreatedOn = new DateTime( 2011, 11, 1 ),
                                         ModifiedOn = new DateTime( 2011, 11, 1 ),
                                         MasterPasswordId = Guid.NewGuid( ),
                                         Note = "First password"
                                     },
                                 new PasswordDigestDocumentBuilder
                                     {
                                         Key = "efgh",
                                         Iteration = 10,
                                         Hash = new byte[ ] { 0x0A, 0x0B },
                                         PasswordGenerator = PasswordGenerators2.Full,
                                         CreatedOn = new DateTime( 2011, 11, 2 ),
                                         ModifiedOn = new DateTime( 2011, 11, 3 ),
                                         MasterPasswordId = Guid.NewGuid( ),
                                         Note = "Second password"
                                     },
                             };
        }

        [Test]
        public void SaveWritesPasswordListToXmlAsPerSerializer( )
        {
            // Set up

            // Exercise
            _passwordData.SavePasswords( _passwords );
            // Verify
            Assert.That( _inMemoryPasswordStore.Content, Is.EqualTo( _serializer.ToXml( _passwords ) ) );
        }
        [Test]
        public void LoadReadsPasswordListFromXmlAsPerSerializer( )
        {
            // Set up
            _inMemoryPasswordStore.Content = _serializer.ToXml( _passwords );
            // Exercise
            IEnumerable<PasswordDigestDocument> passwords = _passwordData.LoadPasswords( );
            // Verify
            Assert.That( passwords, Is.EquivalentTo( _passwords ) );
        }
    }
}
