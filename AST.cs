using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace TimeCalcREFACTORED
{
    public class ASTNode { }

    public class OperatorNode(ASTNode left, char op, ASTNode right) : ASTNode
    {
        public readonly ASTNode Left = left;
        public readonly char Operator = op;
        public readonly ASTNode Right = right;
    }

    /// <summary>
    /// Листовой узел AST
    /// </summary>
    public class OperandNode(object value) : ASTNode
    {
        public readonly object Value = value;
    }

    /// <summary>
    /// Abstract Syntax Tree
    /// </summary>
    public class AST
    {
        private Stack<Token> _tokens;
        public readonly ASTNode Root;

        /// <summary>
        /// Конструктор AST
        /// </summary>
        /// <param name="tokens"></param>
        /// <exception cref="FormatException"></exception>
        public AST(Stack<Token> tokens)
        {
            _tokens = tokens;
            //if (!ParenthesesChecker.CheckParentheses(tokens.ToList()))
            //{
            //    throw new FormatException("Скобки не сбалансированы. Возможно, вы пропустили закрывающую скобку или добавили лишнюю открывающую.");
            //}
            if (tokens.Count == 0)
            {
                throw new FormatException("Введите выражение!");
            }
            Root = ParseExpr();
            if (_tokens.Count != 0)
            {
                throw new FormatException("Не для всех открывающих скобок '(' есть соответствующие закрывающие ')'");
            }
        }

        /// <summary>
        /// Выделение термов и операции + -
        /// </summary>
        /// <returns></returns>
        private ASTNode ParseExpr()
        {
            var node = ParseTerm();

            while (
                _tokens.Count != 0 &&
                _tokens.Peek().Value is char tokenVal &&
                (tokenVal == '+' || tokenVal == '-')
                )
            {
                _tokens.Pop();
                node = new OperatorNode(ParseTerm(), tokenVal, node);
            }

            return node;
        }

        /// <summary>
        /// Выделение факторов и операции * /
        /// </summary>
        /// <returns></returns>
        private ASTNode ParseTerm()
        {
            if (_tokens.Count == 0)
            {
                throw new FormatException("ПЕРЕД '+','*','/' должен стоять операнд");
            }

            var node = ParseFactor();

            while (
                _tokens.Count != 0 &&
                _tokens.Peek().Value is char tokenVal &&
                (tokenVal == '*' || tokenVal == '/')
                )
            {
                _tokens.Pop();
                node = new OperatorNode(ParseTerm(), tokenVal, node);
            }

            return node;
        }

        /// <summary>
        /// Выделение операндов и выражений под скобками
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        private ASTNode ParseFactor()
        {
            ASTNode node;

            Token token = _tokens.Pop();


            if (token.Type == TokenType.Operand)
            {
                node = new OperandNode(token.Value);
            }
            else if (token.Value is char currentChar)
            {
                if (currentChar == ')')
                {
                    node = ParseExpr();

                    if (_tokens.Count == 0)
                        throw new FormatException("Не для всех закрывающих скобок ')' есть соответствующие открывающие '('");

                    token = _tokens.Pop();

                    if (!(token.Value is char nextChar && nextChar == '('))
                    {
                        throw new FormatException("Не для всех закрывающих скобок ')' есть соответствующие открывающие '('");
                    }
                }
                else if (currentChar == '(')
                {
                    throw new FormatException("После открывающей скобки '(' ожидался операнд");
                }
                else
                {
                    throw new FormatException($"После оператора '{currentChar}' должен быть операнд");
                }
            }
            else
            {
                throw new FormatException($"Неизвестный тип токена: {token.Type}");
            }

            return node;
        }
    }
}
