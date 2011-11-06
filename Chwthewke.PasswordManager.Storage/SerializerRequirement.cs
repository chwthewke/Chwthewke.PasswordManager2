using System.Xml.Linq;

namespace Chwthewke.PasswordManager.Storage
{
    internal class SerializerRequirement
    {
        public int StartingVersion { get; set; }
        public string ElementName { get; set; }

        public bool Check( XElement target, int version )
        {
            if ( StartingVersion > version )
                return true;
            return target.Element( ElementName ) != null;
        }
    }
}