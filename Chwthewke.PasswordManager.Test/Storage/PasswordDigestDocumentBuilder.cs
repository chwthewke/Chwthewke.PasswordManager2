﻿using System;
using System.Xml.Linq;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Test.Storage
{
    public class PasswordDigestDocumentBuilder
    {
        public string Key { get; set; }

        public byte[ ] Hash { get; set; }

        public Guid PasswordGenerator { get; set; }

        public int Iteration { get; set; }

        public Guid MasterPasswordId { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string Note { get; set; }

        public PasswordDigest2 Digest
        {
            get { return new PasswordDigest2( Key, Hash, Iteration, PasswordGenerator ); }
            set
            {
                Key = value.Key;
                Hash = value.Hash;
                PasswordGenerator = value.PasswordGenerator;
                Iteration = value.Iteration;
            }
        }

        public PasswordDigestDocumentBuilder( )
        {
            Key = "key";
            Hash = new byte[ ] { 0x00 };
            Note = string.Empty;
            Iteration = 1;
            PasswordGenerator = PasswordGenerators2.AlphaNumeric;
        }

        public static implicit operator PasswordDigestDocument( PasswordDigestDocumentBuilder builder )
        {
            return builder.Build( );
        }

        public PasswordDigestDocument Build( )
        {
            return new PasswordDigestDocument( Digest, MasterPasswordId, CreatedOn, ModifiedOn, Note );
        }

        public XElement ToXml( )
        {
            return new XElement( PasswordSerializer2.PasswordElement,
                                 new XElement( PasswordSerializer2.KeyElement, Key ),
                                 new XElement( PasswordSerializer2.HashElement,
                                               Convert.ToBase64String( Hash ) ),
                                 new XElement( PasswordSerializer2.PasswordSettingsIdElement,
                                               PasswordGenerator.ToString( ) ),
                                 new XElement( PasswordSerializer2.IterationElement,
                                               Iteration.ToString( ) ),
                                 new XElement( PasswordSerializer2.MasterPasswordIdElement,
                                               MasterPasswordId.ToString( ) ),
                                 new XElement( PasswordSerializer2.TimestampElement,
                                               CreatedOn.Ticks ),
                                 new XElement( PasswordSerializer2.ModifiedElement,
                                               ModifiedOn.Ticks ),
                                 new XElement( PasswordSerializer2.NoteElement,
                                               Note ) );
        }

        public static implicit operator XElement( PasswordDigestDocumentBuilder builder )
        {
            return builder.ToXml( );
        }
    }
}