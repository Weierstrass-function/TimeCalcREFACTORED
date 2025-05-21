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
            if (tokens.Count == 0)
                throw new FormatException("Введите выражение");

            _tokens = tokens;
            Root = ParseExpr();
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
                node = new OperatorNode(ParseExpr(), tokenVal, node);
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
                (_tokens.Peek().Value is char tokenVal &&
                (tokenVal != '+' && tokenVal != '-' && tokenVal != '(') ||
                _tokens.Peek().Type == TokenType.Operand
                )
                )
            {
                if (_tokens.Peek().Value is char tokenVal1 &&
                    (tokenVal1 == '*' || tokenVal1 == '/'))
                {

                    _tokens.Pop();
                    node = new OperatorNode(ParseTerm(), tokenVal1, node);
                }
                else
                {
                    node = new OperatorNode(ParseTerm(), '*', node);
                }

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
                    token = _tokens.Pop(); // удаление (
                }
                else
                {
                    throw new FormatException($"После '{currentChar}' должен быть операнд");
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
