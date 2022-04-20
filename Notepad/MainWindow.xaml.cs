
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Notepad
{
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
        string appFileName = "Notepad.exe";
        string directory = Process.GetCurrentProcess().MainModule.FileName;

        public MainWindow()
        {
            InitializeComponent();
            Setup();
            MessageBoxForExit.Visibility = Visibility.Hidden;

        }
        public static void AddStartup(string appName, string path)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.SetValue(appName, "\"" + path + "\"");
            }
        }
        public static void RemoveStartup(string appName)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.DeleteValue(appName, false);
            }
        }
        /// <summary>
        /// Function that checks if there is a saved note button in the file, sends a button as a parameter to the Function "CreateNoteButton()".
        /// Also disables the "Add Tab" button because there is no selected Note Button when you run app for the first time.
        ///  Disables TextBox of Note Button when you are finish with renaming by click wherever on the app.
        /// </summary>
        private void Setup()
        {
            KeyDown += PressEnter;
            MouseLeftButtonDown += OnMouseLeftClick;
            SetAddTabButtonVisibility(false);

            _savingSystem = new SavingSystem();
            ListOfNotes notes = _savingSystem.LoadNoteData();

            if (notes != null)
            {
                for (int i = 0; i < notes.Notes.Count; i++)
                {
                    CreateNoteButton(notes.Notes[i]);
                }
            }


            using (RegistryKey key = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                SetStartupButtonsColor(key.GetValue(appFileName) != null);
            }
        }

        /// <summary>
        /// Button "Add Tab" is desabled on begining. When you click Button "Add Note" then appears(enabled) button "Add Tab".
        /// </summary>
        /// <param name="isVisible"></param>
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

        private void ButtonAddNote_Click(object sender, RoutedEventArgs e)
        {
            CreateNoteButton();
        }
        /// <summary>
        /// Its using for loading saved button notes from Function "Setup()", and for creating new one.
        /// Apperance of the whole Note Button.
        /// </summary>
        /// <param name="noteContent"></param>
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

            buttonForNoteName.Style = buttonForNoteName.TryFindResource("FocusVisual") as Style;
            buttonForNoteName.Style = buttonForNoteName.TryFindResource("ButtonStyle1") as Style;


            string ScenarioNameDisplay = string.Format("Note{0}", _listOfNotes.Count);

            if (noteContent != null) // saved name of Button Note
            {
                if (noteContent.Name != String.Empty)
                {
                    ScenarioNameDisplay = noteContent.Name;
                }
            }
            textBoxForNoteName.Text = ScenarioNameDisplay;

            buttonForNoteName.Content = textBoxForNoteName;
            noteName.Children.Add(buttonForNoteName);

            Note note = new Note();
            note.NoteReferences.NameTextBox = textBoxForNoteName;
            note.NoteReferences.Button = buttonForNoteName;
            if (noteContent != null) //assing tabs
            {
                note.NoteContent = noteContent;
            }

            
            _listOfNotes.Add(note);

            buttonForNoteName.Click += EnableAddTabButton;
            buttonForNoteName.Click += OnClickNameOfNoteButton;
            buttonForNoteName.MouseRightButtonDown += OnRightClickNameOfNote;
            buttonForNoteName.MouseEnter += (sender,e) => OnMouseEnterOverNameOfNote(sender, e, textBoxForNoteName);
            buttonForNoteName.MouseLeave += (sender, e) => OnMouseLeaveOverNameOfNote(sender, e, textBoxForNoteName);
        }

        public void OnMouseEnterOverNameOfNote(object sender, MouseEventArgs e, TextBox textBoxForNoteName)
        {
            textBoxForNoteName.FontSize = 20;
        }

        public void OnMouseLeaveOverNameOfNote(object sender, MouseEventArgs e, TextBox textBoxForNoteName)
        {
            textBoxForNoteName.FontSize = 16;
        }

        /// <summary>
        /// Creating menu for rename and delete Note Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            noteName.Children.Remove(note.NoteReferences.Button);
            _listOfNotes.Remove(note);
            SetAddTabButtonVisibility(false);

            if(_selectedNote == _rightClickedNote)
            {
                CleanTabs();
            }
                
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
            PopUpWindowForSave();
            SaveNote();
            _savingSystem.SaveNotes(_listOfNotes);

        }
        /// <summary>
        /// Function that caches values of Tabs and value of CheckBox for exact Note Button in the ListOfNoteTabs.
        /// </summary>
        private void SaveNote()
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

        private void DisableNoteTextbox(Note note)
        {
            if (note == null)
                return;

            note.NoteContent.Name = note.NoteReferences.NameTextBox.Text;
            note.NoteReferences.NameTextBox.IsEnabled = false;
        }

        private void CleanTabs()
        {

            for (int i = 0; i < _listOfTabs.Count; i++)
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
            for (int i = 0; i < _listOfNotes.Count; i++)
            {
                if (clickedButton == _listOfNotes[i].NoteReferences.Button)
                {
                    _listOfNotes[i].NoteReferences.NameTextBox.Foreground = new SolidColorBrush(Color.FromRgb(231, 24, 95));
                    _listOfNotes[i].NoteReferences.NameTextBox.FontWeight = FontWeights.Bold;
                }
                else
                {
                    _listOfNotes[i].NoteReferences.NameTextBox.Foreground = Brushes.White;
                    _listOfNotes[i].NoteReferences.NameTextBox.FontWeight = FontWeights.Normal;
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
            MessageBoxForExit.Visibility = Visibility.Visible;
            Button button = sender as Button;
            if (button == YesButtonForExit)
            {
                Close();
            }
            if (button == NoButtonForExit)
            {
                MessageBoxForExit.Visibility = Visibility.Hidden;
            }

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

            if (tab.CheckBox.IsChecked == true)
            {
                tab.CheckBox.Foreground = Brushes.Green;
                tab.CheckBox.Content = "DONE";
                tab.TextBox.Foreground = Brushes.Black;
                tab.DeleteButton.BorderBrush = Brushes.Black;
                tab.DeleteButton.Foreground = Brushes.Black;
                tab.Grid.Background = new SolidColorBrush(Color.FromRgb(120, 175, 133));
                tab.TextBox.Background = new SolidColorBrush(Color.FromRgb(120, 175, 133));
                tab.DeleteButton.Style = tab.DeleteButton.TryFindResource("FocusVisual") as Style;
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
                tab.DeleteButton.Style = tab.DeleteButton.TryFindResource("FocusVisual") as Style;
                tab.DeleteButton.Style = tab.DeleteButton.TryFindResource("ButtonStyle2") as Style;
            }
        }

        private void DeleteTab(Tab tab)
        {
            notes.Children.Remove(tab.Grid);
            _listOfTabs.Remove(tab);
        }
        private void EnableAddTabButton(object sender, RoutedEventArgs e)
        {
            SetAddTabButtonVisibility(true);
        }

        private void TextBlock_Click(object sender, RoutedEventArgs e)
        {
            CreateTab("", false);
        }

        private void PopUpWindowForSave()
        {
            TextBlock popupText = new TextBlock();
            popupText.Text = " Saved ";
            popupText.FontSize = 18;
            popupText.Background = Brushes.LightGreen;
            popupText.Foreground = Brushes.White;
            myPopup.Child = popupText;

            myPopup.IsOpen = true;
            myPopup_Opened();

        }
        private void SetStartupButtonsColor(bool isStartupEnabled)
        {
            YesButton.Foreground = isStartupEnabled ? Brushes.Green : Brushes.White;
            NoButton.Foreground = isStartupEnabled ? Brushes.White : Brushes.Red;
        }
        private void OpenWinOnStartButton_Clicked(object sender, RoutedEventArgs args)
        {
            Button clickedButton = sender as Button;
            if (clickedButton == YesButton)
            {
                AddStartup(appFileName, directory);
            }
            if (clickedButton == NoButton)
            {
                RemoveStartup(appFileName);
            }

            SetStartupButtonsColor(clickedButton == YesButton);

        }
        private void myPopup_Opened()
        {
            StartCloseTimer();
        }

        private void StartCloseTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3d);
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= TimerTick;
            myPopup.IsOpen = false;
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
            notes.Children.Add(newGrid);

            Tab tab = new Tab();
            tab.TextBox = newNote;
            tab.CheckBox = newCheckBox;
            tab.DeleteButton = newButtonDelete;
            tab.Grid = newGrid;
            BgColorOfTextBox(tab);
            _listOfTabs.Add(tab);
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }

        }
        private void SaveFromKeyboard(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                PopUpWindowForSave();
                SaveNote();
                _savingSystem.SaveNotes(_listOfNotes);
            }
        }

    }
}


