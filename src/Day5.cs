namespace Patersoft.AOC.AOC22;

using Patersoft.AOC;
using System.Collections.Generic;
//using System.Linq;

public class Day5 : Day
{
    private Stack<char>[]? _stacks = null;
    private List<Instruction> _instructions = new List<Instruction>();

    public Day5(AOCEnvironment env) : base("2022", 5, env) { }

    protected override string ExecutePart1()
    {
        string[] lines = Input.Split('\n');

        Stack<string> crate_lines = new Stack<string>();
        int index = 0;
        for (; index < lines.Length; index++)
        {
            string line = lines[index];
            if (line[1] == '1')
            {
                // This is the crate index line.
                index++;
                break;
            }
            crate_lines.Push(line);
        }

        // Also skip over the blank line.
        index++;

        // Remaining lines are instructions (and there's a blank line at the end we need to skip).
        for (; index < lines.Length - 1; index++)
        {
            _instructions.Add(new Instruction(lines[index]));
        }

        // Determine the number of crates from the length of the line,
        // easier than trying to parse the indices.
        string first_crate_line = crate_lines.Peek();
        int num_stacks = ((int)first_crate_line.Length / 4) + 1;
        _stacks = new Stack<char>[num_stacks + 1];
        for (int stack_ix = 0; stack_ix <= num_stacks; stack_ix++)
        {
            _stacks[stack_ix] = new Stack<char>();
        }

        foreach (string line in crate_lines)
        {
            for (int stack_ix = 1; stack_ix <= num_stacks; stack_ix++)
            {
                char symbol = line[(stack_ix * 4) - 3];
                if (symbol != ' ')
                {
                    _stacks[stack_ix].Push(symbol);
                }
            }
        }

        // We're finally ready to execute the instructions. Start by duplicating the stacks
        // so we can work on a temporary copy. That's actually an annoyingly difficult thing
        // to do in C#.
        var temp_stacks = new Stack<char>[_stacks.Length];
        for (int ix = 1; ix < _stacks.Length; ix++)
        {
            var arr = new char[_stacks[ix].Count];
            _stacks[ix].CopyTo(arr, 0);
            Array.Reverse(arr);
            temp_stacks[ix] = new Stack<char>(arr);
        }

        foreach (Instruction inst in _instructions)
        {
            for (int ix = 0; ix < inst.Count; ix++)
            {
                temp_stacks[inst.Destination].Push(temp_stacks[inst.Source].Pop());
            }
        }

        // And now we can grab the message.
        System.Text.StringBuilder builder = new System.Text.StringBuilder(num_stacks);
        for (int stack_ix = 1; stack_ix <= num_stacks; stack_ix++)
        {
            builder.Append(_stacks[stack_ix].Peek());
        }

        return builder.ToString();
    }

    protected override string ExecutePart2()
    {
        foreach (Instruction inst in _instructions)
        {
            Stack<char> crates_to_move = new Stack<char>(inst.Count);
            for (int ix = 0; ix < inst.Count; ix++)
            {
                crates_to_move.Push(_stacks![inst.Source].Pop());
            }
            foreach (char crate in crates_to_move)
            {
                _stacks![inst.Destination].Push(crate);
            }
        }

        // And now we can grab the message.
        System.Text.StringBuilder builder = new System.Text.StringBuilder(_stacks!.Length);
        for (int stack_ix = 1; stack_ix < _stacks!.Length; stack_ix++)
        {
            builder.Append(_stacks![stack_ix].Peek());
        }

        return builder.ToString();
    }
}

internal class Instruction
{
    internal int Count { get; init; }
    internal int Source { get; init; }
    internal int Destination { get; init; }

    internal Instruction(string input)
    {
        string[] chunks = input.Split(' ');
        Count = int.Parse(chunks[1]);
        Source = int.Parse(chunks[3]);
        Destination = int.Parse(chunks[5]);
    }
}