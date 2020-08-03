using System.Data;
using System.IO;

namespace SqlToFlatFileStandard
{
    public interface IDataWriter
    {
        string CalculatedOutputFilePath { get; }
        void Write();
        void WriteFileHeader(IDataReader reader, StreamWriter fileWriter);

        /// <summary>
        /// If InlineQuery is set, returns the inline query, else attempts to read query from QueryFile.
        /// </summary>
        /// <returns>query text</returns>
        string GetQuery();
    }
}
