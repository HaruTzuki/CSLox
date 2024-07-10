using System.Collections;

namespace Lox
{
    internal class Scanner(string source)
    {
        #region Fields

        /// <summary>
        /// The list of tokens that the Scanner had scanned.
        /// </summary>
        private readonly List<Token> _tokens = [];

        /// <summary>
        /// Reserved Keywords from system.
        /// </summary>
        private readonly Hashtable _keywords = new()
        {
            { "and", TokenType.AND},
            { "class", TokenType.CLASS},
            { "else", TokenType.ELSE},
            { "false", TokenType.FALSE},
            { "for", TokenType.FOR},
            { "fun", TokenType.FUN},
            { "if", TokenType.IF},
            { "nil", TokenType.NIL},
            { "or", TokenType.OR},
            { "print", TokenType.PRINT},
            { "return", TokenType.RETURN},
            { "super", TokenType.SUPER},
            { "this", TokenType.THIS},
            { "true", TokenType.TRUE},
            { "var", TokenType.VAR},
            { "while", TokenType.WHILE},
        };

        /// <summary>
        /// Cursor which indicates every time that Scanner try to identify the token the starting point.
        /// </summary>
        private int _start;
        /// <summary>
        /// Cursor which indicates every time that Scanner try to identify the token the last character that it is read.
        /// </summary>
        private int _current;
        /// <summary>
        /// The line of execution.
        ///     It is helpful for error Handling.
        /// </summary>
        private int _line = 1;

        #endregion

        /// <summary>
        /// Parses the initial text and try to break it into tokens.
        /// </summary>
        /// <returns>Returns the List of the available tokens.</returns>
        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                // We are at the beginning of the next lexeme.
                // Sets the "_current" value to "_start" value because this will run until find the \0 of the file.
                // So, in every parse "_start" cursor but be equal with the last known position of the "_current" cursor.
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(TokenType.EOF, "", null, _line));
            return _tokens;
        }
        
        /// <summary>
        /// Starts parsing the text char by char.
        ///     Trys to tokenise every character.
        /// </summary>
        private void ScanToken()
        {
            // Gets the next character.
            // Initially "_current" cursor is 0. So, when Next() being called the "_current" it will be equal to 1.
            var c = Next();

            switch (c)
            {
                case '(':
                    AddToken(TokenType.LEFT_PAREN);
                    break;
                case ')':
                    AddToken(TokenType.RIGHT_PAREN);
                    break;
                case '{':
                    AddToken(TokenType.LEFT_BRACE);
                    break;
                case '}':
                    AddToken(TokenType.RIGHT_BRACE);
                    break;
                case ',':
                    AddToken(TokenType.COMMA);
                    break;
                case '.':
                    AddToken(TokenType.DOT);
                    break;
                case '-':
                    AddToken(TokenType.MINUS);
                    break;
                case '+':
                    AddToken(TokenType.PLUS);
                    break;
                case ';':
                    AddToken(TokenType.SEMICOLON);
                    break;
                case '*':
                    AddToken(TokenType.STAR);
                    break;
                case '!':
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                case '/':
                    if (Match('/'))
                    {
                        // A command goes until the end of the line.
                        while (Peek() != '\n' && !IsAtEnd())
                        {
                            Next();
                        }
                    }
                    else if (Match('*'))
                    {
                        // A multi line command.
                        MultilineComment();
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace.
                    break;
                case '\n':
                    _line++;
                    break;
                case '"':
                    StringValue();
                    break;
                default:

                    if (char.IsDigit(c))
                    {
                        NumberValue();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        LoxLog.Error(_line, "Unexpected character.");
                    }
                    break;

            }
        }

        private void MultilineComment()
        {
            while (Peek() != '*' && PeekNext() != '/' && !IsAtEnd())
            {
                if (Peek() == '\n')
                {
                    _line++;
                }

                Next();
            }

            if (IsAtEnd())
            {
                LoxLog.Error(_line, "Unterminated Comment");
            }

            Next();
            Next();
        }

        #region Keywords, Numbers and Strings checking

        /// <summary>
        /// Checks from reserved keywords. 
        /// </summary>
        private void Identifier()
        {
            while (IsAlphaNumeric(Peek()))
            {
                Next();
            }

            var text = source.Substring(_start, _current - _start);
            var tokenType = (TokenType)(_keywords[text] ?? TokenType.IDENTIFIER);

            AddToken(tokenType);
        }
    
        /// <summary>
        /// Checks for Numbers.
        /// </summary>
        private void NumberValue()
        {
            // Parses the characters until find a character which is not digit.
            while (char.IsDigit(Peek()))
            {
                Next();
            }
            
            // Look for a fractional part.
            if (Peek() == '.' && char.IsDigit(PeekNext()))
            {
                // Consume the "."
                Next();
                
                // First loop has been interrupted because it finds "."
                // for this reason we need to start over again the loop to check for numbers
                // because Lox identify Integers and Decimal in the same way.
                while (char.IsDigit(Peek()))
                {
                    Next();
                }
            }
            
            AddToken(TokenType.NUMBER, decimal.Parse(source.Substring(_start, _current - _start)));
        }

        /// <summary>
        /// Checks for String.
        /// </summary>
        private void StringValue()
        {
            // From function "ScanToken()" we known the "_current" cursor is equal to '"'. 
            // So, we need to get the next character in order to find the end of the string literal.
            while(Peek() != '"' && !IsAtEnd())
            {
                // Lox supports multi-line strings. We need to take it into consideration
                // in order to increase the line.
                if(Peek() == '\n')
                {
                    _line++;
                }

                Next();
            }
            
            // It must be a check if it is E.O.F. in order to throw an error and the whole process to stop.
            if (IsAtEnd())
            {
                LoxLog.Error(_line, "Unterminated string.");
                return;
            }

            // The closing " " ".
            // Previous while stops when it finds the """.
            Next();

            // Trim the surrounding quotes.
            var stringValue = source.Substring(_start + 1, (_current - _start) - 1);
            AddToken(TokenType.STRING, stringValue);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the next character. It moves the "_current" cursor.
        /// </summary>
        /// <returns></returns>
        private char Next()
        {
            return source[_current++];
        }
    
        /// <summary>
        /// Adds tokens with its values to the list object.
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="literal"></param>
        private void AddToken(TokenType tokenType, object? literal = null)
        {
            var text = source.Substring(_start, _current - _start);
            _tokens.Add(new Token(tokenType, text, literal, _line));
        }

        /// <summary>
        /// Gets the Current character from "_current" cursor.
        /// </summary>
        /// <returns></returns>
        private char Peek()
        {
            return IsAtEnd() ? '\0' : source[_current];
        }

        /// <summary>
        /// Gets the next character from the character that had been returned from Peek()
        /// </summary>
        /// <returns></returns>
        private char PeekNext()
        {
            return _current + 1 >= source.Length ? '\0' : source[_current + 1];
        }

        /// <summary>
        /// Helps with complex operators.
        ///     Checks if expected character is equal with "_current" cursor.
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        private bool Match(char expected)
        {
            if (IsAtEnd())
                return false;

            if (source[_current] != expected)
            {
                return false;
            }

            _current++;
            return true;
        }
        
        /// <summary>
        /// Checks if the character is Letter or Digit.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>If the character is letter or digit.</returns>
        private static bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || char.IsDigit(c);
        }

        /// <summary>
        /// Checks if character is character or underscore.
        ///     Underscore is valid scenario for Lox.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static bool IsAlpha(char c)
        {
            if (c >= 'a' && c <= 'z')
                return true;
            else if (c >= 'A' && c <= 'Z')
                return true;
            else if (c == '_')
                return true;

            return false;
        }
        
        /// <summary>
        /// It returns if the "_current" cursor reach on the end of the text. 
        /// </summary>
        /// <returns></returns>
        private bool IsAtEnd()
        {
            return _current >= source.Length;
        }

        #endregion
    }
}
