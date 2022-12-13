namespace Patersoft.AOC.AOC22;

using Patersoft.AOC;
using Patersoft.AOC.AOC22.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Day13 : Day
{
    private List<PacketSection> _packets = new List<PacketSection>();

    public Day13(AOCEnvironment env) : base("2022", 13, env) { }

    protected override string ExecutePart1()
    {
        foreach (string line in new LineReader(() => new StringReader(Input)))
        {
            if (line.Length == 0) { continue; }
            int index = 1;
            _packets.Add(PacketSection.Parse(line, ref index));
        }

        int indexSum = 0;
        for (int i = 0; i < _packets.Count; i += 2)
        {
            if (_packets[i] <= _packets[i + 1])
            {
                indexSum += (i / 2) + 1;
            }
        }

        return indexSum.ToString();
    }

    protected override string ExecutePart2()
    {
        string divider = "[[2]]";
        int index = 1;
        PacketSection firstDivider = PacketSection.Parse(divider, ref index);
        _packets.Add(firstDivider);
        divider = "[[6]]";
        index = 1;
        PacketSection secondDivider = PacketSection.Parse(divider, ref index);
        _packets.Add(secondDivider);

        _packets.Sort();

        int firstDividerLoc = _packets.IndexOf(firstDivider) + 1;
        int secondDividerLoc = _packets.IndexOf(secondDivider) + 1;
        return (firstDividerLoc * secondDividerLoc).ToString();
    }

    protected override string? GetExampleInput()
    {
        return @"[1,1,3,1,1]
[1,1,5,1,1]

[[1],[2,3,4]]
[[1],4]

[9]
[[8,7,6]]

[[4,4],4,4]
[[4,4],4,4,4]

[7,7,7,7]
[7,7,7]

[]
[3]

[[[]]]
[[]]

[1,[2,[3,[4,[5,6,7]]]],8,9]
[1,[2,[3,[4,[5,6,0]]]],8,9]";
    }

    protected override string? GetExamplePart1Answer()
    {
        return "13";
    }

    protected override string? GetExamplePart2Answer()
    {
        return "140";
    }
}

internal class PacketSection : IComparable, IComparable<PacketSection>
{
    private int? IntValue { get; init; }
    private List<PacketSection> _contents = new List<PacketSection>();

    internal PacketSection(int intValue)
    {
        IntValue = intValue;
    }

    private PacketSection()
    {
        IntValue = null;
    }

    internal static PacketSection Parse(string input, ref int index)
    {
        PacketSection section = new PacketSection();
        StringBuilder? itemSoFar = null;

        while (index < input.Length)
        {
            int currentIndex = index;
            index++;
            if (input[currentIndex] == ']' || input[currentIndex] == ',')
            {
                // We're done with an item. If we have an in-progress item,
                // it was an integer and we need to compile and add it.
                // Otherwise, it was a list and the work is already done.
                if (itemSoFar != null)
                {
                    int value = int.Parse(itemSoFar.ToString());
                    itemSoFar = null;
                    section._contents.Add(new PacketSection(value));
                }

                if (input[currentIndex] == ']')
                {
                    break;
                }
            }
            else if (input[currentIndex] == '[')
            {
                section._contents.Add(Parse(input, ref index));
            }
            else if (input[currentIndex] == ',')
            {
                if (itemSoFar != null)
                {
                    int value = int.Parse(itemSoFar.ToString());
                    itemSoFar = null;
                    section._contents.Add(new PacketSection(value));
                }
                else
                {
                    // Current list item was a list. No-op.
                }
            }
            else
            {
                if (itemSoFar == null)
                {
                    itemSoFar = new StringBuilder();
                }
                itemSoFar.Append(input[currentIndex]);
            }
        }

        return section;
    }

    public int CompareTo(PacketSection? other)
    {
        if (other == null)
        {
            return 1;
        }

        if (this.IntValue.HasValue)
        {
            if (other.IntValue.HasValue)
            {
                // Compare two ints
                return this.IntValue.Value.CompareTo(other.IntValue.Value);
            }
            else
            {
                // Compare int with list: convert int to list
                PacketSection list = new PacketSection();
                list._contents.Add(new PacketSection(this.IntValue.Value));
                return list.CompareTo(other);
            }
        }
        else
        {
            if (other.IntValue.HasValue)
            {
                // Compare list with int: convert int to list
                PacketSection list = new PacketSection();
                list._contents.Add(new PacketSection(other.IntValue.Value));
                return this.CompareTo(list);
            }
            else
            {
                // Compare two lists
                int index;
                for (index = 0; index < this._contents.Count; index++)
                {
                    if (index >= other._contents.Count)
                    {
                        // This list is longer
                        return 1;
                    }

                    int itemComparison = this._contents[index].CompareTo(other._contents[index]);
                    if (itemComparison != 0)
                    {
                        return itemComparison;
                    }
                }

                if (index < other._contents.Count)
                {
                    // Other list is longer
                    return -1;
                }
                else
                {
                    // Lists match
                    return 0;
                }
            }
        }
    }

    public int CompareTo(object? obj)
    {
        PacketSection? other = obj as PacketSection;
        return CompareTo(other);
    }

    public static bool operator <(PacketSection left, PacketSection right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(PacketSection left, PacketSection right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(PacketSection left, PacketSection right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(PacketSection left, PacketSection right)
    {
        return left.CompareTo(right) >= 0;
    }
}