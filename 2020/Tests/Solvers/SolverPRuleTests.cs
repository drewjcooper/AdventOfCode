using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

using static AdventOfCode2020.Solvers.SolverP;

namespace AdventOfCode2020.Solvers
{
    public class SolverPRuleTests
    {
        [Theory, AutoData]
        public void Parse_CreatesExpectedRule(string name)
        {
            var ranges = new[] { new Range(4, 9), new Range(15, 19) };
            var text = $"{name}: {ranges[0]} or {ranges[1]}";
            var expected = new { Name = name, Ranges = ranges };

            var sut = Rule.Parse(text);

            sut.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MemberData(nameof(OrOperatorTestCases))]
        public void OrOperator_CombinesRules(Rule rule1, Rule rule2, Rule expected)
        {
            var result = rule1 | rule2;

            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(4, true)]
        [InlineData(9, false)]
        [InlineData(13, true)]
        [InlineData(16, true)]
        [InlineData(28, true)]
        [InlineData(30, false)]
        public void IsValid_ReturnTrueIfOneRangeContainsValue(int value, bool expected)
        {
            var rule = new Rule("test", new Range(3, 8), new Range(13, 28));

            var result = rule.IsValid(value);

            result.Should().Be(expected);
        }

        public static TheoryData<Rule, Rule, Rule> OrOperatorTestCases =>
            new TheoryData<Rule, Rule, Rule>
            {
                {
                    new Rule("foo", new Range(1, 4), new Range(12, 19)),
                    new Rule("bar", new Range(6, 10), new Range(21, 25)),
                    new Rule("composite", new Range(1, 4), new Range(6, 10), new Range(12, 19), new Range(21, 25))
                },
                {
                    new Rule("foo", new Range(1, 5), new Range(12, 19)),
                    new Rule("bar", new Range(6, 10), new Range(18, 25)),
                    new Rule("composite", new Range(1, 10), new Range(12, 25))
                },
                {
                    new Rule("foo", new Range(1, 5), new Range(12, 17)),
                    new Rule("bar", new Range(6, 11), new Range(18, 25)),
                    new Rule("composite", new Range(1, 25))
                }
            };
    }
}
