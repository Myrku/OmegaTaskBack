using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                    List<string> data = new List<string>();
                    foreach (var value in values)
                    {
                        data.Add(value.ToString().Contains(',') ? $"\"{value}\"" : value.ToString()); // экранирование на если в строке будет разделитель
                    }
                    var line = string.Join(',', data);
                    file.WriteLine(line);
                }
            }
        }
    }
}
