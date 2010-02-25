using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Linq;

namespace Chwthewke.PasswordManager.Storage
{
    public class PasswordStoreSerializer : IPasswordStoreSerializer
    {
        public const string PasswordStoreElement = "password-store";
        public const string PasswordElement = "password";
        public const string KeyElement = "key";
        public const string HashElement = "hash";
        public const string GuidElement = "guid";
        public const string TimestampElement = "timestamp";
        public const string NoteElement = "note";

        public PasswordStoreSerializer( Encoding encoding )
        {
            _encoding = encoding;
        }

        public void Save( IPasswordStore passwordStore, Stream outputStream )
        {
            XElement root = ToXml( passwordStore );
            using ( TextWriter tw = new StreamWriter( outputStream, _encoding ) )
                root.Save( tw );
        }

        private static XElement ToXml( IPasswordStore passwordStore )
        {
            return new XElement( PasswordStoreElement, passwordStore.Passwords.Select( ToXml ) );
        }

        private static XElement ToXml( PasswordInfo password )
        {
            return new XElement( PasswordElement,
                                 new XElement( KeyElement, password.Key ),
                                 new XElement( HashElement, Convert.ToBase64String( password.Hash ) ),
                                 new XElement( GuidElement, password.MasterPasswordId.ToString( ) ),
                                 new XElement( TimestampElement, password.CreationTime.Ticks ),
                                 new XElement( NoteElement, password.Note ) );
        }

        public void Load( IPasswordStore passwordStore, Stream inputStream )
        {
            using ( TextReader tr = new StreamReader( inputStream, _encoding ) )
            {
                XElement xml = XElement.Load( tr );
                LoadFromXml( passwordStore, xml );
            }
        }

        private void LoadFromXml( IPasswordStore passwordStore, XElement xml )
        {
            throw new NotImplementedException( );
        }

        private readonly Encoding _encoding;
    }
}