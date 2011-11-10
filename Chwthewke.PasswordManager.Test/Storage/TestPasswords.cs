using System;
using System.Collections.Generic;
using System.Linq;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Test.Storage
{
    internal static class TestPasswords
    {
        public static readonly PasswordDigestDocument Abcd = new PasswordDigestDocumentBuilder
                                                                 {
                                                                     Key = "abcd",
                                                                     Iteration = 1,
                                                                     Hash = new byte[ ] { 0xAA, 0xBB },
                                                                     PasswordGenerator = PasswordGenerators2.AlphaNumeric,
                                                                     CreatedOn = new DateTime( 2011, 11, 1 ),
                                                                     ModifiedOn = new DateTime( 2011, 11, 1 ),
                                                                     MasterPasswordId = Guid.NewGuid( ),
                                                                     Note = "First password"
                                                                 };

        public static readonly PasswordDigestDocument Efgh = new PasswordDigestDocumentBuilder
                                                                 {
                                                                     Key = "efgh",
                                                                     Iteration = 10,
                                                                     Hash = new byte[ ] { 0x0A, 0x0B },
                                                                     PasswordGenerator = PasswordGenerators2.Full,
                                                                     CreatedOn = new DateTime( 2011, 11, 2 ),
                                                                     ModifiedOn = new DateTime( 2011, 11, 3 ),
                                                                     MasterPasswordId = Guid.NewGuid( ),
                                                                     Note = "Second password"
                                                                 };

        public static readonly PasswordDigestDocument Ijkl = new PasswordDigestDocumentBuilder
                                                                 {
                                                                     Key = "ijkl",
                                                                     Iteration = 10,
                                                                     Hash = new byte[ ] { 0x0A, 0x0B },
                                                                     PasswordGenerator = PasswordGenerators2.Full,
                                                                     CreatedOn = new DateTime( 2011, 11, 2 ),
                                                                     ModifiedOn = new DateTime( 2011, 11, 3 ),
                                                                     MasterPasswordId = Guid.NewGuid( ),
                                                                     Note = "Deleted password"
                                                                 }
            .Build( )
            .Delete( new DateTime( 2011, 11, 4 ) );

        public static readonly PasswordDigestDocument Mnop = new PasswordDigestDocumentBuilder
                                                                 {
                                                                     Key = "mnop",
                                                                     Iteration = 8,
                                                                     Hash = new byte[ ] { 0x3A, 0x7B },
                                                                     PasswordGenerator = PasswordGenerators2.Full,
                                                                     CreatedOn = new DateTime( 2011, 11, 1 ),
                                                                     ModifiedOn = new DateTime( 2011, 11, 9 ),
                                                                     MasterPasswordId = Guid.NewGuid( ),
                                                                     Note = "Fourth password"
                                                                 };
    }
}