using System.Runtime.ExceptionServices;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using static AdventOfCode.Solvers.SolverE;

namespace AdventOfCode.Solvers;

public class SolverETests(ITestOutputHelper testOutputHelper)
{
    [Theory]
    [InlineData(5, 5, 5L, 5L)]
    [InlineData(95, 5, 95L, 5L)]
    [InlineData(10, 5, 40L, 5L)]
    [InlineData(5, 15, 5L, 5L, 40L, 5L, 15L, 5L)]
    [InlineData(12, 50, 15L, 10L, 35L, 25L)]
    public void CategoryMap_Map_MapsAnIntervalCorrectly(int from, int count, params long[] values)
    {
        var interval = Interval.Create(from, count);
        var sut = new CategoryMap(["40 10 5", "40 25 10", "37 60 10"]);
        var expected = Interval.CreateMany(values);

        var result = sut.Map(interval);

        result.Should().BeEquivalentTo(expected);
    }

    // [Theory]
    // [InlineData(79, 81)]
    // [InlineData(14, 14)]
    // [InlineData(55, 57)]
    // [InlineData(13, 13)]
    // public void MapTree_MapFrom_MapsValueCorrectly(int start, int expected)
    // {
    //     var sut = MapTree.Create(["50 98 2", "52 50 48"]);

    //     var result = sut.MapFrom(start);

    //     result.Should().Be(expected);
    // }

    // [Theory]
    // [InlineData(new[] { "40 98 2" }, "52 50 40", new[] { "50 - 89 (+2)", "98 - 99 (-58)" })]
    // [InlineData(new[] { "20 10 5" }, "35 15 15", new[] { "10 - 14 (+30)", "15 - 29 (+20)" })]
    // [InlineData(
    //     new[] { "35 15 15" },
    //     "50 40 5",
    //     new[] { "15 - 19 (+20)", "20 - 24 (+30)", "25 - 29 (+20)", "40 - 44 (+10)" })]
    // [InlineData(
    //     new[] { "20 15 5", "30 25 5" },
    //     "5 10 25",
    //     new[] { "10 - 14 (-5)", "15 - 19 (0)", "20 - 24 (-5)", "25 - 29 (0)", "30 - 34 (-5)" })]
    // public void MapTree_Append_RangeMap_CombinesMaps(string[] tree, string second, string[] expected)
    // {
    //     var sut = MapTree.Create(tree);

    //     var result = sut.Append(new IntervalMap(second));

    //     result.Should().BeEquivalentTo(expected);
    // }

    // [Theory]
    // [InlineData("52 50 48", 60, 75, "50 - 59 (+2)", "60 - 75 (+2)", "76 - 97 (+2)")]
    // [InlineData("52 50 48", 10, 20, null, null, "50 - 97 (+2)")]
    // [InlineData("52 50 48", 110, 120, "50 - 97 (+2)", null, null)]
    // public void RangeMap_Split(string map, int first, int second, string? left, string? middle, string? right)
    // {
    //     var sut = new IntervalMap(map);

    //     var result = sut.Split(first, second);

    //     Stringify(result).Should().Be((left, middle, right));
    // }

    // private static (string?, string?, string?) Stringify((IntervalMap? Left, IntervalMap? Middle, IntervalMap? Right) maps)
    //     => (maps.Left?.ToString(), maps.Middle?.ToString(), maps.Right?.ToString());

    // [Theory]
    // [MemberData(nameof(ExampleTestCase))]
    // public void MapTree_OrdersRangeMapsAscending(IEnumerable<string>[] maps)
    // {
    //     var sut = MapTree.Create(maps[0]);

    //     foreach (var map in maps.Skip(1))
    //     {
    //         sut.Should().BeInAscendingOrder();
    //         testOutputHelper.WriteLine($"Appending {string.Join("; ", map)}");
    //         sut = sut.Append(map.Select(r => new IntervalMap(r)));
    //     }
    // }

    // public static TheoryData<IEnumerable<string>[]> ExampleTestCase =>
    //     new()
    //     {
    //         new string[][]
    //         {
    //             [ "50 98 2", "52 50 48" ],
    //             [ "0 15 37", "37 52 2", "39 0 15" ],
    //             [ "49 53 8", "0 11 42", "42 0 7", "57 7 4" ],
    //             [ "88 18 7", "18 25 70" ],
    //             [ "45 77 23", "81 45 19", "68 64 13" ],
    //             [ "0 69 1", "1 0 69" ],
    //             [ "60 56 37", "56 93 4" ]
    //         }
    //     };
}