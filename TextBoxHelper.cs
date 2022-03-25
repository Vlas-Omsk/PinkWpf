using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace PinkWpf
{
    public static class TextBoxHelper
    {
        public static string GetTextFromChanges(string oldText, string newText, ICollection<TextChange> changes, out int selectionStartOffset, Func<string, int, string> validator = null)
        {
            oldText = oldText ?? string.Empty;
            selectionStartOffset = 0;
            foreach (var change in changes)
            {
                if (change.RemovedLength > 0)
                {
                    var offsetAfter = change.Offset + change.RemovedLength;
                    oldText = oldText.Substring(0, change.Offset) + oldText.Substring(offsetAfter, oldText.Length - offsetAfter);
                }
                if (change.AddedLength > 0)
                {
                    var addedString = newText.Substring(change.Offset, change.AddedLength);
                    var addedStringLength = addedString.Length;
                    if (validator != null)
                    {
                        addedString = validator(addedString, addedStringLength);
                        selectionStartOffset -= addedStringLength - addedString.Length;
                    }
                    oldText = oldText.Insert(change.Offset, addedString);
                }
            }
            return oldText;
        }
    }
}
