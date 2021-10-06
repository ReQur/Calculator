using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CalculatorWPFprj.Annotations;
using GalaSoft.MvvmLight.CommandWpf;
using sharpcalc;


namespace CalculatorWPFprj
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            CalculateCommand = new RelayCommand<string>(x =>
            {
                Calculator myCalculator = new Calculator();
                Equation = myCalculator.Calc(x);
            }, x => string.IsNullOrWhiteSpace(x) == false);

            KeyboardCommand = new RelayCommand<string>(x =>
            {
                Equation += x;
            }, x => string.IsNullOrWhiteSpace(x) == false);
        }
        public ICommand CalculateCommand { get; }
        public ICommand KeyboardCommand { get; }

        private string _equation;
        public string Equation
        {
            get => _equation;

            set
            {
                if (_equation == value) return;
                _equation = value;
                _equation = _equation.Replace(".", ",");
                OnPropertyChanged(nameof(Equation));
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
                OnPropertyChanged(nameof(Answer));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
