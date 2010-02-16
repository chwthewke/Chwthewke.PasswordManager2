using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Storage.Serialization
{
    internal class CsvPasswordSerializer : ISerializer
    {
        private static readonly IList<PasswordType> PasswordTypes = new List<PasswordType>
                                                                        {
                                                                            PasswordType.AlphaNumeric,
                                                                            PasswordType.Ascii
                                                                        };

        private readonly IExportablePasswordCollection _repository;

        private const char Separator = ',';

        private static readonly Regex Parser =
            new Regex( @"(.*)" + Separator + @"([\d]+)" + Separator + "([a-zA-Z0-9+=/]+)" );

        public CsvPasswordSerializer( IExportablePasswordCollection repository )
        {
            _repository = repository;
        }

        public void Save( TextWriter writer )
        {
            foreach ( PasswordDTO dto in _repository.ExportPasswords( ) )
            {
                writer.WriteLine( "{1}{0}{2}{0}{3}", Separator,
                                  dto.Key, PasswordTypes.IndexOf( dto.PasswordType ), Convert.ToBase64String( dto.Hash ) );
            }

            writer.Flush( );
        }

        public void Load( TextReader reader )
        {
            _repository.ImportPasswords( ReadPasswords( reader ) );
        }

        private static IEnumerable<PasswordDTO> ReadPasswords( TextReader reader )
        {
            string line;
            while ( ( line = reader.ReadLine( ) ) != null )
            {
                MatchCollection matches = Parser.Matches( line );
                if ( matches.Count != 1 )
                    continue;

                PasswordDTO dto;
                try
                {
                    dto = GetPasswordDTOFromMatch( matches[ 0 ] );
                }
                catch ( Exception )
                {
                    continue;
                }
                yield return dto;
            }
        }

        private static PasswordDTO GetPasswordDTOFromMatch( Match match )
        {
            string key = match.Groups[ 1 ].Value;

            PasswordType passwordType = PasswordTypes[ int.Parse( match.Groups[ 2 ].Value ) ];

            byte[ ] hash = Convert.FromBase64String( match.Groups[ 3 ].Value );

            return new PasswordDTO { Key = key, PasswordType = passwordType, Hash = hash };
        }
    }
}