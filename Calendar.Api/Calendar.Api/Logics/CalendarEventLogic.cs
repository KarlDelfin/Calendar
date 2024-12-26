using System.Globalization;
using Calendar.Api.DatabaseConnection;
using Calendar.Api.DTO;
using Calendar.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Calendar.Api.Logics
{
    public class CalendarEventLogic
    {
        private readonly CalendarContext _context;
        public CalendarEventLogic(CalendarContext context)
        {
           _context = context; 
        }

        public async Task<List<CalendarEventDTO_GET>> GetCalendarEvent(Guid calendarId, DateTime dateTimeStarted, DateTime dateTimeEnded)
        {
            var data = await (from ce in _context.CalendarEvents
                              join c in _context.Calendars on ce.CalendarId equals c.CalendarId
                              where ce.CalendarId == calendarId && ((ce.DateTimeStarted <= dateTimeEnded && ce.DateTimeEnded >= dateTimeStarted) ||
                                    (ce.DateTimeStarted >= dateTimeStarted && ce.DateTimeStarted <= dateTimeEnded) ||
                                    (ce.DateTimeEnded >= dateTimeStarted && ce.DateTimeEnded <= dateTimeEnded))
                              orderby ce.DateTimeStarted ascending
                              select new CalendarEventDTO_GET
                              {
                                  CalendarEventId = ce.CalendarEventId,
                                  CalendarId = ce.CalendarId,
                                  CalendarName = c.Name,
                                  EventName = ce.Name,
                                  EventDescription = ce.Description,
                                  EventColor = ce.Color,
                                  IsRecurring = ce.IsRecurring,
                                  DateTimeStarted = ce.DateTimeStarted,
                                  DateTimeEnded = ce.DateTimeEnded,
                                  CalendarEventGroupId = ce.CalendarEventGroupId
                              }).ToListAsync();
            return data;
        }

        public async Task<CalendarEventDTO_GET> GetCalendarEventByCalendarEventId(Guid calendarEventId)
        {
            var data = await (from ce in _context.CalendarEvents
                              join c in _context.Calendars on ce.CalendarId equals c.CalendarId
                              where ce.CalendarEventId == calendarEventId
                              orderby ce.DateTimeStarted ascending
                              select new CalendarEventDTO_GET
                              {
                                  CalendarEventId = ce.CalendarEventId,
                                  CalendarId = ce.CalendarId,
                                  CalendarName = c.Name,
                                  EventName = ce.Name,
                                  EventDescription = ce.Description,
                                  EventColor = ce.Color,
                                  IsRecurring = ce.IsRecurring,
                                  DateTimeStarted = ce.DateTimeStarted,
                                  DateTimeEnded = ce.DateTimeEnded,
                                  CalendarEventGroupId = ce.CalendarEventGroupId
                              }).FirstOrDefaultAsync();
            return data;
        }

        public async Task<List<CalendarEventDTO_GET>> GetCalendarEventByCalendarEventGroupId(Guid calendarEventGroupId)
        {
            var data = await (from ce in _context.CalendarEvents
                              join c in _context.Calendars on ce.CalendarId equals c.CalendarId
                              where ce.CalendarEventGroupId == calendarEventGroupId
                              orderby ce.DateTimeStarted ascending
                              select new CalendarEventDTO_GET
                              {
                                  CalendarEventId = ce.CalendarEventId,
                                  CalendarId = ce.CalendarId,
                                  CalendarName = c.Name,
                                  EventName = ce.Name,
                                  EventDescription = ce.Description,
                                  EventColor = ce.Color,
                                  IsRecurring = ce.IsRecurring,
                                  DateTimeStarted = ce.DateTimeStarted,
                                  DateTimeEnded = ce.DateTimeEnded,
                                  CalendarEventGroupId = ce.CalendarEventGroupId
                              }).ToListAsync();
            return data;
        }

        public async Task<bool> AddNonRecurringCalendarEvent(CalendarEventDTO_POST dto)
        {
            int success = 0;

            var data = new CalendarEvent();

            data.CalendarEventId = Guid.NewGuid();
            data.CalendarId = dto.CalendarId;
            data.Name = dto.EventName;
            data.Description = dto.EventDescription;
            data.Color = dto.EventColor;
            data.IsRecurring = false;
            data.DateTimeStarted = dto.DateTimeStarted;
            data.DateTimeEnded = dto.DateTimeEnded;
            data.CalendarEventGroupId = null;

            _context.CalendarEvents.Add(data);
            success = await _context.SaveChangesAsync();

            return success > 0;
        }

        public async Task<bool> AddRecurringCalendarEvent(List<CalendarEventDTO_POST> dto)
        {
            int success = 0;
            Guid calendarEventGroupId = Guid.NewGuid();

            var data = dto.Select(x => new CalendarEvent
            {
                CalendarEventId = Guid.NewGuid(),
                CalendarId = x.CalendarId,
                Name = x.EventName,
                Description = x.EventDescription,
                Color = x.EventColor,
                IsRecurring = true,
                DateTimeStarted = x.DateTimeStarted,
                DateTimeEnded = x.DateTimeEnded,
                CalendarEventGroupId = calendarEventGroupId
            });

            _context.CalendarEvents.AddRange(data);
            success = await _context.SaveChangesAsync();

            return success > 0;
        }

        public async Task<bool> MoveResizeEvent(Guid calendarEventId, CalendarEventDTO_MOVE dto)
        {
            int success = 0;
            var data = await _context.CalendarEvents.FirstOrDefaultAsync(x => x.CalendarEventId == calendarEventId);
            
            data.DateTimeStarted = dto.DateTimeStarted;
            data.DateTimeEnded = dto.DateTimeEnded;

            _context.CalendarEvents.Update(data);
            success = await _context.SaveChangesAsync();

            return success > 0;
        }

        public async Task<bool> DeleteCalendarEventByCalendarEventId(Guid calendarEventId)
        {
            int success = 0;
            var data = await _context.CalendarEvents.FirstOrDefaultAsync(x => x.CalendarEventId == calendarEventId);

            _context.CalendarEvents.Remove(data);
            success = await _context.SaveChangesAsync();

            return success > 0;
        }

        public async Task<bool> DeleteCalendarEventByCalendarEventGroupId(Guid calendarEventGroupId)
        {
            int success = 0;
            var data = await _context.CalendarEvents.Where(x => x.CalendarEventGroupId == calendarEventGroupId).ToListAsync();

            _context.CalendarEvents.RemoveRange(data);
            success = await _context.SaveChangesAsync();

            return success > 0;

        }
    }
}
