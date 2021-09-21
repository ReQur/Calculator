using System;
using System.Collections.Generic;
using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;


namespace sharpcalc
{
    public class Calculator
    {
        private string numberchar = "0123456789,";
        private char s;
        private string input_string;

        private Stack<string> Numbers;
        private Stack<string> Stack;

        private Dictionary<string, string> cupMatrix = new Dictionary<string, string>
        {
            {"$E", "<="}, {"E$", "="},
            {"E+", "="}, {"+T", "<="}, {"E-", "="}, {"-T", "<="},
            {"T*", "="}, {"*M", "="}, {"T/", "="}, {"/M", "="},
            {"-M", "<="}, {"E)", "="}, {"(E", "<="},

            {"$T", "<"},
            {"$M", "<"}, {"$-", "<"}, {"$(", "<"}, {"$C", "<"},
            {"+M", "<"}, {"+-", "<"}, {"+(", "<"},
            {"+C", "<"}, {"*-", "<"}, {"*(", "<"}, {"*C", "<"}, {"--", "<"}, {"-+", "<"},
            {"/-", "<"}, {"/(", "<"}, {"/C", "<"}, {"-(", "<"},
            {"-C", "<"}, {"(T", "<"}, {"(M", "<"},
            {"(-", "<"}, {"((", "<"}, {"(C", "<"},

            {"T$", ">"}, {"M$", ">"}, {")$", ">"}, {"C$", ">"},
            {"T+", ">"}, {"M+", ">"}, {")+", ">"}, {"C+", ">"},{"T-", ">"},{"M-", ">"},
            {"M*", ">"}, {"M/", ">"}, {")*", ">"}, {")/", ">"},{")-", ">"},{"C-", ">"},
            {"C*", ">"}, {"C/", ">"}, {"T)", ">"}, {"M)", ">"},
            {"))", ">"}, {"C)", ">"}
        };

        private Dictionary<string, string> phraseMatrix = new Dictionary<string, string>
        {
            {"$<=E=$", "S"},
            {"<T>", "E"}, {"<E=+=T>", "E"},{"<E=-=T>", "E"},
            {"<M>", "T"}, {"<T=*=M>", "T"},{"<T=/=M>", "T"},
            {"<-=M>", "M"}, {"<-<=M>", "M"},{"<(=E=)>", "M"},{"<C>", "M"},{"<(<=E=)>", "M"},
            {"<=T>", "E"}, {"<=E=+=T>", "E"},{"<=E=-=T>", "E"},
            {"<=M>", "T"}, {"<=T=*=M>", "T"},{"<=T=/=M>", "T"},{"<=T=/<=M>", "T"},
            {"<=-<=M>", "M"}, {"<=-=M>", "M"}, {"<=(=E=)>", "M"}, {"<=E=+<=T>", "E"}, {"<=E=-<=T>", "E"}
        };

        public Calculator()
        {
            Stack = new Stack<string>();
            Numbers = new Stack<string>();


        }

        public string Calc(string input)
        {
            if (input.Trim().Length < 1) throw new ArgumentException("String is Empty");
            input_string = "$" + input + "$";
            Get();
            Stack.Push(s.ToString());
            Run();
            Stack.Clear();
            return Numbers.Pop();

        }

        public void Get()
        {
            string number = "";
            input_string = input_string.TrimStart();

            s = input_string[0];
            input_string = input_string.Remove(0, 1);

            while (numberchar.Contains(s))
            {
                number += s;
                if (numberchar.Contains(input_string[0]))
                {
                    s = input_string[0];
                    input_string = input_string.Remove(0, 1);
                }
                else break;
            }

            if (number.Length != 0)
            {
                Numbers.Push(number);
                s = 'C';
            }

        }

        public void Analization()
        {
            string cup = Stack.Peek() + s;

            if (cupMatrix[cup].Length == 0) throw new ArgumentException("Wrong input string", "\"" + cup + "\"" + " is unkown couple of elements");

            if (cupMatrix[cup].Length == 2)
            {
                Stack.Push("<");
                Stack.Push("=");
                Stack.Push(s.ToString());
            }
            else
            if (cupMatrix[cup].Length == 1)
            {
                string act = cupMatrix[cup];

                if (act != ">")
                {
                    Stack.Push(act);
                    Stack.Push(s.ToString());
                }
                else
                {

                    Stack.Push(act);

                    char ths = s;
                    Convolution();
                    Analization();
                    s = ths;
                    Analization();
                }

            }
        }

        public void Run()
        {
            Get();

            Analization();


            if (input_string.Length != 0)
                Run();
        }

        public void Convolution()
        {
            Stack<string> tmpStack = new Stack<string>(new Stack<string>(Stack));
            string phrase = "";
            int state = 0;
            string tmps;
            bool f = true;

            while (f)
            {
                if (tmpStack.Count <= 0) break;
                tmps = tmpStack.Pop();
                phrase = tmps + phrase;
                switch (state)
                {
                    case 0:
                        if (tmps == "=")
                            state = 1;
                        if (tmps == "<")
                            state = 2;
                        if (tmps == "$")
                            state = 2;
                        break;

                    case 1:
                        state = 0;
                        if (tmps == "$")
                            state = 2;
                        break;

                    case 2:
                        f = false;
                        break;

                }
            }

            while (phrase.Length != 0 && !phraseMatrix.ContainsKey(phrase)) 
            {
                while (true)
                {
                    phrase = phrase.Remove(0, 1);
                    if (phrase[0] == '<')
                        break;
                    if (phrase.Length == 0)
                        break;
                }

            }

            if (phrase.Length <= 0)
                throw new ArgumentException("Wrong lexeme");
                
            string phrs = phraseMatrix[phrase];
            //cout << phrase << " switched on " << phrs->second << endl;
            s = Char.Parse(phrs);

            foreach (var ch in phrase) Stack.Pop();
            //Stack = new Stack<string>(Stack.Take(Stack.Count - phrase.Length));

            Calculation(phrase);
        }

        public int Operands(string phrase)
        {
            int q = 0;
            while (phrase.Length > 0)
            {
                if (phrase.EndsWith("E") || phrase.EndsWith("T") || phrase.EndsWith("M")) q++;
                phrase = phrase.Remove(phrase.Length - 1, 1);
            }
            return q;
        }


        public void Calculation(string phrase)
        {
            if (Operands(phrase) < 2)
                if (phrase.Contains("-") && phrase.Contains("M"))
                {
                    var tmp = Convert.ToInt32(Numbers.Pop());
                    tmp *= -1;
                    Numbers.Push(tmp.ToString());
                    return;
                }
                else return;
            if (phrase.Contains('*'))
            {
                double b = double.Parse(Numbers.Pop());
                double a = double.Parse(Numbers.Pop());
                double tmp = a * b;
                Numbers.Push(tmp.ToString());
            }

            else if (phrase.Contains('/'))
            {
                double b = double.Parse(Numbers.Pop());
                double a = double.Parse(Numbers.Pop());
                double tmp = a / b;
                Numbers.Push(tmp.ToString());
            }

            else if (phrase.Contains('-'))
            {
                double b = double.Parse(Numbers.Pop());
                double a = double.Parse(Numbers.Pop());
                double tmp = a - b;
                Numbers.Push(tmp.ToString());
            }

            else if (phrase.Contains('+'))
            {
                double b = double.Parse(Numbers.Pop());
                double a = double.Parse(Numbers.Pop());
                double tmp = a + b;
                Numbers.Push(tmp.ToString());
            }

        }

    }


    public class Tests
    {
        private Calculator calc;
        [SetUp]
        public void Setup()
        {
            calc = new Calculator();
        }

        //[Test]
        //public void Test1()
        //{
        //    calc = new Calculator();

        //    calc.Calc("7+8*(10+(-5))").Should().Be("47");
        //}
        //[Test]
        //public void Test2()
        //{
        //    calc = new Calculator();

        //    calc.Calc("  -1 -1 -11-1-13").Should().Be("-27");
        //}
        
        //[Test]
        //public void Test3()
        //{
        //    calc = new Calculator();

        //    calc.Calc("5*5*5*5*5").Should().Be("3125");
        //}

        [Test]
        public void Should_BeSuccess_WhenCalculationIsRight()
        {
            calc = new Calculator();

            calc.Calc("4,5+5,5*3,6").Should().Be("24,3");
            calc.Calc("5*5*5*5*5").Should().Be("3125");
            calc.Calc("  -1 -1 -11-1-13").Should().Be("-27");
            calc.Calc("7+8*(10+(-5))").Should().Be("47");
        }

        [Test]
        public void Should_ThrowException_WhenStringIsEmpty()
        {
            calc = new Calculator();

            Assert.Throws<ArgumentException>(() => calc.Calc(" "));
        }


        [Test]
        public void Should_ThrowException_WhenStringIsIncorrect()
        {
            calc = new Calculator();

            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() => calc.Calc(" 1**1"));
            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() => calc.Calc("randomstring"));
        }
    }
}