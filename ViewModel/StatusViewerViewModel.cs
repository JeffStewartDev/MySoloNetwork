using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using MSN.Model;
using MSN.ModelContext;

namespace MSN.ViewModel
{
    public class StatusViewerViewModel
    {
        public List<UpdatePostItem> Entries { get; set; } = new List<UpdatePostItem>();
        public StatusUpdateEntry CurrentEntry { get; set; } = new StatusUpdateEntry();
        public StatusUpdateEntry EditEntry { get; set; }
        public MSNContext context { get; set; }
        public int RequestOffset { get; set; } = 0;
        public int TakeAmount { get; set; } = 10;
        public bool ShowMore { get; set; } = true;

        public StatusViewerViewModel()
        {
            context = new MSNContext();
            Entries = new List<UpdatePostItem>();
            Entries.InsertRange(0, GetEntries(DateTime.Now));
            
            ShowMore = Entries.Count < TakeAmount ? false : ShowMore;
        }
        public async Task PostEntryAsync()
        {
            List<Model.StatusUpdateEntry> modelList =
         context.StatusUpdateEntries.Where(x => x.Id.Equals(CurrentEntry.Id)).ToList();

            if (modelList.Count == 0)
            {
                context.StatusUpdateEntries.Add(CurrentEntry);
                await context.SaveChangesAsync();
            }
            CurrentEntry = new Model.StatusUpdateEntry();
            EditEntry = null;
        }

        public void PostEntry()
        {
            List<Model.StatusUpdateEntry> modelList =
         context.StatusUpdateEntries.Where(x => x.Id.Equals(CurrentEntry.Id)).ToList();

            if (modelList.Count == 0)
            {
                CurrentEntry.DateCreated = DateTime.Now;
                context.StatusUpdateEntries.Add(CurrentEntry);
            }
            context.SaveChanges();
            CurrentEntry = new Model.StatusUpdateEntry();
            EditEntry = null;
        }
        public void PostEditedEntry()
        {
            StatusUpdateEntry modelList =
         context.StatusUpdateEntries.Where(x => x.Id.Equals(EditEntry.Id)).FirstOrDefault();

            if (modelList != null)
            {
                context.SaveChanges();
            }
            CurrentEntry = new Model.StatusUpdateEntry();
            EditEntry = null;
        }
        public List<UpdatePostItem> GetEntries(DateTime earliestDate)
        {
            List<UpdatePostItem> result = new List<UpdatePostItem>();
            var statuses = context.StatusUpdateEntries.Where(x => x.Visible && DateTime.Compare(x.DateCreated, earliestDate) < 0).OrderByDescending(x => x.DateCreated).Take(TakeAmount)
                .Select(x => new UpdatePostItem() { PostType = PostTypes.StatusUpdate, Id = x.Id, DateUpdated = x.DateUpdated, DateCreated = x.DateCreated });
            var images = context.ImagePosts.Where(x => x.Visible && x.AlbumId == null && DateTime.Compare(x.DateCreated, earliestDate) < 0).OrderByDescending(x => x.DateCreated).Take(TakeAmount)
                .Select(x => new UpdatePostItem() { PostType = PostTypes.Image, Id = x.Id, DateUpdated = x.DateUpdated, DateCreated = x.DateCreated });
            var notes = context.Notes.Where(x => x.Visible && DateTime.Compare(x.DateCreated, earliestDate) < 0).OrderByDescending(x => x.DateCreated).Take(TakeAmount)
                .Select(x => new UpdatePostItem() { PostType = PostTypes.Note, Id = x.Id, DateUpdated = x.DateUpdated, DateCreated = x.DateCreated });

            var events = context.EventItems.Where(x => x.Visible && DateTime.Compare(x.DateCreated, earliestDate) < 0).OrderByDescending(x => x.DateCreated).Take(TakeAmount)
                .Select(x => new UpdatePostItem() { PostType = PostTypes.Event, Id = x.Id, DateUpdated = x.DateUpdated, DateCreated = x.DateCreated });

            var albums = context.Albums.Where(x => x.Visible && DateTime.Compare(x.DateCreated, earliestDate) < 0).OrderByDescending(x => x.DateCreated).Take(TakeAmount)
                .Select(x => new UpdatePostItem() { PostType = PostTypes.Album, Id = x.Id, DateUpdated = x.DateUpdated, DateCreated = x.DateCreated });

            result.AddRange(notes);
            result.AddRange(statuses);
            result.AddRange(images);
            result.AddRange(albums);
            result.AddRange(events);
            List<UpdatePostItem> output = result.OrderByDescending(x => x.DateCreated).Take(TakeAmount).ToList();
            return output;
        }
        public List<UpdatePostItem> GetRecentEntries(DateTime dateFrom)
        {
            List<Guid> guidIDs = Entries.Select(y => y.Id).ToList();
            List<UpdatePostItem> result = GetEntries(dateFrom).Where(x => !guidIDs.Contains(x.Id)).ToList();
            ShowMore = result.Count() > 0 && result.Count() > TakeAmount;
            return result;
        }
        public void SetCurrentEntryForEdit(Guid selectedId)
        {
            EditEntry = context.StatusUpdateEntries.Where(x => x.Id == selectedId).FirstOrDefault();

        }
        public void SetCurrentEntry(Guid selectedId)
        {
            CurrentEntry = context.StatusUpdateEntries.Where(x => x.Id == selectedId).FirstOrDefault();

        }
        public void LoadOlderPosts()
        {
            UpdatePostItem earliestItem = Entries.OrderBy(x => x.DateCreated).FirstOrDefault();
            DateTime earliestDate = earliestItem?.DateCreated ?? DateTime.Now;
            RequestOffset++;
            List<UpdatePostItem> result = GetRecentEntries(earliestDate);
            ShowMore = result.Count() > 0 && result.Count() > TakeAmount;
            Entries.AddRange(result);
        }
        public void RefreshPostsForNewAddition()
        {
            Entries.InsertRange(0, GetRecentEntries(DateTime.Now));
        }
    }
}