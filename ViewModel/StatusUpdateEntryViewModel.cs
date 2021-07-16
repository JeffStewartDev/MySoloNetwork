using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSN.Model;
using MSN.ModelContext;

namespace MSN.ViewModel
{
    public class StatusUpdateEntryViewModel
    {
        public MSNContext context { get; set; }

        public StatusUpdateEntryViewModel()
        {
            context = new MSNContext();
        }

        public void PostEntry()
        {
            Entry.DateCreated = DateTime.Now;
            context.StatusUpdateEntries.Add(Entry);
            context.SaveChanges();
            Entry = new StatusUpdateEntry();
        }
        public void ClearEntry()
        {
            Entry = new StatusUpdateEntry();
        }
        public void DefineNewEntry()
        {
            Entry = new StatusUpdateEntry();
        }


        public StatusUpdateEntry Entry { get; set; }

        public bool IsEditMode { get; set; } = false;


        public StatusUpdateEntry GetEntry(Guid Id)
        {
            StatusUpdateEntry result =  context.StatusUpdateEntries.Where(x => x.Id == Id).FirstOrDefault();
            return result;
        }

        public void SetEntry(Guid Id)
        {
            Entry = GetEntry(Id);
        }


        public void SavePostEntryEdit()
        {
            context.SaveChanges();

            IsEditMode = false;
            SetEntry(Entry.Id);
        }

        public void CancelPostEntryEdit()
        {
            IsEditMode = false;
            SetEntry(Entry.Id);
        }
        public void EnterEditMode()
        {
            IsEditMode = true;
        }
        public void DeleteEntry()
        {
            Entry.Visible = false;
            context.SaveChanges();
        }
    }
}