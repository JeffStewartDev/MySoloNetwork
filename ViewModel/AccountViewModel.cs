using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using MSN.Model;
using MSN.ModelContext;
using MSN.ViewModel.Util;
using MSN.Model.Util;
using System.IO;

namespace MSN.ViewModel
{
    public class AccountViewModel
    {
        public string Full_Name { get; set; }
        public UploadedFileItem OriginalHome_Page_Profile_Picture { get; set; }
        public UploadedFileItem OriginalHome_Page_Jumbotron_Picture { get; set; }
        public UploadedFileItem Home_Page_Profile_Picture { get; set; }
        public UploadedFileItem Home_Page_Jumbotron_Picture { get; set; }
        public ServerFileInfo fileInfo { get; set; }

        public void SaveSettings()
        {
            SetFull_Name();
            SetHome_Page_Profile_Picture();
            SetHome_Page_Jumbotron_Picture();
        }
        public AccountViewModel(ServerFileInfo FileInformation)
        {
            fileInfo=FileInformation;
            Full_Name = GetFull_Name();
            Home_Page_Profile_Picture = GetHome_Page_Profile_Picture();
            Home_Page_Jumbotron_Picture = GetHome_Page_Jumbotron_Picture();
            OriginalHome_Page_Profile_Picture = Home_Page_Profile_Picture;
            OriginalHome_Page_Jumbotron_Picture = Home_Page_Jumbotron_Picture;
        }

        public void ResetFull_Name()
        {
            Full_Name = GetFull_Name();
        }
        public void ResetHome_Page_Profile_Picture()
        {
            Home_Page_Profile_Picture = OriginalHome_Page_Profile_Picture;
        }
        public void ResetHome_Page_Jumbotron_Picture()
        {
            Home_Page_Jumbotron_Picture = OriginalHome_Page_Jumbotron_Picture;
        }
        protected string GetFull_Name()
        {
            using (MSNContext context = new MSNContext())
                return context.ApplicationSettings.Where(x => x.Name == Constants.Full_Name)?.FirstOrDefault()?.Value ?? string.Empty;
        }
        public void SetFull_Name()
        {
            using (MSNContext context = new MSNContext())
            {
                ApplicationSettings entry = context.ApplicationSettings.Where(x => x.Name == Constants.Full_Name)?.FirstOrDefault();
                if (entry == null)
                {
                    entry = new ApplicationSettings
                    {
                        Name = Constants.Full_Name,
                        Value = Full_Name
                    };
                    context.ApplicationSettings.Add(entry);
                }
                else
                {
                    entry.Value = Full_Name;
                }
                context.SaveChanges();
            }
        }

        protected UploadedFileItem GetHome_Page_Profile_Picture()
        {
            using MSNContext context = new MSNContext();

            string location =
                 context.ApplicationSettings.Where(x => x.Name == Constants.Home_Page_Profile_Picture)?.FirstOrDefault()?.Value ?? string.Empty;

            UploadedFileItem item = new(fileInfo);
            item.ProcessAbsoluteLocation(location);
            return item;
        }
        public void SetHome_Page_Profile_Picture()
        {
            using (MSNContext context = new MSNContext())
            {
                // Assuming there will be an unsafe to wwwroot copy of the current file.
                MovePictureToWWWRootImagesFolder(Home_Page_Profile_Picture);
                string filename = Home_Page_Profile_Picture.AbsoluteFilePath;

                ApplicationSettings entry = context.ApplicationSettings.Where(x => x.Name == Constants.Home_Page_Profile_Picture)?.FirstOrDefault();
                if (entry == null)
                {
                    entry = new ApplicationSettings
                    {
                        Name = Constants.Home_Page_Profile_Picture,
                        Value = filename
                    };
                    context.ApplicationSettings.Add(entry);
                }
                else
                {
                    entry.Value = filename;
                }
                context.SaveChanges();
            }
            Home_Page_Profile_Picture = GetHome_Page_Profile_Picture();
            OriginalHome_Page_Profile_Picture = Home_Page_Profile_Picture;
        }
        protected bool MovePictureToWWWRootImagesFolder(UploadedFileItem item)
        {
            // Assuming there will be an unsafe to wwwroot copy of the current file.
            string tempFilePath =item.TempFilePath;

            if (File.Exists(tempFilePath))
            {
                string newFileName = Path.ChangeExtension(Path.GetRandomFileName(), "jpg");
                string newFilePath = Path.Combine(fileInfo.ImagesRootFolder, newFileName);
                File.Move(tempFilePath, newFilePath, true);
                item.AbsoluteFilePath = newFilePath.Replace(fileInfo.WWWRootFolder, "");
                item.TempFilePath = null;
                item.WWWRootFilePath = newFilePath;
                item.FileName = newFileName;
                return true;
            }
            else
                return false;
        }
        protected UploadedFileItem GetHome_Page_Jumbotron_Picture()
        {
            using MSNContext context = new MSNContext();
            string location = context.ApplicationSettings.Where(x => x.Name == Constants.Home_Page_Jumbotron_Picture)?.FirstOrDefault()?.Value ?? string.Empty;
            
            UploadedFileItem item = new(fileInfo);
            item.ProcessAbsoluteLocation(location);
            return item;
        }
        public void SetHome_Page_Jumbotron_Picture()
        {
            using (MSNContext context = new MSNContext())
            {
               
                MovePictureToWWWRootImagesFolder(Home_Page_Jumbotron_Picture);
                string filename = Home_Page_Jumbotron_Picture.AbsoluteFilePath;

                ApplicationSettings entry = context.ApplicationSettings.Where(x => x.Name == Constants.Home_Page_Jumbotron_Picture)?.FirstOrDefault();
                if (entry == null)
                {
                    entry = new ApplicationSettings
                    {
                        Name = Constants.Home_Page_Jumbotron_Picture,
                        Value = filename
                    };
                    context.ApplicationSettings.Add(entry);
                }
                else
                {
                    entry.Value = filename;
                }
                context.SaveChanges();
            }
            Home_Page_Jumbotron_Picture = GetHome_Page_Jumbotron_Picture();
            OriginalHome_Page_Jumbotron_Picture = Home_Page_Jumbotron_Picture;
        }
    }
}