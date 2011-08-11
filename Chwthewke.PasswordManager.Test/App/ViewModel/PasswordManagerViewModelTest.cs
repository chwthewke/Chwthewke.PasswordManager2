using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Storage;
using NUnit.Framework;
using Autofac;

namespace Chwthewke.PasswordManager.Test.App.ViewModel
{
    [TestFixture]
    internal class PasswordManagerViewModelTest
    {
        private PasswordManagerViewModel _viewModel;
        private IPasswordDatabase _database;

        [SetUp]
        public void SetUpViewModel( )
        {
            IContainer container = AppSetUp.Container;
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
    }
}