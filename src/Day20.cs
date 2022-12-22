namespace Patersoft.AOC.AOC22;

using Patersoft.AOC;
using Patersoft.AOC.AOC22.Utils;
using System.Collections.Generic;
using System.Linq;

public class Day20 : Day
{
    private const long DECRYPTION_KEY = 811589153;
    private List<Number> _originalOrder = new List<Number>();

    public Day20(AOCEnvironment env) : base("2022", 20, env) { }

    protected override string ExecutePart1()
    {
        foreach (string line in new LineReader(() => new StringReader(Input)))
        {
            Number num = new Number(long.Parse(line));
            _originalOrder.Add(num);
        }

        LinkedList<Number> currentOrder = GetLinkedList();
        Mix(currentOrder);
        long coordinates = GetCoordinates(currentOrder);
        currentOrder.Clear();

        return coordinates.ToString();
    }

    protected override string ExecutePart2()
    {
        foreach (Number num in _originalOrder)
        {
            num.Value *= DECRYPTION_KEY;
        }
        LinkedList<Number> currentOrder = GetLinkedList();
        for (int i = 0; i < 10; i++)
        {
            Mix(currentOrder);
        }

        return GetCoordinates(currentOrder).ToString();
    }

    protected override string? GetExampleInput()
    {
        return @"1
2
-3
3
-2
0
4";
    }

    protected override string? GetExamplePart1Answer()
    {
        return "3";
    }

    protected override string? GetExamplePart2Answer()
    {
        return "1623178306";
    }

    private LinkedList<Number> GetLinkedList()
    {
        LinkedList<Number> list = new LinkedList<Number>();
        foreach (Number num in _originalOrder)
        {
            list.AddLast(num.Node);
        }
        return list;
    }

    private long GetCoordinates(LinkedList<Number> currentOrder)
    {
        int zeroOffset = currentOrder.TakeWhile(n => n.Value != 0).Count();
        return (currentOrder.ElementAt((1000 + zeroOffset) % _originalOrder.Count).Value +
            currentOrder.ElementAt((2000 + zeroOffset) % _originalOrder.Count).Value +
            currentOrder.ElementAt((3000 + zeroOffset) % _originalOrder.Count).Value);
    }

    private void Mix(LinkedList<Number> currentOrder)
    {
        foreach (Number num in _originalOrder)
        {
            LinkedListNode<Number> node = num.Node;
            int placesToMove = (int)(num.Value % (_originalOrder.Count - 1));
            if (placesToMove != 0)
            {
                if (num.Value > 0)
                {
                    for (int i = 0; i < placesToMove; i++)
                    {
                        node = node.Next ?? currentOrder.First!;
                    }
                }
                else
                {
                    for (int i = 0; i >= placesToMove; i--)
                    {
                        node = node.Previous ?? currentOrder.Last!;
                    }
                }
                currentOrder.Remove(num.Node);
                currentOrder.AddAfter(node, num.Node);
            }
        }
    }

    private void Output(LinkedList<Number> currentOrder)
    {
        LinkedListNode<Number> node = currentOrder.First!;
        for (int i = 0; i < _originalOrder.Count; i++)
        {
            Console.WriteLine($"{i}: {node.Value.Value}");
            node = node.Next ?? currentOrder.First!;
        }
    }
}

internal class Number
{
    internal long Value { get; set; }
    internal LinkedListNode<Number> Node { get; init; }

    internal Number(long value)
    {
        this.Value = value;
        this.Node = new LinkedListNode<Number>(this);
    }
}