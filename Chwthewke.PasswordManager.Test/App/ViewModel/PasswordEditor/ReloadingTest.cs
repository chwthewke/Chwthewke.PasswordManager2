﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;
using Chwthewke.PasswordManager.Test.Engine;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor
{
    [ TestFixture ]
    public class ReloadingTest : PasswordEditorTestBase
    {
        [ Test ]
        public void ReloadingDatabaseUpdatesDirtiness( )
        {
            // Set up
            AddPassword( "abc", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            ViewModel = ViewModelFactory.PasswordEditorFor( "abc" );
            ViewModel.UpdateMasterPassword( "123".ToSecureString( ) );

            Assert.That( ViewModel.SaveCommand.CanExecute( null ), Is.False );
            bool canSaveChanged = false;
            ViewModel.SaveCommand.CanExecuteChanged += ( s, e ) => canSaveChanged = true;


            AddPassword( "abc", "A note", PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            // Exercise
            ViewModel.UpdateFromStore( );
            // Verify
            Assert.That( ViewModel.SaveCommand.CanExecute( null ), Is.True );
            Assert.That( canSaveChanged, Is.True );
        }

        [ Test ]
        public void ReloadingDatabaseKeepsNoteAndSelectedGenerator( )
        {
            // Set up
            AddPassword( "abc", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            ViewModel = ViewModelFactory.PasswordEditorFor( "abc" );
            ViewModel.UpdateMasterPassword( "123".ToSecureString( ) );


            AddPassword( "abc", "A note", PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            // Exercise
            ViewModel.UpdateFromStore( );
            // Verify
            Assert.That( ViewModel.Note, Is.EqualTo( string.Empty ) );
            Assert.That( ViewModel.Slots.First( s => s.Generator == PasswordGenerators.Full ).IsSelected, Is.True );
        }
    }
}