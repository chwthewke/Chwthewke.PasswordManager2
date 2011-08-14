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
// ReSharper disable UnusedAutoPropertyAccessor.Global
        public IPasswordStore InMemoryPasswordStore { get; set; }

        public IPasswordDatabase Database { get; set; }

        public IPasswordSerializer Serializer { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global

        [SetUp]
        public void SetUpDatabaseWithMockStore( )
        {
            IContainer container =
                AppSetUp.TestContainer(
                    b =>
                        {
                            b.RegisterType<InMemoryPasswordStore>( ).As<IPasswordStore>( ).SingleInstance( );
                            b.Register<Func<IPasswordStore>>( c => ( ( ) => c.Resolve<IPasswordStore>( ) ) )
                                .As<Func<IPasswordStore>>( );
                        } );

            container.InjectProperties( this );
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

        [Test]
        public void ReloadLoadsPasswordsFromSource( )
        {
            // Set up
            IList<PasswordDigest> passwordDigests =
                new List<PasswordDigest>
                    {
                        new PasswordDigestBuilder { Key = "abc" },
                    };
            Serializer.Save( passwordDigests, InMemoryPasswordStore );

            // Exercise

            Database.Reload( );

            // Verify
            Assert.That( Database.Passwords, Is.EquivalentTo( passwordDigests ) );
        }

        [Test]
        public void SetSourceSavesOldSourceToNewSource( )
        {
            // Set up
            IList<PasswordDigest> passwordDigests =
                new List<PasswordDigest>
                    {
                        new PasswordDigestBuilder { Key = "abc" },
                    };
            Serializer.Save( passwordDigests, InMemoryPasswordStore );

            var newSource = new InMemoryPasswordStore( );
            // Exercise
            Database.Source = newSource;
            // Verify
            Assert.That( Serializer.Load( newSource ), Is.EquivalentTo( passwordDigests ) );
            Assert.That( Database.Passwords, Is.EquivalentTo( passwordDigests ) );
        }

        [Test]
        public void AddPasswordSavesToSource( )
        {
            // Set up
            PasswordDigest password = new PasswordDigestBuilder { Key = "def" };
            // Exercise
            Database.AddOrUpdate( password );
            // Verify
            Assert.That( Serializer.Load( InMemoryPasswordStore ), Is.EquivalentTo( new List<PasswordDigest> { password } ) );
        }

        [Test]
        public void UpdatePasswordSavesToSource( )
        {
            // Set up
            PasswordDigest password = new PasswordDigestBuilder { Key = "def" };
            Serializer.Save( new List<PasswordDigest> { password }, InMemoryPasswordStore );


            PasswordDigest updatedPassword = new PasswordDigestBuilder { Key = "def", Hash = new byte[] { 0xf3, 0xdd } };
            // Exercise
            Database.AddOrUpdate( updatedPassword );
            // Verify
            Assert.That( Serializer.Load( InMemoryPasswordStore ), Is.EquivalentTo( new List<PasswordDigest> { updatedPassword } ) );
        }

        [Test]
        public void RemoveRemotelyUpdatedPasswordDeletesIt( ) // SHOULD IT ?
        {
            // Set up
            Database.AddOrUpdate( new PasswordDigestBuilder { Key = "def", Hash = new byte[] { 0x01 } } );

            PasswordDigest updatedPassword = new PasswordDigestBuilder { Key = "def", Hash = new byte[] { 0x03 } };
            Serializer.Save( new List<PasswordDigest> { updatedPassword }, InMemoryPasswordStore );

            // Exercise
            Database.Remove( "def" );
            // Verify

            Assert.That( Database.Passwords, Is.Empty );
            Assert.That( Serializer.Load( InMemoryPasswordStore ), Is.Empty );
        }
    }
}