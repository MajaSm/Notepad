
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Notepad
{
    public class StackOfNotes 
    {

        private ListOfNotes _listOfNotes;

        private StackPanel _stackPanel;
        private Note _lastEditedNote;
        private Note _selectedNote;
        private Note _rightClickedNote;
        private Button _buttonAddNote;

        public delegate void NoteClickedDelegate(int noteIndex);
        public event NoteClickedDelegate OnNoteClicked;

        public StackOfNotes(StackPanel stackPanel,Button buttonAddNote, ListOfNotes listOfNotes, List<NoteContent> loadedNotes)
        {
            _buttonAddNote = buttonAddNote;
            _stackPanel = stackPanel;
            _listOfNotes = listOfNotes;

            _buttonAddNote.Click += ButtonAddNote_Click;

            LoadPreviousNotes(loadedNotes);
            OnStartUp();
        }

        public void OnEnterPressed()
        {
            _lastEditedNote.NoteReferences.NameTextBox.IsEnabled = false;
            DisableNoteTextbox(_rightClickedNote);
        }

        private void OnStartUp()
        {
            if (_lastEditedNote != null)
            {
                _lastEditedNote.NoteReferences.NameTextBox.IsEnabled = false;
            }
        }

        private void ButtonAddNote_Click(object sender, RoutedEventArgs e)
        {
            CreateNoteButton();
        }

        private void CreateNoteButton(NoteContent noteContent = null)
        {
            TextBox textBoxForNoteName = new TextBox();
            Button buttonForNoteName = new Button();

            textBoxForNoteName.IsEnabled = false;
            textBoxForNoteName.TextWrapping = TextWrapping.Wrap;
            textBoxForNoteName.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            textBoxForNoteName.BorderThickness = new Thickness(0, 0, 0, 0);
            textBoxForNoteName.FontSize = 16;
            textBoxForNoteName.HorizontalContentAlignment = HorizontalAlignment.Center;
            textBoxForNoteName.VerticalContentAlignment = VerticalAlignment.Center;
            textBoxForNoteName.Foreground = Brushes.White;
            textBoxForNoteName.ContextMenu = new ContextMenu();

            buttonForNoteName.Width = 258;
            buttonForNoteName.MinHeight = 50;
            buttonForNoteName.Foreground = Brushes.White;
            buttonForNoteName.Height = textBoxForNoteName.Height;
            buttonForNoteName.Background = new SolidColorBrush(Color.FromRgb(39, 37, 55));
            buttonForNoteName.BorderThickness = new Thickness(0, 0, 0, 0);

            buttonForNoteName.Style = buttonForNoteName.TryFindResource("ButtonStyle1") as Style;
            textBoxForNoteName.Style = textBoxForNoteName.TryFindResource("TextBoxEnabledStyle") as Style;

            string ScenarioNameDisplay = string.Format("Note{0}",  _listOfNotes.Notes.Count);

            if (noteContent != null) // saved name of Button Note
            {
                if (noteContent.Name != String.Empty)
                {
                    ScenarioNameDisplay = noteContent.Name;
                }
            }

            textBoxForNoteName.Text = ScenarioNameDisplay;

            buttonForNoteName.Content = textBoxForNoteName;
            _stackPanel.Children.Add(buttonForNoteName);

            Note note = new Note();
            note.NoteReferences.NameTextBox = textBoxForNoteName;
            note.NoteReferences.Button = buttonForNoteName;

            if (noteContent != null) //assing tabs
            {
                note.NoteContent = noteContent;
            }

            _listOfNotes.Notes.Add(note);

            buttonForNoteName.Click += OnClickNameOfNoteButton;
            buttonForNoteName.MouseRightButtonDown += OnRightClickNameOfNote;
            buttonForNoteName.MouseEnter += (sender, e) => OnMouseEnterOverNameOfNote(sender, e, textBoxForNoteName);
            buttonForNoteName.MouseLeave += (sender, e) => OnMouseLeaveOverNameOfNote(sender, e, textBoxForNoteName);

            SetNoteAsSelected(note);
            SetNoteTextEditState(note, true);
        }

        public void OnRightClickNameOfNote(object sender, MouseEventArgs e)
        {
            Button clickedButton = sender as Button;

            DisableNoteTextbox(_rightClickedNote);

            _rightClickedNote = FindNoteFromButton(clickedButton);
            
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItemRename = new MenuItem();
            MenuItem menuItemDelete = new MenuItem();

            menuItemRename.Header = "Rename";
            menuItemDelete.Header = "Delete";

            menuItemRename.Click += OnRenameClicked;
            menuItemDelete.Click += OnDeleteNoteClicked;

            contextMenu.Items.Add(menuItemRename);
            contextMenu.Items.Add(menuItemDelete);

            clickedButton.ContextMenu = contextMenu;

            contextMenu.Style = contextMenu.TryFindResource("ContextMenu") as Style;
        }

        private void OnDeleteNoteClicked(object sender, RoutedEventArgs e)
        {
            DeleteNote(_rightClickedNote);
        }

        /// <summary>
        /// Function that deletes specified note by removing its button reference from windows childrens.
        /// It will also remove it from the listOfNotes, clear all tabs and disable "Add Tab" button.
        /// </summary>
        /// <param name="note">Note that will get deleted.</param>
        private void DeleteNote(Note note)
        {
            _stackPanel.Children.Remove(note.NoteReferences.Button);
            _listOfNotes.Notes.Remove(note);                
        }

        private void SetNoteAsSelected(Note note)
        {
            _selectedNote = note;

            for (int i = 0; i < _listOfNotes.Notes.Count; i++)
            {
                _listOfNotes.Notes[i].NoteReferences.NameTextBox.Foreground = Brushes.White;
                _listOfNotes.Notes[i].NoteReferences.NameTextBox.FontWeight = FontWeights.Normal;
            }

            _selectedNote.NoteReferences.NameTextBox.Foreground = new SolidColorBrush(Color.FromRgb(231, 24, 95));
            _selectedNote.NoteReferences.NameTextBox.FontWeight = FontWeights.Bold;
        }

        private void LoadPreviousNotes(List<NoteContent> loadedNotes)
        {
            if (loadedNotes != null)
            {
                for (int i = 0; i < loadedNotes.Count; i++)
                {
                    CreateNoteButton(loadedNotes[i]);
                }
                if (_listOfNotes.Notes.Count > 0)
                {
                    SetNoteAsSelected(_listOfNotes.Notes[0]);
                }
            }
        }

        private void SetNoteTextEditState(Note note, bool isEnabled)
        {
            if(_lastEditedNote != null )
            {
                _lastEditedNote.NoteReferences.NameTextBox.IsEnabled = false;
            }

            if (isEnabled)
            {
                note.NoteReferences.NameTextBox.IsEnabled = true;
                SetFocusOnTextbox(note.NoteReferences.NameTextBox);
                _lastEditedNote = note;
            }
            else
            {
                note.NoteReferences.NameTextBox.IsEnabled = false;
            }
        }

        private async void SetFocusOnTextbox(TextBox textBox) 
        {
            while (true)
            {
                if(textBox.Focus())
                {
                    break;
                }

                await Task.Delay(100);
            }
        }

        private void OnRenameClicked(object sender, EventArgs eventArgs)
        {
            SetNoteTextEditState(_rightClickedNote, true);
        }

        public void OnMouseLeftClick()
        {
            if(_lastEditedNote != null)
            {
                _lastEditedNote.NoteReferences.NameTextBox.IsEnabled = false;
            }
            DisableNoteTextbox(_rightClickedNote);
        }

        public void OnMouseEnterOverNameOfNote(object sender, MouseEventArgs e, TextBox textBoxForNoteName)
        {
            textBoxForNoteName.FontSize = 20;
        }

        public void OnMouseLeaveOverNameOfNote(object sender, MouseEventArgs e, TextBox textBoxForNoteName)
        {
            textBoxForNoteName.FontSize = 16;
        }

        private void DisableNoteTextbox(Note note)
        {
            if (note == null)
                return;

            note.NoteContent.Name = note.NoteReferences.NameTextBox.Text;
            note.NoteReferences.NameTextBox.IsEnabled = false;
        }

        private void OnClickNameOfNoteButton(object sender, RoutedEventArgs args)
        {
            Button clickedButton = sender as Button;
            SetNoteAsSelected(FindNoteFromButton(clickedButton));
            OnNoteClicked?.Invoke(FindNoteIndexFromButton(clickedButton));
        }
        private Note FindNoteFromButton(Button button)
        {
            foreach (Note note in _listOfNotes.Notes)
            {
                if (button == note.NoteReferences.Button)
                {
                    return note;
                }
            }
            return null;
        }

        private int FindNoteIndexFromButton(Button button)
        {
            for(int i = 0; i < _listOfNotes.Notes.Count; i++)
            {
                if(button == _listOfNotes.Notes[i].NoteReferences.Button)
                {
                    return i;
                }
            }
            return 0;
        }

    }
}
