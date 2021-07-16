using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace MSN.Model.Util
{
    public class UploadedFileItem
    {
        
        public string FileName { get; set; }
        
        public string WWWRootFilePath { get; set; }
        public string AbsoluteFilePath { get; set; }
        public string TempFilePath { get; set; }
        ServerFileInfo fileInfo;
        public UploadedFileItem(ServerFileInfo info)
        {
            fileInfo=info;
            // Assuming the stored path is an absolute web link.
        }
        public void ProcessAbsoluteLocation(string AbsoluteLocation)
        {
            AbsoluteFilePath = AbsoluteLocation;
            TempFilePath = null;
            WWWRootFilePath = null;
            FileName=null;
            if (AbsoluteFilePath.Contains(fileInfo.UnsafeImagesRootFolder.Replace(fileInfo.AppRootDirectory,"")))
            // If it's a temp/unsafe file
                TempFilePath = $"{fileInfo.AppRootDirectory?.TrimEnd(Path.DirectorySeparatorChar)}{AbsoluteFilePath}";
                
            if (AbsoluteFilePath.Contains(fileInfo.ImagesRootFolder.Replace(fileInfo.AppRootDirectory,"")))
            // If it's a Web Image file
                WWWRootFilePath =  $"{fileInfo.AppRootDirectory?.TrimEnd(Path.DirectorySeparatorChar)}{AbsoluteFilePath}";
                string filename=TempFilePath??WWWRootFilePath;
                if (File.Exists(filename))
                {
                    FileInfo infoItem = new FileInfo(filename);
                    FileName = infoItem.Name;
                    FileName = FileName.Replace(fileInfo.UnsafeImagesRootFolder,string.Empty).Replace(fileInfo.ImagesRootFolder,string.Empty);
                }
        }
    }
}
