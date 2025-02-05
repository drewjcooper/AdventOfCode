using FluentAssertions;
using Xunit;

namespace AdventOfCode2022.Solvers;

public class SolverYSnafuTests
{
    [Theory]
    [InlineData("1=-0-2", 1747)]
    [InlineData("12111", 906)]
    [InlineData("2=0=", 198)]
    [InlineData("21", 11)]
    [InlineData("2=01", 201)]
    [InlineData("111", 31)]
    [InlineData("20012", 1257)]
    [InlineData("112", 32)]
    [InlineData("1=-1=", 353)]
    [InlineData("1-12", 107)]
    [InlineData("12", 7)]
    [InlineData("1=", 3)]
    [InlineData("122", 37)]
    public void Parse_Succeeds(string snafu, long expected)
    {
        long result = SolverY.Snafu.Parse(snafu);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, "1")]
    [InlineData(2, "2")]
    [InlineData(3, "1=")]
    [InlineData(4, "1-")]
    [InlineData(5, "10")]
    [InlineData(6, "11")]
    [InlineData(7, "12")]
    [InlineData(8, "2=")]
    [InlineData(9, "2-")]
    [InlineData(10, "20")]
    [InlineData(15, "1=0")]
    [InlineData(20, "1-0")]
    [InlineData(2022, "1=11-2")]
    [InlineData(12345, "1-0---0")]
    [InlineData(314159265, "1121-1110-1=0")]
    public void ToString_Succeeds(long value, string expected)
    {
        SolverY.Snafu sut = value;

        var result = sut.ToString();

        result.Should().Be(expected);
    }

    [Fact]
    public void Addition_GivesExpectedResult()
    {
        var sut1 = SolverY.Snafu.Parse("1");
        var sut2 = SolverY.Snafu.Parse("2");

        var result = sut1 + sut2;

        ((long)result).Should().Be(3);
    }
}