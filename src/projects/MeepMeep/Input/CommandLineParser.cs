using CommandLine;

namespace MeepMeep.Input
{
    /// <summary>
    /// Simple command line parser.
    /// </summary>
    public class CommandLineParser
    {
        protected readonly Parser InnerParser;

        public CommandLineParser()
        {
            InnerParser = new Parser(cfg =>
            {
                cfg.CaseSensitive = false;
                cfg.IgnoreUnknownArguments = false;
            });
        }

        public virtual bool Parse(MeepMeepOptions options, params string[] args)
        {
            return InnerParser.ParseArguments(args, options);
        }
    }
}