using AdventOfCode.Solvers;
using FluentAssertions;
using Xunit;

namespace AdventOfCode.Tests.Solvers;

public class SolverUTests
{
    [Theory]
    [InlineData(2, "26", 40)]
    [InlineData(2, "35", 33)]
    [InlineData(2, "53", 39)]
    [InlineData(2, "62", 34)]
    [InlineData(3, "26", 102)]
    [InlineData(3, "35", 83)]
    [InlineData(3, "53", 99)]
    [InlineData(3, "62", 84)]
    [InlineData(2, "029A", 68)]
    [InlineData(2, "980A", 60)]
    [InlineData(2, "179A", 68)]
    [InlineData(2, "456A", 64)]
    [InlineData(2, "379A", 64)]
    public void GetButtonCount_GeneratesMinimumMovesOverThreeRobots(int robotCount, string code, int expectedMinimumLength)
    {
        var result = SolverU.GetButtonCount(code, SolverU.CreateRobotChain(robotCount));

        result.Should().Be(expectedMinimumLength);
    }

    [Theory]
    [InlineData(2, "26", 40)]
    [InlineData(2, "35", 33)]
    [InlineData(2, "53", 39)]
    [InlineData(2, "62", 34)]
    [InlineData(3, "26", 102)]
    [InlineData(3, "35", 83)]
    [InlineData(3, "53", 99)]
    [InlineData(3, "62", 84)]
    [InlineData(4, "26", 252)]
    [InlineData(4, "35", 209)]
    [InlineData(4, "53", 247)]
    [InlineData(4, "62", 210)]
    [InlineData(2, "029A", 68)]
    [InlineData(2, "980A", 60)]
    [InlineData(2, "179A", 68)]
    [InlineData(2, "456A", 64)]
    [InlineData(2, "379A", 64)]
    public void GetButtonPresses_GeneratesMinimumMovesOverThreeRobots(int robotCount, string code, int expectedMinimumLength)
    {
        var robot = SolverU.CreateRobotChain(robotCount);

        var result = robot.GetButtonPresses(code).Select(p => p.Length).Distinct();

        result.Should().HaveCount(1).And.Contain(expectedMinimumLength);
    }
}