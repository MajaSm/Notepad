using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using System.Windows.Media;

namespace Notepad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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
        List<Note> _listOfNotes = new List<Note>();

        private Note _selectedNote;
        private Note _rightClickedNote;
        private SavingSystem _savingSystem;

        public MainWindow()
        {
            InitializeComponent();

            KeyDown += PressEnter;
            MouseLeftButtonDown += OnMouseLeftClick;

            _savingSystem = new SavingSystem();

            ListOfNotes notes = _savingSystem.LoadNoteData();

            if(notes != null)
            {
                for(int i = 0; i < notes.Notes.Count; i++)
                {
                    CreateNoteButton(notes.Notes[i]);
                }
            }

            SetAddTabButtonVisibility(false);
        }
        private void SetAddTabButtonVisibility(bool isVisible)
        {
            ButtonAddTab.IsEnabled = isVisible;
            if (isVisible)
            {
                ButtonAddTab.Visibility = Visibility.Visible;
            }
            else
            {
                ButtonAddTab.Visibility = Visibility.Hidden;
            }
        }

       

        private void ButtonAddScenario_Click(object sender, RoutedEventArgs e)
        {
            CreateNoteButton();
        }

        private void CreateNoteButton(NoteContent noteContent = null)
        {
            TextBox textBoxForNoteName = new TextBox();
            Button buttonForNotepad = new Button();

            textBoxForNoteName.IsEnabled = false;
            textBoxForNoteName.TextWrapping = TextWrapping.Wrap;
            textBoxForNoteName.Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            textBoxForNoteName.BorderThickness = new Thickness(0, 0, 0, 0);
            textBoxForNoteName.FontSize = 16;
            textBoxForNoteName.HorizontalContentAlignment = HorizontalAlignment.Center;
            textBoxForNoteName.VerticalContentAlignment = VerticalAlignment.Center;
            textBoxForNoteName.Foreground = Brushes.White;
            textBoxForNoteName.ContextMenu = new ContextMenu();

            buttonForNotepad.Width = 200;
            buttonForNotepad.MinHeight = 50;
            buttonForNotepad.Foreground = Brushes.White;
            buttonForNotepad.Height = textBoxForNoteName.Height;
            buttonForNotepad.Background = new SolidColorBrush(Color.FromArgb(27, 25, 37, 100));
            buttonForNotepad.Margin = new Thickness(30, 5, 10, 10);

            string ScenarioNameDisplay = string.Format("Note{0}", _listOfNotes.Count);
            if (noteContent != null)
            {
                if (noteContent.Name != String.Empty)
                {
                    ScenarioNameDisplay = noteContent.Name;
                }
            }
            textBoxForNoteName.Text = ScenarioNameDisplay;

            buttonForNotepad.Content = textBoxForNoteName;
            scenarioName.Children.Add(buttonForNotepad);


            Note note = new Note();
            note.NoteReferences.NameTextBox = textBoxForNoteName;
            note.NoteReferences.Button = buttonForNotepad;
            if(noteContent != null)
            {
                note.NoteContent = noteContent;
            }
            _listOfNotes.Add(note);

            buttonForNotepad.Click += ButtonAddTab_Click;
            buttonForNotepad.Click += OnClickNameOfNoteButton;
            buttonForNotepad.MouseRightButtonDown += OnRightClickNameOfNote;
        }

        public void OnRightClickNameOfNote(object sender, MouseEventArgs e)
        {
            Button clickedButton = sender as Button;

            DisableNoteTextbox(_rightClickedNote);

            _rightClickedNote = FindNoteFromButton(clickedButton);

            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItemRename= new MenuItem();
            MenuItem menuItemDelete = new MenuItem();

            menuItemRename.Header = "Rename";
            menuItemDelete.Header = "Delete";

            menuItemRename.Click += OnRenameClicked;
            menuItemDelete.Click += OnDeleteNoteClicked;

            contextMenu.Items.Add(menuItemRename);
            contextMenu.Items.Add(menuItemDelete);

            clickedButton.ContextMenu = contextMenu;
          
        }

        private void OnDeleteNoteClicked(object sender, RoutedEventArgs e)
        {
            DeleteNote(_rightClickedNote);
        }

        private void DeleteNote(Note note)
        {
            scenarioName.Children.Remove(note.NoteReferences.Button);
            _listOfNotes.Remove(note);
            SetAddTabButtonVisibility(false);
            CleanTabs();
        }
        private void OnRenameClicked(object sender, EventArgs eventArgs)
        {
            _rightClickedNote.NoteReferences.NameTextBox.IsEnabled = true;
            _rightClickedNote.NoteReferences.NameTextBox.Focus();
        }

        private void OnMouseLeftClick(object sender, MouseEventArgs e)
        {
            DisableNoteTextbox(_rightClickedNote);
        }

        private void PressEnter(object sender, KeyEventArgs e)
        {
            if (Key.Enter == e.Key)
            {
                DisableNoteTextbox(_rightClickedNote);
            }
        }

        private void ButtonSaveAll_Click(object sender, RoutedEventArgs e)
        {
            SaveNote();
            _savingSystem.SaveNotes(_listOfNotes);
        }

        private void SaveNote()
        {
            if (_selectedNote == null)
                return;

            _selectedNote.NoteContent.ListOfNoteTabs.Clear();

            for(int i = 0; i < _listOfTabs.Count; i++)
            {
                NoteTab noteContent = new NoteTab();
                noteContent.Note = _listOfTabs[i].TextBox.Text;
                noteContent.Done = _listOfTabs[i].CheckBox.IsChecked.Value;
                _selectedNote.NoteContent.ListOfNoteTabs.Add(noteContent);
            }
        }

        private void DisableNoteTextbox(Note note)
        {
            if (note == null)
                return;

            note.NoteContent.Name = note.NoteReferences.NameTextBox.Text;
            note.NoteReferences.NameTextBox.IsEnabled = false;
        }
       
        private void CleanTabs()
        {
           for(int i = 0;i < _listOfTabs.Count; i++)
            {
                _listOfTabs[i].Grid.Children.Clear();
                notes.Children.Remove(_listOfTabs[i].Grid);
            } 
            _listOfTabs.Clear();
        }

        private void LoadNotes()
        {
            if (_selectedNote == null)
                return;
            
            for (int i = 0; i < _selectedNote.NoteContent.ListOfNoteTabs.Count; i++)
            {
                CreateTab(_selectedNote.NoteContent.ListOfNoteTabs[i].Note, _selectedNote.NoteContent.ListOfNoteTabs[i].Done);
            }
            
        }
        private Note FindNoteFromButton(Button button)
        {
            foreach (Note note in _listOfNotes)
            {
                if (button == note.NoteReferences.Button)
                {
                    return note;
                }
            }
            return null;
        }
        private void OnClickNameOfNoteButton(object sender, RoutedEventArgs args)
        {
            SaveNote();

            Button clickedButton = sender as Button;
            for (int i  = 0; i< _listOfNotes.Count;i++)
            {
                if(clickedButton == _listOfNotes[i].NoteReferences.Button)
                {
                    _listOfNotes[i].NoteReferences.Button.Background = new SolidColorBrush(Color.FromRgb(73, 76, 108));
                }
                else
                {
                    _listOfNotes[i].NoteReferences.Button.Background = new SolidColorBrush(Color.FromArgb(27, 25, 37, 100));
                }
            }
        
            _selectedNote = FindNoteFromButton(clickedButton);
            
            CleanTabs();
            LoadNotes();
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
            SaveNote();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs args)
        {
            Close();
           
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs args)
        {
           WindowState = WindowState.Minimized;
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
            SaveNote();
        }
        
        private void BgColorOfTextBox(Tab tab)
        {
            if(tab.CheckBox.IsChecked == true)
            {
                tab.Grid.Background = new SolidColorBrush(Color.FromRgb(209, 255, 213));
                tab.TextBox.Background = new SolidColorBrush(Color.FromRgb(209, 255, 213));
            }
            else
            {
                tab.Grid.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                tab.TextBox.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }

        private void DeleteTab(Tab tab)
        {
            notes.Children.Remove(tab.Grid);
            _listOfTabs.Remove(tab);
        }
        private void ButtonAddTab_Click(object sender, RoutedEventArgs e)
        {
            SetAddTabButtonVisibility(true);
        }

        private void TextBlock_Click(object sender, RoutedEventArgs e)
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

            newButtonDelete.Click += ButtonDelete_Click;
            newCheckBox.Click += CheckBoxDone_Check;
            
            rowDef.MinHeight = 15;
            newGrid.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
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
          
            newCheckBox.Content = "Done";
            newCheckBox.Foreground = Brushes.Green;
            newCheckBox.IsChecked = isChecked;
            newCheckBox.Margin = new Thickness(30,0,0,0);
            newCheckBox.Width = 50;
            newCheckBox.Height = 40;
            newCheckBox.FontSize = 15;
       
            newNote.Width = 550;
            newNote.TextWrapping = TextWrapping.Wrap;
            newNote.Margin = new Thickness(0, 5, 0, 5);
            newNote.Text = text;
            newNote.HorizontalAlignment = HorizontalAlignment.Left;
            newNote.BorderThickness = new Thickness(0, 0, 0, 0);
            
            newButtonDelete.Width = 35;
            newButtonDelete.Height = 25;
            string buttonDeleteName = string.Format("X");

            newButtonDelete.Content = buttonDeleteName;
            newButtonDelete.Foreground = Brushes.DimGray;
            newButtonDelete.Background = Brushes.WhiteSmoke;
            newButtonDelete.BorderThickness = new Thickness(0.6);
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
            notes.Children.Add(newGrid);

            Tab tab = new Tab();
            tab.TextBox = newNote;
            tab.CheckBox = newCheckBox;
            tab.DeleteButton = newButtonDelete;
            tab.Grid = newGrid;
            BgColorOfTextBox(tab);
            _listOfTabs.Add(tab);

        }

    }
}
