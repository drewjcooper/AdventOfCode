using AdventOfCode2020.Helpers;
using AdventOfCode2020.Input;
using FluentAssertions;
using Xunit;

namespace AdventOfCode2020.Solvers
{
    public class SolverTests
    {
        [Theory]
        [MemberData(nameof(TestCases))]
        public void Solve_GivesExpectedAnswer(string puzzleId, string input, string expected)
        {
            var id = PuzzleId.Parse(puzzleId);
            var sut = Solver.Get(id, PuzzleInput.From(input));

            var result = sut.Solve(id);

            result.Should().Be(expected);
        }

        public static TheoryData<string, string, string> TestCases =>
            new TheoryData<string, string, string>
            {
                {
"Z1",
"",
"No Solver available"
                },
                {
"A1",
@"1721
979
366
299
675
1456",
"514579"
                },
                {
"A2",
@"1721
979
366
299
675
1456",
"241861950"
                },
                {
"B1",
@"1-3 a: abcde
1-3 b: cdefg
2-9 c: ccccccccc",
"2"
                },
                {
"B2",
@"1-3 a: abcde
1-3 b: cdefg
2-9 c: ccccccccc",
"1"
                },
                {
"C1",
@"..##.......
#...#...#..
.#....#..#.
..#.#...#.#
.#...##..#.
..#.##.....
.#.#.#....#
.#........#
#.##...#...
#...##....#
.#..#...#.#",
"7"
                },
                {
"D1",
@"ecl:gry pid:860033327 eyr:2020 hcl:#fffffd
byr:1937 iyr:2017 cid:147 hgt:183cm

iyr:2013 ecl:amb cid:350 eyr:2023 pid:028048884
hcl:#cfa07d byr:1929

hcl:#ae17e1 iyr:2013
eyr:2024
ecl:brn pid:760753108 byr:1931
hgt:179cm

hcl:#cfa07d eyr:2025 pid:166559648
iyr:2011 ecl:brn hgt:59in",
"2"
                },
                {
"D2",
@"eyr:1972 cid:100
hcl:#18171d ecl:amb hgt:170 pid:186cm iyr:2018 byr:1926

iyr:2019
hcl:#602927 eyr:1967 hgt:170cm
ecl:grn pid:012533040 byr:1946

hcl:dab227 iyr:2012
ecl:brn hgt:182cm pid:021572410 eyr:2020 byr:1992 cid:277

hgt:59cm ecl:zzz
eyr:2038 hcl:74454a iyr:2023
pid:3556412378 byr:2007",
"0"
                },
                {
"D2",
@"pid:087499704 hgt:74in ecl:grn iyr:2012 eyr:2030 byr:1980
hcl:#623a2f

eyr:2029 ecl:blu cid:129 byr:1989
iyr:2014 pid:896056539 hcl:#a97842 hgt:165cm

hcl:#888785
hgt:164cm byr:2001 iyr:2015 cid:88
pid:545766238 ecl:hzl
eyr:2022

iyr:2010 hgt:158cm hcl:#b6652a ecl:blu byr:1944 eyr:2021 pid:093154719",
"4"
                },
                {
"E1",
@"BFFFBBFRRR
FFFBBBFRRR
BBFFBBFRLL",
"820"
                },
                {
"E1",
"BFFFBBFRRR",
"567"
                },
                {
"E1",
"FFFBBBFRRR",
"119"
                },
                {
"F1",
@"abc

a
b
c

ab
ac

a
a
a
a

b",
"11"
                },
                {
"F2",
@"abc

a
b
c

ab
ac

a
a
a
a

b",
"6"
                },
                {
"G1",
@"light red bags contain 1 bright white bag, 2 muted yellow bags.
dark orange bags contain 3 bright white bags, 4 muted yellow bags.
bright white bags contain 1 shiny gold bag.
muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.
shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.
dark olive bags contain 3 faded blue bags, 4 dotted black bags.
vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.
faded blue bags contain no other bags.
dotted black bags contain no other bags.",
"4"
                },
                {
"G2",
@"light red bags contain 1 bright white bag, 2 muted yellow bags.
dark orange bags contain 3 bright white bags, 4 muted yellow bags.
bright white bags contain 1 shiny gold bag.
muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.
shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.
dark olive bags contain 3 faded blue bags, 4 dotted black bags.
vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.
faded blue bags contain no other bags.
dotted black bags contain no other bags.",
"32"
                },
                {
"G2",
@"shiny gold bags contain 2 dark red bags.
dark red bags contain 2 dark orange bags.
dark orange bags contain 2 dark yellow bags.
dark yellow bags contain 2 dark green bags.
dark green bags contain 2 dark blue bags.
dark blue bags contain 2 dark violet bags.
dark violet bags contain no other bags.",
"126"
                },
                {
"H1",
@"nop +0
acc +1
jmp +4
acc +3
jmp -3
acc -99
acc +1
jmp -4
acc +6",
"5"
                },
                {
"H2",
@"nop +0
acc +1
jmp +4
acc +3
jmp -3
acc -99
acc +1
jmp -4
acc +6",
"8"
                },
                {
"I1",
@"-5
35
20
15
25
47
40
62
55
65
95
102
117
150
182
127
219
299
277
309
576",
"127"
                },
                {
"I2",
@"-5
35
20
15
25
47
40
62
55
65
95
102
117
150
182
127
219
299
277
309
576",
"62"
                },
                {
"J1",
@"16
10
15
5
1
11
7
19
6
12
4",
"35"
                },
                {
"J1",
@"28
33
18
42
31
14
46
20
48
47
24
23
49
45
19
38
39
11
1
32
25
35
8
17
7
9
4
2
34
10
3",
"220"
                },
                {
"J2",
@"16
10
15
5
1
11
7
19
6
12
4",
"8"
                },
                {
"J2",
@"28
33
18
42
31
14
46
20
48
47
24
23
49
45
19
38
39
11
1
32
25
35
8
17
7
9
4
2
34
10
3",
"19208"
                },
                {
"K1",
@"L.LL.LL.LL
LLLLLLL.LL
L.L.L..L..
LLLL.LL.LL
L.LL.LL.LL
L.LLLLL.LL
..L.L.....
LLLLLLLLLL
L.LLLLLL.L
L.LLLLL.LL",
"37"
                },
                {
"K2",
@"L.LL.LL.LL
LLLLLLL.LL
L.L.L..L..
LLLL.LL.LL
L.LL.LL.LL
L.LLLLL.LL
..L.L.....
LLLLLLLLLL
L.LLLLLL.L
L.LLLLL.LL",
"26"
                },
                {
"L1",
@"F10
N3
F7
R90
F11",
"25"
                },
                {
"L2",
@"F10
N3
F7
R90
F11",
"286"
                },
                {
"M1",
@"939
7,13,x,x,59,x,31,19",
"295"
                },
                {
"M2",
@"939
7,13,x,x,59,x,31,19",
"1068781"
                },
                {
"M2",
@"939
17,x,13,19",
"3417"
                },
                {
"M2",
@"939
67,7,59,61",
"754018"
                },
                {
"M2",
@"939
67,x,7,59,61",
"779210"
                },
                {
"M2",
@"939
67,7,x,59,61",
"1261476"
                },
                {
"M2",
@"939
1789,37,47,1889",
"1202161486"
                },
                {
"N1",
@"mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X
mem[8] = 11
mem[7] = 101
mem[8] = 0",
"165"
                },
                {
"N2",
@"mask = 000000000000000000000000000000X1001X
mem[42] = 100
mask = 00000000000000000000000000000000X0XX
mem[26] = 1",
"208"
                },
                { "O1", "0,3,6", "436" },
                { "O1", "1,3,2", "1" },
                { "O1", "2,1,3", "10" },
                { "O1", "1,2,3", "27" },
                { "O1", "2,3,1", "78" },
                { "O1", "3,2,1", "438" },
                { "O1", "3,1,2", "1836" },
                // { "O2", "0,3,6", "175594" },
                // { "O2", "1,3,2", "2578" },
                // { "O2", "2,1,3", "3544142" },
                // { "O2", "1,2,3", "261214" },
                // { "O2", "2,3,1", "6895259" },
                // { "O2", "3,2,1", "18" },
                // { "O2", "3,1,2", "362" },
                {
"P1",
@"class: 1-3 or 5-7
row: 6-11 or 33-44
seat: 13-40 or 45-50

your ticket:
7,1,14

nearby tickets:
7,3,47
40,4,50
55,2,20
38,6,12",
"71"
                },
                {
"P2",
@"departure class: 0-1 or 4-19
departure row: 0-5 or 8-19
seat: 0-13 or 16-19

your ticket:
11,12,13

nearby tickets:
3,9,18
15,1,5
5,14,9",
"132"
                },
                {
"Q1",
@".#.
..#
###",
"112"
                },
                {
"Q2",
@".#.
..#
###",
"848"
                },
                {
"R1",
@"1 + 2 * 3 + 4 * 5 + 6
1 + (2 * 3) + (4 * (5 + 6))
2 * 3 + (4 * 5)
5 + (8 * 3 + 9 + 3 * 4 * 3)
5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))
((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2",
$"{71 + 51 + 26 + 437 + 12240 + 13632}"
                },
                {
"R2",
@"1 + 2 * 3 + 4 * 5 + 6
1 + (2 * 3) + (4 * (5 + 6))
2 * 3 + (4 * 5)
5 + (8 * 3 + 9 + 3 * 4 * 3)
5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))
((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2",
$"{231 + 51 + 46 + 1445 + 669060 + 23340}"
                },
                {
"S1",
@"0: 4 1 5
1: 2 3 | 3 2
2: 4 4 | 5 5
3: 4 5 | 5 4
4: ""a""
5: ""b""

ababbb
bababa
abbbab
aaabbb
aaaabbb",
$"2"
                },
                {
"S2",
@"42: 9 14 | 10 1
9: 14 27 | 1 26
10: 23 14 | 28 1
1: ""a""
11: 42 31
5: 1 14 | 15 1
19: 14 1 | 14 14
12: 24 14 | 19 1
16: 15 1 | 14 14
31: 14 17 | 1 13
6: 14 14 | 1 14
2: 1 24 | 14 4
0: 8 11
13: 14 3 | 1 12
15: 1 | 14
17: 14 2 | 1 7
23: 25 1 | 22 14
28: 16 1
4: 1 1
20: 14 14 | 1 15
3: 5 14 | 16 1
27: 1 6 | 14 18
14: ""b""
21: 14 1 | 1 14
25: 1 1 | 1 14
22: 14 14
8: 42
26: 14 22 | 1 20
18: 15 15
7: 14 5 | 1 21
24: 14 1

abbbbbabbbaaaababbaabbbbabababbbabbbbbbabaaaa
bbabbbbaabaabba
babbbbaabbbbbabbbbbbaabaaabaaa
aaabbbbbbaaaabaababaabababbabaaabbababababaaa
bbbbbbbaaaabbbbaaabbabaaa
bbbababbbbaaaaaaaabbababaaababaabab
ababaaaaaabaaab
ababaaaaabbbaba
baabbaaaabbaaaababbaababb
abbbbabbbbaaaababbbbbbaaaababb
aaaaabbaabaaaaababaa
aaaabbaaaabbaaa
aaaabbaabbaaaaaaabbbabbbaaabbaabaaa
babaaabbbaaabaababbaabababaaab
aabbbbbaabbbaaaaaabbbbbababaaaaabbaaabba",
$"12"
                }
        };
    }
}
