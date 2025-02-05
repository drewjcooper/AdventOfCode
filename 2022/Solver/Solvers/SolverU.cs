using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdventOfCode2022.Helpers;
using AdventOfCode2022.Input;

namespace AdventOfCode2022.Solvers;

public class SolverU : Solver
{
    private readonly Dictionary<string, Monkey> _monkeys;
    private readonly Action<string> _log;

    public SolverU(PuzzleInput input, Action<string> log)
    {
        _monkeys = input.Lines.Select(l => Monkey.Parse(l)).ToDictionary(m => m.Name);
        _log = log;
    }
    
    protected override string SolvePart1() => _monkeys["root"].GetExpression(_monkeys, _log).Evaluate().ToString();

    protected override string SolvePart2()
    {
        var (left, right) = _monkeys["root"].GetExpressions(_monkeys, _log, true);
        var expression = /*new ValueExpression(6125) */ (left - right);

        _log.Invoke($"Left: {left}, Right: {right}, Solving: {expression} = 0");

        return expression.Solve(Zero, _log).Evaluate().ToString();
    }

    private abstract class Monkey
    {
        protected Monkey(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public abstract IExpression GetExpression(Dictionary<string, Monkey> monkeys, Action<string> log, bool withHuman = false);
        public abstract (IExpression, IExpression) GetExpressions(Dictionary<string, Monkey> monkeys, Action<string> log, bool withHuman = true);
            
        public static Monkey Parse(string line)
        {
            var (name, operation) = line.Split(':', StringSplitOptions.TrimEntries);
            var operationParts = operation.Split(' ');

            return operationParts.Length == 1
                ? new ValueMonkey(name, long.Parse(operationParts[0]))
                : new OperationMonkey(name, operationParts[0], operationParts[2], operationParts[1][0]);
        }
    }

    private class ValueMonkey : Monkey
    {
        private long _value;
        
        public ValueMonkey(string name, long value)
            : base (name)
        {
            _value = value;
        }

        public override IExpression GetExpression(Dictionary<string, Monkey> _, Action<string> log, bool withHuman) 
            => Name == "humn" && withHuman ? new HumanExpression(_value) : new ValueExpression(_value);

        public override (IExpression, IExpression) GetExpressions(Dictionary<string, Monkey> monkeys, Action<string> log, bool withHuman)
        {
            throw new NotImplementedException();
        }
    }

    private class OperationMonkey : Monkey
    {
        private readonly string _name1;
        private readonly string _name2;
        private readonly char _operator;
        
        public OperationMonkey(string name, string name1, string name2, char @operator)
            : base (name)
        {
            _name1 = name1;
            _name2 = name2;
            _operator = @operator;
        }

        public override IExpression GetExpression(Dictionary<string, Monkey> monkeys, Action<string> log, bool withHuman)
        {
            var (left, right) = GetExpressions(monkeys, log, withHuman);

            var expression = _operator switch
            {
                '+' => left + right,
                '-' => left - right,
                '*' => left * right,
                '/' => left / right,
                _ => throw new Exception($"Unexpected operator: {_operator}")
            };

            // if (left is not ValueExpression || right is not ValueExpression)
            // {
            //     log.Invoke($"Left: {left}, Op: {_operator}, Right: {right}, Result: {expression}");
            // }

            return expression;
        }
            
        public override (IExpression, IExpression) GetExpressions(Dictionary<string, Monkey> monkeys, Action<string> log, bool withHuman)
            => (monkeys[_name1].GetExpression(monkeys, log, withHuman), monkeys[_name2].GetExpression(monkeys, log, withHuman));
    }

    private static readonly ValueExpression Zero = new(0);
    private static readonly ValueExpression One = new(1);

    public interface IExpression
    {
        long Evaluate();
        IExpression Solve(IExpression targetValue, Action<string> _);

        public static IExpression operator -(IExpression x)
            => x switch 
            {
                ValueExpression v => -v,
                FractionExpression f => -f,
                AdditionExpression a => -a,
                SubtractionExpression s => -s,
                MultiplicationExpression m => -m,
                DivisionExpression d => -d,
                HumanExpression h => -h,
                _ => throw new ArgumentException($"Unexpected expression type {x.GetType().Name}: {x}", nameof(x))
            };

        public static IExpression operator +(IExpression x, IExpression y)
            => x switch
            {
                ValueExpression v => v + y,
                FractionExpression f => f + y,
                AdditionExpression a => a + y,
                SubtractionExpression s => s + y,
                MultiplicationExpression m => m + y,
                DivisionExpression d => d + y,
                HumanExpression h => y + h,
                _ => throw new ArgumentException($"Unexpected expression type {x.GetType().Name}: {x}", nameof(x))
            };

        public static IExpression operator -(IExpression x, IExpression y)
            => x switch
            {
                ValueExpression v => v + -y,
                FractionExpression f => f + -y,
                AdditionExpression a => a + -y,
                SubtractionExpression s => s + -y,
                MultiplicationExpression m => m + -y,
                DivisionExpression d => d + -y,
                HumanExpression h => -y + h,
                _ => throw new ArgumentException($"Unexpected expression type {x.GetType().Name}: {x}", nameof(x))
            };

        public static IExpression operator *(IExpression x, IExpression y)
            => x switch
            {
                ValueExpression v => v * y,
                FractionExpression f => f * y,
                AdditionExpression a => a * y,
                SubtractionExpression s => s * y,
                MultiplicationExpression m => m * y,
                DivisionExpression d => d * y,
                HumanExpression h => y * h,
                _ => throw new ArgumentException($"Unexpected expression type {x.GetType().Name}: {x}", nameof(x))
            };

        public static IExpression operator /(IExpression x, IExpression y)
            => x switch
            {
                ValueExpression v => v / y,
                FractionExpression f => f / y,
                AdditionExpression a => a.Left / y + a.Right / y,
                SubtractionExpression s => s.Left / y + -s.Right / y,
                MultiplicationExpression m => m.Left / y * m.Right,
                DivisionExpression d => d.Left / y / d.Right,
                HumanExpression h => One / y * h,
                _ => throw new ArgumentException($"Unexpected expression type {x.GetType().Name}: {x}", nameof(x))
            };
    }

    public record struct ValueExpression(long Value) : IExpression
    {
        public long Evaluate() => Value;
        public IExpression Solve(IExpression targetValue, Action<string> _) 
            => throw new Exception($"No solution: {Value} = {targetValue}");

        public override string ToString() => Value.ToString();

        public static IExpression operator -(ValueExpression x) => new ValueExpression(-x.Value);

        public static IExpression operator +(ValueExpression x, IExpression y) 
            => x == Zero ? y : y switch
            {
                ValueExpression v => new ValueExpression(x.Value + v.Value),
                FractionExpression f => f + x,
                AdditionExpression (ValueExpression v, IExpression e) => x + v + e,
                AdditionExpression a => x + a.Left + a.Right,
                SubtractionExpression (ValueExpression v, IExpression e) => x + v - e,
                SubtractionExpression (IExpression e, ValueExpression v) => x - v + e,
                _ => new AdditionExpression(x, y)
            };
    
        public static IExpression operator -(ValueExpression x, ValueExpression y) 
            => new ValueExpression(x.Value - y.Value);

        public static IExpression operator *(ValueExpression x, IExpression y) 
            => x == One ? y : y switch
            {
                ValueExpression v => new ValueExpression(x.Value * v.Value),
                FractionExpression f => f * x,
                AdditionExpression a => x * a.Left + x * a.Right,
                SubtractionExpression s => x * s.Left + -x * s.Right,
                MultiplicationExpression m => x * m.Left * m.Right,
                DivisionExpression d => x * d.Left / d.Right,
                _ => new MultiplicationExpression(x, y)
            };
    
        public static IExpression operator /(ValueExpression x, IExpression y) 
            => y switch
            {
                ValueExpression v => new FractionExpression(x.Value, v.Value).Simplify(),
                FractionExpression f => new FractionExpression(x.Value * f.Denominator, f.Numerator).Simplify(),
                MultiplicationExpression m => x / m.Left * m.Right,
                DivisionExpression d => x / d.Left / d.Right,
                _ => new DivisionExpression(x, y)
            };
    
        // public static IExpression operator /(ValueExpression x, ValueExpression y) 
        //     => new FractionExpression(x.Value, y.Value).Reduce();
    }

    public record struct FractionExpression(long Numerator, long Denominator) : IExpression
    {
        public FractionExpression Reciprocal => new FractionExpression(Denominator, Numerator);

        public long Evaluate() => Numerator / Denominator;

        public IExpression Simplify()
        {
            var gcd = GCD(Math.Abs(Numerator), Math.Abs(Denominator));
            if (Numerator < 0 && Denominator < 0) { gcd = -gcd; }

            return gcd == Denominator 
                ? new ValueExpression(Numerator / gcd) 
                : new FractionExpression(Numerator / gcd, Denominator / gcd);

            static long GCD(long a, long b)
            {
                var r = a % b;
                return r == 0 ? b : GCD(b, r);
            } 
        }

        public IExpression Solve(IExpression targetValue, Action<string> _) 
            => throw new Exception($"No solution: {this} = {targetValue}");

        public override string ToString() => $"{Numerator}/{Denominator}";

        public static IExpression operator -(FractionExpression x) 
            => new FractionExpression(-x.Numerator, x.Denominator);

        public static IExpression operator +(FractionExpression x, IExpression y)
            => y switch
            {
                ValueExpression v => x + v,
                FractionExpression f => x + f,
                _ => new AdditionExpression(x, y)
            };
        
        public static IExpression operator +(FractionExpression x, FractionExpression y) 
            => new FractionExpression(
                x.Numerator * y.Denominator + y.Numerator * x.Denominator,
                x.Denominator * y.Denominator).Simplify();

        public static IExpression operator +(FractionExpression x, ValueExpression y) 
            => new FractionExpression(x.Numerator + y.Value * x.Denominator, x.Denominator).Simplify();

        public static IExpression operator *(FractionExpression x, IExpression y)
            => y switch
            {
                ValueExpression v => x * v,
                FractionExpression f => x * f,
                AdditionExpression a => x * a.Left + x * a.Right,
                SubtractionExpression s => x * s.Left + -x * s.Right,
                MultiplicationExpression m => x * m.Left * m.Right,
                DivisionExpression d => x * d.Left / d.Right,
                _ => new MultiplicationExpression(x, y)
            };
        
        public static IExpression operator *(FractionExpression f, ValueExpression v) 
            => new FractionExpression(v.Value * f.Numerator, f.Denominator).Simplify();

        public static IExpression operator *(FractionExpression x, FractionExpression y) 
        {
            var e1 = new FractionExpression(x.Numerator, y.Denominator).Simplify();
            var e2 = new FractionExpression(y.Numerator, x.Denominator).Simplify();

            return (e1, e2) switch
            {
                (FractionExpression f1, FractionExpression f2) => 
                    new FractionExpression(f1.Numerator * f2.Numerator, f1.Denominator * f2.Denominator).Simplify(),
                (ValueExpression v1, ValueExpression v2) => v1 * v2
            };
        }
    
        public static IExpression operator /(FractionExpression x, IExpression y) 
            => y switch
            {
                ValueExpression v => new FractionExpression(x.Numerator, x.Denominator * v.Value).Simplify(),
                FractionExpression f => new FractionExpression(x.Numerator * f.Denominator, x.Denominator * f.Numerator).Simplify(),
                _ => new DivisionExpression(x, y)
            };

        internal FractionExpression Reciprocate() => new(Denominator, Numerator);
    }

    public record struct HumanExpression(long Value) : IExpression
    {
        public long Evaluate() => Value;
        public IExpression Solve(IExpression targetValue, Action<string> _) => targetValue;

        public override string ToString() => Value < 0 ? "-x" : "x";
        
        public static IExpression operator -(HumanExpression x) => new HumanExpression(-x.Value);
    }

    public record struct AdditionExpression(IExpression Left, IExpression Right) : IExpression
    {
        public long Evaluate() => Left.Evaluate() + Right.Evaluate();

        public IExpression Solve(IExpression targetValue, Action<string> log)
        {
            log.Invoke($"Solving: {this} = {targetValue}");
            
            return this switch
            {
                (ValueExpression v, IExpression e) => e.Solve(targetValue - v, log),
                (IExpression e, ValueExpression v) => e.Solve(targetValue - v, log),
                (FractionExpression f, IExpression e) => e.Solve(targetValue - f, log)
            };
        }

        public override string ToString() => $"({Left} + {Right})";

        public static IExpression operator -(AdditionExpression x) => new AdditionExpression(-x.Left, -x.Right);

        public static IExpression operator +(AdditionExpression x, IExpression y)
            => y switch
            {
                ValueExpression v => v + x,
                FractionExpression f => f + x,
                AdditionExpression a => x.Left + a.Left + x.Right + a.Right,
                SubtractionExpression s => x.Left + s.Left + x.Right + -s.Right,
                // MultiplicationExpression m => x.Left * m + x.Right * m,
                // DivisionExpression d => x.Left * d.Left / d.Right + x.Right * d.Left / d.Right,
                _ => new MultiplicationExpression(x, y)
            };
    
        public static IExpression operator -(AdditionExpression x, IExpression y)
            => y switch
            {
                ValueExpression v => x.Left + -v + x.Right,
                FractionExpression f => x.Left + -f + x.Right,
                // AdditionExpression a => x.Left + a.Left + x.Right + a.Right,
                // SubtractionExpression s => x.Left + s.Left + x.Right + -s.Right,
                // MultiplicationExpression m => x.Left * m + x.Right * m,
                // DivisionExpression d => x.Left * d.Left / d.Right + x.Right * d.Left / d.Right,
                _ => new SubtractionExpression(x, y)
            };
    
        public static IExpression operator *(AdditionExpression x, IExpression y)
            => y switch
            {
                ValueExpression v => v * x,
                FractionExpression f => f * x,
                AdditionExpression a => x.Left * a.Left + x.Left * a.Right + x.Right * a.Left + x.Right * a.Right,
                SubtractionExpression s => x.Left * s.Left + x.Left * -s.Right + x.Right * s.Left + x.Right * -s.Right,
                MultiplicationExpression m => x.Left * m.Left * m.Right + x.Right * m.Left * m.Right,
                DivisionExpression d => x.Left * d.Left / d.Right + x.Right * d.Left / d.Right,
                _ => new MultiplicationExpression(x, y)
            };
    }

    public record struct SubtractionExpression(IExpression Left, IExpression Right) : IExpression
    {
        public long Evaluate() => Left.Evaluate() - Right.Evaluate();

        public IExpression Solve(IExpression targetValue, Action<string> log)
        {
            log.Invoke($"Solving: {this} = {targetValue}");
            
            return this switch
            {
                (ValueExpression v, IExpression e) => -e.Solve(targetValue - v, log),
                (IExpression e, ValueExpression v) => e.Solve(targetValue + v, log),
                // (FractionExpression f, IExpression e) => -e.Solve(targetValue - f)
            };
        }

        public override string ToString() => $"({Left} - {Right})";

        public static IExpression operator -(SubtractionExpression x) => new SubtractionExpression(-x.Left, -x.Right);

        public static IExpression operator +(SubtractionExpression x, IExpression y) 
            => y switch
            {
                ValueExpression v => v + x
            };

        public static IExpression operator *(SubtractionExpression x, IExpression y)
            => y switch
            {
                ValueExpression v => v * x,
                FractionExpression f => f * x,
                AdditionExpression a => x.Left * a.Left + x.Left * a.Right + -x.Right * a.Left + -x.Right * a.Right,
                SubtractionExpression s => x.Left * s.Left + x.Left * -s.Right + -x.Right * s.Left + x.Right * s.Right,
                MultiplicationExpression m => x.Left * m.Left * m.Right + -x.Right * m.Left * m.Right,
                DivisionExpression d => x.Left * d.Left / d.Right + -x.Right * d.Left / d.Right,
                _ => new MultiplicationExpression(x, y)
            };

        public static IExpression operator /(SubtractionExpression x, IExpression y)
            => y switch
            {
                ValueExpression v => x.Left / y + -x.Right / y,
                FractionExpression f => x * f.Reciprocal,
                // AdditionExpression a => x.Left * a.Left + x.Left * a.Right + -x.Right * a.Left + -x.Right * a.Right,
                // SubtractionExpression s => x.Left * s.Left + x.Left * -s.Right + -x.Right * s.Left + x.Right * s.Right,
                // MultiplicationExpression m => x.Left * m.Left * m.Right + -x.Right * m.Left * m.Right,
                // DivisionExpression d => x.Left * d.Left / d.Right + -x.Right * d.Left / d.Right,
                _ => new DivisionExpression(x, y)
            };
    }

    public record struct MultiplicationExpression(IExpression Left, IExpression Right) : IExpression
    {
        public long Evaluate() => Left.Evaluate() * Right.Evaluate();

        public IExpression Solve(IExpression targetValue, Action<string> log)
        {
            log.Invoke($"Solving: {this} = {targetValue}");
            
            return this switch
            {
                (ValueExpression v, IExpression e) => e.Solve(targetValue / v, log),
                (FractionExpression f, IExpression e) => e.Solve(targetValue * f.Reciprocal, log)
            };
        }

        public override string ToString() => $"({Left} * {Right})";

        public static IExpression operator -(MultiplicationExpression x) 
            => new MultiplicationExpression(-x.Left, x.Right);

        public static IExpression operator +(MultiplicationExpression x, IExpression y) 
            => y switch
            {
                ValueExpression v => v + x
            };

        public static IExpression operator *(MultiplicationExpression x, IExpression y)
            => y switch
            {
                ValueExpression v => v * x,
                FractionExpression f => f * x,
                AdditionExpression a => a.Left * x + a.Right * x,
                SubtractionExpression s => s.Left * x + s.Right * -x,
                MultiplicationExpression m => x.Left * m.Left * x.Right * m.Right,
                DivisionExpression d => x.Left * d.Left * x.Right / d.Right,
                _ => new MultiplicationExpression(x, y)
            };

        public static IExpression operator /(MultiplicationExpression x, IExpression y)
            => y switch
            {
                ValueExpression v => One / v * x,
                FractionExpression f => f.Reciprocal * x,
                // AdditionExpression a => a.Left * x + a.Right * x,
                // SubtractionExpression s => s.Left * x + s.Right * -x,
                // MultiplicationExpression m => x.Left * m.Left * x.Right * m.Right,
                // DivisionExpression d => x.Left * d.Left * x.Right / d.Right,
                _ => new DivisionExpression(x, y)
            };
    }

    internal record struct DivisionExpression(IExpression Left, IExpression Right) : IExpression
    {
        public long Evaluate() => Left.Evaluate() / Right.Evaluate();

        public IExpression Solve(IExpression targetValue, Action<string> log)
        {
            log.Invoke($"Solving: {this} = {targetValue}");
            
            return this switch
            {
                // (ValueExpression v, IExpression e) => (e * targetValue).Solve(v),
                // (FractionExpression f, IExpression e) => (e * targetValue).Solve(f)
            };
        }

        public override string ToString() => $"({Left} / {Right})";

        public static IExpression operator -(DivisionExpression x) => new DivisionExpression(-x.Left, x.Right);

        public static IExpression operator +(DivisionExpression x, IExpression y) 
            => y switch
            {
                ValueExpression v => v + x
            };

        public static IExpression operator *(DivisionExpression x, IExpression y)
            => y switch
            {
                ValueExpression v => v * x,
                FractionExpression f => f * x,
                AdditionExpression a => a.Left * x + a.Right * x,
                SubtractionExpression s => s.Left * x + s.Right * -x,
                MultiplicationExpression m => x.Left * m.Left * m.Right / x.Right,
                DivisionExpression d => x.Left * d.Left / x.Right / d.Right,
                _ => new DivisionExpression(x, y)
            };

        public static IExpression operator /(DivisionExpression x, IExpression y)
            => y switch
            {
                ValueExpression v => One / v * x,
                FractionExpression f => f.Reciprocal * x,
                // AdditionExpression a => a.Left * x + a.Right * x,
                // SubtractionExpression s => s.Left * x + s.Right * -x,
                // MultiplicationExpression m => x.Left * m.Left * x.Right * m.Right,
                // DivisionExpression d => x.Left * d.Left * x.Right / d.Right,
                _ => new DivisionExpression(x, y)
            };
    }
}

internal static class IExpressionExtensions
{
    internal static void Deconstruct(
        this SolverU.IExpression expression,
        out SolverU.IExpression left,
        out SolverU.IExpression right)
    {
        (left, right) = expression switch
        {
            SolverU.AdditionExpression expr => (expr.Left, expr.Right),
            SolverU.SubtractionExpression expr => (expr.Left, expr.Right),
            SolverU.DivisionExpression expr => (expr.Left, expr.Right),
            SolverU.MultiplicationExpression expr => (expr.Left, expr.Right),
            _ => throw new Exception($"Can't deconstruct '{expression}'")
        };
    }
}