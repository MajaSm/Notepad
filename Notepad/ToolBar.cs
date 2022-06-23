using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace Notepad
{

    class ToolBar
    {
        private Popup _popup;
        private Button _yesButton;
        private Button _noButton;
        private Button _saveAllButton;
        private ListOfNotes _listOfNotes;
        private SavingSystem _savingSystem;
        string appFileName = "Task Notes.exe";
        string directory = Process.GetCurrentProcess().MainModule.FileName;

        public ToolBar(Popup popup, Button yesButton, Button noButton, Button saveAllButton, ListOfNotes listOfNotes, SavingSystem savingSystem)
        {
            _popup = popup;
            _yesButton = yesButton;
            _noButton = noButton;
            _saveAllButton = saveAllButton;
            _savingSystem = savingSystem;
            _listOfNotes = listOfNotes;
            _savingSystem.OnNotesSaved += PopUpWindowForSave;
            _saveAllButton.Click += ButtonSaveAll_Click;
            _yesButton.Click += OpenWinOnStartButton_Clicked;
            _noButton.Click += OpenWinOnStartButton_Clicked;
            OnStartup();
        }

        private void ButtonSaveAll_Click(object sender, RoutedEventArgs e)
        {
            _savingSystem.SaveNotes(_listOfNotes.Notes);
        }

        private void PopUpWindowForSave()
        {
            TextBlock popupText = new TextBlock();
            popupText.Text = " Saved ";
            popupText.FontSize = 18;
            popupText.Background = Brushes.LightGreen;
            popupText.Foreground = Brushes.White;
            _popup.Child = popupText;

            _popup.IsOpen = true;
            OnPopupOpened();
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

        private void OpenWinOnStartButton_Clicked(object sender, RoutedEventArgs args)
        {
            Button clickedButton = sender as Button;
            if (clickedButton == _yesButton)
            {
                AddStartup(appFileName, directory);
            }
            if (clickedButton == _noButton)
            {
                RemoveStartup(appFileName);
            }

            SetStartupButtonsColor(clickedButton == _yesButton);
        }
        private void OnStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey
              ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                SetStartupButtonsColor(key.GetValue(appFileName) != null);
            }
        }

        private void SetStartupButtonsColor(bool isStartupEnabled)
        {
            _yesButton.Foreground = isStartupEnabled ? Brushes.Green : Brushes.White;
            _noButton.Foreground = isStartupEnabled ? Brushes.White : Brushes.Red;
        }
       
        private void OnPopupOpened()
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
            _popup.IsOpen = false;
        }

    }

}
