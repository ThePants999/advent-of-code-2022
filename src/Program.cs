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
        });
        runner.Run(1, 5);
    }
}
