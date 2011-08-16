using System;
using System.Linq;
using Autofac;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using Chwthewke.PasswordManager.Test.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel
{
    [TestFixture]
    [Ignore( "Change dependency in PasswordEditorController." )]
    public class PasswordListTest
    {
        public PasswordListViewModel PasswordList { get; set; }

        public IPasswordDatabase PasswordDatabase { get; set; }

        public Func<IPasswordEditorController> ControllerFactory { get; set; }


        [SetUp]
        public void SetUpContainer( )
        {
            AppSetUp.TestContainer( b => b.RegisterType<InMemoryPasswordStore>( ).As<IPasswordStore>( ) ).InjectProperties( this );

            //PasswordDatabase.Source = new InMemoryPasswordStore( );
        }

        [Test]
        public void ListHasPasswords( )
        {
            // Setup
            PasswordDatabase.AddOrUpdate( new PasswordDigestBuilder { Key = "abc" } );
            PasswordDatabase.AddOrUpdate( new PasswordDigestBuilder { Key = "abde" } );
            PasswordDatabase.AddOrUpdate( new PasswordDigestBuilder { Key = "abcd" } );
            // Exercise
            PasswordList.UpdateList( );
            // Verify
            Assert.That( PasswordList.Items.Select( x => x.Name ).ToArray( ),
                         Is.EqualTo( new[] { "abc", "abcd", "abde" } ) );
        }

        [Test]
        public void AddEmptyEditorToList( )
        {
            // Setup
            // Exercise
            PasswordList.OpenEditorCommand.Execute( null );
            // Verify
            Assert.That( PasswordList.Editors.Count, Is.EqualTo( 1 ) );
            Assert.That( PasswordList.Editors[ 0 ].Key, Is.EqualTo( string.Empty ) );
            Assert.That( PasswordList.Editors[ 0 ].IsKeyReadonly, Is.False );
        }

        [Test]
        public void LoadPasswordIntoNewEditor( )
        {
            // Setup
            PasswordDatabase.AddOrUpdate( new PasswordDigestBuilder { Key = "abc" } );
            PasswordList.UpdateList( );
            // Exercise
            PasswordList.OpenNewEditor( PasswordList.Items[ 0 ] );
            // Verify
            Assert.That( PasswordList.Editors.Count, Is.EqualTo( 1 ) );
            Assert.That( PasswordList.Editors[ 0 ].Key, Is.EqualTo( "abc" ) );
            Assert.That( PasswordList.Editors[ 0 ].IsKeyReadonly, Is.True );
        }

        [Test]
        public void PasswordListIsUpdatedByEditorChange( )
        {
            // Setup
            PasswordList.OpenEditorCommand.Execute( null );
            var editor = PasswordList.Editors[ 0 ];
            // Exercise
            editor.Key = "abcd";
            editor.UpdateMasterPassword( "1234".ToSecureString( ) );
            editor.Slots[ 0 ].IsSelected = true;
            editor.SaveCommand.Execute( null );
            // Verify
            Assert.That( PasswordDatabase.Passwords.Count( ), Is.EqualTo( 1 ) );
            Assert.That( PasswordList.Items.Select( it => it.Name ).ToArray( ), Is.EqualTo( new[] { "abcd" } ) );
        }

        [Test]
        public void EditorIsClosedByItsRequest( )
        {
            // Setup
            PasswordList.OpenEditorCommand.Execute( null );
            // Exercise
            PasswordList.Editors[ 0 ].CloseCommand.Execute( null );
            // Verify
            Assert.That( PasswordList.Editors, Is.Empty );
        }

        [Test]
        public void PasswordListIsNoLongerUpdatedByClosedEditorChange( )
        {
            // Setup
            PasswordList.OpenEditorCommand.Execute( null );
            var editor = PasswordList.Editors[ 0 ];
            // Exercise
            editor.CloseCommand.Execute( null );
            editor.Key = "abcd";
            editor.UpdateMasterPassword( "1234".ToSecureString( ) );
            editor.Slots[ 0 ].IsSelected = true;
            editor.SaveCommand.Execute( null );
            // Verify
            Assert.That( PasswordDatabase.Passwords.Count( ), Is.EqualTo( 1 ) );
            Assert.That( PasswordList.Items.Select( it => it.Name ), Is.Empty );
        }
    }
}