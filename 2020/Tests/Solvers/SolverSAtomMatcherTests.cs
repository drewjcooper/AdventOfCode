using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

using static AdventOfCode2020.Solvers.SolverS;

namespace AdventOfCode2020.Solvers
{
    public class SolverSAtomMatcherTests
    {
        [Theory]
        [InlineData('a', "abgcabda", 7)]
        [InlineData('a', "abgcabda", 4)]
        [InlineData('a', "abgcabda", 0)]
        [InlineData('b', "abgcabda", 1)]
        [InlineData('b', "abgcabda", 5)]
        [InlineData('c', "abgcabda", 3)]
        public void Match_IndexedCharMatches_ReturnsTrue(char ch, string input, int index)
        {
            var sut = new AtomMatcher(ch);

            var result = sut.Match(input, ref index);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData('a', "abgcabda", 2)]
        [InlineData('a', "abgcabda", 3)]
        [InlineData('a', "abgcabda", 6)]
        [InlineData('b', "abgcabda", 0)]
        [InlineData('b', "abgcabda", 4)]
        [InlineData('c', "abgcabda", 5)]
        [InlineData('c', "abgcabda", 10)]
        public void Match_IndexedCharDoesntMatch_ReturnsFalse(char ch, string input, int index)
        {
            var sut = new AtomMatcher(ch);

            var result = sut.Match(input, ref index);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData('a', "abgcabda", 7)]
        [InlineData('a', "abgcabda", 4)]
        [InlineData('a', "abgcabda", 0)]
        [InlineData('b', "abgcabda", 1)]
        [InlineData('b', "abgcabda", 5)]
        [InlineData('c', "abgcabda", 3)]
        public void Match_IndexedCharMatches_IncrementsIndex(char ch, string input, int index)
        {
            var initial = index;
            var sut = new AtomMatcher(ch);

            sut.Match(input, ref index);

            index.Should().Be(initial + 1);
        }

        [Theory]
        [InlineData('a', "abgcabda", 2)]
        [InlineData('a', "abgcabda", 3)]
        [InlineData('a', "abgcabda", 6)]
        [InlineData('b', "abgcabda", 0)]
        [InlineData('b', "abgcabda", 4)]
        [InlineData('c', "abgcabda", 5)]
        public void Match_IndexedCharDoesntMatch_DoesNotChangeIndex(char ch, string input, int index)
        {
            var initial = index;
            var sut = new AtomMatcher(ch);

            sut.Match(input, ref index);

            index.Should().Be(initial);
        }
    }
}
