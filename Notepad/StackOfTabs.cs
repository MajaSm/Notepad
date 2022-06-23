using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Notepad
{
    public class StackOfTabs
    {
        public struct Tab
        {
            public TextBox TextBox;
            public CheckBox CheckBox;
            public Button DeleteButton;
            public Grid Grid;
            public Color Color;
        }

        List<Tab> _listOfTabs = new List<Tab>();
        private StackPanel _stackPanel;
        private Note _selectedNote;
        private Button _buttonAddTab;
        private ListOfNotes _listOfNotes;

        public StackOfTabs(StackPanel stackPanel, ListOfNotes listOfNotes ,Button buttonAddTab)
        {
            _stackPanel = stackPanel;
            _buttonAddTab = buttonAddTab;
            _listOfNotes = listOfNotes;
            _buttonAddTab.Click += CreatingTab_Click;
        }

        private void SetAddTabButtonVisibility(bool value)
        {
            if(value == true)
            {
                _buttonAddTab.Visibility = Visibility.Visible;
            }
            else
            {
                _buttonAddTab.Visibility = Visibility.Hidden;
            }
        }
     
        public void SetContent(int noteIndex)
        {
            SaveTabs();
            CleanTabs();
            if (_listOfNotes.Notes.Count <= noteIndex)
                return;

            _selectedNote = _listOfNotes.Notes[noteIndex];

            SetAddTabButtonVisibility(_selectedNote != null);

            if (_selectedNote == null)
                return;

            for (int i = 0; i < _selectedNote.NoteContent.ListOfNoteTabs.Count; i++)
            {
                CreateTab(_selectedNote.NoteContent.ListOfNoteTabs[i].Note, _selectedNote.NoteContent.ListOfNoteTabs[i].Done);
            }
        }

        private void CleanTabs()
        {
            for (int i = 0; i < _listOfTabs.Count; i++)
            {
                _listOfTabs[i].Grid.Children.Clear();
                _stackPanel.Children.Remove(_listOfTabs[i].Grid);
            }
            _listOfTabs.Clear();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs args)
        {
            Button clickedButton = sender as Button;

            foreach (Tab tab in _listOfTabs)
            {
                if (clickedButton == tab.DeleteButton)
                {
                    DeleteTab(tab);
                    break;
                }
            }
            SaveTabs();
        }

        private void CheckBoxDone_Check(object sender, RoutedEventArgs args)
        {
            CheckBox clickedCheckBox = sender as CheckBox;

            foreach (Tab tab in _listOfTabs)
            {
                if (clickedCheckBox == tab.CheckBox)
                {
                    BgColorOfTextBox(tab);
                    break;
                }
            }

            SaveTabs();
        }

        private void SaveTabs()
        {
            if (_selectedNote == null)
                return;

            _selectedNote.NoteContent.ListOfNoteTabs.Clear(); //when you click on another Note Button, Tabs from previous Note are removed.

            for (int i = 0; i < _listOfTabs.Count; i++)
            {
                NoteTab noteContent = new NoteTab();
                noteContent.Note = _listOfTabs[i].TextBox.Text;
                noteContent.Done = _listOfTabs[i].CheckBox.IsChecked.Value;
                _selectedNote.NoteContent.ListOfNoteTabs.Add(noteContent);
            }
        }

        private void BgColorOfTextBox(Tab tab)
        {
            if (tab.CheckBox.IsChecked == true)
            {
                tab.CheckBox.Foreground = Brushes.Green;
                tab.CheckBox.Content = "DONE";
                tab.TextBox.Foreground = Brushes.Black;
                tab.DeleteButton.BorderBrush = Brushes.Black;
                tab.DeleteButton.Foreground = Brushes.Black;
                tab.Grid.Background = new SolidColorBrush(Color.FromRgb(120, 175, 133));
                tab.TextBox.Background = new SolidColorBrush(Color.FromRgb(120, 175, 133));
                tab.DeleteButton.Style = tab.DeleteButton.TryFindResource("ButtonStyle3") as Style;
            }
            else
            {
                tab.CheckBox.Foreground = new SolidColorBrush(Color.FromRgb(231, 24, 95));
                tab.CheckBox.Content = "TO-DO";
                tab.DeleteButton.BorderBrush = Brushes.WhiteSmoke;
                tab.DeleteButton.Foreground = Brushes.WhiteSmoke;
                tab.TextBox.Foreground = new SolidColorBrush(Color.FromRgb(209, 208, 215));
                tab.Grid.Background = new SolidColorBrush(Color.FromRgb(57, 54, 79));
                tab.TextBox.Background = new SolidColorBrush(Color.FromRgb(57, 54, 79));
                tab.DeleteButton.Style = tab.DeleteButton.TryFindResource("ButtonStyle2") as Style;
            }
        }

        private void DeleteTab(Tab tab)
        {
            _stackPanel.Children.Remove(tab.Grid);
            _listOfTabs.Remove(tab);
        }

        private void CreatingTab_Click(object sender, RoutedEventArgs e)
        {
            CreateTab("", false);
        }

        private void CreateTab(string text, bool isChecked)
        {
            TextBox newNote = new TextBox();
            CheckBox newCheckBox = new CheckBox();
            Button newButtonDelete = new Button();
            Grid newGrid = new Grid();
            RowDefinition rowDef = new RowDefinition();
            newNote.AcceptsReturn = true;
            newButtonDelete.Click += ButtonDelete_Click;
            newCheckBox.Click += CheckBoxDone_Check;

            rowDef.MinHeight = 15;
            newGrid.Margin = new Thickness(0, 0, 0, 20);
            newGrid.RowDefinitions.Add(rowDef);

            ColumnDefinition colDef1 = new ColumnDefinition();
            ColumnDefinition colDef2 = new ColumnDefinition();
            ColumnDefinition colDef3 = new ColumnDefinition();
            ColumnDefinition colDef4 = new ColumnDefinition();

            colDef1.Width = new GridLength(550);
            colDef2.Width = new GridLength(100);
            colDef3.Width = new GridLength(5);
            colDef4.Width = new GridLength(50);

            newGrid.ColumnDefinitions.Add(colDef1);
            newGrid.ColumnDefinitions.Add(colDef2);
            newGrid.ColumnDefinitions.Add(colDef3);
            newGrid.ColumnDefinitions.Add(colDef4);

            newGrid.Margin = new Thickness(0, 5, 23, 5);

            newCheckBox.Content = "TO-DO";
            newCheckBox.FontWeight = FontWeights.Bold;
            newCheckBox.Foreground = Brushes.Green;
            newCheckBox.IsChecked = isChecked;
            newCheckBox.Margin = new Thickness(30, 0, 0, 0);
            newCheckBox.Width = 50;
            newCheckBox.Height = 40;
            newCheckBox.FontSize = 15;

            newNote.Width = 550;
            newNote.TextWrapping = TextWrapping.Wrap;
            newNote.Margin = new Thickness(0, 5, 0, 5);
            newNote.Text = text;
            newNote.FontSize = 14;
            newNote.Foreground = new SolidColorBrush(Color.FromRgb(209, 208, 215));
            newNote.HorizontalAlignment = HorizontalAlignment.Left;
            newNote.BorderThickness = new Thickness(0, 0, 0, 0);

            newButtonDelete.Width = 35;
            newButtonDelete.Height = 25;
            string buttonDeleteName = string.Format("X");
            newButtonDelete.FontWeight = FontWeights.Bold;

            newButtonDelete.Content = buttonDeleteName;
            newButtonDelete.Foreground = Brushes.WhiteSmoke;
            newButtonDelete.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            newButtonDelete.BorderThickness = new Thickness(0.6);
            newButtonDelete.BorderBrush = Brushes.WhiteSmoke;
            newButtonDelete.VerticalAlignment = VerticalAlignment.Center;
            newButtonDelete.HorizontalAlignment = HorizontalAlignment.Right;
            newButtonDelete.Margin = new Thickness(0, 0, 5, 0);

            Grid.SetRow(newNote, 0);
            Grid.SetColumn(newNote, 0);
            Grid.SetRow(newCheckBox, 0);
            Grid.SetColumn(newCheckBox, 1);
            Grid.SetRow(newButtonDelete, 0);
            Grid.SetColumn(newButtonDelete, 3);

            newGrid.Children.Add(newNote);
            newGrid.Children.Add(newCheckBox);
            newGrid.Children.Add(newButtonDelete);
            _stackPanel.Children.Add(newGrid);

            Tab tab = new Tab();
            tab.TextBox = newNote;
            tab.CheckBox = newCheckBox;
            tab.DeleteButton = newButtonDelete;
            tab.Grid = newGrid;
            BgColorOfTextBox(tab);

            newNote.LostKeyboardFocus += OnKeyboardFocusChanged;

            _listOfTabs.Add(tab);
        }

        private void OnKeyboardFocusChanged(object sender, KeyboardFocusChangedEventArgs e)
        {
            SaveTabs();
        }
    }
}
