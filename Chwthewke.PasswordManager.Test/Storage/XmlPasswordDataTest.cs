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
        private InMemoryTextResource _inMemoryTextResource;
        private IList<PasswordDigestDocument> _passwords;
        private PasswordSerializer _serializer;

        [ SetUp ]
        public void SetupPasswordData( )
        {
            _inMemoryTextResource = new InMemoryTextResource( );
            _serializer = new PasswordSerializer( );
            _passwordData = new XmlPasswordData( _serializer, _inMemoryTextResource );

            _passwords = new[ ] { TestPasswords.Abcd, TestPasswords.Efgh }.ToList( );
        }

        [ Test ]
        public void SaveWritesPasswordListToXmlAsPerSerializer( )
        {
            // Set up

            // Exercise
            _passwordData.SavePasswords( _passwords );
            // Verify
            Assert.That( _inMemoryTextResource.Content, Is.EqualTo( _serializer.ToXml( _passwords ) ) );
        }

        [ Test ]
        public void LoadReadsPasswordListFromXmlAsPerSerializer( )
        {
            // Set up
            _inMemoryTextResource.Content = _serializer.ToXml( _passwords );
            // Exercise
            IEnumerable<PasswordDigestDocument> passwords = _passwordData.LoadPasswords( );
            // Verify
            Assert.That( passwords, Is.EquivalentTo( _passwords ) );
        }
    }
}