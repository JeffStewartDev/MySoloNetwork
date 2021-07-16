using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSN.Model;
using MSN.ModelContext;

namespace MSN.ViewModel
{
    public class NoteViewerViewModel
    {
        private List<UpdatePostItem> entries = new List<UpdatePostItem>();

        public List<UpdatePostItem> Entries { get => entries; set => entries = value; }
        public Note CurrentNote { get; set; } = new Note();
        public MSNContext context { get; set; }
        public int RequestOffset { get; set; } = 0;
        public int TakeAmount { get; set; } = 20;
        public bool ShowMore { get; set; } = true;

        public NoteViewerViewModel()
        {
            context = new MSNContext();

        }
        public async Task PostNoteAsync()
        {
            List<Model.Note> modelList =
         context.Notes.Where(x => x.Id.Equals(CurrentNote.Id)).ToList();

            if (modelList.Count == 0)
            {
                context.Notes.Add(CurrentNote);
                await context.SaveChangesAsync();
            }
            CurrentNote = new Model.Note();
        }

        public void PostNote()
        {
            List<Model.Note> modelList =
         context.Notes.Where(x => x.Id.Equals(CurrentNote.Id)).ToList();

            if (modelList.Count == 0)
            {
                CurrentNote.DateCreated = DateTime.Now;
                context.Notes.Add(CurrentNote);
            }
            context.SaveChanges();
            CurrentNote = new Model.Note();
        }
        public void PostEditedNote()
        {
            Note modelList =
         context.Notes.Where(x => x.Id.Equals(CurrentNote.Id)).FirstOrDefault();

            if (modelList != null)
            {
                context.SaveChanges();
            }
            CurrentNote = new Model.Note();
        }
        public void NewNote() => CurrentNote = new Note();
        public void GetNotes()
        {
            var notes = context.Notes.Where(x => x.Visible).Select(x => new UpdatePostItem() { PostType = PostTypes.Note, Id = x.Id, DateUpdated = x.DateUpdated, DateCreated = x.DateCreated });
            Entries.AddRange(notes.OrderByDescending(x => x.DateCreated).Skip(RequestOffset * TakeAmount).Take(TakeAmount));
            ShowMore = notes.Count() > Entries.Count;
        }
        public void SetNote(Guid id)
        {
            CurrentNote= context.Notes.Where(x => x.Id.Equals(id)).FirstOrDefault();
        }

        public void DeleteCurrentNote()
        {
            CurrentNote.Visible = false;
            CurrentNote.DateUpdated = DateTime.Now;
            context.SaveChanges();
            CurrentNote = null;
        }
        public void SaveCurrentNote()
        {
            CurrentNote.DateUpdated = DateTime.Now;
            CurrentNote.Visible = true;
            if (context.Notes.Where(x => x.Id.Equals(CurrentNote.Id)).Count()==0)
            {
                CurrentNote.DateCreated = CurrentNote.DateUpdated;
                context.Notes.Add(CurrentNote);
            }
            context.SaveChanges();
        }
        
        public void LoadOlderPosts()
        {
            RequestOffset++;
            GetNotes();
        }
    }
}