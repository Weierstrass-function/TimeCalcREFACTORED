using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeCalcREFACTORED
{
    public enum TokenType
    {
        Symbol,
        Operand
    }

    /// <summary>
    /// Токен
    /// Хранит тип и объект
    /// </summary>
    public class Token
    {
        public TokenType Type { get; }
        public object Value { get; }

        private Token(TokenType type, object value)
        {
            Type = type;
            Value = value;
        }

        public static Token CreateSymbol(char value)
        {
            return new Token(TokenType.Symbol, value);
        }

        public static Token CreateOperand(object value)
        {
            return new Token(TokenType.Operand, value);
        }
    }

    /// <summary>
    /// Лексический анализатор
    /// </summary>
    public class Tokenizer
    {
        private static readonly HashSet<char> _allowedChars = ['+', '-', '*', '/', '(', ')'];

        /// <summary>
        /// Удаляет пустоту и делит все оставшееся на операнды и символы
        /// Операнды: int или Time, в т.ч с унарным минусом
        /// Символы: '+', '-', '*', '/', '(', ')'
        /// </summary>
        /// <param name="input">строка входного выражения</param>
        /// <returns>Стек - выражение в обратном порядке</returns>
        /// <exception cref="FormatException"></exception>
        public static Stack<Token> Tokenize(string input)
        {
            var tokens = new Stack<Token>();

            int i = 0;
            int bracketCounter = 0;
            while (i < input.Length)
            {
                if (char.IsWhiteSpace(input[i]))
                {
                    i++;
                }

                // операнд если:
                else if (
                            char.IsDigit(input[i]) ||

                            (
                                input[i] == '-' && // унарный, если перед ним:
                                (
                                    tokens.Count == 0 || // ничего нет
                                    // любой оператор или '('
                                    (tokens.Peek().Value is char tokenValue &&
                                    tokenValue != ')') // но не ')'
                                )
                            )
                        )
                {
                    if (tokens.TryPeek(out var last) && last.Type == TokenType.Operand)
                         throw new FormatException("Между операндами должны стоять скобки или операторы");

                    int start = i;
                    i++;
                    while (i < input.Length && char.IsDigit(input[i]) ||
                        i < input.Length && input[i] == ':')
                    {
                        i++;
                    }    
                        
                    string numOrTime = input[start..i];

                    if (int.TryParse(numOrTime, out int num))
                    {
                        tokens.Push(Token.CreateOperand(num));
                    }
                    else if (Time.TryParse(numOrTime, out Time time))
                    {
                        tokens.Push(Token.CreateOperand(time));
                    }
                    else
                    {
                        throw new FormatException($"'{numOrTime}' - не является допустимым операндом, допустимые целые числа от {int.MinValue} до {int.MaxValue} или время в виде часы:минуты (допустим унарный минус) где часы и минуты от 0 до {int.MaxValue}");
                    }
                }

                // символ
                else if (_allowedChars.Contains(input[i]))
                {
                    if (input[i] == '(')
                    {
                        bracketCounter++;
                    }
                    else if (input[i] == ')')
                    {
                        bracketCounter--;
                    }

                    if (bracketCounter < 0)
                    {
                        throw new FormatException("Скобка закрылась но не открылась");
                    }

                    tokens.Push(Token.CreateSymbol(input[i]));
                    i++;
                }

                else
                {
                    if (input[i] == ':')
                        throw new FormatException($"справа и слева от ':' должны стоять числа");

                    throw new FormatException($"Недопустимый символ '{input[i]}'");
                }
            }

            if (bracketCounter > 0)
            {
                throw new FormatException("Скобка открылась но не закрылась");
            }

            return tokens;
        }
    }
}
