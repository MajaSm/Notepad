using System.Collections.Generic;
using System.Windows.Controls;

namespace Notepad
{
    public class NoteTab
    {
        public string Note; 
        public bool Done;

    }
    public class NoteContent
    {
        public string Name = string.Empty;
        public List<NoteTab> ListOfNoteTabs = new List<NoteTab>();
    }

    public class NoteReferences
    {
        public TextBox NameTextBox;
        public Button Button;
    }

    public class Note
    {
        public NoteContent NoteContent = new NoteContent();
        public NoteReferences NoteReferences = new NoteReferences();
    }

    public class ListOfNotes 
    {
        public List<Note> Notes;

        public ListOfNotes()
        {
            Notes = new List<Note>();
        }
    }

    public class ListOfNoteContents
    {
        public List<NoteContent> Notes;

        public ListOfNoteContents() 
        {
            Notes = new List<NoteContent>();
        }

        public ListOfNoteContents(List<NoteContent> notes)
        {
            Notes = notes;
        }
    }

}
