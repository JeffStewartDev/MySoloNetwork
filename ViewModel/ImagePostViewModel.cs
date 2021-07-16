using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSN.Model;
using MSN.ModelContext;
using MSN.ViewModel.Util;
using System.Drawing;
using System.IO;
using MSN.Model.Util;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace MSN.ViewModel
{
    public class ImagePostViewModel
    {
        public List<string> ImageFiles { get; set; } = new();
        public ImagePost Entry { get; set; } = new ImagePost();
        public int RequestOffset { get; set; } = 0;
        public int TakeAmount { get; set; } = 20;

        public MSNContext context { get; set; }= new MSNContext();

        public void MakeMainImage()
        {
            if(Entry!=null)
            {
                Album album = context.Albums.Where(x => x.Id == Entry.AlbumId).FirstOrDefault();
                    if (album != null)
                    {
                        album.MainImageId = Entry.Id;
                        context.SaveChanges();
                    }
            }
        }
        public ImagePost GetEntry(Guid Id)
        {
            return context.ImagePosts.Where(x => x.Id == Id).FirstOrDefault();
        }

        public void SetEntry(Guid Id)
        {
            Entry = GetEntry(Id);
        }
        public void SaveUpdatedEntry()
        {
            context.SaveChanges();
        }

        public void DeleteEntry()
        {
            Entry.Visible = false;
            context.SaveChanges();
        }

        public async Task<int> PostImage(ImagePost newImage, string imageItem, ServerFileInfo fileInfo, NavigationManager NavigationManager)
        {
            int result = 0;
            try
            {
                    using Bitmap bitmap = new Bitmap(imageItem);
                    double desiredWidth = 1920;
                    double desiredHeight = 1080;
                    string greatestLength = bitmap.Height >= bitmap.Width ? "h" : "w";
                    double desiredScale = 1;
                    double desiredHScale = 1;
                    double desiredWScale = 1;
                    bool isOverHeight = bitmap.Height > desiredHeight;
                    bool isOverWidth = bitmap.Width > desiredWidth;
                    if (isOverHeight)
                    { desiredHScale = bitmap.Height <= desiredHeight ? 1D : desiredHeight / bitmap.Height; }

                    if (isOverWidth)
                        desiredWScale = bitmap.Width <= desiredWidth ? 1D : desiredWidth / bitmap.Width;

                    desiredScale = Math.Min(desiredHScale, desiredWScale);
                    int scaledHeight = Convert.ToInt16(((double)bitmap.Height) * desiredScale);
                    int scaledWidth = Convert.ToInt16(((double)bitmap.Width) * desiredScale);


                var path = Path.Combine(fileInfo.ImagesRootFolder.Replace(fileInfo.WWWRootFolder, ""),Path.ChangeExtension(Path.GetRandomFileName(), "jpg"));

                await using FileStream fs = new($"{fileInfo.WWWRootFolder}{path}", FileMode.Create);




                        using Bitmap scaledBitmap = desiredScale == 1 ? bitmap : new Bitmap(bitmap, new Size(scaledWidth, scaledHeight));
                        scaledBitmap.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);

                        scaledBitmap.Dispose();
                        bitmap.Dispose();
                        fs.Dispose();

                        using MSNContext imagePostContext = new();
                        List<Model.ImagePost> modelList = imagePostContext.ImagePosts.Where(x => x.Id.Equals(newImage.Id) || x.ImageFileLocation.Equals(path)).ToList();

                        if (modelList.Count == 0)
                        {
                            newImage.ImageFileLocation=path;
                            newImage.DateCreated = DateTime.Now;
                            newImage.Type = "image/jpeg";
                            imagePostContext.ImagePosts.Add(newImage);
                        }
                        else
                            throw new Exception("Image Already Exists. Try again.");
                    
                    await imagePostContext.SaveChangesAsync();
                    imagePostContext.Dispose();
                    result = 1;
                
            }
            catch (System.ArgumentOutOfRangeException )
            {
                //Size too large.
                result = 0;
                throw;
            }
            catch (System.ArgumentException )
            {
                //Invalid File
                result = 0;
            }
            catch (Exception)
            {
                result = 0;
                throw;
            }
            return await Task.FromResult<int>(result);
        }
        public async Task<int> PostNewImageAsync(string Img, Guid? AlbumId, ServerFileInfo fileInfo, NavigationManager NavigationManager)
        {
            ImagePost newImage;
            if (AlbumId == null)
                newImage = new ImagePost() { Comment=Entry.Comment, ImageFileLocation = Img };
            else
                newImage = new ImagePost
                {
                    AlbumId = AlbumId.Value,
                    ImageFileLocation = Img
                };
                
            try
            {
                int result = await PostImage(newImage, Img, fileInfo, NavigationManager);
                return await Task<int>.FromResult(result);                
            }
            catch (System.Exception)
            {                
                throw;
            }
            
        }
        public async Task<int> PostAlbumImages(Guid AlbumId, ServerFileInfo fileInfo, NavigationManager NavigationManager)
        {
            int savedCount = 0;
            foreach (var item in ImageFiles)
                savedCount += await PostNewImageAsync(item, AlbumId, fileInfo, NavigationManager);
            return savedCount;
        }
        public List<ImagePost> GetAlbumImages(Guid AlbumId)
        {
            return context.ImagePosts.Where(x => x.Visible).OrderByDescending(x => x.DateCreated).ToList();
        }

        public List<UpdatePostItem> AlbumEntries { get; set; } = new List<UpdatePostItem>();
        public bool ShowMore { get; set; } = true;
        public void LoadAlbumEntries(Guid AlbumId)
        {
            List<UpdatePostItem> result = new List<UpdatePostItem>();
            var images = context.ImagePosts.Where(x => x.Visible && x.AlbumId == AlbumId).Select(x => new UpdatePostItem() { PostType = PostTypes.Image, Id = x.Id, DateUpdated = x.DateUpdated });
            result.AddRange(images);
            AlbumEntries.AddRange(result.OrderByDescending(x => x.DateUpdated).Skip(RequestOffset * TakeAmount).Take(TakeAmount));
            ShowMore = result.Count() > 0;
        }
        public void LoadOlderAlbumPosts(Guid AlbumId)
        {
            RequestOffset++;
            LoadAlbumEntries(AlbumId);
        }
        public async Task<string> SaveImage(UploadedFileItem imageItem, ServerFileInfo fileInfo, string filePath = null)
        {
            string result = string.Empty;
            try
            {
                    string filename=string.IsNullOrWhiteSpace(imageItem.TempFilePath)?imageItem.WWWRootFilePath:imageItem.TempFilePath;
                    using Bitmap bitmap = new Bitmap(filename);
                    double desiredWidth = 1920;
                    double desiredHeight = 1080;
                    string greatestLength = bitmap.Height >= bitmap.Width ? "h" : "w";
                    double desiredScale = 1;
                    double desiredHScale = 1;
                    double desiredWScale = 1;
                    bool isOverHeight = bitmap.Height > desiredHeight;
                    bool isOverWidth = bitmap.Width > desiredWidth;
                    if (isOverHeight)
                    { desiredHScale = bitmap.Height <= desiredHeight ? 1D : desiredHeight / bitmap.Height; }

                    if (isOverWidth)
                        desiredWScale = bitmap.Width <= desiredWidth ? 1D : desiredWidth / bitmap.Width;

                    desiredScale = Math.Min(desiredHScale, desiredWScale);
                    int scaledHeight = Convert.ToInt16(((double)bitmap.Height) * desiredScale);
                    int scaledWidth = Convert.ToInt16(((double)bitmap.Width) * desiredScale);
                    using Bitmap scaledBitmap = desiredScale == 1 ? bitmap : new Bitmap(bitmap, new Size(scaledWidth, scaledHeight));
                    
                    string fn=filePath??filename;
                    
                    if (File.Exists(fn))
                        File.Delete(fn);
                        

                    scaledBitmap.Save(fn, System.Drawing.Imaging.ImageFormat.Jpeg);
                    bitmap.Dispose();
                  
            }
            catch (System.ArgumentOutOfRangeException )
            {
                //Size too large.
                result = "File size is too large. Try a file size less than 10MB.";
            }
            catch (Exception ex)
            {
                result = ex.InnerException == null? ex.Message: ex.InnerException.Message;
            }
            return await Task.FromResult<string>(result);
        }

    }
}