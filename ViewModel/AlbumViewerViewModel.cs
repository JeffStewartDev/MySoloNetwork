using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MSN.Model;
using MSN.Model.Util;
using MSN.ModelContext;

namespace MSN.ViewModel
{
    public class AlbumViewerViewModel
    {
    [Inject]
    public MSN.Model.Util.ServerFileInfo fileInfo { get; set; }
        private List<AlbumPostItem> entries = new List<AlbumPostItem>();

        public List<AlbumPostItem> Entries { get => entries; set => entries = value; }
        public Album CurrentAlbum { get; set; } = new Album() { ImagePosts = new List<ImagePost>() };
        public MSNContext context { get; set; }
        public int RequestOffset { get; set; } = 0;
        public int TakeAmount { get; set; } = 20;
        public bool ShowMore { get; set; } = true;
        public ImagePostViewModel AlbumImageModel { get; set; } = new ImagePostViewModel();

        public AlbumViewerViewModel()
        {
            context = new MSNContext();
        }
        public  string GetFirstImage()
        {
            ImagePost image = context.ImagePosts.Where(x => x.Visible == true && x.Id == CurrentAlbum.MainImageId).FirstOrDefault(); // && CurrentAlbum.MainImageId != null
            if (image == null)
                image =  context.ImagePosts.Where(x => x.Visible == true && x.AlbumId == CurrentAlbum.Id).OrderBy(x=>x.DateCreated).FirstOrDefault();

            string result = image == null ? "" : image.ImageFileLocation;
            return result;
        }
        public async Task<int> PostAlbumAsync(ServerFileInfo fileInfo, NavigationManager NavigationManager)
        {
            int result = 0;
            List<Model.Album> modelList =
         context.Albums.Where(x => x.Id.Equals(CurrentAlbum.Id)).ToList();

            if (modelList.Count == 0)
            {
                CurrentAlbum.DateCreated = DateTime.Now;
                context.Albums.Add(CurrentAlbum);
            }
            
                CurrentAlbum.DateUpdated = DateTime.Now;
            await context.SaveChangesAsync();
            if (AlbumImageModel?.ImageFiles?.Count > 0)
                result = await SaveUploadedImagesAsync(fileInfo, NavigationManager);
            return result;
        }


        private async Task<int> SaveUploadedImagesAsync(ServerFileInfo fileInfo, NavigationManager NavigationManager)
        {
            return await AlbumImageModel.PostAlbumImages(CurrentAlbum.Id, fileInfo, NavigationManager);
        }

        public void PostEditedAlbum()
        {
            Album modelList =
         context.Albums.Where(x => x.Id.Equals(CurrentAlbum.Id)).FirstOrDefault();

            if (modelList != null)
            {
                context.SaveChanges();
            }
            CurrentAlbum = new Model.Album();
        }
        public void NewAlbum() => CurrentAlbum = new Album();
        public void GetAlbums()
        {
            var Albums = context.Albums.Include(x => x.ImagePosts.Where(x => x.Visible == true)).Where(x => x.Visible).Select(x => new AlbumPostItem() { Id = x.Id, DateUpdated = x.DateUpdated, MainPhoto = x.ImagePosts.FirstOrDefault() });
            Entries.AddRange(Albums.OrderByDescending(x => x.DateUpdated).Skip(RequestOffset * TakeAmount).Take(TakeAmount));
            ShowMore = Albums.Count() > Entries.Count;
        }
        public void SetAlbum(Guid id)
        {
            CurrentAlbum = context.Albums.Include(x => x.ImagePosts.Where(x => x.Visible == true)).Where(x => x.Id.Equals(id)).FirstOrDefault();
            //CurrentAlbum.ImagePosts = CurrentAlbum.ImagePosts.Where(x => x.Visible == true).ToList();
        }

        public void DeleteCurrentAlbum()
        {
            CurrentAlbum.Visible = false;
            CurrentAlbum.DateUpdated = DateTime.Now;
            context.SaveChanges();
            CurrentAlbum = null;
        }
        public void SaveCurrentAlbum()
        {
            CurrentAlbum.DateUpdated = DateTime.Now;
            if (context.Albums.Where(x => x.Id.Equals(CurrentAlbum.Id)).Count() == 0)
            {
                CurrentAlbum.DateCreated = CurrentAlbum.DateUpdated;
                context.Albums.Add(CurrentAlbum);
            }
            context.SaveChanges();
        }

        public void LoadOlderPosts()
        {
            RequestOffset++;
            GetAlbums();
        }
    }
}