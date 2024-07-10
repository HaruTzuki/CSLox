namespace Lox
{
    internal readonly struct Token(TokenType tokenType, string lexeme, object? literal, int line)
    {
        private readonly int _line = line;

        public override string ToString()
        {
            return $"{tokenType} {lexeme} {literal}";
        }
    }
}
