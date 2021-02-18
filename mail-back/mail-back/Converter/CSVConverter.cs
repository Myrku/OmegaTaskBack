using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace mail_back.Converter
{
    public class CSVConverter : ICSVConvert
    {
        // Метод создает csv файл на основе списка элементов
        public void Convert<T>(List<T> items, string fileName)
        {
            using (StreamWriter file = new StreamWriter(fileName))
            {
                var properties = typeof(T).GetProperties();
                var propsName = properties.Select(p => p.Name);
                file.WriteLine(string.Join(',', propsName));
                foreach (var row in items)
                {
                    var values = properties.Select(p => p.GetValue(row, null));
                    var line = string.Join(',', values);
                    file.WriteLine(line);
                }
            }
        }
    }
}
