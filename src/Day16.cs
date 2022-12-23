namespace Patersoft.AOC.AOC22;

using Patersoft.AOC;
using Patersoft.AOC.AOC22.Utils;
using System.Collections.Generic;

public class Day16 : Day
{
    public const int TIME_LIMIT = 30;
    public const int ELEPHANT_TRAINING_TIME = 4;

    private Dictionary<string, Valve> _valves = new Dictionary<string, Valve>();
    private List<Valve> _significantValves = new List<Valve>();

    public Day16(AOCEnvironment env) : base("2022", 16, env) { }

    protected override string ExecutePart1()
    {
        ParseInput();

        for (int source = 0; source < _significantValves.Count; source++)
        {
            Valve start = _significantValves[source];
            for (int destination = 0; destination < _significantValves.Count; destination++)
            {
                if (source == destination)
                {
                    start.SignificantValveDistances.Add(0);
                } else {
                    start.SignificantValveDistances.Add(BFS(start, _significantValves[destination]));
                    ResetForNewSearch();
                }
            }
        }

        Valve startValve = _valves["AA"];
        SearchState bestState = DFS(startValve, false);
        return bestState.PressureReleased.ToString();
    }

    protected override string ExecutePart2()
    {
        Valve startValve = _valves["AA"];
        SearchState bestState = DFS(startValve, true);
        return bestState.PressureReleased.ToString();
    }

    protected override string? GetExampleInput()
    {
        return @"Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
Valve BB has flow rate=13; tunnels lead to valves CC, AA
Valve CC has flow rate=2; tunnels lead to valves DD, BB
Valve DD has flow rate=20; tunnels lead to valves CC, AA, EE
Valve EE has flow rate=3; tunnels lead to valves FF, DD
Valve FF has flow rate=0; tunnels lead to valves EE, GG
Valve GG has flow rate=0; tunnels lead to valves FF, HH
Valve HH has flow rate=22; tunnel leads to valve GG
Valve II has flow rate=0; tunnels lead to valves AA, JJ
Valve JJ has flow rate=21; tunnel leads to valve II";
    }

    protected override string? GetExamplePart1Answer()
    {
        return "1651";
    }

    protected override string? GetExamplePart2Answer()
    {
        return "1707";
    }

    private void ParseInput()
    {
        int currentIndex = 0;

        foreach (string line in new LineReader(() => new StringReader(Input)))
        {
            string valveName = line.Substring(6, 2);
            Valve valve = GetOrCreateValve(valveName);

            string flowRateStr = line.Substring(23, 2);
            if (flowRateStr[1] == ';')
            {
                flowRateStr = flowRateStr.Remove(1);
            }
            int flowRate = int.Parse(flowRateStr);
            valve.FlowRate = flowRate;

            if (flowRate > 0 || valveName == "AA")
            {
                _significantValves.Add(valve);
                valve.SignificantValveIndex = currentIndex;
                currentIndex++;
            }

            string otherValvesChunk = line.Split("valve")[1];
            if (otherValvesChunk[0] == 's')
            {
                otherValvesChunk = otherValvesChunk.Substring(2);
            } else {
                otherValvesChunk = otherValvesChunk.Substring(1);
            }

            string[] otherValves = otherValvesChunk.Split(", ");
            foreach (string otherValveName in otherValves)
            {
                Valve otherValve = GetOrCreateValve(otherValveName);
                valve.DirectConnections.Add(otherValve);
            }
        }
    }

    private Valve GetOrCreateValve(string valveName)
    {
        Valve? valve = _valves.GetValueOrDefault(valveName);
        if (valve == null)
        {
            valve = new Valve(valveName);
            _valves.Add(valveName, valve);
        }
        return valve;
    }

    private SearchState DFS(Valve start, bool elephantHelping)
    {
        Stack<SearchState> stack = new Stack<SearchState>();
        SearchState initial = new SearchState(start, elephantHelping);
        SearchState bestState = initial;
        stack.Push(initial);

        long statesConsidered = 0;

        while (stack.Count > 0)
        {
            SearchState current = stack.Pop();
            statesConsidered++;

            if (current.PressureReleased > bestState.PressureReleased)
            {
                bestState = current;
            }

            if (current.TimeElapsed < TIME_LIMIT)
            {
                for (int next = 0; next < _significantValves.Count; next++)
                {
                    if (next != current.CurrentValve.SignificantValveIndex && !current.NodeVisited(next))
                    {
                        stack.Push(new SearchState(current, _significantValves[next]));
                    }
                }
            }
        }

        return bestState;
    }

    private int BFS(Valve start, Valve end)
    {
        Queue<Valve> q = new Queue<Valve>();

        start.DistanceInCurrentSearch = 0;
        start.VisitedInCurrentSearch = true;
        q.Enqueue(start);

        while (q.Count > 0)
        {
            Valve current = q.Dequeue();

            foreach (Valve candidate in current.DirectConnections)
            {
                if (!candidate.VisitedInCurrentSearch)
                {
                    candidate.VisitedInCurrentSearch = true;
                    candidate.DistanceInCurrentSearch = current.DistanceInCurrentSearch + 1;
                    if (candidate == end)
                    {
                        // Add one to the return value to reflect opening the valve at that
                        // location.
                        return candidate.DistanceInCurrentSearch + 1;
                    }
                    q.Enqueue(candidate);
                }
            }
        }

        return -1;
    }

    private void ResetForNewSearch()
    {
        foreach (Valve valve in _valves.Values)
        {
            valve.ResetForNewSearch();
        }
    }
}

internal class SearchState
{
    internal Valve CurrentValve { get; init; }
    private Valve? _otherPartyValve;
    private int _visitedNodes;
    internal int PressureReleased { get; init; }
    internal int TimeElapsed { get; init; }
    private int _otherPartyTravelTime;
    private bool _twoParties;

    internal SearchState(Valve initialValve, bool twoParties)
    {
        CurrentValve = initialValve;
        _visitedNodes = AddNodeToVisitedMask(0, initialValve);
        PressureReleased = 0;
        _twoParties = twoParties;

        if (_twoParties)
        {
            TimeElapsed = Day16.ELEPHANT_TRAINING_TIME;
        } else {
            TimeElapsed = 0;
        }
    }

    internal SearchState(SearchState previousState, Valve nextNode)
    {
        _visitedNodes = AddNodeToVisitedMask(previousState._visitedNodes, nextNode);
        _twoParties = previousState._twoParties;

        PressureReleased = previousState.PressureReleased;
        int timeToReachNode = previousState.CurrentValve.SignificantValveDistances[nextNode.SignificantValveIndex];
        int timeLeft = Day16.TIME_LIMIT - previousState.TimeElapsed - timeToReachNode;
        if (timeLeft > 0)
        {
            PressureReleased += (timeLeft * nextNode.FlowRate);
        }

        if (!_twoParties)
        {
            CurrentValve = nextNode;
            TimeElapsed = previousState.TimeElapsed + timeToReachNode;
            _otherPartyValve = null;
            _otherPartyTravelTime = 0;
        } else {
            if (previousState._otherPartyValve == null)
            {
                // This is the first state after the initial one, in a two-party search.
                CurrentValve = previousState.CurrentValve;
                TimeElapsed = previousState.TimeElapsed;
                _otherPartyValve = nextNode;
                _otherPartyTravelTime = timeToReachNode;
            } else if (timeToReachNode <= previousState._otherPartyTravelTime) {
                // This node will be reached before the other party reaches theirs.
                CurrentValve = nextNode;
                TimeElapsed = previousState.TimeElapsed + timeToReachNode;
                _otherPartyValve = previousState._otherPartyValve;
                _otherPartyTravelTime = previousState._otherPartyTravelTime - timeToReachNode;
            } else {
                // The other party will reach their node first, so swap parties.
                CurrentValve = previousState._otherPartyValve;
                TimeElapsed = previousState.TimeElapsed + previousState._otherPartyTravelTime;
                _otherPartyValve = nextNode;
                _otherPartyTravelTime = timeToReachNode - previousState._otherPartyTravelTime;
            }
        }
    }

    internal bool NodeVisited(int nodeIndex)
    {
        return (_visitedNodes & (1 << nodeIndex)) != 0;
    }

    protected int AddNodeToVisitedMask(int oldMask, Valve newNode)
    {
        return oldMask | (1 << newNode.SignificantValveIndex);
    }
}

internal class Valve
{
    internal string Name { get; init; }
    internal int FlowRate { get; set; } = 0;
    internal List<Valve> DirectConnections = new List<Valve>();
    internal List<int> SignificantValveDistances = new List<int>();
    internal int SignificantValveIndex = -1;
    internal bool VisitedInCurrentSearch = false;
    internal int DistanceInCurrentSearch = int.MaxValue;

    internal Valve(string name)
    {
        Name = name;
    }

    internal void ResetForNewSearch()
    {
        VisitedInCurrentSearch = false;
        DistanceInCurrentSearch = int.MaxValue;
    }
}