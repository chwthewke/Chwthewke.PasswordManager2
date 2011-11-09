using System.IO;
using Autofac;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Util;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel
{
    [ TestFixture ]
    internal class PasswordManagerViewModelTest
    {
        private PasswordManagerViewModel _viewModel;
        private IPasswordDatabase _database;
        private readonly Mock<IFileSelectionService> _fileSelectionServiceMock = new Mock<IFileSelectionService>( );

        [ SetUp ]
        public void SetUpViewModel( )
        {
            IContainer container = AppSetUp.TestContainer(
                new MockModule<IFileSelectionService>( _fileSelectionServiceMock )
                );
            _viewModel = container.Resolve<PasswordManagerViewModel>( );
            _database = container.Resolve<IPasswordDatabase>( );
        }

        [ Test ]
        public void TestSetInternalStorage( )
        {
            // Set up
            // Exercise
            _viewModel.SelectInternalStorageCommand.Execute( null );
            // Verify
            Assert.That( _database.Source, Is.InstanceOf<SettingsTextResource>( ) );
            Assert.That( _viewModel.ExternalStorageSelected, Is.False );
            Assert.That( _viewModel.InternalStorageSelected, Is.True );
        }

        [ Test ]
        public void TestSetExternalStorage( )
        {
            // Set up
            _fileSelectionServiceMock
                .Setup( fss => fss.SelectExternalPasswordFile( It.IsAny<DirectoryInfo>( ) ) )
                .Returns( new FileInfo( "Dummy File" ) );
            // Exercise
            _viewModel.SelectExternalStorageCommand.Execute( null );
            // Verify
            Assert.That( _database.Source, Is.InstanceOf<FileTextResource>( ) );
            Assert.That( _viewModel.ExternalStorageSelected, Is.True );
            Assert.That( _viewModel.InternalStorageSelected, Is.False );
        }
    }
}