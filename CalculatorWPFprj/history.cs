using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;


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
        static public void read_from_file(ObservableCollection<HElement> hElements)
        {
            string path = @"A:\mming\Calculator\Calculator\CalculatorWPFprj\history.json";
            using (StreamReader sr = File.OpenText(path))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    hElements.Add(JsonSerializer.Deserialize<HElement>(s));
                }
            }
        }

        static public void write_to_file(ObservableCollection<HElement> hElements)
        {
            string path = @"A:\mming\Calculator\Calculator\CalculatorWPFprj\history.json";
            string jsonHistory = JsonSerializer.Serialize(hElements);
            
            using (StreamWriter sw = File.CreateText(path))
            {
                foreach (HElement element in hElements)
                {
                    sw.WriteLine(JsonSerializer.Serialize(element));
                }
                
            }
        }
    }
}
