using System;

namespace PinkWpf.Animation.PathMarkupSyntaxParser
{
    public class InvalidTokenException : Exception
    {
        const short range = 200;

        private InvalidTokenException(string message) : base(message)
        {
        }

        public static InvalidTokenException Create(string message, int pos, string content)
        {
            var startPos = pos - range;
            var arrowPos = (int)range;
            if (startPos < 0)
            {
                startPos = 0;
                arrowPos = pos;
            }
            var endPos = pos + range;
            if (endPos > content.Length)
                endPos = content.Length;
            var length = endPos - startPos;
            return new InvalidTokenException(
                $"{message} (Position: {pos})\r\n\r\nDetails:\r\n{content.Substring(startPos, length).Insert(arrowPos, " --->")}"
            );
        }
    }
}
