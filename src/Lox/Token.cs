namespace Lox
{
    public readonly struct Token(TokenType tokenType, string lexeme, object? literal, int line)
    {
        public TokenType TokenType { get; } = tokenType;
        public string Lexeme { get; } = lexeme;
        public object? Literal { get; } = literal;
        
        public readonly int _line = line;

        public override string ToString()
        {
            return $"{TokenType} {Lexeme} {Literal}";
        }
    }
}
