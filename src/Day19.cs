namespace Patersoft.AOC.AOC22;

using Patersoft.AOC;
using Patersoft.AOC.AOC22.Utils;
using System.Collections.Generic;
using System.Linq;

public class Day19 : Day
{
    List<Blueprint> _blueprints = new List<Blueprint>();

    public Day19(AOCEnvironment env) : base("2022", 19, env) { }

    protected override string ExecutePart1()
    {
        foreach (string line in new LineReader(() => new StringReader(Input)))
        {
            _blueprints.Add(new Blueprint(line));
        }

        return _blueprints.Select(bp => bp.CalculateMaxGeodes(24) * bp.ID).Sum().ToString();
    }

    protected override string ExecutePart2()
    {
        if (_blueprints.Count >= 3)
        {
            return (_blueprints[0].CalculateMaxGeodes(32) * _blueprints[1].CalculateMaxGeodes(32) * _blueprints[2].CalculateMaxGeodes(32)).ToString();
        }
        else
        {
            return (_blueprints[0].CalculateMaxGeodes(32) * _blueprints[1].CalculateMaxGeodes(32)).ToString();
        }
    }

    protected override string? GetExampleInput()
    {
        return @"Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.
Blueprint 2: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 8 clay. Each geode robot costs 3 ore and 12 obsidian.";
    }

    protected override string? GetExamplePart1Answer()
    {
        return "33";
    }

    protected override string? GetExamplePart2Answer()
    {
        return "3472";
    }
}

internal class Blueprint
{
    private const int TIME_LIMIT = 24;

    internal int ID { get; init; }
    internal int OreOreCost { get; init; }
    internal int ClayOreCost { get; init; }
    internal int ObsidianOreCost { get; init; }
    internal int ObsidianClayCost { get; init; }
    internal int GeodeOreCost { get; init; }
    internal int GeodeObsidianCost { get; init; }

    internal Blueprint(string inputLine)
    {
        string[] chunks = inputLine.Split(' ');
        ID = int.Parse(chunks[1].Remove(chunks[1].Length - 1));
        OreOreCost = int.Parse(chunks[6]);
        ClayOreCost = int.Parse(chunks[12]);
        ObsidianOreCost = int.Parse(chunks[18]);
        ObsidianClayCost = int.Parse(chunks[21]);
        GeodeOreCost = int.Parse(chunks[27]);
        GeodeObsidianCost = int.Parse(chunks[30]);
    }

    internal int CalculateMaxGeodes(int timeLimit)
    {
        State initialState = new State(this);
        int bestQuality = 0;
        Stack<State> stack = new Stack<State>();
        stack.Push(initialState);

        while (stack.Count > 0)
        {
            State state = stack.Pop();
            if (state.Minute == timeLimit)
            {
                if (state.Geodes > bestQuality)
                {
                    bestQuality = state.Geodes;
                }
            }
            else
            {
                IEnumerable<State> nextStates = state.Tick();
                foreach (State nextState in nextStates)
                {
                    stack.Push(nextState);
                }
            }
        }

        return bestQuality;
    }
}

internal enum RobotTypes
{
    None,
    Ore,
    Clay,
    Obsidian,
    Geode
}

internal class State
{
    private static readonly RobotTypes[] _robotTypes = (RobotTypes[])Enum.GetValues(typeof(RobotTypes));
    private readonly Blueprint _blueprint;
    internal int Minute { get; init; }
    private readonly int _oreRobots;
    private readonly int _clayRobots;
    private readonly int _obsidianRobots;
    private readonly int _geodeRobots;
    private readonly int _ore;
    private readonly int _clay;
    private readonly int _obsidian;
    internal int Geodes { get; init; }

    internal State(Blueprint blueprint)
    {
        _blueprint = blueprint;
        Minute = 0;

        _oreRobots = 1;
        _clayRobots = 0;
        _obsidianRobots = 0;
        _geodeRobots = 0;

        _ore = 0;
        _clay = 0;
        _obsidian = 0;
        Geodes = 0;
    }

    private State(State previous, RobotTypes constructionType)
    {
        _blueprint = previous._blueprint;
        Minute = previous.Minute + 1;

        _ore = previous._ore + previous._oreRobots;
        _clay = previous._clay + previous._clayRobots;
        _obsidian = previous._obsidian + previous._obsidianRobots;
        Geodes = previous.Geodes + previous._geodeRobots;

        _oreRobots = previous._oreRobots;
        _clayRobots = previous._clayRobots;
        _obsidianRobots = previous._obsidianRobots;
        _geodeRobots = previous._geodeRobots;

        switch (constructionType)
        {
            case RobotTypes.Ore:
                _ore -= _blueprint.OreOreCost;
                _oreRobots++;
                break;

            case RobotTypes.Clay:
                _ore -= _blueprint.ClayOreCost;
                _clayRobots++;
                break;

            case RobotTypes.Obsidian:
                _ore -= _blueprint.ObsidianOreCost;
                _clay -= _blueprint.ObsidianClayCost;
                _obsidianRobots++;
                break;

            case RobotTypes.Geode:
                _ore -= _blueprint.GeodeOreCost;
                _obsidian -= _blueprint.GeodeObsidianCost;
                _geodeRobots++;
                break;
        }
    }

    private bool CanBuild(RobotTypes type)
    {
        return type switch
        {
            RobotTypes.None => true,
            RobotTypes.Ore => _ore >= _blueprint.OreOreCost,
            RobotTypes.Clay => _ore >= _blueprint.ClayOreCost,
            RobotTypes.Obsidian => _ore >= _blueprint.ObsidianOreCost && _clay >= _blueprint.ObsidianClayCost,
            RobotTypes.Geode => _ore >= _blueprint.GeodeOreCost && _obsidian >= _blueprint.GeodeObsidianCost,
            _ => throw new Exception()
        };
    }

    internal List<State> Tick()
    {
        List<State> nextStates = new List<State>();
        if (CanBuild(RobotTypes.Geode))
        {
            nextStates.Add(new State(this, RobotTypes.Geode));

            // If we can build a geode robot, it's definitely the right decision.
            return nextStates;
        }

        if (CanBuild(RobotTypes.Obsidian))
        {
            nextStates.Add(new State(this, RobotTypes.Obsidian));

            if (_clay - _clayRobots >= _blueprint.ObsidianClayCost)
            {
                // It's occasionally worth delaying an obsidian build
                // by a minute to allow one more clay build, but not
                // more than that.
                return nextStates;
            }
        }

        if (CanBuild(RobotTypes.Clay) && _clayRobots < _blueprint.ObsidianClayCost)
        {
            // Optimisation: we never want more clay robots than are needed to build an
            // obsidian robot every minute.
            nextStates.Add(new State(this, RobotTypes.Clay));
        }
        if (CanBuild(RobotTypes.Ore) && _oreRobots < 4)
        {
            // Hypothesis: we never want more than 4 ore robots.
            nextStates.Add(new State(this, RobotTypes.Ore));
        }
        if (_ore < 5)
        {
            // Optimisation: we never want to save up more than 5 ore.
            nextStates.Add(new State(this, RobotTypes.None));
        }

        return nextStates;
    }
}