using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using Dapper;


namespace CalculatorWPFprj
{
    public class HElement
    {
        private string _equation;
        public string Equation
        {
            get => _equation;
            set
            {
                if (_equation == value) return;

                _equation = value;
            }
        }
        private string _answer;
        public string Answer
        {
            get => _answer;

            set
            {
                if (_answer == value) return;

                _answer = value;
            }
        }
        public HElement(string equation, string answer)
        {
            _equation = equation;
            _answer = answer;
        }
        public HElement()
        {
        }
    }



    public interface IHistoryStorage
    {
        void read(ObservableCollection<HElement> hElements);
        void write(ObservableCollection<HElement> hElements, HElement hElement);
    }

    class MemHistoryStorage : IHistoryStorage
    {
        public void read(ObservableCollection<HElement> hElements)
        {
            
        }

        public void write(ObservableCollection<HElement> hElements, HElement hElement)
        {
            hElements.Add(hElement);
        }
    }

    class FileHistoryStorage : IHistoryStorage
    {
        public void read(ObservableCollection<HElement> hElements)
        {
            string path = @"C:\Users\xdd_2\source\repos\CalculatorWPFprj\CalculatorWPFprj\history.json";
            using (StreamReader sr = File.OpenText(path))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    hElements.Add(JsonSerializer.Deserialize<HElement>(s));
                }
            }
        }

        public void write(ObservableCollection<HElement> hElements, HElement hElement)
        {
            string path = @"C:\Users\xdd_2\source\repos\CalculatorWPFprj\CalculatorWPFprj\history.json";

            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine(JsonSerializer.Serialize(hElement));
            }
        }
    }

    class dbHistoryStorage : IHistoryStorage
    {
        public void read(ObservableCollection<HElement> hElements)
        {
            using (var connection =
                new System.Data.SQLite.SQLiteConnection("Data Source=C:\\Programming\\SQLiteStudio\\HistoryDataBase"))
            {
                connection.Open();
                var result = connection.Query<HElement>("select * from HistoryDataTable");

                foreach (var element in result)
                {
                    hElements.Add(element);
                }

            }
        }

        public void write(ObservableCollection<HElement> hElements, HElement hElement)
        {
            using (var connection =
                new System.Data.SQLite.SQLiteConnection("Data Source=C:\\Programming\\SQLiteStudio\\HistoryDataBase"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "Insert into HistoryDataTable (Equation, Answer) values ('" + hElement.Equation.ToString() + "', '"
                                      + hElement.Answer.ToString() + "')";
                command.ExecuteReader();

            }
        }
    }
}
