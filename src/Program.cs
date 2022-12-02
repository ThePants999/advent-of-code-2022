namespace Patersoft.AOC.AOC22;

using Patersoft.AOC;
using Microsoft.Extensions.Logging;

public class AOC22
{
    public static void Main(string[] args)
    {
        using ILoggerFactory loggerFactory =
            LoggerFactory.Create(builder =>
                builder.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.SingleLine = true;
                    options.TimestampFormat = "HH:mm:ss ";
                }).SetMinimumLevel(LogLevel.Warning));
        ILogger<AOCRunner> logger = loggerFactory.CreateLogger<AOCRunner>();

        AOCRunner runner = AOCRunner.BuildRunner(logger, new System.Type[] {
            typeof(Day1),
            typeof(Day2),
        });
        runner.Run(2, 2);
    }
}
