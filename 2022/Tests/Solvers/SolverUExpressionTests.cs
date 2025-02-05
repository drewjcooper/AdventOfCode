using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using static AdventOfCode2022.Solvers.SolverU;

namespace AdventOfCode2022.Solvers;

public class SolverUExpressionTests
{
    [Theory]
    [MemberData(nameof(ExpressionEvaluationTestCases))]
    public void Evaluate_GivesExpectedResult(IExpression expression, long expected)
    {
        var result = expression.Evaluate();

        result.Should().Be(expected);
    }

    // [Theory]
    // [MemberData(nameof(ExpressionReductionTestCases))]
    // public void Reduce_GivesExpectedResult(IExpression expression, IExpression expected)
    // {
    //     var result = expression.Reduce();

    //     result.Should().BeEquivalentTo(expected, opts => opts.IncludingAllRuntimeProperties());
    // }

    [Theory]
    [MemberData(nameof(ExpressionAdditionTestCases))]
    public void Addition_GivesExpectedResult(IExpression x, IExpression y, IExpression expected)
    {
        var result = x + y;

        result.Should().BeEquivalentTo(expected, opts => opts.IncludingAllRuntimeProperties());
    }

    [Theory]
    [MemberData(nameof(ExpressionSubtractionTestCases))]
    public void Subtraction_GivesExpectedResult(IExpression x, IExpression y, IExpression expected)
    {
        var result = x - y;

        result.Should().BeEquivalentTo(expected, opts => opts.IncludingAllRuntimeProperties());
    }

    [Theory]
    [MemberData(nameof(ExpressionMultiplicationTestCases))]
    public void Multiplication_GivesExpectedResult(IExpression x, IExpression y, IExpression expected)
    {
        var result = x * y;

        result.Should().BeEquivalentTo(expected, opts => opts.IncludingAllRuntimeProperties());
    }

    [Theory]
    [MemberData(nameof(SolverTestCases))]
    public void Solve_GivesExpectedAnswer(IExpression expression, long expected)
    {
        var result = expression.Solve(V(0)).Evaluate();

        result.Should().Be(expected);
    }

    public static TheoryData<IExpression, long> ExpressionEvaluationTestCases =>
        new()
        {
            { V(5), 5 },
            { A(V(3), V(7)), 10 },
            { S(V(3), V(7)), -4 },
            { M(V(3), V(7)), 21 },
            { D(V(42), V(7)), 6 }
        };

    public static TheoryData<IExpression, IExpression> ExpressionReductionTestCases =>
        new()
        {
            { V(5), V(5) },
            { A(V(3), V(7)), V(10) },
            { S(V(3), V(7)), V(-4) },
            { M(V(3), V(7)), V(21) },
            { D(V(42), V(7)), V(6) },
            { D(V(3), V(5)), F(3, 5) },
            { M(V(5), A(V(4), V(3))), V(35) },
            { M(A(V(5), V(4)), V(3)), V(27) },
            { D(V(42), A(V(4), V(3))), V(6) },
            { D(A(V(5), V(7)), V(3)), V(4) },
            { D(V(43), A(V(4), V(3))), F(43, 7) },
            { D(A(V(5), V(8)), V(3)), F(13, 3) },
            { A(V(42), M(V(4), V(3))), V(54) },
            { A(D(V(21), V(7)), V(3)), V(6) },
            { S(V(42), M(V(4), V(3))), V(30) },
            { S(D(V(21), V(7)), V(3)), V(0) },
            { A(V(3), H()), A(V(3), H()) },
            { A(V(0), H()), H() },
            { A(H(), V(0)), H() },
            { S(V(3), H()), S(V(3), H()) },
            { M(V(3), H()), M(V(3), H()) },
            { D(V(42), H()), D(V(42), H()) },
            { A(H(), V(7)), A(V(7), H()) },
            { S(H(), V(7)), A(V(-7), H()) },
            { M(H(), V(7)), M(V(7), H()) },
            { D(H(), V(7)), M(F(1, 7), H()) },
            { A(V(5), A(V(4), H())), A(V(9), H()) },
            { A(A(V(4), H()), V(5)), A(V(9), H()) },
            { A(V(5), A(H(), V(4))), A(V(9), H()) },
            { A(A(H(), V(4)), V(5)), A(V(9), H()) },
            { A(V(5), S(V(4), H())), S(V(9), H()) },
            { A(S(V(4), H()), V(5)), S(V(9), H()) },
            { A(V(5), S(H(), V(4))), A(V(1), H()) },
            { A(S(H(), V(4)), V(5)), A(V(1), H()) },
            { A(V(5), M(V(4), H())), A(V(5), M(V(4), H())) },
            { A(M(V(4), H()), V(5)), A(V(5), M(V(4), H())) },
            { A(V(5), M(H(), V(4))), A(V(5), M(V(4), H())) },
            { A(M(H(), V(4)), V(5)), A(V(5), M(V(4), H())) },
            { A(V(5), D(V(4), H())), A(V(5), D(V(4), H())) },
            { A(D(V(4), H()), V(5)), A(V(5), D(V(4), H())) },
            { A(V(5), D(H(), V(4))), A(V(5), M(F(1, 4), H())) },
            { A(D(H(), V(4)), V(5)), A(V(5), M(F(1, 4), H())) },
            { A(F(2, 5), F(3, 7)), F(29, 35) },
            { A(F(1, 3), F(1, 6)), F(1, 2) },
            { S(F(2, 5), F(3, 7)), F(-1, 35) },
            { S(F(1, 3), F(1, 6)), F(1, 6) },
            { M(F(2, 5), F(3, 7)), F(6, 35) },
            { M(F(1, 3), F(1, 6)), F(1, 18) }
        };

    public static TheoryData<IExpression, IExpression, IExpression> ExpressionAdditionTestCases =>
        new()
        {
            { V(5), H(), A(V(5), H()) },
            { H(), V(3), A(V(3), H()) },
            { V(5), M(V(7), H()), A(V(5), M(V(7), H())) },
            { M(V(7), H()), V(3), A(V(3), M(V(7), H())) },
            { V(5), D(V(2), H()), A(V(5), D(V(2), H())) },
            { D(V(2), H()), V(3), A(V(3), D(V(2), H())) },
            { V(5), V(3), V(8) },
            { V(5), F(1, 2), F(11, 2) },
            { F(1, 3), F(1, 6), F(1, 2) },
            { F(1, 3), F(5, 3), V(2) },
            { F(2, 7), F(5, 3), F(41, 21) },
            { V(5), A(V(3), H()), A(V(8), H()) },
            { V(5), A(V(-5), H()), H() },
            { V(5), S(V(3), H()), A(V(8), -H()) },
            { V(5), S(V(-5), H()), -H() },
        };

    public static TheoryData<IExpression, IExpression, IExpression> ExpressionSubtractionTestCases =>
        new()
        {
            { V(5), H(), A(V(5), -H()) },
            { H(), V(3), A(V(-3), H()) },
            { V(5), M(V(7), H()), A(V(5), M(V(-7), H())) },
            { M(V(7), H()), V(3), A(V(-3), M(V(7), H())) },
            { V(5), D(V(2), H()), A(V(5), D(V(-2), H())) },
            { D(V(2), H()), V(3), A(V(-3), D(V(2), H())) },
            { V(5), V(3), V(2) },
            { V(5), F(1, 2), F(9, 2) },
            { F(1, 2), F(1, 6), F(1, 3) },
            { F(1, 3), F(7, 3), V(-2) },
            { F(2, 7), F(5, 3), F(-29, 21) },
            { V(5), A(V(3), H()), A(V(2), -H()) },
            { V(5), A(V(5), H()), -H() },
            { V(5), S(V(3), H()), A(V(2), H()) },
            { V(5), S(V(-5), H()), A(V(10), H()) },
        };

    public static TheoryData<IExpression, IExpression, IExpression> ExpressionMultiplicationTestCases =>
        new()
        {
            { V(1), V(3), V(3) },
            { V(5), V(3), V(15) },
            { V(-4), F(3, 2), V(-6) },
            { F(1, 4), V(14), F(7, 2) },
            { F(2, 3), F(4, 5), F(8, 15) },
            { V(3), A(V(5), H()), A(V(15), M(V(3), H())) },
            { A(V(5), H()), V(3), A(V(15), M(V(3), H())) },
            { V(3), S(V(5), H()), A(V(15), M(V(-3), H())) },
            { S(V(5), H()), V(3), A(V(15), M(V(-3), H())) },
            { V(7), D(V(3), H()), D(V(21), H()) },
            { F(7, 2), D(V(3), H()), D(F(21, 2), H()) },
            { D(V(3), H()), V(7), D(V(21), H()) },
            { V(7), M(V(3), H()), M(V(21), H()) },
            { F(7, 2), M(V(3), H()), M(F(21, 2), H()) },
            { M(V(3), H()), V(7), M(V(21), H()) },
            { M(V(3), H()), F(1, 3), H() },
        };

    public static TheoryData<IExpression, long> SolverTestCases =>
        new()
        {
            { H(), 0 },
            { A(V(5), H()), -5 },
            { A(H(), V(-7)), 7 },
            { S(V(6), H()), 6 },
            { S(H(), V(-7)), -7 },
            
        };

    private static HumanExpression H() => new(1);
    private static ValueExpression V(long value) => new(value);
    private static FractionExpression F(long numerator, long denominator) => new(numerator, denominator);
    private static AdditionExpression A(IExpression left, IExpression right) => new(left, right);
    private static SubtractionExpression S(IExpression left, IExpression right) => new(left, right);
    private static MultiplicationExpression M(IExpression left, IExpression right) => new(left, right);
    private static DivisionExpression D(IExpression left, IExpression right) => new(left, right);
}