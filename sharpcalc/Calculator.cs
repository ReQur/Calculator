using System;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;
using FluentAssertions;


namespace sharpcalc
{
    public class Calculator
    {
        private string numberchar = "0123456789,";

        class Wrap
        {
            public char s;
            public string input_string;

            public Stack<string> Numbers;
            public Stack<string> Stack;

            public Wrap()
            {
                Numbers = new Stack<string>();
                Stack = new Stack<string>();
            }

        }

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

        }

        public string Calc(string input)
        {
            Wrap wrap = new Wrap();
            if (input.Trim().Length < 1) throw new ArgumentException("String is Empty");
            wrap.input_string = "$" + input + "$";
            wrap = Get(wrap);
            wrap.Stack.Push(wrap.s.ToString());
            wrap = Run(wrap);
            wrap.Stack.Clear();
            return wrap.Numbers.Pop();
        }

        private Wrap Get(Wrap wrap)
        {
            string number = "";
            wrap.input_string = wrap.input_string.TrimStart();

            wrap.s = wrap.input_string[0];
            wrap.input_string = wrap.input_string.Remove(0, 1);

            while (numberchar.Contains(wrap.s))
            {
                number += wrap.s;
                if (numberchar.Contains(wrap.input_string[0]))
                {
                    wrap.s = wrap.input_string[0];
                    wrap.input_string = wrap.input_string.Remove(0, 1);
                }
                else break;
            }

            if (number.Length != 0)
            {
                wrap.Numbers.Push(number);
                wrap.s = 'C';
            }

            return wrap;

        }

        private Wrap Analization(Wrap wrap)
        {
            string cup = wrap.Stack.Peek() + wrap.s;

            if (cupMatrix[cup].Length == 0) throw new ArgumentException("Wrong input string", "\"" + cup + "\"" + " is unkown couple of elements");

            if (cupMatrix[cup].Length == 2)
            {
                wrap.Stack.Push("<");
                wrap.Stack.Push("=");
                wrap.Stack.Push(wrap.s.ToString());
            }
            else
            if (cupMatrix[cup].Length == 1)
            {
                string act = cupMatrix[cup];

                if (act != ">")
                {
                    wrap.Stack.Push(act);
                    wrap.Stack.Push(wrap.s.ToString());
                }
                else
                {

                    wrap.Stack.Push(act);

                    char ths = wrap.s;
                    wrap = Convolution(wrap);
                    wrap = Analization(wrap);
                    wrap.s = ths;
                    wrap = Analization(wrap);
                }

            }

            return wrap;
        }

        private Wrap Run(Wrap wrap)
        {
            wrap = Get(wrap);

            wrap = Analization(wrap);


            if (wrap.input_string.Length != 0)
                wrap = Run(wrap);
            return wrap;
        }

        private Wrap Convolution(Wrap wrap)
        {
            Stack<string> tmpStack = new Stack<string>(new Stack<string>(wrap.Stack));
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
            wrap.s = Char.Parse(phrs);

            foreach (var ch in phrase) wrap.Stack.Pop();
            //Stack = new Stack<string>(Stack.Take(Stack.Count - phrase.Length));

            wrap = Calculation(phrase, wrap);
            return wrap;
        }

        private int Operands(string phrase)
        {
            int q = 0;
            while (phrase.Length > 0)
            {
                if (phrase.EndsWith("E") || phrase.EndsWith("T") || phrase.EndsWith("M")) q++;
                phrase = phrase.Remove(phrase.Length - 1, 1);
            }
            return q;
        }


        private Wrap Calculation(string phrase, Wrap wrap)
        {
            if (Operands(phrase) < 2)
                if (phrase.Contains("-") && phrase.Contains("M"))
                {
                    var tmp = Convert.ToInt32(wrap.Numbers.Pop());
                    tmp *= -1;
                    wrap.Numbers.Push(tmp.ToString());
                    return wrap;
                }
                else return wrap;
            if (phrase.Contains('*'))
            {
                double b = double.Parse(wrap.Numbers.Pop());
                double a = double.Parse(wrap.Numbers.Pop());
                double tmp = a * b;
                wrap.Numbers.Push(tmp.ToString());
            }

            else if (phrase.Contains('/'))
            {
                double b = double.Parse(wrap.Numbers.Pop());
                double a = double.Parse(wrap.Numbers.Pop());
                double tmp = a / b;
                wrap.Numbers.Push(tmp.ToString());
            }

            else if (phrase.Contains('-'))
            {
                double b = double.Parse(wrap.Numbers.Pop());
                double a = double.Parse(wrap.Numbers.Pop());
                double tmp = a - b;
                wrap.Numbers.Push(tmp.ToString());
            }

            else if (phrase.Contains('+'))
            {
                double b = double.Parse(wrap.Numbers.Pop());
                double a = double.Parse(wrap.Numbers.Pop());
                double tmp = a + b;
                wrap.Numbers.Push(tmp.ToString());
            }

            return wrap;
        }

    }
}