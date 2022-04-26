using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Notepad
{
    class SavingSystem
    {
        private string _fileName;

        public delegate void EmptyDelegate();
        public event EmptyDelegate OnNotesSaved;

        public void SaveNotes(List<Note> notes)
        {
            _fileName = String.Format(@"{0}\note.json", AppContext.BaseDirectory);

            List<NoteContent> listOfNoteContents = new List<NoteContent>();
            for(int i = 0; i < notes.Count; i++)
            {
                listOfNoteContents.Add(notes[i].NoteContent);
            }

            ListOfNoteContents listOfNotes = new ListOfNoteContents(listOfNoteContents);

            string noteJson = JsonConvert.SerializeObject(listOfNotes);

            File.WriteAllText(_fileName, noteJson);
            OnNotesSaved?.Invoke();
            return;
        }

        public ListOfNoteContents LoadNoteData()
        {
            _fileName = String.Format(@"{0}\note.json", AppContext.BaseDirectory);

            if (File.Exists(_fileName))
            {
                string noteJson = File.ReadAllText(_fileName);
                ListOfNoteContents notes = JsonConvert.DeserializeObject<ListOfNoteContents>(noteJson);
                return notes;
            }

            return new ListOfNoteContents();
        }
    }
}
