using System.Collections.Generic;
using System.Windows.Controls;

namespace Notepad
{
    public class NoteContent
    {
        public string Note;
        public bool Done;

    }
    public class Note
    {
        public TextBox NameOfButton;
        public Button Button;
        public List<NoteContent> ListOfNoteContents = new List<NoteContent>();
    }
    

}
