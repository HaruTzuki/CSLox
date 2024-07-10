using Lox;

namespace CSLox.Tests;

public class ScanningTest
{
    [Fact]
    public void Correct_Tokenise()
    {
        var code = "var age = 15;";

        var scanner = new Scanner(code);
        var tokens = scanner.ScanTokens();
        
        // We expect 6 tokens
        Assert.Equal(6, tokens.Count);
        
        // If previous assert it is correct then
        // we need to add some extra assertion checks.

        Assert.Equal(TokenType.VAR, tokens[0].TokenType);
        Assert.Equal(TokenType.IDENTIFIER, tokens[1].TokenType);
        Assert.Equal(TokenType.EQUAL, tokens[2].TokenType);
        Assert.Equal(TokenType.NUMBER, tokens[3].TokenType);
        Assert.Equal(TokenType.SEMICOLON, tokens[4].TokenType);
        
        // Last on must be for event EOF
        Assert.Equal(TokenType.EOF, tokens[5].TokenType);
    }
}