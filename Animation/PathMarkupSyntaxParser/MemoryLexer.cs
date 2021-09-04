using System;
using System.Collections.Generic;
using System.Linq;

namespace PinkWpf.Animation.PathMarkupSyntaxParser
{
    public class MemoryLexer : IDisposable
    {
        public string Content { get; set; }
        public List<Token> Tokens { get; private set; } = new();

        private int _startPosition;
        private int _position;
        private TokenType _type;
        private object _value;

        private char? Current => _position >= Content.Length ? null : Content[_position];
        private string CurrentString => Content.Substring(_startPosition, _position - _startPosition);

        public void Tokenize(string content)
        {
            Content = content;
            Tokens = new();
            Token token;
            for (_position = 0; _position < Content.Length;)
            {
                token = Get();
                if (token.Type == TokenType.Invalid)
                    throw InvalidTokenException.Create("Unknown element!", _position, Content);
                if (token.Type != TokenType.Invisible)
                    Tokens.Add(token);

                if (_position == _startPosition)
                    _position++;
            }
            _startPosition = 0;
            _position = 0;
            _value = null;
        }

        private Token Get()
        {
            _startPosition = _position;
            _type = TokenType.Invalid;
            _value = null;

            switch (Current)
            {
                case ',':
                    _type = TokenType.Invisible;
                    break;
                case '-':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '0':
                    ReadNumber();
                    break;
                default:
                    if (Current == '\0' || char.IsWhiteSpace(Current.Value))
                        ReadWhiteSpace();
                    else
                        ReadOther();
                    break;
            }

            return new Token(_type, _startPosition, _position - _startPosition, _value);
        }

        private void ReadWhiteSpace()
        {
            while (Current.HasValue && (char.IsWhiteSpace(Current.Value) || Current == '\0'))
                _position++;
            _type = TokenType.Invisible;
        }

        private readonly static char[] numberChars =
        {
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
            '.',
            '-'
        };
        private void ReadNumber()
        {
            bool isDouble = false;
            while (Current.HasValue && numberChars.Contains(Current.Value))
            {
                if (Current == '.')
                {
                    if (isDouble)
                        throw new Exception("Invalid double number");
                    else
                        isDouble = true;
                }
                if (Current == '-' && _position != _startPosition)
                    throw new Exception("Invalid number");

                _position++;
            }

            var str = CurrentString;

            if (!double.TryParse(str.Replace('.', ','), out double value))
                throw new Exception($"Invalid double number {str}");
            _value = value;
            _type = TokenType.Number;
        }

        private void ReadOther()
        {
            while (Current.HasValue && char.IsLetter(Current.Value))
                _position++;

            _value = CurrentString;
            _type = TokenType.String;
        }

        public void Dispose()
        {
            Content = null;
            Tokens = null;
        }
    }
}
