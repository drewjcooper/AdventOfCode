using FluentAssertions;
using Xunit;

using static AdventOfCode2020.Solvers.SolverP;

namespace AdventOfCode2020.Solvers
{
    public class SolverPTicketValidatorTests
    {
        private readonly TicketValidator sut;

        public SolverPTicketValidatorTests()
        {
            sut = new TicketValidator(
                Rule.Parse("class: 1-3 or 5-7"),
                Rule.Parse("class: 6-11 or 33-44"),
                Rule.Parse("class: 13-40 or 45-50"));
        }

        [Theory]
        [InlineData(new[] { 7, 3, 47 }, new int[0])]
        [InlineData(new[] { 40, 4, 50 }, new [] { 4 })]
        [InlineData(new[] { 55, 2, 20, 0 }, new [] { 55, 0 })]
        [InlineData(new[] { 38, 6, 12 }, new [] { 12 })]
        public void GetInvalidValues_ReturnsValuesNotMatchingAnyRules(int[] values, int[] expected)
        {
            var ticket = new Ticket(values);

            var result = sut.GetInvalidValues(ticket);

            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(new[] { 7, 3, 47 }, true)]
        [InlineData(new[] { 40, 4, 50 }, false)]
        [InlineData(new[] { 55, 2, 20, 0 }, false)]
        [InlineData(new[] { 38, 6, 12 }, false)]
        public void IsValid_ReturnsTrueIfAllValuesAreValid(int[] values, bool expected)
        {
            var ticket = new Ticket(values);

            var result = sut.IsValid(ticket);

            result.Should().Be(expected);
        }
    }
}
