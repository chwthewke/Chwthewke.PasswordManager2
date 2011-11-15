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
        public PasswordManagerViewModel2 ViewModel { get; set; }

        public Mock<IFileSelectionService> FileSelectionServiceMock { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global
// ReSharper restore MemberCanBePrivate.Global

        [ SetUp ]
        public void SetUpViewModel( )
        {
            TestInjection.TestContainer( TestInjection.Mock<IFileSelectionService>( ) ).InjectProperties( this );
        }

        [ Test ]
        public void TestSetInternalStorage( )
        {
            // Set up
            // Exercise
            ViewModel.SelectInternalStorageCommand.Execute( null );
            // Verify
            Assert.That( ViewModel.ExternalStorageSelected, Is.False );
            Assert.That( ViewModel.InternalStorageSelected, Is.True );
        }

        [ Test ]
        public void TestSetExternalStorage( )
        {
            // Set up
            FileSelectionServiceMock
                .Setup( fss => fss.SelectExternalPasswordFile( It.IsAny<DirectoryInfo>( ) ) )
                .Returns( new FileInfo( "Dummy File" ) );
            // Exercise
            ViewModel.SelectExternalStorageCommand.Execute( null );
            // Verify
            Assert.That( ViewModel.ExternalStorageSelected, Is.True );
            Assert.That( ViewModel.InternalStorageSelected, Is.False );
        }

        // TODO need to do more to test password storage. Probably with real "test" files.
    }
}