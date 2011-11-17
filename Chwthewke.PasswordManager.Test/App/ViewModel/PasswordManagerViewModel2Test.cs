using System.IO;
using Autofac;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Storage;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel
{
    [ TestFixture ]
    public class PasswordManagerViewModel2Test
    {
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
        public PasswordManagerViewModel ViewModel { get; set; }

        public Mock<IFileSelectionService> FileSelectionServiceMock { get; set; }

        public Mock<IStorageConfiguration> StorageConfigurationMock { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global
// ReSharper restore MemberCanBePrivate.Global

        [ SetUp ]
        public void SetUpViewModel( )
        {
            TestInjection
                .TestContainer( TestInjection.Mock<IFileSelectionService>( ),
                                TestInjection.Mock<IStorageConfiguration>( ) )
                .InjectProperties( this );
        }

        [ Test ]
        public void SetInternalStorageDelegatesToStorageConfiguration( )
        {
            // Set up
            StorageConfigurationMock.Setup( sc => sc.StorageType ).Returns( StorageType.Internal );
            // Exercise
            ViewModel.SelectInternalStorageCommand.Execute( null );
            // Verify
            StorageConfigurationMock.Verify( sc => sc.SelectInternalStorage( ) );
            Assert.That( ViewModel.InternalStorageSelected, Is.True );
            Assert.That( ViewModel.ExternalStorageSelected, Is.False );
        }

        [ Test ]
        public void SetExternalStorageDelegatesToStorageConfiguration( )
        {
            // Set up
            FileInfo fileInfo = new FileInfo( "Dummy File" );
            FileSelectionServiceMock
                .Setup( fss => fss.SelectExternalPasswordFile( It.IsAny<DirectoryInfo>( ) ) )
                .Returns( fileInfo );
            StorageConfigurationMock.Setup( sc => sc.StorageType ).Returns( StorageType.External );
            // Exercise
            ViewModel.SelectExternalStorageCommand.Execute( null );
            // Verify
            StorageConfigurationMock.Verify( sc => sc.SelectExternalStorage( fileInfo ) );
            Assert.That( ViewModel.ExternalStorageSelected, Is.True );
            Assert.That( ViewModel.InternalStorageSelected, Is.False );
        }

        // TODO need to do more to test password storage. Probably with real "test" files.
    }
}