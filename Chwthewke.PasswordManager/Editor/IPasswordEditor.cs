using System.Collections.Generic;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Editor
{
    public interface IPasswordEditor
    {
        IPasswordDocument NewDocument( );

        IEnumerable<IPasswordFactory> PasswordFactories { get; }

        void GeneratePasswords( IPasswordDocument document, byte[ ] masterPassword );
    }
}