using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using CalculatorWPFprj.Annotations;
using GalaSoft.MvvmLight.CommandWpf;



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
            DataContext = new MainViewModel(new MemHistoryStorage());
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            
            Console.WriteLine(e.Key);
        }
    }

    public class MainViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly IHistoryStorage _historyStorage;

        public MainViewModel(IHistoryStorage historyStorage)
        {
            _historyStorage = historyStorage;
            History = new ObservableCollection<HElement>();

            CalculateCommand = new RelayCommand<string>(x =>
            {
                if (!string.IsNullOrWhiteSpace(Answer))
                {
                    _historyStorage.write(History, new HElement(UIEquation, Answer));
                    Equation = Answer;
                }
                else
                {
                    Equation = "Incorrect input string";
                }
                Calculated = true;
                Answer = "";

            }, x => string.IsNullOrWhiteSpace(x) == false);

            IKeyboardCommand = new RelayCommand<string>(x =>
            {
                if (x == "+" || x == "-" || x == "*" || x == "/")
                {
                    if (string.IsNullOrWhiteSpace(Equation)) return; 

                    if(Equation.EndsWith(x)) return;
                    
                    if (Calculated)
                    {
                        Calculated = false;
                    }
                }
                else
                {
                    if (Calculated)
                    {
                        Calculated = false;
                        Equation = "";
                    }
                }
                
                Equation += x;
            }, x => string.IsNullOrWhiteSpace(x) == false);

            DeleteCommand = new RelayCommand<string>(x =>
            {
                if (Calculated)
                {
                    Calculated = false;
                    Equation = "";
                }
                else
                {
                    Equation = Equation.Substring(0, Equation.Length - 1);
                }
            }, x => string.IsNullOrWhiteSpace(x) == false);

            PKeyboardCommand = new RelayCommand<Button>(x =>
            {
                //IKeyboardCommand.Execute(x.CommandParameter);
                typeof(System.Windows.Controls.Primitives.ButtonBase).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(x, new object[0]);
                PressButton(x);
            }, x => x != null);

            ChangeHistoryVisibilityCommand = new RelayCommand<Button>(x =>
            {
                if(HistoyVisibility == "Collapsed")
                {
                    _historyStorage.read(History);
                    HistoyVisibility = "Visible";
                }
                else
                {
                    HistoyVisibility = "Collapsed";
                }
            });

        }
        private void PressButton(Button x)
        {
            var methodInfo = typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic);
            methodInfo?.Invoke(x, new object[] { true });
            var context = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Run(() => Thread.Sleep(100)).ContinueWith(_ =>
                methodInfo?.Invoke(x, new object[] {false}), context);
        }

        public ICommand CalculateCommand { get; }
        public ICommand IKeyboardCommand { get; }
        public ICommand PKeyboardCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ChangeHistoryVisibilityCommand { get; }


        private string _deletesymbol = "⌫";
        public string DeleteSymbol
        {
            get => _deletesymbol;
        }

        private bool _calculated = false;
        public bool Calculated
        {
            get => _calculated;

            set
            {
                if (_calculated == value) return;
                _calculated = value;

                if (_calculated)
                {
                    _deletesymbol = "C";
                }
                else
                {
                    _deletesymbol = "⌫";
                }
                OnPropertyChanged(nameof(Calculated));
                OnPropertyChanged(nameof(DeleteSymbol));
            }
        }

        private string _equation = "";
        public string Equation
        {
            get => _equation;

            set
            {
                try
                {
                    Calculator myCalculator = new Calculator();
                    Answer = myCalculator.Calc(value);
                    _errorsDictionary[nameof(Equation)] = null;
                    _errorsDictionary[nameof(UIEquation)] = null;
                }
                catch (Exception ex)
                {
                    _errorsDictionary[nameof(Equation)] = ex.Message;
                    _errorsDictionary[nameof(UIEquation)] = ex.Message;
                    Answer = "";
                }
                if (_equation == value) return;
                _equation = value;
                _equation = _equation.Replace(".", ",");
                OnPropertyChanged(nameof(Equation));
                OnPropertyChanged(nameof(UIEquation));
            }
        }


        private string _uiequation;
        public string UIEquation
        {
            get
            {
                _uiequation = Equation.Length * 35 > WindowWidth ? ("..." + Equation.Substring(Equation.Length - 1 - WindowWidth / 35, WindowWidth / 35 + 1)) : Equation;
                _uiequation = _uiequation.Replace("*", "✕");
                _uiequation = _uiequation.Replace("/", "÷");
                return _uiequation;
            }
        }


        public int WindowWidth
        {
            get => Convert.ToInt32(((System.Windows.Controls.Panel)Application.Current.MainWindow.Content).ActualWidth);
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

        private string _histoyvisibility = "Collapsed";
        public string HistoyVisibility
        {
            get => _histoyvisibility;

            set
            {
                if (_histoyvisibility == value) return;

                _histoyvisibility = value;
                OnPropertyChanged(nameof(HistoyVisibility));
            }
        }


        public ObservableCollection<HElement> History { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private Dictionary<string, string> _errorsDictionary = new Dictionary<string, string>();

        public string Error
        {
            get
            {
                return string.Join(Environment.NewLine,
                    _errorsDictionary.Values.Where(x => string.IsNullOrWhiteSpace(x) == false));
            }
        }
        public string this[string equat] => _errorsDictionary.TryGetValue(equat, out var error) ? error : null;
    }
}
