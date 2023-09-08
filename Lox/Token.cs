namespace Lox
{
    public class Token
    {
        private TokenType type { get; set; }
        private string? lexeme { get; set; }
        private object? literal { get; set; }
        private int? line { get; set; }

        public Token(
            TokenType type,
            string lexeme,
            object? literal,
            int line)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        public override string ToString()
        {
            return $"{type} {lexeme} {literal}";
        }
    }
}