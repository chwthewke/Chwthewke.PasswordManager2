using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Util;
using Moq;
using NUnit.Framework;
using Autofac;

namespace Chwthewke.PasswordManager.Test.App.ViewModel
{
    [TestFixture]
    internal class PasswordManagerViewModelTest
    {
        private PasswordManagerViewModel _viewModel;
        private IPasswordDatabase _database;
        private readonly Mock<IFileSelectionService> _fileSelectionServiceMock = new Mock<IFileSelectionService>( );

        [SetUp]
        public void SetUpViewModel( )
        {
            IContainer container = AppSetUp.TestContainer(
                new MockModule<IFileSelectionService>( _fileSelectionServiceMock )
                );
            _viewModel = container.Resolve<PasswordManagerViewModel>( );
            _database = container.Resolve<IPasswordDatabase>( );
        }

        [Test]
        public void TestSetInternalStorage( )
        {
            // Set up
            // Exercise
            _viewModel.SelectInternalStorageCommand.Execute( null );
            // Verify
            Assert.That( _database.Source, Is.InstanceOf<InternalPasswordStore>( ) );
        }

        [Test]
        public void TestSetExternalStorage( )
        {
            // Set up
            _fileSelectionServiceMock
                .Setup( fss => fss.SelectExternalPasswordFile( It.IsAny<DirectoryInfo>( ) ) )
                .Returns( new FileInfo( "Dummy File" ) );
            // Exercise
            _viewModel.SelectExternalStorageCommand.Execute( null );
            // Verify
            Assert.That( _database.Source, Is.InstanceOf<ExternalPasswordStore>( ) );
        }
    }
}