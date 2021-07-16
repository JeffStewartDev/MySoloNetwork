using System;
using System.Collections.Generic;
using System.Linq;
using MSN.Model;
using MSN.ModelContext;
using MSN.ViewModel.Util;

namespace MSN.ViewModel
{
    public class CalendarViewModel
    {
        public MSNContext context { get; set; } = new MSNContext();

        public CalendarViewModel()
        {
            GenerateMonth();
            GenerateMinAndMaxYears();
        }

        public void GenerateMinAndMaxYears()
        {
            MaxDate = GetMaxDate();
            MinDate = GetMinDate();
        }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
        public string Year { get; set; } = DateTime.Now.Year.ToString();
        public string Month { get; set; } = DateTime.Now.Month.ToString();

        public CalendarMonth CurrentMonth { get; set; } = new CalendarMonth();

        public DateTime GetMaxDate()
        {
            List<DateTime> dates = new();
            try
            {
                if (context.EventItems.Where(x => x.Visible).Count() > 0)
                    dates.Add(context.EventItems.Where(x => x.Visible).Max(x => x.DateStarts));
            }
            catch (System.Exception) { }
            try
            {
                if (context.Notes.Where(x => x.Visible).Count() > 0)
                dates.Add(context.Notes.Where(x => x.Visible).Max(x => x.DateCreated));
            }
            catch (System.Exception) { }
            try
            {
                if (context.StatusUpdateEntries.Where(x => x.Visible).Count() > 0)
                dates.Add(context.StatusUpdateEntries.Where(x => x.Visible).Max(x => x.DateCreated));
            }
            catch (System.Exception) { }
            try
            {
                if (context.ImagePosts.Where(x => x.Visible).Count() > 0)
                dates.Add(context.ImagePosts.Where(x => x.Visible).Max(x => x.DateCreated));
            }
            catch (System.Exception) { }
            try
            {
                if (context.Albums.Where(x => x.Visible).Count() > 0)
                dates.Add(context.Albums.Where(x => x.Visible).Max(x => x.DateCreated));
            }
            catch (System.Exception) { }
            dates.Add(DateTime.Now);
            return dates.Max();
        }
        public DateTime GetMinDate()
        {
            List<DateTime> dates = new();

            try
            {
                if (context.ImagePosts.Where(x => x.Visible).Count() > 0)
                dates.Add(context.ImagePosts.Where(x => x.Visible).Max(x => x.DateCreated));
            }
            catch (System.Exception) { }
            try
            {
                if (context.Notes.Where(x => x.Visible).Count() > 0)
                dates.Add(context.Notes.Where(x => x.Visible).Max(x => x.DateCreated));
            }
            catch (System.Exception) { }
            try
            {
                if (context.StatusUpdateEntries.Where(x => x.Visible).Count() > 0)
                dates.Add(context.StatusUpdateEntries.Where(x => x.Visible).Max(x => x.DateCreated));
            }
            catch (System.Exception) { }
            try
            {
                if (context.Albums.Where(x => x.Visible).Count() > 0)
                dates.Add(context.Albums.Where(x => x.Visible).Max(x => x.DateCreated));
            }
            catch (System.Exception) { }
            try
            {
                if (context.EventItems.Where(x => x.Visible).Count() > 0)
                dates.Add(context.EventItems.Where(x => x.Visible).Max(x => x.DateStarts));
            }
            catch (System.Exception) { }
            dates.Add(new DateTime(1949, 1, 1));
            return dates.Min();
        }

        public void GenerateMonth() // Don't ask me why I did it this way....
        {
            CurrentMonth.Clear();



            int year, month;
            if (int.TryParse(Year, out year) && int.TryParse(Month, out month))
                try
                {
                    List<UpdatePostItem> result = new List<UpdatePostItem>();
                    List<UpdatePostItem> statuses = context.StatusUpdateEntries
                                                            .Where(x => x.Visible &&
                                                                        x.DateCreated.Year == year &&
                                                                        x.DateCreated.Month == month)
                                                            .Select(x => new UpdatePostItem()
                                                            {
                                                                PostType = PostTypes.StatusUpdate,
                                                                Id = x.Id,
                                                                DateUpdated = x.DateUpdated,
                                                                DateCreated = x.DateCreated
                                                            })
                                                            .ToList();
                    List<UpdatePostItem> images = context.ImagePosts
                                                            .Where(x => x.Visible &&
                                                                        !x.AlbumId.HasValue &&
                                                                        x.DateCreated.Year == year &&
                                                                        x.DateCreated.Month == month)
                                                            .Select(x => new UpdatePostItem()
                                                            {
                                                                PostType = PostTypes.Image,
                                                                Id = x.Id,
                                                                DateUpdated = x.DateUpdated,
                                                                DateCreated = x.DateCreated
                                                            })
                                                            .ToList();
                    List<UpdatePostItem> notes = context.Notes
                                                            .Where(x => x.Visible &&
                                                                        x.DateCreated.Year == year &&
                                                                        x.DateCreated.Month == month)
                                                            .Select(x => new UpdatePostItem()
                                                            {
                                                                PostType = PostTypes.Note,
                                                                Id = x.Id,
                                                                DateUpdated = x.DateUpdated,
                                                                DateCreated = x.DateCreated
                                                            })
                                                            .ToList();
                    List<UpdatePostItem> albums = context.Albums.Where(
                                                                        x => x.Visible &&
                                                                        x.DateCreated.Year == year &&
                                                                        x.DateCreated.Month == month)
                                                                .Select(x => new UpdatePostItem()
                                                                {
                                                                    PostType = PostTypes.Album,
                                                                    Id = x.Id,
                                                                    DateUpdated = x.DateUpdated,
                                                                    DateCreated = x.DateCreated
                                                                })
                                                                .ToList();
                    List<UpdatePostItem> events = context.EventItems.Where(x => x.Visible &&
                                                                        x.DateCreated.Year == year &&
                                                                        x.DateCreated.Month == month)
                                                                    .Select(x => new UpdatePostItem()
                                                                    {
                                                                        PostType = PostTypes.Event,
                                                                        Id = x.Id,
                                                                        DateUpdated = x.DateStarts,
                                                                        DateCreated = x.DateStarts
                                                                    })
                                                                    .ToList();
                    result.AddRange(notes);
                    result.AddRange(statuses);
                    result.AddRange(images);
                    result.AddRange(albums);
                    result.AddRange(events);
                    List<UpdatePostItem> Posts = result.OrderByDescending(x => x.DateCreated).ToList();

                    int weekNum = 0;
                    int dayNum = 1;
                    CalendarWeek currentWeek = new CalendarWeek();
                    DateTime incDate = new DateTime(year, month, dayNum);
                    for (int i = 1; i <= 32; i++)
                    {
                        if (incDate.Month != month)
                        {
                            if (incDate.AddDays(-1).DayOfWeek != DayOfWeek.Saturday)
                                CurrentMonth.CalendarWeeks.Add(currentWeek);
                            break;
                        }

                        switch (incDate.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                currentWeek.Sunday.Day = i;
                                currentWeek.Sunday.Date = incDate;
                                currentWeek.Sunday.Posts = Posts.Where(x => x.DateCreated.Day == i).ToList();
                                break;
                            case DayOfWeek.Monday:
                                currentWeek.Monday.Day = i;
                                currentWeek.Monday.Date = incDate;
                                currentWeek.Monday.Posts = Posts.Where(x => x.DateCreated.Day == i).ToList();
                                break;
                            case DayOfWeek.Tuesday:
                                currentWeek.Tuesday.Day = i;
                                currentWeek.Tuesday.Date = incDate;
                                currentWeek.Tuesday.Posts = Posts.Where(x => x.DateCreated.Day == i).ToList();
                                break;
                            case DayOfWeek.Wednesday:
                                currentWeek.Wednesday.Day = i;
                                currentWeek.Wednesday.Date = incDate;
                                currentWeek.Wednesday.Posts = Posts.Where(x => x.DateCreated.Day == i).ToList();
                                break;
                            case DayOfWeek.Thursday:
                                currentWeek.Thursday.Day = i;
                                currentWeek.Thursday.Date = incDate;
                                currentWeek.Thursday.Posts = Posts.Where(x => x.DateCreated.Day == i).ToList();
                                break;
                            case DayOfWeek.Friday:
                                currentWeek.Friday.Day = i;
                                currentWeek.Friday.Date = incDate;
                                currentWeek.Friday.Posts = Posts.Where(x => x.DateCreated.Day == i).ToList();
                                break;
                            case DayOfWeek.Saturday:
                                currentWeek.Saturday.Day = i;
                                currentWeek.Saturday.Date = incDate;
                                currentWeek.Saturday.Posts = Posts.Where(x => x.DateCreated.Day == i).ToList();
                                currentWeek.WeekNumber = weekNum++;
                                CurrentMonth.CalendarWeeks.Add(currentWeek);
                                currentWeek = new CalendarWeek();
                                break;
                            default:
                                break;
                        }
                        incDate = incDate.AddDays(1);
                    }

                }
                catch (Exception)
                {

                    throw;
                }
        }
    }
}