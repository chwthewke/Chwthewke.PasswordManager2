using System;
using Autofac.Core;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Editor;
using Chwthewke.PasswordManager.Test.Storage;
using NUnit.Framework;
using Autofac;
using ITimeProvider = Chwthewke.PasswordManager.Editor.ITimeProvider;

namespace Chwthewke.PasswordManager.Test.App.ViewModel
{
    [ TestFixture ]
    public class PasswordListEntryViewModelTest
    {
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
        public IGuidToColorConverter GuidToColor { get; set; }

        public IFuzzyDateFormatter FuzzyDateFormatter { get; set; }

        public StubTimeProvider TimeProvider { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global
// ReSharper restore MemberCanBePrivate.Global

        [ SetUp ]
        public void SetUpDependencies( )
        {
            IModule stubTimeProvider = TestInjection.ModuleOf( b =>
                                                                   {
                                                                       b.RegisterType<StubTimeProvider>( ).SingleInstance( );
                                                                       b.RegisterType<StubTimeProvider>( ).As<ITimeProvider>( );
                                                                   } );
            TestInjection.TestContainer( stubTimeProvider ).InjectProperties( this );
        }

        [ Test ]
        public void EntryPresentsTextFromDocument( )
        {
            // Set up

            PasswordDigestDocument password = new PasswordDigestDocumentBuilder
                                                  {
                                                      Key = "ABCD",
                                                      Iteration = 2,
                                                      PasswordGenerator = PasswordGenerators.Full,
                                                      CreatedOn = new DateTime( 2011, 11, 11 ),
                                                      ModifiedOn = new DateTime( 2011, 11, 14 ),
                                                      Note = "Need this for EFGH"
                                                  };


            // Exercise
            var viewModel = new PasswordListEntryViewModel( password, GuidToColor, FuzzyDateFormatter );

            // Verify

            Assert.That( viewModel.Name, Is.EqualTo( "ABCD" ) );
            Assert.That( viewModel.Note, Is.EqualTo( "Need this for EFGH" ) );
        }

        [ Test ]
        public void ViewModelPresentsDatesFormattedByFuzzyDateFormatter( )
        {
            // Set up
            TimeProvider.Now = new DateTime( 2011, 11, 14, 8, 0, 0 );

            PasswordDigestDocument password = new PasswordDigestDocumentBuilder
                                                  {
                                                      Key = "ABCD",
                                                      Iteration = 2,
                                                      PasswordGenerator = PasswordGenerators.Full,
                                                      CreatedOn = new DateTime( 2011, 11, 11 ),
                                                      ModifiedOn = new DateTime( 2011, 11, 14 ),
                                                      Note = "Need this for EFGH"
                                                  };


            // Exercise
            var viewModel = new PasswordListEntryViewModel( password, GuidToColor, FuzzyDateFormatter );

            // Verify
            Assert.That( viewModel.CreationDate, Is.EqualTo( FuzzyDateFormatter.Format( new DateTime( 2011, 11, 11 ) ) ) );
            Assert.That( viewModel.ModificationDate, Is.EqualTo( FuzzyDateFormatter.Format( new DateTime( 2011, 11, 14 ) ) ) );
        }

        [ Test ]
        public void ViewModelPresentsGeneratorUsingResources( )
        {
            // Set up
            PasswordDigestDocument password = new PasswordDigestDocumentBuilder
                                                  {
                                                      Key = "ABCD",
                                                      Iteration = 2,
                                                      PasswordGenerator = PasswordGenerators.Full,
                                                      CreatedOn = new DateTime( 2011, 11, 11 ),
                                                      ModifiedOn = new DateTime( 2011, 11, 14 ),
                                                      Note = "Need this for EFGH"
                                                  };


            // Exercise
            var viewModel = new PasswordListEntryViewModel( password, GuidToColor, FuzzyDateFormatter );

            // Verify
            Assert.That( viewModel.GeneratorName, Is.EqualTo( Resources.PasswordGeneratorccf1451c4b3045a499b0d54ec3c3a7ee ) );
        }

        [ Test ]
        public void ViewModelPresentsMasterPasswordId( )
        {
            // Set up

            Guid guid = Guid.Parse( "ADB07A84-ED76-420F-9570-D0684DD044FE" );
            PasswordDigestDocument password = new PasswordDigestDocumentBuilder
                                                  {
                                                      Key = "ABCD",
                                                      Iteration = 2,
                                                      PasswordGenerator = PasswordGenerators.Full,
                                                      CreatedOn = new DateTime( 2011, 11, 11 ),
                                                      ModifiedOn = new DateTime( 2011, 11, 14 ),
                                                      Note = "Need this for EFGH",
                                                      MasterPasswordId = guid
                                                  };


            // Exercise
            var viewModel = new PasswordListEntryViewModel( password, GuidToColor, FuzzyDateFormatter );

            // Verify
            Assert.That( viewModel.MasterPasswordGuid, Is.EqualTo( guid ) );
            Assert.That( viewModel.MasterPasswordColor, Is.EqualTo( GuidToColor.Convert( guid ) ) );
        }

        [ Test ]
        public void NoteIsVisibleWhenNonEmpty( )
        {
            // Set up
            PasswordDigestDocument password = new PasswordDigestDocumentBuilder
                                                  {
                                                      Key = "ABCD",
                                                      Iteration = 2,
                                                      PasswordGenerator = PasswordGenerators.Full,
                                                      CreatedOn = new DateTime( 2011, 11, 11 ),
                                                      ModifiedOn = new DateTime( 2011, 11, 14 ),
                                                      Note = "Need this for EFGH"
                                                  };


            // Exercise
            var viewModel = new PasswordListEntryViewModel( password, GuidToColor, FuzzyDateFormatter );

            // Verify
            Assert.That( viewModel.NoteVisible, Is.True );
        }

        [ Test ]
        public void NoteIsNotVisibleWhenEmpty( )
        {
            // Set up
            PasswordDigestDocument password = new PasswordDigestDocumentBuilder
                                                  {
                                                      Key = "ABCD",
                                                      Iteration = 2,
                                                      PasswordGenerator = PasswordGenerators.Full,
                                                      CreatedOn = new DateTime( 2011, 11, 11 ),
                                                      ModifiedOn = new DateTime( 2011, 11, 14 ),
                                                      Note = ""
                                                  };


            // Exercise
            var viewModel = new PasswordListEntryViewModel( password, GuidToColor, FuzzyDateFormatter );

            // Verify
            Assert.That( viewModel.NoteVisible, Is.False );
        }
    }
}