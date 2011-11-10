using System;
using System.Collections.Generic;
using System.Linq;

namespace Chwthewke.PasswordManager.Storage
{
    public class XmlPasswordData : IPasswordData
    {
        public static IPasswordData From( ITextResource textResource )
        {
            return new XmlPasswordData( new PasswordSerializer2( ), textResource );
        }

        internal XmlPasswordData( PasswordSerializer2 serializer, ITextResource store )
        {
            if ( serializer == null )
                throw new ArgumentNullException( "serializer" );
            if ( store == null )
                throw new ArgumentNullException( "store" );
            _serializer = serializer;
            _store = store;
        }

        public IList<PasswordDigestDocument> LoadPasswords( )
        {
            return _serializer.Load( _store ).ToList( );
        }

        public void SavePasswords( IList<PasswordDigestDocument> passwords )
        {
            _serializer.Save( passwords, _store );
        }

        private readonly PasswordSerializer2 _serializer;
        private readonly ITextResource _store;
    }
}