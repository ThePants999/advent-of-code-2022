namespace Patersoft.AOC.AOC22;

using Patersoft.AOC;
using Patersoft.AOC.AOC22.Utils;
using System.Collections.Generic;
//using System.Linq;

public class Day21 : Day
{
    Dictionary<string, Day21Monkey> _monkeys = new Dictionary<string, Day21Monkey>();

    public Day21(AOCEnvironment env) : base("2022", 21, env) { }

    protected override string ExecutePart1()
    {
        foreach (string line in new LineReader(() => new StringReader(Input)))
        {
            Day21Monkey.Parse(line, _monkeys);
        }

        return _monkeys["root"].Resolve();
    }

    protected override string ExecutePart2()
    {
        OperationMonkey root = (OperationMonkey)_monkeys["root"];
        Day21Monkey monkey1 = _monkeys[root.Monkey1];
        Day21Monkey monkey2 = _monkeys[root.Monkey2];

        _monkeys.Remove("humn");
        _monkeys.Add("humn", new HumanMonkey());
        return $"{monkey1.Resolve()} = {monkey2.Resolve()}";
        /*Console.WriteLine($"Before: {monkey1.Resolve()}, {monkey2.Resolve()}");
        Console.WriteLine($"After: {monkey1.Resolve()}, {monkey2.Resolve()}");*/

        /*int testNum = 0;
        while (true)
        {
            _monkeys.Remove("humn");
            _monkeys.Add("humn", new NumberMonkey(testNum));
            if (monkey1.Resolve() == monkey2.Resolve())
            {
                return testNum.ToString();
            }
            testNum++;
        }*/
    }

    protected override string? GetExampleInput()
    {
        return @"root: pppw + sjmn
dbpl: 5
cczh: sllz + lgvd
zczc: 2
ptdq: humn - dvpt
dvpt: 3
lfqf: 4
humn: 5
ljgn: 2
sjmn: drzm * dbpl
sllz: 4
pppw: cczh / lfqf
lgvd: ljgn * ptdq
drzm: hmdt - zczc
hmdt: 32";
    }

    protected override string? GetExamplePart1Answer()
    {
        return "152";
    }

    protected override string? GetExamplePart2Answer()
    {
        return "301";
    }
}

internal abstract class Day21Monkey
{
    internal abstract string Resolve();

    internal static void Parse(string input, Dictionary<string, Day21Monkey> monkeysDict)
    {
        string name = input.Substring(0, 4);
        string rhs = input.Substring(6);
        Day21Monkey monkey;
        if (rhs.Contains(' '))
        {
            monkey = new OperationMonkey(monkeysDict, rhs);
        } else {
            monkey = new NumberMonkey(long.Parse(rhs));
        }
        monkeysDict.Add(name, monkey);
    }
}

internal class NumberMonkey : Day21Monkey
{
    private readonly long _number;

    internal NumberMonkey(long number)
    {
        _number = number;
    }

    internal override string Resolve()
    {
        return _number.ToString();
    }
}

internal class HumanMonkey : Day21Monkey
{
    internal HumanMonkey() { }

    internal override string Resolve()
    {
        return "X";
    }
}

internal enum Operators
{
    Add,
    Subtract,
    Multiply,
    Divide,
}

internal class OperationMonkey : Day21Monkey
{
    private readonly Dictionary<string, Day21Monkey> _monkeys;
    internal string Monkey1 { get; init; }
    internal string Monkey2 { get; init; }
    private readonly Operators _operator;

    internal OperationMonkey(Dictionary<string, Day21Monkey> monkeys, string operation)
    {
        _monkeys = monkeys;
        string[] chunks = operation.Split(' ');
        Monkey1 = chunks[0];
        Monkey2 = chunks[2];
        _operator = chunks[1][0] switch {
            '+' => Operators.Add,
            '-' => Operators.Subtract,
            '*' => Operators.Multiply,
            '/' => Operators.Divide,
            _ => throw new Exception()
        };
    }

    internal override string Resolve()
    {
        string operand1 = _monkeys[Monkey1].Resolve();
        string operand2 = _monkeys[Monkey2].Resolve();
        long operand1Long;
        long operand2Long;
        if (long.TryParse(operand1, out operand1Long) && long.TryParse(operand2, out operand2Long))
        {
            return _operator switch {
                Operators.Add => (operand1Long + operand2Long).ToString(),
                Operators.Subtract => (operand1Long - operand2Long).ToString(),
                Operators.Multiply => (operand1Long * operand2Long).ToString(),
                Operators.Divide => (operand1Long / operand2Long).ToString(),
                _ => throw new Exception()
            };
        } else {
            return _operator switch {
                Operators.Add => $"({operand1} + {operand2})",
                Operators.Subtract => $"({operand1} - {operand2})",
                Operators.Multiply => $"({operand1} * {operand2})",
                Operators.Divide => $"({operand1} / {operand2})",
            };
        };
    }
}