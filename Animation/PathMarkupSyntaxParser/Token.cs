namespace PinkWpf.Animation.PathMarkupSyntaxParser
{
    public class Token
    {
        public TokenType Type { get; set; } = TokenType.Invalid;
        public int Position { get; set; }
        public int Length { get; set; }
        public object Value { get; set; }

        public Token()
        {
        }

        public Token(TokenType type, int position, int length, object value)
        {
            Type = type;
            Position = position;
            Length = length;
            Value = value;
        }
    }
}
