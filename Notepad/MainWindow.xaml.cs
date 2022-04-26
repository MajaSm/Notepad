
using System.Windows;
using System.Windows.Input;

namespace Notepad
{
    public partial class MainWindow : Window
    {
        private StackOfNotes _stackOfNotes;
        private StackOfTabs _stackOfTabs;
        private ToolBar _leftCorner;
        private PopUpWindow _popUpWindow;
        private ListOfNotes _listOfNotes;
        private SavingSystem _savingSystem;

        public MainWindow()
        {
            InitializeComponent();
            Setup();

            _stackOfNotes.OnNoteClicked += OnNoteClicked;
        }
       
        private void OnNoteClicked(int noteIndex)
        {
            _stackOfTabs.SetContent(noteIndex);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _stackOfNotes.OnEnterPressed();
            }

            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                _savingSystem.SaveNotes(_listOfNotes.Notes);
            }
        }

        private void Setup()
        {
            MouseDown += Window_MouseDown;
            _savingSystem = new SavingSystem();
            ListOfNoteContents loadedNotes = _savingSystem.LoadNoteData();
            _listOfNotes = new ListOfNotes();
            _stackOfNotes = new StackOfNotes(noteName, ButtonAddNote, _listOfNotes, loadedNotes.Notes);

            _stackOfTabs = new StackOfTabs(notes, _listOfNotes, ButtonAddTab);
            _stackOfTabs.SetContent(0);
            _leftCorner = new ToolBar(myPopup, YesButton, NoButton, SaveAllButton, _listOfNotes, _savingSystem);
            _popUpWindow = new PopUpWindow(MessageBox);
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs args)
        {
            _popUpWindow.Show(
                "Confirmation",
                "Are you sure you want to close Aplication?",
                "Yes",
                "No",
                Close,
                null
                );
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs args)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _stackOfNotes.OnMouseLeftClick();
                DragMove();
            }
        }
    }
}


