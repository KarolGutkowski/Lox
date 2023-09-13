using static Lox.TokenType;
namespace Lox
{
    internal class Scanner
    {
        private string Source { get; }
        private List<Token> tokens = new List<Token>();

        public Scanner(string source)
        {
            Source = source;
        }

        private int start = 0;
        private int current = 0;
        private int line = 1;
        public List<Token> scanTokens()
        {
            while (!isAtEnd())
            {
                // We are at the beginning of the next lexeme.
                start = current;
                scanToken();
            }

            tokens.Add(new Token(EOF, "", null, line));
            return tokens;
        }


        private static Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
        {
            {"and",    AND},
            {"class",  CLASS},
            {"else",   ELSE},
            {"false",  FALSE},
            {"for",    FOR},
            {"fun",    FUN},
            {"if",     IF},
            {"nil",    NIL},
            {"or",     OR},
            {"print",  PRINT},
            {"return", RETURN},
            {"super",  SUPER},
            {"this",   THIS},
            {"true",   TRUE},
            {"var",    VAR},
            {"while",  WHILE }
        };
    private bool isAtEnd()
        {
            return current >= Source.Length;
        }

        private void scanToken()
        {
            char c = advance();
            switch(c)
            {
                case '(': addToken(LEFT_PAREN); break;
                case ')': addToken(RIGHT_PAREN); break;
                case '{': addToken(LEFT_BRACE); break;
                case '}': addToken(RIGHT_BRACE); break;
                case ',': addToken(COMMA); break;
                case '.': addToken(DOT); break;
                case '-': addToken(MINUS); break;
                case '+': addToken(PLUS); break;
                case ';': addToken(SEMICOLON); break;
                case '*': addToken(STAR); break;
                case '!':
                    addToken(match('=') ? BANG_EQUAL : BANG);
                    break;
                case '=':
                    addToken(match('=') ? EQUAL_EQUAL : EQUAL);
                    break;
                case '<':
                    addToken(match('=') ? LESS_EQUAL : LESS);
                    break;
                case '>':
                    addToken(match('=') ? GREATER_EQUAL : GREATER);
                    break;
                case '/':
                    if (match('/'))
                    {
                        // A comment goes until the end of the line.
                        while (peek() != '\n' && !isAtEnd()) advance();
                    }
                    else if(match('*'))
                    {
                        getMultLineComment();
                    }
                    else
                    {
                        addToken(SLASH);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace.
                    break;

                case '\n':
                    line++;
                    break;
                case '"': 
                    getString(); 
                    break;
                default:
                    if (isDigit(c))
                    {
                        number();
                    }
                    else if (isAlpha(c))
                    {
                        identifier();
                    }
                    else
                    {
                        Lox.error(line, "Unexpected character.");
                    }
                    break;
            }
        }

        private void getMultLineComment()
        {
            while((peek()!='*' || peekNext()!='/') && !isAtEnd())
            {
                if (peek() == '\n') line++;
                advance();

                if (isAtEnd())
                {
                    returnUnfinishedMutliLineCommentError();
                    return;
                }

                if (peek() == '\n') line++;
                advance();
            }

            if (isAtEnd())
            {
                returnUnfinishedMutliLineCommentError();
                return;
            }

            advance();
            advance();
        }

        private void returnUnfinishedMutliLineCommentError()
        {
            Lox.error(line, "Unclosed multiline comment (expected format /* comment */).");
            return;
        }

        private void identifier()
        {
            while (isAlphaNumeric(peek())) advance();

            string text = Source.Substring(start, current - start);
            TokenType type;
            bool mappedToKeyWord = keywords.TryGetValue(text, out type);

            if (!mappedToKeyWord)
                type = IDENTIFIER;

            addToken(type);
        }

        private bool isAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                    c == '_';
        }

        private bool isAlphaNumeric(char c)
        {
            return isAlpha(c) || isDigit(c);
        }

        private void number()
        {
            while(isDigit(peek())) 
                advance();

            if(peek() == '.' && isDigit(peekNext()))
            {
                advance();
                while (isDigit(peek())) 
                    advance();
            }

            addToken(NUMBER, Double.Parse(Source.Substring(start, current - start)));
        }

        private char peekNext()
        {
            if (current + 1 >= Source.Length) return '\0';
            return Source.ElementAt(current+1);
        }

        private bool isDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private void getString()
        {
            while (peek() != '"' && !isAtEnd())
            {
                if (peek() == '\n') line++;
                advance();
            }

            if (isAtEnd())
            {
                Lox.error(line, "Unterminated string.");
                return;
            }

            advance();

            string value = Source.Substring(start + 1, (current -1) - (start + 1));
            addToken(STRING, value);
        }

        private char peek()
        {
            if (isAtEnd()) return '\0';
            return Source.ElementAt(current);
        }

        private bool match(char expected)
        {
            if (isAtEnd()) return false;
            if(Source.ElementAt(current)!=expected)
                return false;

            current++;
            return true;
        }

        private char advance()
        {
            return Source.ElementAt(current++);
        }

        private void addToken(TokenType type)
        {
            addToken(type, null);
        }

        private void addToken(TokenType type, Object? literal)
        {
            String text = Source.Substring(start, current-start);
            tokens.Add(new Token(type, text, literal, line));
        }


    }

}