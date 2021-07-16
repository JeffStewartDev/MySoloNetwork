using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSN.Model;
using MSN.ModelContext;

namespace MSN.ViewModel
{
    public class EventViewModel
    {
        public MSNContext context { get; set; }

        public EventViewModel()
        {
            context = new MSNContext();
        }

        public void PostEntry()
        {
            Entry.DateCreated = DateTime.Now;
            context.EventItems.Add(Entry);
            context.SaveChanges();
            Entry = new EventItem();
        }
        public void ClearEntry()
        {
            Entry = new EventItem();
        }
        public void DefineNewEntry()
        {
            Entry = new EventItem();
        }


        public EventItem Entry { get; set; }

        public bool IsEditMode { get; set; } = false;


        public EventItem GetEntry(Guid Id)
        {
            EventItem result =  context.EventItems.Where(x => x.Id == Id).FirstOrDefault();
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