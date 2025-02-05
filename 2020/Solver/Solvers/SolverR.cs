using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2020.Input;

namespace AdventOfCode2020.Solvers
{
    public class SolverR : Solver
    {
        private readonly IEnumerable<string>[] tokens;

        public SolverR(PuzzleInput input)
        {
            tokens = input.Lines
                .Select(l => new Tokeniser(l))
                .Select(t => t.Tokens)
                .ToArray();
        }

        protected override string SolvePart1() => Solve(new Parser());

        protected override string SolvePart2() => Solve(new ParserWithPrecedence());

        protected string Solve(Parser parser) =>
            tokens.Select(t => parser.Parse(t)).Select(p => p.Evaluate()).Sum().ToString();

        public class Tokeniser
        {
            private static readonly Regex pattern = new Regex(@"^(?:(?<token>\(|\)|\d+|\*|\+)\s?)+$");

            public Tokeniser(string input)
            {
                Tokens = pattern.Match(input.Trim()).Groups["token"].Captures.Select(c => c.Value);
            }

            public IEnumerable<string> Tokens { get; }
        }

        public class Parser
        {
            public Expression Parse(IEnumerable<string> tokens) => ParseExpression(new Queue<string>(tokens));

            public virtual Expression ParseExpression(Queue<string> tokens)
            {
                var left = ParseOperand(tokens);

                while (tokens.TryPeek(out _))
                {
                    var operation = ParseOperation(tokens);
                    var right = ParseOperand(tokens);

                    left = new BinaryExpression(left, operation, right);

                    if (tokens.TryPeek(out var token) && token == ")")
                    {
                        tokens.Dequeue();
                        break;
                    }
                }

                return left;
            }

            public Operation ParseOperation(Queue<string> tokens)
            {
                var token = tokens.Dequeue();

                return token switch
                {
                    "+" => Operation.Addition,
                    "*" => Operation.Multiplication,
                    _ => throw new FormatException($"Expected an operator ('+' or '*') but found {token}.")
                };
            }

            public Expression ParseOperand(Queue<string> tokens)
            {
                var token = tokens.Dequeue();
                if (token == "(")
                {
                    return ParseExpression(tokens);
                }

                if (long.TryParse(token, out var value))
                {
                    return new ConstantExpression(value);
                }

                throw new FormatException($"Expected a number or left paren, but found {token}.");
            }
        }

        public class ParserWithPrecedence : Parser
        {
            public override Expression ParseExpression(Queue<string> tokens)
            {
                var operands = new Stack<Expression>();
                operands.Push(ParseOperand(tokens));

                while (tokens.Count > 0)
                {
                    var operation = ParseOperation(tokens);
                    var right = ParseOperand(tokens);

                    operands.Push(operation switch
                    {
                        Addition => new BinaryExpression(operands.Pop(), operation, right),
                        _ => right
                    });

                    if (tokens.TryPeek(out var token) && token == ")")
                    {
                        tokens.Dequeue();
                        break;
                    }
                }

                var expression = operands.Pop();

                while (operands.Count > 0)
                {
                    expression = new BinaryExpression(operands.Pop(), Operation.Multiplication, expression);
                }

                return expression;
            }
        }

        public abstract class Expression
        {
            public abstract long Evaluate();
        }

        public class ConstantExpression : Expression
        {
            private readonly long value;

            public ConstantExpression(long value)
            {
                this.value = value;
            }

            public override long Evaluate() => value;
        }

        public class BinaryExpression : Expression
        {
            private readonly Expression left;
            private readonly Operation operation;
            private readonly Expression right;

            public BinaryExpression(Expression left, Operation operation, Expression right)
            {
                this.left = left;
                this.operation = operation;
                this.right = right;
            }

            public override long Evaluate() => operation.Apply(left.Evaluate(), right.Evaluate());
        }

        public abstract class Operation
        {
            public static readonly Operation Addition = new Addition();
            public static readonly Operation Multiplication = new Multiplication();

            public abstract long Apply(long x, long y);
        }

        public class Addition : Operation
        {
            public override long Apply(long x, long y) => x + y;
        }

        public class Multiplication : Operation
        {
            public override long Apply(long x, long y) => x * y;
        }
    }
}
