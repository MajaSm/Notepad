using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Notepad
{
    class SavingSystem
    {
        private string _fileName;
      
        public Note SaveNotes(List<Note> notes)
        {
            _fileName = String.Format(@"{0}\note.json", AppContext.BaseDirectory);

            List<NoteContent> listOfNoteContents = new List<NoteContent>();
            for(int i = 0; i < notes.Count; i++)
            {
                listOfNoteContents.Add(notes[i].NoteContent);
            }


            ListOfNotes listOfNotes = new ListOfNotes(listOfNoteContents);

            string noteJson = JsonConvert.SerializeObject(listOfNotes);

            File.WriteAllText(_fileName, noteJson);
            return null;
        }

        public ListOfNotes LoadNoteData()
        {
            _fileName = String.Format(@"{0}\note.json", AppContext.BaseDirectory);

            if (File.Exists(_fileName))
            {
                string noteJson = File.ReadAllText(_fileName);
                ListOfNotes notes = JsonConvert.DeserializeObject<ListOfNotes>(noteJson);
                return notes;
            }

            return null;
        }
    }
}
