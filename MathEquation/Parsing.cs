using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MathEquation
{
    /// <summary>
    /// Единственный класс, отвечающий за парсинг выражений.
    /// </summary>
    class Parsing
    {
        //список операторов
        public List<string> Operators { get; set; }
        //Выполняемые действия
        public Dictionary<string, Func<double,double,double>> Actions { get; set; }
        //Математические функции
        public Dictionary<string, Func<double, double>> Functions { get; set; }
        //Математические константы
        public Dictionary<string, double> MathConst { get; set; }
        
        public Parsing()
        {
            Operators = new List<string>()
            {
                "^",
                "*",
                "/",
                ":",
                "+",
                "-"
            };

            Actions = new Dictionary<string, Func<double, double, double>>()
            {
                ["^"] = Math.Pow,
                ["*"] = (x, y) => x * y,
                ["/"] = (x, y) => x / y,
                [":"] = (x, y) => x / y,
                ["+"] = (x, y) => x + y,
                ["-"] = (x, y) => x - y,
            };

            Functions = new Dictionary<string, Func<double, double>>()
            {
                ["abs"] = enterdata => Math.Abs(enterdata),

                ["cos"] = enterdata => Math.Cos(enterdata),
                ["cosh"] = enterdata => Math.Cosh(enterdata),
                ["acos"] = enterdata => Math.Acos(enterdata),
                ["arccos"] = enterdata => Math.Acos(enterdata),

                ["sin"] = enterdata => Math.Sin(enterdata),
                ["sinh"] = enterdata => Math.Sinh(enterdata),
                ["asin"] = enterdata => Math.Asin(enterdata),
                ["arcsin"] = enterdata => Math.Asin(enterdata),

                ["tan"] = enterdata => Math.Tan(enterdata),
                ["tanh"] = enterdata => Math.Tanh(enterdata),
                ["atan"] = enterdata => Math.Atan(enterdata),
                ["arctan"] = enterdata => Math.Atan(enterdata),

                ["sqrt"] = enterdata => Math.Sqrt(enterdata),

                ["sign"] = enterdata => Math.Sign(enterdata),
                ["exp"] = enterdata => Math.Exp(enterdata),

                ["floor"] = enterdata => Math.Floor(enterdata),
                ["ceil"] = enterdata => Math.Ceiling(enterdata),
                ["ceiling"] = enterdata => Math.Ceiling(enterdata),
                ["round"] = enterdata => Math.Round(enterdata),

                ["ln"] = enterdata => Math.Log10(enterdata)
            };

            MathConst = new Dictionary<string, double>()
            {
                ["pi"] = 3.14159265358979,
                ["e"] = 2.71828182845905,
                ["fi"] = 1.61803398874989,

            };
        }

        public double Parse(string expression, string valuable)
        {
            if (string.IsNullOrEmpty(expression))
                throw new Exception("Пожалуйста, введите выражение");
            else
                return CalculateLogic(GetLexems(expression, valuable));
        }

        //нижележащий метод нормализирует выражение, введеное пользователем

        private string StringCorrection(string expression)
        {
            expression = expression.Replace(" ", string.Empty);
            expression = expression.Replace("+-", "-");         //замена повторяющихся знаков
            expression = expression.Replace("++", "+");
            expression = expression.Replace("--", "+");

            expression = Regex.Replace(expression, "\\b(sqr|sqrt)\\b", "sqrt",
                RegexOptions.IgnoreCase);
            expression = Regex.Replace(expression, "\\b(tang|tn)\\b", "tan",
                RegexOptions.IgnoreCase);
            expression = Regex.Replace(expression, "\\b(sinus|sn)\\b", "sin",
                RegexOptions.IgnoreCase);
            expression = Regex.Replace(expression, "\\b(cosinus|cs)\\b", "cos",
                RegexOptions.IgnoreCase);
            return expression;

        }
        #region Main
        //в данном методе разбираем заданное пользователем выражение на отдельные лексемы (числа и операторы)
        private List<string> GetLexems(string enterexp, string valuable) 
        {
            string expression = StringCorrection(enterexp);       //форматируем выражение
            var lexem = "";
            var lexems = new List<string>();    //здесь хранится итоговая коллекция лексем

            for (int i = 0; i < expression.Length; ++i)
            {
                var symbol = expression[i];

                if (char.IsDigit(symbol))
                {
                    lexem += symbol;

                    //нижестоящий цикл проходит до конца числа (на тот случай, если число n-значное или дробное)

                    while (i + 1 < expression.Length && (char.IsDigit(expression[i + 1]) || expression[i + 1] == '.'))
                        lexem += expression[++i];
                    lexems.Add(lexem);
                    lexem = "";
                    continue;
                }

                if (char.IsLetter(symbol))           
                {
                    //не очень хороший код, позволяющий отличить константу "е" от переменной
                    if ((i != expression.Length - 1 && symbol == 'e' && !char.IsLetter(expression[i + 1])) || ((i == expression.Length - 1) && (symbol == 'e')))
                    {
                        lexem += symbol;
                        lexems.Add(lexem);
                        continue;
                    }
                    //данный участок кода распознает переменную
                    else if (i == 0 && !char.IsLetter(expression[i + 1]) || (i == expression.Length - 1 && !char.IsLetter(expression[i - 1])) || (!char.IsLetter(expression[i + 1]) && !char.IsLetter(expression[i - 1])))
                    {
                        if (string.IsNullOrEmpty(valuable))
                            throw new ArgumentException("Введите значение переменной");
                        if (i != 0 && (char.IsDigit(expression[i - 1]) || expression[i - 1] == ')')) //если умножение не указано явно
                            lexems.Add("*");
                        lexems.Add(valuable);
                        continue;
                    }
                    //распознавание других функций и констант
                    else
                    {
                        if (i != 0 && (char.IsDigit(expression[i - 1]) || expression[i - 1] == ')'))
                            lexems.Add("*");
                        lexem += symbol;
                        while (i + 1 < expression.Length && char.IsLetter(expression[i + 1]))
                            lexem += expression[++i];
                        lexems.Add(lexem);
                        lexem = "";
                        continue;
                    }
                        

                }

                /*при соблюдении нижнестоящегоусловия будет создаваться лексема, 
                например (-5) вместо (-) и (5), если + или - стоят в начале предложения или перед скобками*/

                if (i + 1 < expression.Length && (symbol == '-' || symbol == '+') && char.IsDigit(expression[i + 1])
                    && (i == 0 || Operators.IndexOf(expression[i - 1].ToString()) != -1 ||
                     i - 1 > 0 && expression[i - 1] == '('))
                {
                    lexem += symbol;
                    while (i + 1 < expression.Length && (char.IsDigit(expression[i + 1]) || expression[i + 1] == '.'))
                        lexem += expression[++i];
                    lexems.Add(lexem);
                    lexem = "";
                    continue;
                }

                if (symbol == '(')
                {
                    if (i != 0 && (char.IsDigit(expression[i - 1]) || symbol == ')'))
                    {
                        lexems.Add("*");
                        lexems.Add("(");
                    }
                    else
                        lexems.Add("(");
                }
                else
                    lexems.Add(symbol.ToString());
            }
            return lexems;
        }

        private double CalculateLogic(List<string> lexems)          //обработка лексем, в частности значений в скобках
        {
            while (lexems.IndexOf("(") != -1)
            {
                var startBracket = lexems.LastIndexOf("(");                 //открывающая скобка    
                var endBracket = lexems.IndexOf(")", startBracket);         //закрывающая
                if (startBracket >= endBracket)
                    throw new ArithmeticException("Нет закрывающей скобки");
                List<string> bracketData = new List<string>();
                for (int i = startBracket + 1; i < endBracket; ++i)         //добавляем все значения и операции внутри скобок
                    bracketData.Add(lexems[i]);
                double resultBracket = CalculateProcess(bracketData);       //вычисляем значение

                lexems[startBracket] = resultBracket.ToString();           //подставляем подсчитанную скобку в коллекию лексем

                lexems.RemoveRange(startBracket + 1, endBracket - startBracket);
            }
            return (CalculateProcess(lexems));      //считаем все остальное
        }

        private List<string> MathConstParse(List<string> lexems)      //замена констант на их значения
        {
            for (int i = 0; i < lexems.Count; ++i)
                if (MathConst.Keys.Contains(lexems[i]))
                    lexems[i] = MathConst[lexems[i]].ToString();
            return MathFunctionParse(lexems);

        }

        private List<string> MathFunctionParse(List<string> lexems)       //замена матфункций на вычисленные значения
        {
            for (int i = 0; i < lexems.Count; ++i)
                if (Functions.Keys.Contains(lexems[i]))
                {
                    double number = double.Parse(lexems[i + 1]);
                    var temp = Functions[lexems[i]](number);
                    lexems[i] = temp.ToString();
                    lexems.RemoveRange(i + 1, 1);

                }
            return lexems;

        }

        //главный вычислительный метод
        private double CalculateProcess(List<string> lexems)
        {
            var newlexems = MathConstParse(lexems);
            foreach (var op in Operators)               //проходим по всем возможным операциям в порядке их приоритета
            {
                while (newlexems.IndexOf(op) != -1)        //выполняем операции до тех пор, пока не останется конечный результ, и возвращаем его
                {
                    var opPlace = newlexems.IndexOf(op);

                    var numberA = double.Parse(newlexems[opPlace - 1]);
                    var numberB = double.Parse(newlexems[opPlace + 1]);

                    var result = Actions[op](numberA, numberB);

                    newlexems[opPlace - 1] = result.ToString();
                    newlexems.RemoveRange(opPlace, 2);
                }
            }
            return double.Parse(newlexems[0]);
        }
        #endregion
    }

}
