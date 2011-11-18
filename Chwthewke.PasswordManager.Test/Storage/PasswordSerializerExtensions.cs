using System.Collections.Generic;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Test.Storage
{
    internal static class PasswordSerializerExtensions
    {
        public static string ToXml( this PasswordSerializer serializer, IEnumerable<PasswordDigestDocument> passwords )
        {
            var store = new InMemoryTextResource( );
            serializer.Save( passwords, store );
            return store.Content;
        }

        public static IEnumerable<PasswordDigestDocument> FromXml( this PasswordSerializer serializer, string xml )
        {
            var store = new InMemoryTextResource { Content = xml };
            return serializer.Load( store );
        }
    }
}