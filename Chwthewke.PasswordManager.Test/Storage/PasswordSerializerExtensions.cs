using System.Collections.Generic;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Test.Storage
{
    internal static class PasswordSerializerExtensions
    {
        public static string ToXml( this PasswordSerializer2 serializer, IEnumerable<PasswordDigestDocument> passwords )
        {
            var store = new InMemoryPasswordStore( );
            serializer.Save( passwords, store );
            return store.Content;
        }

        public static IEnumerable<PasswordDigestDocument> FromXml( this PasswordSerializer2 serializer, string xml )
        {
            var store = new InMemoryPasswordStore { Content = xml };
            return serializer.Load( store );
        }
    }
}