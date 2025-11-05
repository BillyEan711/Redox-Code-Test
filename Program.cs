using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Redox_Code_Test
{
    // Exercise 2: Event class
    public class Event
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime DateTime { get; set; }

        public Event(string name, string location, DateTime dateTime)
        {
            Name = name;
            Location = location;
            DateTime = dateTime;
        }

        public override string ToString()
        {
            return $"{Name} at {Location} on {DateTime:yyyy-MM-dd HH:mm}";
        }
    }

    // Exercise 2: EventScheduler class
    public class EventScheduler
    {
        private List<Event> events;
        private const string StorageFile = "events.json";

        public EventScheduler()
        {
            events = new List<Event>();
            LoadEvents();
        }

        public void ScheduleEvent(Event eventItem)
        {
            // Check for double-booking (same date and time)
            var conflictingEvent = events.FirstOrDefault(e => e.DateTime == eventItem.DateTime);
            if (conflictingEvent != null)
            {
                Console.WriteLine($"Warning: Event '{eventItem.Name}' conflicts with existing event '{conflictingEvent.Name}' at {eventItem.DateTime:yyyy-MM-dd HH:mm}");
                Console.WriteLine("Event not scheduled due to conflict.");
                return;
            }

            events.Add(eventItem);
            SaveEvents();
            Console.WriteLine($"Event scheduled: {eventItem}");
        }

        public void CancelEvent(string eventName)
        {
            var eventToRemove = events.FirstOrDefault(e => e.Name.Equals(eventName, StringComparison.OrdinalIgnoreCase));
            if (eventToRemove != null)
            {
                events.Remove(eventToRemove);
                SaveEvents();
                Console.WriteLine($"Event cancelled: {eventToRemove}");
            }
            else
            {
                Console.WriteLine($"Event '{eventName}' not found.");
            }
        }

        public List<Event> GetUpcomingEvents()
        {
            return events.Where(e => e.DateTime > DateTime.Now)
                        .OrderBy(e => e.DateTime)
                        .ToList();
        }

        private void SaveEvents()
        {
            try
            {
                var json = JsonSerializer.Serialize(events, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(StorageFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving events: {ex.Message}");
            }
        }

        private void LoadEvents()
        {
            try
            {
                if (File.Exists(StorageFile))
                {
                    var json = File.ReadAllText(StorageFile);
                    events = JsonSerializer.Deserialize<List<Event>>(json) ?? new List<Event>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading events: {ex.Message}");
                events = new List<Event>();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Redox Code Test ===\n");

            // Exercise 1: LINQ Query
            Console.WriteLine("Exercise 1: LINQ Query");
            Exercise1_LinqQuery();

            Console.WriteLine("\n" + new string('=', 50) + "\n");

            // Exercise 2: Event Scheduler
            Console.WriteLine("Exercise 2: Event Scheduler");
            Exercise2_EventScheduler();
        }

        static void Exercise1_LinqQuery()
        {
            // Create a list of integers from 1 to 100
            var numbers = Enumerable.Range(1, 100).ToList();
            Console.WriteLine("Created list of integers from 1 to 100.");

            // Use LINQ to find all even numbers and print them
            var evenNumbers = numbers.Where(n => n % 2 == 0).ToList();
            Console.WriteLine($"\nEven numbers ({evenNumbers.Count} total):");
            Console.WriteLine(string.Join(", ", evenNumbers));

            // Use a loop to find numbers divisible by 3 or 5, but not 3 and 5
            var specialNumbers = new List<int>();
            foreach (var number in numbers)
            {
                bool divisibleBy3 = number % 3 == 0;
                bool divisibleBy5 = number % 5 == 0;
                
                // Divisible by 3 or 5, but not both
                if ((divisibleBy3 || divisibleBy5) && !(divisibleBy3 && divisibleBy5))
                {
                    specialNumbers.Add(number);
                }
            }

            Console.WriteLine($"\nNumbers divisible by 3 or 5, but not 3 and 5 ({specialNumbers.Count} total):");
            Console.WriteLine(string.Join(", ", specialNumbers));
        }

        static void Exercise2_EventScheduler()
        {
            var scheduler = new EventScheduler();

            // Demonstrate scheduling events
            Console.WriteLine("Scheduling some sample events...\n");
            
            var meetingTime = DateTime.Now.AddDays(1);
            scheduler.ScheduleEvent(new Event("Team Meeting", "Conference Room A", meetingTime));
            scheduler.ScheduleEvent(new Event("Project Review", "Conference Room B", DateTime.Now.AddDays(2)));
            scheduler.ScheduleEvent(new Event("Training Session", "Training Room", DateTime.Now.AddDays(3)));
            
            // Try to schedule a conflicting event (same exact time as Team Meeting)
            Console.WriteLine("\nTrying to schedule a conflicting event:");
            scheduler.ScheduleEvent(new Event("Duplicate Meeting", "Conference Room C", meetingTime));

            // Show upcoming events
            Console.WriteLine("\nUpcoming events:");
            var upcomingEvents = scheduler.GetUpcomingEvents();
            foreach (var evt in upcomingEvents)
            {
                Console.WriteLine($"- {evt}");
            }

            // Cancel an event
            Console.WriteLine("\nCancelling 'Team Meeting':");
            scheduler.CancelEvent("Team Meeting");

            // Show updated upcoming events
            Console.WriteLine("\nUpdated upcoming events:");
            upcomingEvents = scheduler.GetUpcomingEvents();
            foreach (var evt in upcomingEvents)
            {
                Console.WriteLine($"- {evt}");
            }

            Console.WriteLine("\nEvents are automatically saved to 'events.json' file for persistence.");
        }
    }
}
