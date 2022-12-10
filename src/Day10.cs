namespace Patersoft.AOC.AOC22;

using Patersoft.AOC;
using Patersoft.AOC.AOC22.Utils;
using System.Collections.Generic;
using System.Text;

public class Day10 : Day
{
    CRT? _crt;

    public Day10(AOCEnvironment env) : base("2022", 10, env) { }

    protected override string ExecutePart1()
    {
        _crt = new CRT(Input);

        while (true)
        {
            try
            {
                _crt.Tick();
            }
            catch (ProgramCompleteException)
            {
                break;
            }
        }

        return _crt.SignalStrength.ToString();
    }

    protected override string ExecutePart2()
    {
        return _crt!.Image;
    }

    protected override string? GetExampleInput()
    {
        return @"addx 15
addx -11
addx 6
addx -3
addx 5
addx -1
addx -8
addx 13
addx 4
noop
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx -35
addx 1
addx 24
addx -19
addx 1
addx 16
addx -11
noop
noop
addx 21
addx -15
noop
noop
addx -3
addx 9
addx 1
addx -3
addx 8
addx 1
addx 5
noop
noop
noop
noop
noop
addx -36
noop
addx 1
addx 7
noop
noop
noop
addx 2
addx 6
noop
noop
noop
noop
noop
addx 1
noop
noop
addx 7
addx 1
noop
addx -13
addx 13
addx 7
noop
addx 1
addx -33
noop
noop
noop
addx 2
noop
noop
noop
addx 8
noop
addx -1
addx 2
addx 1
noop
addx 17
addx -9
addx 1
addx 1
addx -3
addx 11
noop
noop
addx 1
noop
addx 1
noop
noop
addx -13
addx -19
addx 1
addx 3
addx 26
addx -30
addx 12
addx -1
addx 3
addx 1
noop
noop
noop
addx -9
addx 18
addx 1
addx 2
noop
noop
addx 9
noop
noop
noop
addx -1
addx 2
addx -37
addx 1
addx 3
noop
addx 15
addx -21
addx 22
addx -6
addx 1
noop
addx 2
addx 1
noop
addx -10
noop
noop
addx 20
addx 1
addx 2
addx 2
addx -6
addx -11
noop
noop
noop";
    }

    protected override string? GetExamplePart1Answer()
    {
        return "13140";
    }

    protected override string? GetExamplePart2Answer()
    {
        return @"
##..##..##..##..##..##..##..##..##..##..
###...###...###...###...###...###...###.
####....####....####....####....####....
#####.....#####.....#####.....#####.....
######......######......######......####
#######.......#######.......#######.....
";
    }
}

internal class ProgramCompleteException : Exception
{
    public ProgramCompleteException()
    {
    }

    public ProgramCompleteException(string message) : base(message)
    {
    }

    public ProgramCompleteException(string message, Exception inner) : base(message, inner)
    {
    }
}

internal class CRT
{
    private int _cycle = 1;
    private int _register = 1;
    private Queue<int?> _instructions = new Queue<int?>();
    private int? _currentInstruction = null;
    private int _signalStrength = 0;

    internal int SignalStrength { get { return _signalStrength; } }
    private StringBuilder _image = new StringBuilder();
    internal string Image { get { return _image.ToString(); } }

    internal CRT(string instructions)
    {
        foreach (string line in new LineReader(() => new StringReader(instructions)))
        {
            if (line[0] == 'n')
            {
                _instructions.Enqueue(null);
            } else {
                _instructions.Enqueue(int.Parse(line.Split(' ')[1]));
            }
        }

        _image.Append(Environment.NewLine);
    }

    internal void Tick()
    {
        int delta = 0;
        if (_currentInstruction.HasValue)
        {
            // We're part way through processing an add instruction.
            // Do so at the end of this cycle, and don't process a new
            // instruction.
            delta = _currentInstruction.Value;
            _currentInstruction = null;
        } else {
            try
            {
                _currentInstruction = _instructions.Dequeue();
            }
            catch (InvalidOperationException e)
            {
                throw new ProgramCompleteException("Program has run to completion.", e);
            }
        }

        // Draw pixel
        int position = (_cycle - 1) % 40;
        if ((position == _register) || (position == _register + 1) || (position == _register - 1))
        {
            _image.Append('#');
        } else {
            _image.Append('.');
        }
        if (position == 39)
        {
            _image.Append(Environment.NewLine);
        }

        if (((_cycle + 20) % 40 == 0) && (_cycle <= 220))
        {
            _signalStrength += (_register * _cycle);
        }

        _register += delta;
        _cycle++;
    }
}