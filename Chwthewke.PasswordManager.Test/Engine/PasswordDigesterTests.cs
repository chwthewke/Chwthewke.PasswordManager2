using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class PasswordDigesterTests
    {
        [ SetUp ]
        public void SetUpPasswordDigester( )
        {
            _hashMock = new Mock<IHash>( );
            _timeProviderMock = new Mock<ITimeProvider>( );
            _digester = new PasswordDigester( _hashMock.Object, _timeProviderMock.Object );
        }

        private PasswordDigester _digester;

        private Mock<IHash> _hashMock;
        private Mock<ITimeProvider> _timeProviderMock;
    }
}