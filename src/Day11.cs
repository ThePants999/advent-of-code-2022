namespace Patersoft.AOC.AOC22;

using Patersoft.AOC;
using Patersoft.AOC.AOC22.Utils;
using System.Collections.Generic;
using System.Linq;

public class Day11 : Day
{
    public Day11(AOCEnvironment env) : base("2022", 11, env) { }

    protected override string ExecutePart1()
    {
        Forest forest = new Forest(Input, true);

        for (int i = 0; i < 20; i++)
        {
            forest.PerformRound();
        }

        return forest.MonkeyBusiness.ToString();
    }

    protected override string ExecutePart2()
    {
        Forest forest = new Forest(Input, false);

        for (int i = 0; i < 10000; i++)
        {
            forest.PerformRound();
        }

        return forest.MonkeyBusiness.ToString();
    }

    protected override string? GetExampleInput()
    {
        return @"Monkey 0:
  Starting items: 79, 98
  Operation: new = old * 19
  Test: divisible by 23
    If true: throw to monkey 2
    If false: throw to monkey 3

Monkey 1:
  Starting items: 54, 65, 75, 74
  Operation: new = old + 6
  Test: divisible by 19
    If true: throw to monkey 2
    If false: throw to monkey 0

Monkey 2:
  Starting items: 79, 60, 97
  Operation: new = old * old
  Test: divisible by 13
    If true: throw to monkey 1
    If false: throw to monkey 3

Monkey 3:
  Starting items: 74
  Operation: new = old + 3
  Test: divisible by 17
    If true: throw to monkey 0
    If false: throw to monkey 1";
    }

    protected override string? GetExamplePart1Answer()
    {
        return "10605";
    }

    protected override string? GetExamplePart2Answer()
    {
        return "2713310158";
    }
}

internal enum OperationTypes
{
    Add,
    Multiply,
    Square,
}

internal class Operation
{
    private OperationTypes _type;
    private int _operand = 0;

    internal Operation(string input)
    {
        string[] chunks = input.Split(' ');
        try
        {
            _operand = int.Parse(chunks[chunks.Length - 1]);
            if (chunks[chunks.Length - 2] == "+")
            {
                _type = OperationTypes.Add;
            } else {
                _type = OperationTypes.Multiply;
            }
        }
        catch (FormatException)
        {
            _type = OperationTypes.Square;
        }
    }

    internal int Perform(int input)
    {
        return _type switch {
            OperationTypes.Add => input + _operand,
            OperationTypes.Multiply => input * _operand,
            OperationTypes.Square => input * input,
            _ => throw new NotImplementedException(),
        };
    }
}

internal class Forest
{
    List<Monkey> _monkeys = new List<Monkey>();
    internal int Lcm { get; init; }

    internal Forest (string input, bool worryDegrades)
    {
        Lcm = 1;
        IEnumerator<string> lines = new LineReader(() => new StringReader(input)).GetEnumerator();
        lines.MoveNext();
        while (true)
        {
            try
            {
                Monkey newMonkey = new Monkey(lines, this, worryDegrades);
                _monkeys.Add(newMonkey);
                Lcm = MathUtils.LowestCommonMultiple(Lcm, newMonkey.Test);
            }
            catch (InputException)
            {
                break;
            }
        }
    }

    internal void PerformRound()
    {
        foreach (Monkey monkey in _monkeys)
        {
            monkey.InspectItems();
        }
    }

    internal void ThrowToMonkey(int item, int monkey)
    {
        _monkeys[monkey].CatchItem(item);
    }

    internal long MonkeyBusiness {
        get
        {
            List<int> activity = _monkeys.Select<Monkey, int>(monkey => monkey.ItemsInspected).ToList();
            activity.Sort();
            return (long)activity[activity.Count - 1] * (long)activity[activity.Count - 2];
        }
    }
}

internal class Monkey
{
    private Queue<int> _items = new Queue<int>();
    private Operation _op;
    internal int Test { get; init; }
    private int _trueMonkey;
    private int _falseMonkey;
    private Forest _forest;
    private int _itemsInspected = 0;
    internal int ItemsInspected { get { return _itemsInspected; } }
    private bool _worryDegrades;

    internal Monkey(IEnumerator<string> input, Forest forest, bool worryDegrades)
    {
        _forest = forest;
        _worryDegrades = worryDegrades;

        // Skip over the first line, which has the monkey index.
        if (!input.MoveNext())
        {
            // We've reached the end of the input.
            throw new InputException("No more monkeys");
        }

        // Next line has the starting items.
        string startingItems = input.Current;
        input.MoveNext();
        string[] startingItemsChunks = startingItems.Split(", ");
        // First item contains all the preceding text.
        _items.Enqueue(int.Parse(startingItemsChunks[0].Substring(startingItemsChunks[0].Length - 2)));
        for (int i = 1; i < startingItemsChunks.Length; i++)
        {
            _items.Enqueue(int.Parse(startingItemsChunks[i]));
        }

        // Next line has the operation.
        _op = new Operation(input.Current);
        input.MoveNext();

        // Then the test, the true line and the false line, and finally skip the blank line.
        string[] chunks = input.Current.Split(' ');
        Test = int.Parse(chunks[chunks.Length - 1]);
        input.MoveNext();
        chunks = input.Current.Split(' ');
        _trueMonkey = int.Parse(chunks[chunks.Length - 1]);
        input.MoveNext();
        chunks = input.Current.Split(' ');
        _falseMonkey = int.Parse(chunks[chunks.Length - 1]);
        input.MoveNext();
        input.MoveNext();
    }

    internal void CatchItem(int item)
    {
        _items.Enqueue(item);
    }

    internal void InspectItems()
    {
        while (_items.Count > 0)
        {
            int item = _items.Dequeue();
            _itemsInspected++;
            item %= _forest.Lcm;
            item = _op.Perform(item);
            if (_worryDegrades)
            {
                item /= 3;
            }
            if (item % Test == 0)
            {
                _forest.ThrowToMonkey(item, _trueMonkey);
            } else {
                _forest.ThrowToMonkey(item, _falseMonkey);
            }
        }
    }
}