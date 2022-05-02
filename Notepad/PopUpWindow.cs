using System;
using System.Windows;
using System.Windows.Controls;

namespace Notepad
{
    class PopUpWindow
    {
        private const string TITLE_ELEMENT_NAME = "MesageBoxTitleText";
        private const string DESCRIPTION_ELEMENT_NAME = "MesageBoxDescriptionText";
        private const string LEFT_OPTION_BUTTON_ELEMENT_NAME = "MessageBoxLeftButton";
        private const string RIGHT_OPTION_BUTTON_ELEMENT_NAME = "MessageBoxRightButton";

        private Border _window;
        private Label _titleLabel;
        private Label _descriptionLabel;
        private Button _leftOptionButton;
        private Button _rightOptionButton;
        private Action _leftOptionAction;
        private Action _rightOptionAction;
        private SavingSystem _savingSystem;
        private ListOfNotes _listOfNotes;

        public PopUpWindow(Border window, SavingSystem savingSystem, ListOfNotes listOfNotes)
        {
            _window = window;
            _savingSystem = savingSystem;
            _listOfNotes = listOfNotes;
            GetReferences();

            _leftOptionButton.Click += (sender, args) => OnLeftButtonClick();
            _rightOptionButton.Click += (sender, args) => OnRightButtonClick();

            SetPopUpVisibility(false);
        }

        public void Show(string titleText, string descriptionText, string leftOptionButtonText, string rightOptionButtonText, Action leftOptionAction, Action rightOptionAction)
        {
            _titleLabel.Content = titleText;
            _descriptionLabel.Content = descriptionText;
            _leftOptionButton.Content = leftOptionButtonText;
            _rightOptionButton.Content = rightOptionButtonText;
            _leftOptionAction = leftOptionAction;
            _rightOptionAction = rightOptionAction;
            SetPopUpVisibility(true);
        }

        private void GetReferences()
        {
            object foundObject;
            foundObject = _window.FindName(TITLE_ELEMENT_NAME);
            if(foundObject is Label)
            {
                _titleLabel = (Label)foundObject;
            }

            foundObject = _window.FindName(DESCRIPTION_ELEMENT_NAME);
            if (foundObject is Label)
            {
                _descriptionLabel = (Label)foundObject;
            }

            foundObject = _window.FindName(LEFT_OPTION_BUTTON_ELEMENT_NAME);
            if (foundObject is Button)
            {
                _leftOptionButton = (Button)foundObject;
            }

            foundObject = _window.FindName(RIGHT_OPTION_BUTTON_ELEMENT_NAME);
            if (foundObject is Button)
            {
                _rightOptionButton = (Button)foundObject;
            }
        }

        private void OnLeftButtonClick()
        {
            SetPopUpVisibility(false);
            if (_leftOptionAction == null)
            {
                return;
            }
            _savingSystem.SaveNotes(_listOfNotes.Notes);
            _leftOptionAction.Invoke();
        }

        private void OnRightButtonClick()
        {
            SetPopUpVisibility(false);
            if (_rightOptionAction == null)
            {
                return;
            }
            _rightOptionAction.Invoke();
        }

        private void SetPopUpVisibility(bool isVisible)
        {
            if(isVisible)
            {
                _window.Visibility = Visibility.Visible;
            }
            else
            {
                _window.Visibility = Visibility.Hidden;
            }
        }
    }
}
