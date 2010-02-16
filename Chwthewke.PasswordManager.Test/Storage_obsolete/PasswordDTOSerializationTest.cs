using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage.Serialization;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Tests.Storage
{
    [ TestFixture ]
    public class PasswordDTOSerializationTest
    {
        [ Test ]
        public void TestSerializeDTO( )
        {
            // Setup
            PasswordDTO dto = new PasswordDTO
                                  {
                                      Key = "my,Key",
                                      PasswordType = PasswordType.Ascii,
                                      Hash = new byte[ ] { 0x01, 0x55, 0x4d, 0xfe }
                                  };
            Mock<IExportablePasswordCollection> mockRepo = new Mock<IExportablePasswordCollection>( );
            mockRepo.Setup( r => r.ExportPasswords( ) ).Returns( new Collection<PasswordDTO> { dto } );
            // Exercise
            StringWriter writer = new StringWriter( );
            new CsvPasswordSerializer( mockRepo.Object ).Save( writer );
            // Verify
            Assert.That( writer.ToString( ), Is.EqualTo( "my,Key,1,AVVN/g==\r\n" ) );
        }

        [ Test ]
        public void TestDeserializeDTO( )
        {
            // Setup
            TextReader reader = new StringReader( "my,Key,1,AVVN/g==\r\n" );
            PasswordDTO expectedDto = new PasswordDTO
                                          {
                                              Key = "my,Key",
                                              PasswordType = PasswordType.Ascii,
                                              Hash = new byte[ ] { 0x01, 0x55, 0x4d, 0xfe }
                                          };
            Mock<IExportablePasswordCollection> mockRepo = new Mock<IExportablePasswordCollection>( );
            // Exercise
            new CsvPasswordSerializer( mockRepo.Object ).Load( reader );
            // Verify
            mockRepo.Verify(
                r =>
                r.ImportPasswords( It.Is<IEnumerable<PasswordDTO>>( dtos => VerifyDTOInEnumerable( dtos, expectedDto ) ) ) );
        }

        private static bool VerifyDTOInEnumerable( IEnumerable<PasswordDTO> passwordDtos, PasswordDTO expectedDto )
        {
            List<PasswordDTO> dtos = passwordDtos.ToList( );
            return dtos.Count == 1 &&
                   expectedDto.Equals( dtos[ 0 ] );
        }
    }
}