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

        List<Tab> listOfTabs = new List<Tab>();
        List<Note> listOfNotes = new List<Note>();

        private Note selectedNote;
        private Note rightClickedNote;

        bool FirstRun = true;

        // OpenFileDialog chooseFileDialog = new OpenFileDialog();

        public MainWindow()
        {
            InitializeComponent();
            //IsOpenFirstRun();
            SetAddTabButtonVisibility(false);

        }
        private void IsOpenFirstRun()
        {

          if (FirstRun == true)
          {
              SaveFileDialog dlg = new SaveFileDialog();
              /*dlg.FileName = "Scenario";
              dlg.DefaultExt = ".txt";*/

              // Show save file dialog box
              Nullable<bool> result = dlg.ShowDialog();

              
              FirstRun = false;
              //Properties.Settings.Default.Save();
          } 
           
          
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

            TextBox textBoxForNoteName = new TextBox();
            Button buttonForNotepad = new Button();

            textBoxForNoteName.IsEnabled = false;
            textBoxForNoteName.TextWrapping = TextWrapping.Wrap;
            textBoxForNoteName.Background = new SolidColorBrush(Color.FromArgb(0,255, 255, 255));
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
            buttonForNotepad.Margin = new Thickness(30,5,10,10);

            string ScenarioNameDisplay = string.Format("Note{0}", listOfNotes.Count);
            textBoxForNoteName.Text = ScenarioNameDisplay;

            buttonForNotepad.Content = textBoxForNoteName;
            scenarioName.Children.Add(buttonForNotepad);
            

            Note note = new Note(); 
            note.NameOfButton = textBoxForNoteName;
            note.Button = buttonForNotepad;
            listOfNotes.Add(note);

            buttonForNotepad.Click += ButtonAddTab_Click;
            buttonForNotepad.Click += OnClickNameOfNoteButton;
            buttonForNotepad.MouseRightButtonDown += OnRightClickNameOfNote;
           }

        public void OnRightClickNameOfNote(object sender, MouseEventArgs e)
        {
            Button clickedButton = sender as Button;
            
            rightClickedNote = FindNoteFromButton(clickedButton);

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
            DeleteNote(rightClickedNote);
        }

        private void DeleteNote(Note note)
        {
            scenarioName.Children.Remove(note.Button);
            listOfNotes.Remove(note);
            SetAddTabButtonVisibility(false);
            CleanTabs();
        }
        private void OnRenameClicked(object sender, EventArgs eventArgs)
        {
            rightClickedNote.NameOfButton.IsEnabled = true;
            rightClickedNote.NameOfButton.Focus();
            rightClickedNote.NameOfButton.KeyDown += PressEnter;
            rightClickedNote.NameOfButton.MouseLeftButtonDown += OnMouseClick;
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
           rightClickedNote.NameOfButton.IsEnabled = false;
        }

        private void PressEnter(object sender, KeyEventArgs e)
        {
            if(Key.Enter == e.Key)
            {
                rightClickedNote.NameOfButton.IsEnabled = false;
            }
        }

        private void SaveNote()
        {
            if (selectedNote == null)
                return;

            selectedNote.ListOfNoteContents.Clear();

            for(int i = 0; i < listOfTabs.Count; i++)
            {
                NoteContent noteContent = new NoteContent();
                noteContent.Note = listOfTabs[i].TextBox.Text;
                noteContent.Done = listOfTabs[i].CheckBox.IsChecked.Value;
                selectedNote.ListOfNoteContents.Add(noteContent);
            }
        }
       
        private void CleanTabs()
        {
           for(int i = 0;i < listOfTabs.Count; i++)
            {
                listOfTabs[i].Grid.Children.Clear();
                notes.Children.Remove(listOfTabs[i].Grid);
            }
            listOfTabs.Clear();
        }

        private void LoadNotes()
        {
            if (selectedNote == null)
                return;
            
            for (int i = 0; i < selectedNote.ListOfNoteContents.Count; i++)
            {
                CreateTab(selectedNote.ListOfNoteContents[i].Note, selectedNote.ListOfNoteContents[i].Done);
            }
        }
        private Note FindNoteFromButton(Button button)
        {
            foreach (Note note in listOfNotes)
            {
                if (button == note.Button)
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
            for (int i  = 0; i< listOfNotes.Count;i++)
            {
                if(clickedButton == listOfNotes[i].Button)
                {
                    listOfNotes[i].Button.Background = new SolidColorBrush(Color.FromRgb(73, 76, 108));
                }
                else
                {
                    listOfNotes[i].Button.Background = new SolidColorBrush(Color.FromArgb(27, 25, 37, 100));
                }
            }
        
            selectedNote = FindNoteFromButton(clickedButton);
            
            CleanTabs();
            LoadNotes();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs args)
        {
            Button clickedButton = sender as Button;

            foreach (Tab tab in listOfTabs) 
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

            foreach (Tab tab in listOfTabs)
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
            listOfTabs.Remove(tab);
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
            listOfTabs.Add(tab);

        }

    }
}
