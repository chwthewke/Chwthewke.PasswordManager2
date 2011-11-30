using System;
using System.Security;
using Autofac;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.App.Services;
using Chwthewke.PasswordManager.Test.Storage;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor
{
    public abstract class PasswordEditorTestBase
    {
// ReSharper disable UnusedAutoPropertyAccessor.Global
        public IGuidToColorConverter GuidToColorConverter { get; set; }
        public Mock<IClipboardService> ClipboardServiceMock { get; set; }

        public PasswordEditorViewModelFactory ViewModelFactory { get; set; }
        public PasswordEditorViewModel ViewModel;

        public IPasswordDerivationEngine Engine { get; set; }
        public IPasswordManagerStorage Storage { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global

        protected IPasswordRepository PasswordRepository
        {
            get { return Storage.PasswordRepository; }
        }


        [ SetUp ]
        public void SetUpPasswordEditorViewModel( )
        {
            TestInjection
                .TestContainer( TestInjection.Mock<IClipboardService>( ),
                                ImmediateScheduler.Module )
                .InjectProperties( this );

            PasswordRepository.PasswordData = new InMemoryPasswordData( );

            ViewModel = ViewModelFactory.NewPasswordEditor( );
        }

        protected PasswordDigestDocument AddPassword( string key, Guid generator, int iteration, SecureString masterPassword, string note )
        {
            PasswordRepository.SavePassword(
                new PasswordDigestDocumentBuilder
                    {
                        Digest = Engine.Derive( new PasswordRequest( key, masterPassword, iteration, generator ) ).Digest,
                        Note = note
                    } );
            return PasswordRepository.LoadPassword( key );
        }

        protected string DerivedPassword( Guid generator, string key, SecureString masterPassword, int iterations )
        {
            return Engine.Derive( new PasswordRequest( key, masterPassword, iterations, generator ) ).Password;
        }
    }
}