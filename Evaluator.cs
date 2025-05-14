using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeCalcREFACTORED
{
    /// <summary>
    /// Класс для вычисления выражения по AST
    /// </summary>
    public class Evaluator
    {
        /// <summary>
        /// Вычислить значение выражения по AST
        /// </summary>
        /// <param name="node">Корень AST</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static object Evaluate(ASTNode node)
        {
            return node switch
            {
                OperandNode operand => operand.Value,
                OperatorNode oper => EvaluateOperator(oper),
                _ => throw new InvalidOperationException("Неизвестный узел")
            };
        }

        /// <summary>
        /// Обработка узла оператора
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static object EvaluateOperator(OperatorNode node)
        {
            dynamic left = Evaluate(node.Left);
            dynamic right = Evaluate(node.Right);

            return node.Operator switch
            {
                '+' => TryAdd(left, right),
                '-' => TrySub(left, right),
                '*' => TryMul(left, right),
                '/' => TryDiv(left, right),
                _ => throw new InvalidOperationException($"Неизвестный оператор: {node.Operator}")
            };
        }

        private static dynamic TrySub(dynamic left, dynamic right)
        {
            dynamic result;
            result = left - right;

            if (left is int && right is int)
            {
                if (left > 0 && right < 0)
                {
                    if (result < 0)
                        throw new InvalidOperationException("Числа слишком большие для '-'");
                }
                else if (left < 0 && right > 0)
                {
                    if (result > 0)
                        throw new InvalidOperationException("Числа слишком большие для '-'");
                }
            }

            return result;
        }

        private static dynamic TryAdd(dynamic left, dynamic right)
        {
            dynamic result;
            result = left + right;
            
            if (left is int && right is int)
            {
                if (left > 0 && right > 0)
                {
                    if (result < 0)
                        throw new InvalidOperationException("Числа слишком большие для '+'");
                }
                else if (left < 0 && right < 0)
                {
                    if (result > 0)
                        throw new InvalidOperationException("Числа слишком большие для '+'");
                }
                else if (result > 0)
                {
                    throw new InvalidOperationException("Числа слишком большие для '+'");
                }
            }

            return result;
        }


        /// <summary>
        /// Пытается умножить операнды
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>left*right</returns>
        /// <exception cref="InvalidOperationException">Time * Time</exception>
        private static dynamic TryMul(dynamic left, dynamic right)
        {
            dynamic result;
            try 
            {
                result = left * right;
            }
            catch
            {
                throw new InvalidOperationException($"Умножение {left} на {right} невозможно");
            }

            if (left is int intLeft && right is int intRight)
            {
                long bigLeft = (long)intLeft;
                long bigRight = (long)intRight;
                long bigResult = bigLeft * bigRight;
                if (bigResult != result)
                {
                    throw new InvalidOperationException("Числа слишком большие для '*'");
                }
            }

            return result;
        }

        /// <summary>
        /// Пытается поделить операнды
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        /// <exception cref="DivideByZeroException">Деление на 0</exception>
        /// <exception cref="InvalidOperationException">int / Time</exception>
        private static dynamic TryDiv(dynamic left, dynamic right)
        {
            try 
            {
                return left / right;
            }
            catch (DivideByZeroException)
            {
                throw new DivideByZeroException("Деление на ноль");
            }
            catch
            {
                throw new InvalidOperationException("Деление числа на время");
            }
        }
    }
}
