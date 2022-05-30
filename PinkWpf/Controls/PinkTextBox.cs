using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PinkWpf.Controls
{
    public class PinkTextBox : TextBox
    {
        private bool _lockTextChanged = false;

        private void ReplaceTextWithoutTriggers(string value)
        {
            var selectionStart = SelectionStart;
            var selectionLength = SelectionLength;
            _lockTextChanged = true;
            SetCurrentValue(TextProperty, value);
            _lockTextChanged = false;
            Select(selectionStart, selectionLength);
        }

        private void UpdateMask()
        {
            if (HasPasswordChar)
            {
                var length = 0;
                if (SourceText != null)
                    length = SourceText.Length;
                ReplaceTextWithoutTriggers(new string(PasswordChar.Value, length));
            }
            else
            {
                ReplaceTextWithoutTriggers(SourceText);
            }
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                if (HasText)
                    SetCurrentHasText(false);
            }
            else
            {
                if (!HasText)
                    SetCurrentHasText(true);
            }

            if (_lockTextChanged)
                return;

            SetCurrentSourceText(TextBoxHelper.GetTextFromChanges(SourceText, Text, e.Changes, out int selectionStartOffset, ValidateText));
            UpdateMask();

            base.OnTextChanged(e);
        }

        private string ValidateText(string addedString, int length)
        {
            if (AvailableSymbolsRegex != null)
            {
                var matches = AvailableSymbolsRegex.Matches(addedString);
                addedString = string.Concat(matches.OfType<Match>().Select(match => match.Value));
                SelectionStart -= length - addedString.Length;
            }
            return addedString;
        }

        #region CornerRadiusProperty
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public readonly static DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(PinkTextBox));
        #endregion

        #region PlaceholderProperty
        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public readonly static DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(PinkTextBox));
        #endregion

        #region PlaceholderForegroundProperty
        public Brush PlaceholderForeground
        {
            get => (Brush)GetValue(PlaceholderForegroundProperty);
            set => SetValue(PlaceholderForegroundProperty, value);
        }

        public readonly static DependencyProperty PlaceholderForegroundProperty =
            DependencyProperty.Register(nameof(PlaceholderForeground), typeof(Brush), typeof(PinkTextBox));
        #endregion

        #region HasTextProperty
        public bool HasText
        {
            get => (bool)GetValue(HasTextProperty);
        }

        private void SetCurrentHasText(bool value)
        {
            SetValue(HasTextPropertyKey, value);
        }

        private readonly static DependencyPropertyKey HasTextPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(HasText), typeof(bool), typeof(PinkTextBox), new PropertyMetadata());

        public readonly static DependencyProperty HasTextProperty = HasTextPropertyKey.DependencyProperty;
        #endregion

        #region PasswordCharProperty
        public char? PasswordChar
        {
            get => (char?)GetValue(PasswordCharProperty);
            set => SetValue(PasswordCharProperty, value);
        }

        public readonly static DependencyProperty PasswordCharProperty =
            DependencyProperty.Register(nameof(PasswordChar), typeof(char?), typeof(PinkTextBox), new PropertyMetadata(OnPasswordCharChanged));

        private static void OnPasswordCharChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (char?)e.NewValue;
            var textBoxPlaceholder = (PinkTextBox)d;
            if (newValue.HasValue != textBoxPlaceholder.HasPasswordChar)
                textBoxPlaceholder.SetCurrentHasPasswordChar(newValue.HasValue);

            if (newValue.HasValue)
            {
                if (e.NewValue != e.OldValue && textBoxPlaceholder.HasPasswordChar)
                    textBoxPlaceholder.UpdateMask();
            }
            else
            {
                textBoxPlaceholder.ReplaceTextWithoutTriggers(textBoxPlaceholder.SourceText);
            }
        }
        #endregion

        #region HasPasswordCharProperty
        public bool HasPasswordChar
        {
            get => (bool)GetValue(HasPasswordCharProperty);
        }

        private void SetCurrentHasPasswordChar(bool value)
        {
            SetValue(HasPasswordCharPropertyKey, value);
        }

        private readonly static DependencyPropertyKey HasPasswordCharPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(HasPasswordChar), typeof(bool), typeof(PinkTextBox), new PropertyMetadata());

        public readonly static DependencyProperty HasPasswordCharProperty = HasPasswordCharPropertyKey.DependencyProperty;
        #endregion

        #region SourceTextProperty
        public string SourceText
        {
            get => (string)GetValue(SourceTextProperty);
            set => SetValue(SourceTextProperty, value);
        }

        private void SetCurrentSourceText(string value)
        {
            SetValue(SourceTextProperty, value);
        }

        public readonly static DependencyProperty SourceTextProperty =
            DependencyProperty.Register(nameof(SourceText), typeof(string), typeof(PinkTextBox), new FrameworkPropertyMetadata(OnSourceTextChanged) { BindsTwoWayByDefault = true });

        private static void OnSourceTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBoxPlaceholder = (PinkTextBox)d;
            textBoxPlaceholder.UpdateMask();
        }
        #endregion

        #region AvailableSymbolsRegex
        public Regex AvailableSymbolsRegex
        {
            get => (Regex)GetValue(AvailableSymbolsRegexProperty);
            set => SetValue(AvailableSymbolsRegexProperty, value);
        }

        public readonly static DependencyProperty AvailableSymbolsRegexProperty =
            DependencyProperty.Register(nameof(AvailableSymbolsRegex), typeof(Regex), typeof(PinkTextBox));
        #endregion
    }
}
