using System.Linq;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;
using Autofac;
using Chwthewke.PasswordManager.Test.Engine;

namespace Chwthewke.PasswordManager.Test.App.ViewModel
{
    [ TestFixture ]
    public class DerivedPasswordViewModelTest
    {
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
        public IPasswordManagerEditor Editor { get; set; }

        public DerivedPasswordViewModel.Factory Factory { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global
// ReSharper restore MemberCanBePrivate.Global

        [ SetUp ]
        public void SetUp( )
        {
            TestInjection.TestContainer( ).InjectProperties( this );

            _editorModel = Editor.EmptyModel( );
            _derivedPasswordModel = _editorModel.DerivedPasswords.First( dp => dp.Generator == PasswordGenerators.LegacyFull );

            _viewModel = Factory( _derivedPasswordModel, _editorModel );
        }

        [ Test ]
        public void ContentPropertyChanged( )
        {
            // Setup
            bool propertyChanged = false;
            _viewModel.PropertyChanged += ( s, e ) => { propertyChanged |= e.PropertyName == "Content"; };
            // Exercise
            _viewModel.Content = "abc";
            // Verify
            Assert.That( propertyChanged );
        }

        [ Test ]
        public void IsSelectedPropertyChangedToTrue( )
        {
            // Setup
            bool propertyChanged = false;
            _viewModel.PropertyChanged += ( s, e ) => { propertyChanged |= e.PropertyName == "IsSelected"; };
            // Exercise
            _viewModel.IsSelected = true;
            // Verify
            Assert.That( propertyChanged );
        }

        [ Test ]
        public void IsSelectedPropertyChangedToFalse( )
        {
            // Setup
            _viewModel.IsSelected = true;
            bool propertyChanged = false;
            _viewModel.PropertyChanged += ( s, e ) => { propertyChanged |= e.PropertyName == "IsSelected"; };
            // Exercise
            _viewModel.IsSelected = false;
            // Verify
            Assert.That( propertyChanged );
        }

        [ Test ]
        public void GeneratorDescriptionIsReadFromResources( )
        {
            // Set up

            // Exercise

            // Verify
            Assert.That( _viewModel.GeneratorDescription,
                         Is.EqualTo( Resources.PasswordGeneratorDescriptionccf1451c4b3045a499b0d54ec3c3a7ee ) );
        }

        [ Test ]
        public void GeneratorNameIsReadFromResources( )
        {
            // Set up

            // Exercise

            // Verify
            Assert.That( _viewModel.Name,
                         Is.EqualTo( Resources.PasswordGeneratorccf1451c4b3045a499b0d54ec3c3a7ee ) );
        }

        [ Test ]
        public void SelectAndRefreshMarksViewModelSelected( )
        {
            // Set up
            bool propertyChanged = false;
            _viewModel.PropertyChanged += ( s, e ) => { propertyChanged |= e.PropertyName == "IsSelected"; };

            _editorModel.SelectedPassword = _derivedPasswordModel;

            // Exercise
            _viewModel.Refresh( );
            // Verify
            Assert.That( propertyChanged );
            Assert.That( _viewModel.IsSelected, Is.True );
        }

        [ Test ]
        public void UnselectAndUpdateMarksViewModelNotSelected( )
        {
            // Set up
            bool propertyChanged = false;
            _viewModel.PropertyChanged += ( s, e ) => { propertyChanged |= e.PropertyName == "IsSelected"; };

            _editorModel.SelectedPassword = _editorModel.DerivedPasswords.First( p => p != _derivedPasswordModel );

            // Exercise
            _viewModel.Update( );
            // Verify
            Assert.That( propertyChanged, Is.False );
            Assert.That( _viewModel.IsSelected, Is.False );
        }

        [ Test ]
        public void SetKeyAndMasterPasswordAndUpdateSetsContent( )
        {
            // Set up
            bool propertyChanged = false;
            _viewModel.PropertyChanged += ( s, e ) => { propertyChanged |= e.PropertyName == "Content"; };

            _editorModel.Key = "abc";
            _editorModel.MasterPassword = "123".ToSecureString( );
            _editorModel.UpdateDerivedPasswords( );
            // Exercise
            _viewModel.Update( );
            // Verify
            Assert.That( propertyChanged, Is.True );
            Assert.That( _viewModel.Content, Is.EqualTo( "[0dY\\,&<P{" ) );
        }

        [ Test ]
        public void UnSetKeyAndMasterPasswordAndUpdateSetsContentEmpty( )
        {
            // Set up
            _editorModel.Key = "abc";
            _editorModel.MasterPassword = "123".ToSecureString( );
            _editorModel.UpdateDerivedPasswords( );
            _viewModel.Update( );

            bool propertyChanged = false;
            _viewModel.PropertyChanged += ( s, e ) => { propertyChanged |= e.PropertyName == "Content"; };

            _editorModel.MasterPassword = "".ToSecureString( );
            _editorModel.UpdateDerivedPasswords( );
            // Exercise
            _viewModel.Update( );
            // Verify
            Assert.That( propertyChanged, Is.True );
            Assert.That( _viewModel.Content, Is.EqualTo( string.Empty ) );
        }

        private DerivedPasswordViewModel _viewModel;
        private IPasswordEditorModel _editorModel;
        private IDerivedPasswordModel _derivedPasswordModel;
    }
}