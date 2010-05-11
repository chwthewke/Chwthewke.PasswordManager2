using System.Collections.Generic;
using System.IO;

namespace Chwthewke.PasswordManager.Migration
{
    public interface ILegacyItemLoader
    {
        IEnumerable<LegacyItem> Load( TextReader reader );
    }
}