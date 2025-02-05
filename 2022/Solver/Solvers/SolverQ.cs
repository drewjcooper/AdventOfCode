using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Helpers;
using AdventOfCode2022.Input;

namespace AdventOfCode2022.Solvers;

internal class SolverQ : Solver
{
    private readonly Gas _gas;

    public SolverQ(PuzzleInput input)
    {
        _gas = new Gas(input.RawText);
    }
    
    protected override string SolvePart1() => SimulateRocks(2_022).ToString();

    protected override string SolvePart2() => SimulateRocks(1_000_000/*_000_000*/).ToString();

    private long SimulateRocks(long count)
    {
        var tower = new Tower();

        for (long i = 0; i < count; i++)
        {
            var rock = Rock.Shape.Next.CreateRock(tower.Height);
            rock.TryMove(_gas, tower);
            rock.TryMove(_gas, tower);
            rock.TryMove(_gas, tower);
            rock.TryMove(_gas, tower);
            while (rock.TryDrop(tower))
            {
                rock.TryMove(_gas, tower);
            }
            tower.Add(rock);
        }

        return tower.Height;
    }

    private enum Direction
    {
        Left,
        Right
    }

    private class Gas
    {
        private readonly string _directions;
        private int _next;

        public Gas(string directions)
        {
            _directions = directions;
        }

        public Direction Jet
        {
            get
            {
                var direction = _directions[_next] == '<' ? Direction.Left : Direction.Right;
                _next = (_next + 1) % _directions.Length;
                return direction;
            }
        } 
    }

    private class Tower
    {
        private List<byte> _layers = new();
        private long _truncatedHeight;

        public long Height => _layers.Count + _truncatedHeight;

        public IEnumerable<byte> GetLayers(long height)
        {
            for (var i = height - _truncatedHeight; i < _layers.Count; i++)
            {
                yield return _layers[(int)i];
            }
        }

        public void Add(Rock rock)
        {
            var h = (int)(rock.Height - _truncatedHeight);
            foreach (var rockValue in rock.Values)
            {
                if (h == _layers.Count) { _layers.Add(0); }
                _layers[h] |= rockValue;
                if (_layers[h] == 0b111_1111)
                {
                    _truncatedHeight += h + 1;
                    _layers = _layers.Skip(h + 1).ToList();
                }
                h += 1;
            }
        }
    }

    private class Rock
    {
        private readonly Shape _shape;
        private int _offset;
        private long _height;

        private Rock(Shape shape, long height)
        {
            _shape = shape;
            _offset = _shape.StartOffset;
            _height = height;
        }

        public IEnumerable<byte> Values => _shape.Values.Select(v => (byte)(v << _offset));
        public long Height => _height;

        public bool TryMove(Gas gas, Tower tower) 
            => gas.Jet == Direction.Left ? TryMoveLeft(tower) : TryMoveRight(tower);

        private bool TryMoveLeft(Tower tower)
        {
            if (_offset < _shape.MaxOffset && !WouldOverlap(tower, _offset + 1, _height))
            {
                _offset += 1;
                return true;
            }

            return false;
        }

        private bool TryMoveRight(Tower tower)
        {
            if (_offset > 0 && !WouldOverlap(tower, _offset - 1, _height))
            {
                _offset -= 1;
                return true;
            }

            return false;
        }

        public bool TryDrop(Tower tower)
        {
            if (_height > 0 && !WouldOverlap(tower, _offset, _height - 1))
            {
                _height -= 1;
                return true;
            }

            return false;
        }

        private bool WouldOverlap(Tower tower, int offset, long height) 
            => _shape.Values.Zip(tower.GetLayers(height), (s, t) => (s << offset) & t).Any(x => x > 0);

        public record Shape(byte[] Values, int StartOffset, int MaxOffset)
        {
            private static Shape Create(int startOffset, params byte[] shape) 
                => new(shape, startOffset, startOffset + 2);

            private static readonly Shape[] _shapes = new[]
            {
                Create(1, 0b1111),
                Create(2, 0b010, 0b111, 0b010),
                Create(2, 0b111, 0b001, 0b001),
                Create(4, 0b1, 0b1, 0b1, 0b1),
                Create(3, 0b11, 0b11)
            };

            private static int _next;

            public static Shape Next
            {
                get
                {
                    var shape = _shapes[_next];
                    _next = (_next + 1) % _shapes.Length;
                    return shape;
                }
            } 

            public Rock CreateRock(long height) => new Rock(this, height);
        }
    }
}