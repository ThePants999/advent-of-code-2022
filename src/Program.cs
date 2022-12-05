namespace Patersoft.AOC.AOC22;

using Patersoft.AOC;
//using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

public class AOC22
{
    public static void Main(string[] args)
    {
        /*
        using ILoggerFactory loggerFactory =
            LoggerFactory.Create(builder =>
                builder.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.SingleLine = true;
                    options.TimestampFormat = "HH:mm:ss ";
                }).SetMinimumLevel(LogLevel.Warning));
        ILogger<AOCRunner> logger = loggerFactory.CreateLogger<AOCRunner>();*/
        //ILogger<AOCRunner> logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<AOCRunner>.Instance;
        AOCRunner runner = AOCRunner.BuildRunner(NullLogger<AOCRunner>.Instance, new System.Type[] {
            typeof(Day1),
            typeof(Day2),
            typeof(Day3),
            typeof(Day4),
            typeof(Day5),
            typeof(Day6),
            typeof(Day7),
            typeof(Day8),
            typeof(Day9),
            typeof(Day10),
            typeof(Day11),
            typeof(Day12),
            typeof(Day13),
            typeof(Day14),
            typeof(Day15),
            typeof(Day16),
            typeof(Day17),
            typeof(Day18),
            typeof(Day19),
            typeof(Day20),
            typeof(Day21),
            typeof(Day22),
            typeof(Day23),
            typeof(Day24),
            typeof(Day25),
        });
        runner.Run(1, 7);
    }
}
