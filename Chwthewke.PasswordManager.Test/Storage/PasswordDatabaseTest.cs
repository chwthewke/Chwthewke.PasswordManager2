﻿using System;
using System.Collections.Generic;
using Autofac;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.App;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Storage
{
    [TestFixture]
    public class PasswordDatabaseTest
    {
// ReSharper disable UnusedAutoPropertyAccessor.Global
        public IPasswordStore InMemoryPasswordStore { get; set; }

        public IPasswordDatabase Database { get; set; }

        public IPasswordSerializer Serializer { get; set; }

        public Func<IPasswordDatabase> DatabaseFactory { get; set; }
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

            Database.Source = InMemoryPasswordStore;
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
            IPasswordStore newInMemoryPasswordStore = new InMemoryPasswordStore( );
            Serializer.Save( passwordDigests, newInMemoryPasswordStore );

            // Exercise

            IPasswordDatabase database = DatabaseFactory.Invoke( );
            database.Source = newInMemoryPasswordStore;

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
        public void RemoveRemotelyUpdatedPasswordDoesNotDeleteIt( ) // SHOULD IT ?
        {
            // Set up
            Database.AddOrUpdate( new PasswordDigestBuilder { Key = "def", Hash = new byte[] { 0x01 }, ModificationTime = new DateTime( 1 ) } );

            PasswordDigest updatedPassword = new PasswordDigestBuilder
                                                 { Key = "def", Hash = new byte[] { 0x03 }, ModificationTime = new DateTime( 4 ) };
            Serializer.Save( new List<PasswordDigest> { updatedPassword }, InMemoryPasswordStore );

            // Exercise
            Database.Remove( "def" );
            // Verify

            Assert.That( Database.Passwords, Is.EquivalentTo( new List<PasswordDigest> { updatedPassword } ) );
            Assert.That( Serializer.Load( InMemoryPasswordStore ), Is.EquivalentTo( new List<PasswordDigest> { updatedPassword } ) );
        }

        [Test]
        public void ReloadMergesRecentModificationsFromSource( )
        {
            // Set up
            PasswordDigest originalPassword = new PasswordDigestBuilder { Key = "abc", ModificationTime = new DateTime( 1 ) };
            Database.AddOrUpdate( originalPassword );

            PasswordDigest updatedPassword = new PasswordDigestBuilder { Key = "abc", ModificationTime = new DateTime( 3 ) };

            Serializer.Save( new List<PasswordDigest> { updatedPassword }, InMemoryPasswordStore );

            // Exercise
            Database.Reload( );

            // Verify
            Assert.That( Database.Passwords, Is.EquivalentTo( new List<PasswordDigest> { updatedPassword } ) );
        }

        [Test]
        public void ReloadRejectsObsoleteModificationsFromSource( )
        {
            // Set up
            PasswordDigest originalPassword = new PasswordDigestBuilder { Key = "abc", ModificationTime = new DateTime( 3 ) };
            Database.AddOrUpdate( originalPassword );

            PasswordDigest updatedPassword = new PasswordDigestBuilder { Key = "abc", ModificationTime = new DateTime( 1 ) };

            Serializer.Save( new List<PasswordDigest> { updatedPassword }, InMemoryPasswordStore );

            // Exercise
            Database.Reload( );

            // Verify
            Assert.That( Database.Passwords, Is.EquivalentTo( new List<PasswordDigest> { originalPassword } ) );
        }

    }
}