using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Migration;
using Chwthewke.PasswordManager.Storage;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Migration
{
    public class LegacyItemImporterTest
    {
        private Mock<IPasswordStore> _passwordStoreMock;
        private Mock<IPasswordDigester> _passwordDigesterMock;
        private Mock<IMasterPasswordFinder> _masterPasswordFinderMock;
        private Mock<IPasswordStoreSerializer> _serializerMock;
        private LegacyItemImporter _importer;

        [ SetUp ]
        public void SetUpImporter( )
        {
            _passwordStoreMock = new Mock<IPasswordStore>( );

            _passwordDigesterMock = new Mock<IPasswordDigester>( );

            _masterPasswordFinderMock = new Mock<IMasterPasswordFinder>( );

            _serializerMock = new Mock<IPasswordStoreSerializer>( );

            _importer = new LegacyItemImporter( _passwordStoreMock.Object, _passwordDigesterMock.Object,
                                                _masterPasswordFinderMock.Object, _serializerMock.Object );
        }
    }
}