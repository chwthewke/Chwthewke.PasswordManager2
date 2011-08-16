using System.Linq;
using System.Security;
using Autofac;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
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

        public IPasswordEditorControllerFactory ControllerFactory { get; set; }


        [SetUp]
        public void SetUpContainer( )
        {
            IContainer container = AppSetUp.TestContainer( );
            container.InjectProperties( this );

            PasswordDatabase.Source = new InMemoryPasswordStore( );

/*
            ContainerBuilder containerBuilder = new ContainerBuilder( );
            containerBuilder.RegisterModule( new PasswordManagerModule( ) );
            containerBuilder.RegisterModule( new PasswordStorageModule( ) );
            containerBuilder.RegisterModule( new ApplicationServices( ) );
            _container = containerBuilder.Build( );
*/

/*
            PasswordList = new PasswordListViewModel( Resolve<IPasswordRepository>( ),
                                                      Resolve<IPasswordEditorFactory>( ),
                                                      Resolve<IGuidToColorConverter>( ) );
*/
        }

        [Test]
        public void ListHasPasswords( )
        {
            // Setup
            AddPassword( "abc", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            AddPassword( "abde", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            AddPassword( "abcd", string.Empty, PasswordGenerators.Full, "1234".ToSecureString( ) );
            // Exercise
            PasswordList.UpdateList( );
            // Verify
            Assert.That( PasswordList.Items.Select( x => x.Name ).ToArray( ),
                         Is.EqualTo( new[] {"abc", "abcd", "abde"} ) );
        }

        private void AddPassword( string key,
                                  string note,
                                  IPasswordGenerator generator,
                                  SecureString masterPassword )
        {
            IPasswordEditorController controller =
                ControllerFactory.CreatePasswordEditorController( );
            controller.Key = key;
            controller.Note = note;
            controller.SelectedGenerator = generator;
            controller.MasterPassword = masterPassword;

            controller.SavePassword( );
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
            AddPassword( "abc", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
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
            Assert.That( PasswordList.Items.Select( it => it.Name ).ToArray( ), Is.EqualTo( new[] {"abcd"} ) );
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