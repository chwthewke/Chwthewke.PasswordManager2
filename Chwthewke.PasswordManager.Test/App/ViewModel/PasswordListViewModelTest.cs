using System.Linq;
using Autofac;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.App.Services;
using Chwthewke.PasswordManager.Test.Engine;
using Chwthewke.PasswordManager.Test.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel
{
    [ TestFixture ]
    public class PasswordListViewModelTest
    {
// ReSharper disable UnusedAutoPropertyAccessor.Global
        public PasswordListViewModel PasswordList { get; set; }

        public IPasswordManagerStorage Storage { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global

        public IPasswordRepository PasswordRepository
        {
            get { return Storage.PasswordRepository; }
        }

        [ SetUp ]
        public void SetUpContainer( )
        {
            TestInjection.TestContainer( ImmediateScheduler.Module ).InjectProperties( this );

            PasswordRepository.PasswordData = new InMemoryPasswordData( );
        }

        [ Test ]
        public void ListHasPasswords( )
        {
            // Setup
            PasswordRepository.SavePassword( new PasswordDigestDocumentBuilder { Key = "abc" } );
            PasswordRepository.SavePassword( new PasswordDigestDocumentBuilder { Key = "abde" } );
            PasswordRepository.SavePassword( new PasswordDigestDocumentBuilder { Key = "abcd" } );
            // Exercise
            PasswordList.UpdateList( );
            // Verify
            Assert.That( PasswordList.VisibleItems.Select( x => x.Name ).ToArray( ),
                         Is.EqualTo( new[ ] { "abc", "abcd", "abde" } ) );
        }

        [ Test ]
        public void ListInitiallyHasEmptyEditor( )
        {
            // Set up

            // Exercise

            // Verify
            Assert.That( PasswordList.Editors.Count, Is.EqualTo( 1 ) );
            Assert.That( PasswordList.Editors[ 0 ].Key, Is.EqualTo( string.Empty ) );
            Assert.That( PasswordList.Editors[ 0 ].IsKeyReadonly, Is.False );
        }

        [ Test ]
        public void AddEmptyEditorToList( )
        {
            // Setup
            PasswordList.Editors[ 0 ].Key = "ab";
            // Exercise
            PasswordList.OpenEditorCommand.Execute( null );
            // Verify
            Assert.That( PasswordList.Editors.Count, Is.EqualTo( 2 ) );
            Assert.That( PasswordList.Editors[ 1 ].Key, Is.EqualTo( string.Empty ) );
            Assert.That( PasswordList.Editors[ 1 ].IsKeyReadonly, Is.False );
        }

        [ Test ]
        public void AddEmptyEditorToListReusesInitialEditor( )
        {
            // Setup
            // Exercise
            PasswordList.OpenEditorCommand.Execute( null );
            // Verify
            Assert.That( PasswordList.Editors.Count, Is.EqualTo( 1 ) );
            Assert.That( PasswordList.Editors[ 0 ].Key, Is.EqualTo( string.Empty ) );
            Assert.That( PasswordList.Editors[ 0 ].IsKeyReadonly, Is.False );
        }

        [ Test ]
        public void LoadPasswordIntoNewEditor( )
        {
            // Setup
            PasswordList.Editors[ 0 ].Key = "ab";

            PasswordRepository.SavePassword( new PasswordDigestDocumentBuilder { Key = "abc" } );

            PasswordList.UpdateList( );
            // Exercise
            PasswordList.OpenNewEditor( PasswordList.VisibleItems[ 0 ] );
            // Verify
            Assert.That( PasswordList.Editors.Count, Is.EqualTo( 2 ) );
            Assert.That( PasswordList.Editors[ 1 ].Key, Is.EqualTo( "abc" ) );
            Assert.That( PasswordList.Editors[ 1 ].IsKeyReadonly, Is.True );
        }

        [ Test ]
        public void LoadPasswordIntoNewEditorReusesPristineInitialEditor( )
        {
            // Setup
            PasswordRepository.SavePassword( new PasswordDigestDocumentBuilder { Key = "abc" } );

            PasswordList.UpdateList( );
            // Exercise
            PasswordList.OpenNewEditor( PasswordList.VisibleItems[ 0 ] );
            // Verify
            Assert.That( PasswordList.Editors.Count, Is.EqualTo( 1 ) );
            Assert.That( PasswordList.Editors[ 0 ].Key, Is.EqualTo( "abc" ) );
            Assert.That( PasswordList.Editors[ 0 ].IsKeyReadonly, Is.True );
        }

        [ Test ]
        public void PasswordListIsUpdatedByEditorChange( )
        {
            // Setup
            PasswordList.OpenEditorCommand.Execute( null );
            var editor = PasswordList.Editors[ 0 ];
            // Exercise
            editor.Key = "abcd";
            editor.UpdateMasterPassword( "1234".ToSecureString( ) );
            editor.DerivedPasswords[ 0 ].IsSelected = true;
            editor.SaveCommand.Execute( null );
            // Verify
            Assert.That( PasswordRepository.LoadPasswords( ).Count( ), Is.EqualTo( 1 ) );
            Assert.That( PasswordList.VisibleItems.Select( it => it.Name ).ToArray( ), Is.EqualTo( new[ ] { "abcd" } ) );
        }

        [ Test ]
        public void PasswordListIsNoLongerUpdatedByClosedEditorChange( )
        {
            // Setup
            PasswordList.OpenEditorCommand.Execute( null );
            var editor = PasswordList.Editors[ 0 ];
            // Exercise
            editor.CloseSelfCommand.Execute( null );
            editor.Key = "abcd";
            editor.UpdateMasterPassword( "1234".ToSecureString( ) );
            editor.DerivedPasswords[ 0 ].IsSelected = true;
            editor.SaveCommand.Execute( null );
            // Verify
            Assert.That( PasswordRepository.LoadPasswords( ).Count( ), Is.EqualTo( 1 ) );
            Assert.That( PasswordList.VisibleItems.Select( it => it.Name ), Is.Empty );
        }
    }
}