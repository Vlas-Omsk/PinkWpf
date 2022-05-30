using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PinkWpf.Animation.PathMarkupSyntaxParser
{
    public class Lexer : IEnumerable<Token>
    {
        public StreamReader Stream { get; private set; }

        private int _position;
        private int _startPosition;
        private Token _token;
        private StringBuilder _buffer = new StringBuilder();
        private char? _current;

        private char? Next
        {
            get
            {
                var next = Stream.Peek();
                return next == -1 ? null : (char?)next;
            }
        }

        public Lexer(StreamReader stream)
        {
            Stream = stream;
        }

        public Lexer(Stream stream, Encoding encoding) : this(new StreamReader(stream, encoding))
        {
        }

        public Lexer(string source)
        {
            Stream = new StreamReader(new MemoryStream(Encoding.Default.GetBytes(source)));
        }

        public IEnumerator<Token> GetEnumerator()
        {
            Stream.BaseStream.Position = 0;
            _position = 0;
            _startPosition = 0;
            while (!Stream.EndOfStream)
            {
                Get();
                if (_token.Type == TokenType.Invalid)
                    throw InvalidTokenException.Create("Unknown element!", _position, "");
                if (_token.Type != TokenType.Invisible)
                    yield return _token;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void MoveNext()
        {
            _position++;
            var current = Stream.Read();
            _current = current == -1 ? null : (char?)current;
            _buffer.Append(_current);
        }

        private void MoveWhile(Predicate<char> predicate, Action callback = null)
        {
            while (Next.HasValue && predicate(Next.Value))
            {
                MoveNext();
                callback?.Invoke();
            }
        }

        private void Get()
        {
            _token = new Token();
            _buffer.Clear();
            MoveNext();
            _token.Position = _startPosition = _position;

            switch (_current)
            {
                case ',':
                    _token.Type = TokenType.Invisible;
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
                    if (_current == '\0' || char.IsWhiteSpace(_current.Value))
                        ReadWhiteSpace();
                    else
                        ReadOther();
                    break;
            }

            _token.Length = _position - _startPosition;
        }

        private void ReadWhiteSpace()
        {
            MoveWhile(next => char.IsWhiteSpace(next) || next == '\0');
            _token.Type = TokenType.Invisible;
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
            MoveWhile(next => numberChars.Contains(next), () =>
            {
                if (_current == '.')
                {
                    if (isDouble)
                        throw new Exception("Invalid double number");
                    else
                        isDouble = true;
                }
                if (_current == '-' && _position != _startPosition)
                    throw new Exception("Invalid number");
            });

            var str = _buffer.ToString();

            if (!double.TryParse(str.Replace('.', ','), out double value))
                throw new Exception($"Invalid double number {str}");
            _token.Value = value;
            _token.Type = TokenType.Number;
        }

        private void ReadOther()
        {
            MoveWhile(next => char.IsLetter(next));

            _token.Type = TokenType.String;
            _token.Value = _buffer.ToString();
        }
    }
}
