using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace L22_Synenko
{
    public partial class MainWindow : Window
    {
        private string currentFilePath;
        private FontFamily selectedFontFamily;
        private double selectedFontSize;
        private bool isItalic;
        private bool isUnderlined;
        private bool isBold;
        private TextAlignment textAlignment;

        public MainWindow()
        {
            InitializeComponent();

            // Задати значення за замовчуванням
            selectedFontFamily = new FontFamily("Arial");
            selectedFontSize = 12;
            isItalic = false;
            isUnderlined = false;
            isBold = false;
            textAlignment = TextAlignment.Left;

            // Встановити початкові значення для елементів керування
            FontFamilyComboBox.ItemsSource = Fonts.SystemFontFamilies;
            FontFamilyComboBox.SelectedItem = selectedFontFamily;
            FontSizeComboBox.Text = selectedFontSize.ToString();
            TextAlignmentComboBox.SelectedItem = textAlignment;
        }

        private void OpenFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                OpenFile(filePath);
            }
        }

        private void OpenFile(string filePath)
        {
            try
            {
                TextRange textRange = new TextRange(Editor.Document.ContentStart, Editor.Document.ContentEnd);
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    textRange.Load(fileStream, DataFormats.Text);
                }

                currentFilePath = filePath;
                UpdateTitle();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while opening the file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;
                    SaveFile(filePath);
                }
            }
            else
            {
                SaveFile(currentFilePath);
            }
        }

        private void SaveFile(string filePath)
        {
            try
            {
                TextRange textRange = new TextRange(Editor.Document.ContentStart, Editor.Document.ContentEnd);
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    textRange.Save(fileStream, DataFormats.Text);
                }

                currentFilePath = filePath;
                UpdateTitle();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NewFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Editor.Document.Blocks.Clear();
            currentFilePath = null;
            UpdateTitle();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void FontFamilyComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (FontFamilyComboBox.SelectedItem != null)
            {
                selectedFontFamily = (FontFamily)FontFamilyComboBox.SelectedItem;
                ApplyFontProperties();
            }
        }

        private void FontSizeComboBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (double.TryParse(FontSizeComboBox.Text, out double fontSize))
            {
                selectedFontSize = fontSize;
                ApplyFontProperties();
            }
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            isItalic = !isItalic;
            ApplyFontProperties();
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            isUnderlined = !isUnderlined;
            ApplyFontProperties();
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            isBold = !isBold;
            ApplyFontProperties();
        }

        private void TextAlignmentComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (TextAlignmentComboBox.SelectedItem != null)
            {
                textAlignment = (TextAlignment)TextAlignmentComboBox.SelectedItem;
                ApplyTextAlignment();
            }
        }

        private void ApplyFontProperties()
        {
            Editor.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, selectedFontFamily);
            Editor.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, selectedFontSize);
            Editor.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, isItalic ? FontStyles.Italic : FontStyles.Normal);
            Editor.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, isBold ? FontWeights.Bold : FontWeights.Normal);
            Editor.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, isUnderlined ? TextDecorations.Underline : null);
        }

        private void ApplyTextAlignment()
        {
            Editor.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, textAlignment);
        }

        private void UpdateTitle()
        {
            string fileName = string.IsNullOrEmpty(currentFilePath) ? "Untitled" : Path.GetFileName(currentFilePath);
            Title = $"{fileName} - Text Editor";
        }
    }
}
