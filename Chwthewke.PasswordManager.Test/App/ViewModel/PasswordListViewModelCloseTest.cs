using System;
using System.Linq;
using Autofac;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using Chwthewke.PasswordManager.Test.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel
{
    [ TestFixture ]
    public class PasswordListViewModelCloseTest
    {
        private PasswordDigestDocument _abc;
        private PasswordDigestDocument _def;
        private PasswordDigestDocument _ghi;

// ReSharper disable UnusedAutoPropertyAccessor.Global
        public PasswordListViewModel PasswordList { get; set; }

        public IPasswordManagerStorage Storage { get; set; }

        public IPasswordDerivationEngine Engine { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global

        public IPasswordRepository PasswordRepository
        {
            get { return Storage.PasswordRepository; }
        }

        [ SetUp ]
        public void SetUpContainer( )
        {
            TestInjection.TestContainer( ).InjectProperties( this );

            PasswordRepository.PasswordData = new InMemoryPasswordData( );

            var masterPasswordId = Guid.NewGuid( );
            _abc = new PasswordDigestDocumentBuilder
                       {
                           Digest = PasswordDigest( "abc", "123", 1, PasswordGenerators.LegacyFull ),
                           MasterPasswordId = masterPasswordId
                       };

            _def = new PasswordDigestDocumentBuilder
                       {
                           Digest = PasswordDigest( "def", "123", 2, PasswordGenerators.LegacyAlphaNumeric ),
                           MasterPasswordId = masterPasswordId
                       };

            _ghi = new PasswordDigestDocumentBuilder
                       {
                           Digest = PasswordDigest( "ghi", "123", 1, PasswordGenerators.LegacyAlphaNumeric ),
                           MasterPasswordId = masterPasswordId
                       };

            PasswordRepository.SavePassword( _abc );
            PasswordRepository.SavePassword( _def );
            PasswordRepository.SavePassword( _ghi );

            PasswordList.UpdateList( );
            PasswordList.OpenNewEditor( PasswordList.VisibleItems[ 0 ] );
            PasswordList.OpenNewEditor( PasswordList.VisibleItems[ 1 ] );
            PasswordList.OpenNewEditor( PasswordList.VisibleItems[ 2 ] );
        }

        private PasswordDigest PasswordDigest( string key, string masterPassword, int iteration, Guid passwordGenerator )
        {
            return Engine.Derive( new PasswordRequest( key, masterPassword.ToSecureString( ), iteration, passwordGenerator ) ).Digest;
        }

        [ Test ]
        public void CloseRequestForSelfClosesCallingEditor( )
        {
            // Set up
            var defEditor = PasswordList.Editors.First( ed => ed.Key == "def" );
            // Exercise
            PasswordList.EditorRequestedClose( defEditor, new CloseEditorEventArgs( CloseEditorEventType.Self ) );
            // Verify
            Assert.That( PasswordList.Editors, Has.Count.EqualTo( 2 ) );
            Assert.That( PasswordList.Editors.Select( e => e.Key ), Is.EquivalentTo( new[ ] { "abc", "ghi" } ) );
        }

        [ Test ]
        public void CloseRequestForAllClosesAllEditors( )
        {
            // Set up
            var defEditor = PasswordList.Editors.First( ed => ed.Key == "def" );
            // Exercise
            PasswordList.EditorRequestedClose( defEditor, new CloseEditorEventArgs( CloseEditorEventType.All ) );
            // Verify
            Assert.That( PasswordList.Editors, Has.Count.EqualTo( 1 ) );
            Assert.That( PasswordList.Editors.First( ).IsPristine, Is.True );
        }

        [ Test ]
        public void CloseRequestForAllOthersClosesAllEditorsButCaller( )
        {
            // Set up
            var defEditor = PasswordList.Editors.First( ed => ed.Key == "def" );
            // Exercise
            PasswordList.EditorRequestedClose( defEditor, new CloseEditorEventArgs( CloseEditorEventType.AllButSelf ) );
            // Verify
            Assert.That( PasswordList.Editors, Has.Count.EqualTo( 1 ) );
            Assert.That( PasswordList.Editors.First( ).Key, Is.EqualTo( "def" ) );
            Assert.That( PasswordList.Editors.First( ), Is.SameAs( defEditor ) );
        }

        [ Test ]
        public void CloseRequestForAllToTheRightClosesEditorsToTheRight( )
        {
            // Set up
            var defEditor = PasswordList.Editors.First( ed => ed.Key == "def" );
            // Exercise
            PasswordList.EditorRequestedClose( defEditor, new CloseEditorEventArgs( CloseEditorEventType.RightOfSelf ) );
            // Verify
            Assert.That( PasswordList.Editors, Has.Count.EqualTo( 2 ) );
            Assert.That( PasswordList.Editors.Select( e => e.Key ), Is.EquivalentTo( new[ ] { "abc", "def" } ) );
        }

        [ Test ]
        public void CloseRequestForAllToTheRightClosesNothingFromLastEditor( )
        {
            // Set up
            var ghiEditor = PasswordList.Editors.First( ed => ed.Key == "ghi" );
            // Exercise
            PasswordList.EditorRequestedClose( ghiEditor, new CloseEditorEventArgs( CloseEditorEventType.RightOfSelf ) );
            // Verify
            Assert.That( PasswordList.Editors, Has.Count.EqualTo( 3 ) );
        }

        [ Test ]
        public void CloseRequestForInsecureClosesEditorsWithMasterPassword( )
        {
            // Set up
            var defEditor = PasswordList.Editors.First( ed => ed.Key == "def" );
            defEditor.UpdateMasterPassword( "123".ToSecureString(  ) );

            var abcEditor = PasswordList.Editors.First( ed => ed.Key == "abc" );
            abcEditor.UpdateMasterPassword( "12".ToSecureString( ) );

            // Exercise
            PasswordList.EditorRequestedClose( defEditor, new CloseEditorEventArgs( CloseEditorEventType.Insecure ) );

            // Verify
            Assert.That( PasswordList.Editors, Has.Count.EqualTo( 2 ) );
            Assert.That( PasswordList.Editors.Select( e => e.Key ), Is.EquivalentTo( new[ ] { "abc", "ghi" } ) );
        } 


//        [ Test ]
//        public void ListHasPasswords( )
//        {
//            // Setup
//            PasswordRepository.SavePassword( new PasswordDigestDocumentBuilder { Key = "abc" } );
//            PasswordRepository.SavePassword( new PasswordDigestDocumentBuilder { Key = "abde" } );
//            PasswordRepository.SavePassword( new PasswordDigestDocumentBuilder { Key = "abcd" } );
//            // Exercise
//            PasswordList.UpdateList( );
//            // Verify
//            Assert.That( PasswordList.VisibleItems.Select( x => x.Name ).ToArray( ),
//                         Is.EqualTo( new[ ] { "abc", "abcd", "abde" } ) );
//        }
//
//        [ Test ]
//        public void ListInitiallyHasEmptyEditor( )
//        {
//            // Set up
//
//            // Exercise
//
//            // Verify
//            Assert.That( PasswordList.Editors.Count, Is.EqualTo( 1 ) );
//            Assert.That( PasswordList.Editors[ 0 ].Key, Is.EqualTo( string.Empty ) );
//            Assert.That( PasswordList.Editors[ 0 ].IsKeyReadonly, Is.False );
//        }
//
//        [ Test ]
//        public void AddEmptyEditorToList( )
//        {
//            // Setup
//            PasswordList.Editors[ 0 ].Key = "ab";
//            // Exercise
//            PasswordList.OpenEditorCommand.Execute( null );
//            // Verify
//            Assert.That( PasswordList.Editors.Count, Is.EqualTo( 2 ) );
//            Assert.That( PasswordList.Editors[ 1 ].Key, Is.EqualTo( string.Empty ) );
//            Assert.That( PasswordList.Editors[ 1 ].IsKeyReadonly, Is.False );
//        }
//
//        [ Test ]
//        public void AddEmptyEditorToListReusesInitialEditor( )
//        {
//            // Setup
//            // Exercise
//            PasswordList.OpenEditorCommand.Execute( null );
//            // Verify
//            Assert.That( PasswordList.Editors.Count, Is.EqualTo( 1 ) );
//            Assert.That( PasswordList.Editors[ 0 ].Key, Is.EqualTo( string.Empty ) );
//            Assert.That( PasswordList.Editors[ 0 ].IsKeyReadonly, Is.False );
//        }
//
//        [ Test ]
//        public void LoadPasswordIntoNewEditor( )
//        {
//            // Setup
//            PasswordList.Editors[ 0 ].Key = "ab";
//
//            PasswordRepository.SavePassword( new PasswordDigestDocumentBuilder { Key = "abc" } );
//
//            PasswordList.UpdateList( );
//            // Exercise
//            PasswordList.OpenNewEditor( PasswordList.VisibleItems[ 0 ] );
//            // Verify
//            Assert.That( PasswordList.Editors.Count, Is.EqualTo( 2 ) );
//            Assert.That( PasswordList.Editors[ 1 ].Key, Is.EqualTo( "abc" ) );
//            Assert.That( PasswordList.Editors[ 1 ].IsKeyReadonly, Is.True );
//        }
//
//        [ Test ]
//        public void LoadPasswordIntoNewEditorReusesPristineInitialEditor( )
//        {
//            // Setup
//            PasswordRepository.SavePassword( new PasswordDigestDocumentBuilder { Key = "abc" } );
//
//            PasswordList.UpdateList( );
//            // Exercise
//            PasswordList.OpenNewEditor( PasswordList.VisibleItems[ 0 ] );
//            // Verify
//            Assert.That( PasswordList.Editors.Count, Is.EqualTo( 1 ) );
//            Assert.That( PasswordList.Editors[ 0 ].Key, Is.EqualTo( "abc" ) );
//            Assert.That( PasswordList.Editors[ 0 ].IsKeyReadonly, Is.True );
//        }
//
//        [ Test ]
//        public void PasswordListIsUpdatedByEditorChange( )
//        {
//            // Setup
//            PasswordList.OpenEditorCommand.Execute( null );
//            var editor = PasswordList.Editors[ 0 ];
//            // Exercise
//            editor.Key = "abcd";
//            editor.UpdateMasterPassword( "1234".ToSecureString( ) );
//            editor.DerivedPasswords[ 0 ].IsSelected = true;
//            editor.SaveCommand.Execute( null );
//            // Verify
//            Assert.That( PasswordRepository.LoadPasswords( ).Count( ), Is.EqualTo( 1 ) );
//            Assert.That( PasswordList.VisibleItems.Select( it => it.Name ).ToArray( ), Is.EqualTo( new[ ] { "abcd" } ) );
//        }
//
//        [ Test ]
//        public void EditorIsClosedByItsRequest( )
//        {
//            // Setup
//            PasswordList.OpenEditorCommand.Execute( null );
//            // Exercise
//            PasswordList.Editors[ 0 ].CloseCommand.Execute( null );
//            // Verify
//            Assert.That( PasswordList.Editors.Count, Is.EqualTo( 1 ) );
//        }
//
//        [ Test ]
//        public void CloseLastEditorReplacesWithNew( )
//        {
//            // Set up
//            PasswordList.Editors[ 0 ].Key = "abcd";
//            // Exercise
//            PasswordList.Editors[ 0 ].CloseCommand.Execute( null );
//            // Verify
//            Assert.That( PasswordList.Editors.Count, Is.EqualTo( 1 ) );
//            Assert.That( PasswordList.Editors[ 0 ].Key, Is.EqualTo( string.Empty ) );
//            Assert.That( PasswordList.Editors[ 0 ].IsKeyReadonly, Is.False );
//        }
//
//        [ Test ]
//        public void PasswordListIsNoLongerUpdatedByClosedEditorChange( )
//        {
//            // Setup
//            PasswordList.OpenEditorCommand.Execute( null );
//            var editor = PasswordList.Editors[ 0 ];
//            // Exercise
//            editor.CloseCommand.Execute( null );
//            editor.Key = "abcd";
//            editor.UpdateMasterPassword( "1234".ToSecureString( ) );
//            editor.DerivedPasswords[ 0 ].IsSelected = true;
//            editor.SaveCommand.Execute( null );
//            // Verify
//            Assert.That( PasswordRepository.LoadPasswords( ).Count( ), Is.EqualTo( 1 ) );
//            Assert.That( PasswordList.VisibleItems.Select( it => it.Name ), Is.Empty );
//        }
    }
}