namespace Lox
{
    public static class LoxLog
    {
        private static bool HadErrors = false;

        public static void Error(int line, string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error {where}: {message}");
            HadErrors = true;
        }

        public static bool HasErrors()
        {
            return HadErrors;
        }
    }
}
