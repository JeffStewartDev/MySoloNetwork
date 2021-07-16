using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSN.Model.Util
{
    public class ServerFileInfo
    {

        public string WWWRootFolder { get; set; }
        public string ImagesRootFolder { get; set; }
        public string UnsafeImagesRootFolder { get; set; }
        public string AppRootDirectory { get; set; }
        public bool DefaultContextPresent { get; set; } = false;
        public bool SecurityContextPresent { get; set; } = false;

        public bool PerformDatabaseCheck(
            MSN.BlazorServer.Data.MSNBlazorServerContext SecurityContext,
            MSN.ModelContext.MSNContext DefaultContext
        )
        {

            bool AreThereUsers;
            try
            {
                AreThereUsers = (SecurityContext?.Users?.Count() ?? 0) > 0;
            }
            catch (Microsoft.Data.Sqlite.SqliteException)
            {
                AreThereUsers = false;
                SecurityContext.Database.EnsureDeleted();
                SecurityContext.Database.EnsureCreated();
            }

            bool AreThereContextTables;
            try
            {
                bool x = (DefaultContext?.ApplicationSettings?.Count() ?? 0) > 0;
                //true if the above statement doesn't thow an exception.
                AreThereContextTables=true;
            }
            catch (Microsoft.Data.Sqlite.SqliteException)
            {
                AreThereContextTables = false;
                DefaultContext.Database.EnsureDeleted();
                DefaultContext.Database.EnsureCreated();
            }


            return (AreThereUsers && AreThereContextTables);

        }
    }
}
