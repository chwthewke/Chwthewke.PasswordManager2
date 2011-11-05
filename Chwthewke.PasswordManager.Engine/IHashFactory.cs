using System;

namespace Chwthewke.PasswordManager.Engine
{
    [Obsolete]
    internal interface IHashFactory
    {
        IHash GetHash( );
        int HashSize { get; }
    }
}