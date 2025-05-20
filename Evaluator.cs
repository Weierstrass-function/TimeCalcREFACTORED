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

            checked // ошибки по перепрлнению вкл
            {
                try
                {
                    return node.Operator switch
                    {
                        '+' => left + right,
                        '-' => left - right,
                        '*' => TryMul(left, right),
                        '/' => TryDiv(left, right),
                        _ => throw new InvalidOperationException($"Неизвестный оператор: {node.Operator}")
                    };
                }
                catch (OverflowException)
                {
                    throw new OverflowException($"Переполнение при операции '{node.Operator}'");
                }
            }
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
                throw new InvalidOperationException("Умножение времени на время");
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
