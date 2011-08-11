using System.Linq;
using Autofac;
using Chwthewke.PasswordManager.App.Modules;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Modules;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using Chwthewke.PasswordManager.Test.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel
{
    [ TestFixture ]
    public class PasswordListTest
    {
        [ SetUp ]
        public void SetUpContainer( )
        {
            ContainerBuilder containerBuilder = new ContainerBuilder( );
            containerBuilder.RegisterModule( new PasswordManagerModule( ) );
            containerBuilder.RegisterModule( new PasswordStorageModule( ) );
            containerBuilder.RegisterModule( new ApplicationServices( ) );
            _container = containerBuilder.Build( );

            _passwordList = new PasswordListViewModel( _container.Resolve<IPasswordRepository>( ),
                                                       _container.Resolve<IPasswordEditorFactory>( ),
                                                       _container.Resolve<IGuidToColorConverter>( ) );
        }

        [ Test ]
        public void ListHasPasswords( )
        {
            // Setup
            _container.AddPassword( "abc", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            _container.AddPassword( "abde", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            _container.AddPassword( "abcd", string.Empty, PasswordGenerators.Full, "1234".ToSecureString( ) );
            // Exercise
            _passwordList = new PasswordListViewModel( _container.Resolve<IPasswordRepository>( ),
                                                       _container.Resolve<IPasswordEditorFactory>( ),
                                                       _container.Resolve<IGuidToColorConverter>( ) );
            // Verify
            Assert.That( _passwordList.Items.Select( x => x.Name ).ToArray( ),
                         Is.EqualTo( new[ ] { "abc", "abcd", "abde" } ) );
        }

        [ Test ]
        public void AddEmptyEditorToList( )
        {
            // Setup
            // Exercise
            _passwordList.OpenEditorCommand.Execute( null );
            // Verify
            Assert.That( _passwordList.Editors.Count, Is.EqualTo( 1 ) );
            Assert.That( _passwordList.Editors[ 0 ].Key, Is.EqualTo( string.Empty ) );
            Assert.That( _passwordList.Editors[ 0 ].IsKeyReadonly, Is.False );
        }

        [ Test ]
        public void LoadPasswordIntoNewEditor( )
        {
            // Setup
            _container.AddPassword( "abc", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            _passwordList.UpdateList( );
            // Exercise
            _passwordList.OpenNewEditor( _passwordList.Items[ 0 ] );
            // Verify
            Assert.That( _passwordList.Editors.Count, Is.EqualTo( 1 ) );
            Assert.That( _passwordList.Editors[ 0 ].Key, Is.EqualTo( "abc" ) );
            Assert.That( _passwordList.Editors[ 0 ].IsKeyReadonly, Is.True );
        }

        [ Test ]
        public void PasswordListIsUpdatedByEditorChange( )
        {
            // Setup
            _passwordList.OpenEditorCommand.Execute( null );
            var editor = _passwordList.Editors[ 0 ];
            // Exercise
            editor.Key = "abcd";
            editor.UpdateMasterPassword( "1234".ToSecureString( ) );
            editor.Slots[ 0 ].IsSelected = true;
            editor.SaveCommand.Execute( null );
            // Verify
            var store = _container.Resolve<IPasswordRepository>( );
            Assert.That( store.Passwords.Count( ), Is.EqualTo( 1 ) );
            Assert.That( _passwordList.Items.Select( it => it.Name ).ToArray( ), Is.EqualTo( new[ ] { "abcd" } ) );
        }

        [ Test ]
        public void EditorIsClosedByItsRequest( )
        {
            // Setup
            _passwordList.OpenEditorCommand.Execute( null );
            // Exercise
            _passwordList.Editors[ 0 ].CloseCommand.Execute( null );
            // Verify
            Assert.That( _passwordList.Editors, Is.Empty );
        }

        [ Test ]
        public void PasswordListIsNoLongerUpdatedByClosedEditorChange( )
        {
            // Setup
            _passwordList.OpenEditorCommand.Execute( null );
            var editor = _passwordList.Editors[ 0 ];
            // Exercise
            editor.CloseCommand.Execute( null );
            editor.Key = "abcd";
            editor.UpdateMasterPassword( "1234".ToSecureString( ) );
            editor.Slots[ 0 ].IsSelected = true;
            editor.SaveCommand.Execute( null );
            // Verify
            var store = _container.Resolve<IPasswordRepository>( );
            Assert.That( store.Passwords.Count( ), Is.EqualTo( 1 ) );
            Assert.That( _passwordList.Items.Select( it => it.Name ), Is.Empty );
        }

        private IContainer _container;
        private PasswordListViewModel _passwordList;
    }
}