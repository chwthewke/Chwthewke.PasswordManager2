using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Editor
{
    [ TestFixture ]
    public class PasswordEditorSaveTest
    {
        [ SetUp ]
        public void SetUpEditor( )
        {
            _storage = new PasswordStore( PasswordGenerators.All, new Sha512Factory( ) );

            IPasswordGenerator[ ] passwordGenerators = new[ ]
                                                           { PasswordGenerators.AlphaNumeric, PasswordGenerators.Full };
            _editor = new PasswordEditor( passwordGenerators,
                                          new PasswordDigester( Hashes.Sha512Factory, new TimeProvider( ) ),
                                          _storage );
        }

        [ Test ]
        public void SaveNoopsIfNoPasswordsAreGenerated( )
        {
            // Setup
            _editor.Key = "aKey";
            // Exercise
            _editor.SavedSlot = PasswordGenerators.AlphaNumeric;
            // Verify
            Assert.That( _storage.Passwords, Is.Empty );
        }

        [ Test ]
        public void SaveAddsDigestToStore( )
        {
            // Setup
            _editor.Key = "aKey";
            _editor.GeneratePasswords( Util.Secure( "abc" ) );
            // Exercise
            _editor.SavedSlot = PasswordGenerators.AlphaNumeric;
            // Verify
            Assert.That( _storage.Passwords,
                         Is.EquivalentTo( new[ ]
                                              {
                                                  _editor.GeneratedPassword( PasswordGenerators.AlphaNumeric ).
                                                      SavablePasswordDigest
                                              } ) );
        }

        [ Test ]
        public void SaveOtherSlotRemovesPreviousDigestFromStore( )
        {
            // Setup
            _editor.Key = "aKey";
            _editor.GeneratePasswords( Util.Secure( "abc" ) );
            _editor.SavedSlot = PasswordGenerators.AlphaNumeric;
            // Exercise
            _editor.SavedSlot = PasswordGenerators.Full;
            // Verify
            Assert.That( _storage.Passwords,
                         Is.EquivalentTo( new[ ]
                                              {
                                                  _editor.GeneratedPassword( PasswordGenerators.Full ).
                                                      SavablePasswordDigest
                                              } ) );
        }

        [ Test ]
        public void SetSavedSlotToNullRemovesDigestFromStore( )
        {
            // Setup
            _editor.Key = "aKey";
            _editor.GeneratePasswords( Util.Secure( "abc" ) );
            _editor.SavedSlot = PasswordGenerators.AlphaNumeric;
            // Exercise
            _editor.SavedSlot = null;
            // Verify
            Assert.That( _storage.Passwords, Is.Empty );
        }

        [ Test ]
        public void UpdatingNoteAfterSaveIsPropagated( )
        {
            // Setup
            _editor.Key = "aKey";
            _editor.GeneratePasswords( Util.Secure( "abc" ) );
            _editor.SavedSlot = PasswordGenerators.AlphaNumeric;
            string note = "Hey I forgot a note.";
            // Exercise
            _editor.Note = note;
            // Verify
            Assert.That( _storage.FindPasswordInfo( "aKey" ).Note, Is.EqualTo( note ) );
        }

        private IPasswordEditor _editor;
        private IPasswordStore _storage;
    }
}