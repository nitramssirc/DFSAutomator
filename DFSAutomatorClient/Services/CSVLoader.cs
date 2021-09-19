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
    }

    public class CSVLoader : ICSVLoader
    {
        public T[] Load<T>(Stream csvStream)
        {
            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<T>().ToArray();
        }
    }
}
