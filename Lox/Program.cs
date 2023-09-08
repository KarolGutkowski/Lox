
namespace Lox
{
    public class Lox
    {
        static void Main(string[] args)
        {
            if(args.Length > 1)
            {
                Console.WriteLine("Usage: cslox [script]");
                Environment.Exit(64);
            } else if(args.Length == 1)
            {
                runFile(args[0]);
            }
            else
            {
                runPrompt();
            }
        }

        private static void runFile(string path)
        {
            string source = System.IO.File.ReadAllText(path);
            run(source);

            if (_hadError)
                Environment.Exit(65);
        }

        private static void runPrompt()
        {
            for(; ;)
            {
                Console.Write("> ");
                string? line = Console.ReadLine();
                if (line == null) break;
                run(line);
                _hadError = false;
            }
        }

        private static void run(string source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.scanTokens();

            foreach(Token token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        public static void error(int line, string message)
        {
            report(line, "", message);
        }
        private static bool _hadError = false;
        private static void report(int line, string where, string message)
        {
            System.Console.Error.WriteLine($"[line {line}] Error {where} : {message}");
            _hadError = true;
        }
    }
}