using System.Collections.Generic;
using System.Linq;
using Chwthewke.PasswordManager.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Storage
{
    [ TestFixture ]
    public class XmlPasswordDataTest
    {
        private XmlPasswordData _passwordData;
        private InMemoryPasswordStore _inMemoryPasswordStore;
        private IList<PasswordDigestDocument> _passwords;
        private PasswordSerializer2 _serializer;

        [ SetUp ]
        public void SetupPasswordData( )
        {
            _inMemoryPasswordStore = new InMemoryPasswordStore( );
            _serializer = new PasswordSerializer2( );
            _passwordData = new XmlPasswordData( _serializer, _inMemoryPasswordStore );

            _passwords = new[ ] { TestPasswords.Abcd, TestPasswords.Efgh }.ToList( );
        }

        [ Test ]
        public void SaveWritesPasswordListToXmlAsPerSerializer( )
        {
            // Set up

            // Exercise
            _passwordData.SavePasswords( _passwords );
            // Verify
            Assert.That( _inMemoryPasswordStore.Content, Is.EqualTo( _serializer.ToXml( _passwords ) ) );
        }

        [ Test ]
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