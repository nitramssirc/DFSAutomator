using CsvHelper;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DFSAutomatorClient.Services
{
    public interface ICSVLoader
    {
        T[] Load<T>(Stream csvStream);

        string Generate<T>(IEnumerable<T> records);
    }

    public class CSVLoader : ICSVLoader
    {
        public string Generate<T>(IEnumerable<T> records)
        {
            using var memStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memStream);
            using var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
            csv.WriteRecords(records);
            return System.Text.Encoding.UTF8.GetString(memStream.ToArray());
        }

        public T[] Load<T>(Stream csvStream)
        {
            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<T>().ToArray();
        }


    }
}
