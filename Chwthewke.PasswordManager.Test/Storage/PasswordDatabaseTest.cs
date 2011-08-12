using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Autofac;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.App;
using Chwthewke.PasswordManager.Test.Util;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Storage
{
    internal class PasswordDatabaseTest
    {
        public IPasswordStore InMemoryPasswordStore { get; set; }

        public IPasswordDatabase Database { get; set; }

        public IPasswordSerializer Serializer { get; set; }

        [SetUp]
        public void SetUpDatabaseWithMockStore( )
        {
            IContainer container =
                AppSetUp.TestContainer(
                    b => b.RegisterType<InMemoryPasswordStore>( ).As<IPasswordStore>( ) );
            Database = container.Resolve<IPasswordDatabase>( );
            Serializer = container.Resolve<IPasswordSerializer>( );

            container.InjectUnsetProperties( this );
        }

        [Test]
        public void InitLoadsPasswordsFromSource( )
        {
            // Set up
            IList<PasswordDigest> passwordDigests =
                new List<PasswordDigest>
                    {
                        new PasswordDigestBuilder { Key = "abc" },
                    };
            Serializer.Save( passwordDigests, InMemoryPasswordStore );

            // Exercise

            IPasswordDatabase database = 
                new PasswordDatabase( Serializer, ( ) => InMemoryPasswordStore );

            // Verify
            Assert.That( database.Passwords, Is.EquivalentTo( passwordDigests ) );
        }
    }
}