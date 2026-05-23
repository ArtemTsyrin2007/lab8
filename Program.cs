using System;
using System.Collections.Generic;
using System.Linq;

namespace RailwaySystem
{
    class Program
    {
        static readonly List<Train> Trains = new List<Train>();

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                PrintMenu();
                string choice = Ask("Your choice").Trim();

                Console.WriteLine();
                switch (choice)
                {
                    case "1":  AddTrain();            break;   // 1.1
                    case "2":  RemoveTrain();         break;   // 1.2
                    case "3":  ViewAllTrains();       break;   // 1.3
                    case "4":  ViewTrainDetails();    break;   // 1.4
                    case "5":  ViewOccupancyStats();  break;   // 1.5

                    case "6":  AddCarriage();         break;   // 2.1
                    case "7":  RemoveCarriage();      break;   // 2.2
                    case "8":  ViewCarriageSeats();   break;   // 2.3

                    case "9":  AddBooking();          break;   // 3.1
                    case "10": CancelBooking();       break;   // 3.2
                    case "11": ModifyBooking();       break;   // 3.3
                    case "12": ViewBookings();        break;   // 3.4

                    case "13": SearchTrains();        break;   // 4.1
                    case "14": SearchByDate();        break;   // 4.2

                    case "0":
                        Console.WriteLine("Goodbye!");
                        return;

                    default:
                        Warn("Unknown option. Please try again.");
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey(true);
            }
        }
        static void AddTrain()
        {
            string number = Ask("Train number");
            if (string.IsNullOrWhiteSpace(number)) { Warn("Train number cannot be empty."); return; }

            Trains.Add(new Train(number.Trim()));
            Ok($"Train #{number} added.");
        }

        static void RemoveTrain()
        {
            if (!HasTrains()) return;

            string number = Ask("Train number to remove");
            Train? train  = FindTrain(number);
            if (train == null) return;

            Trains.Remove(train);
            Ok($"Train #{number} removed.");
        }

        static void ViewAllTrains()
        {
            if (!HasTrains()) return;

            Header("All Trains");
            foreach (Train t in Trains)
                Console.WriteLine(t);
        }

        static void ViewTrainDetails()
        {
            if (!HasTrains()) return;

            string number = Ask("Train number");
            Train? train  = FindTrain(number);
            if (train == null) return;

            Header($"Train #{train.TrainNumber} — details");
            Console.WriteLine($"  Total carriages : {train.Carriages.Count}");
            Console.WriteLine($"  Total seats     : {train.Carriages.Sum(c => c.TotalSeats)}");
            Console.WriteLine($"  Total bookings  : {train.Carriages.Sum(c => c.Bookings.Count)}");

            if (train.Carriages.Count == 0)
            {
                Console.WriteLine("  (no carriages attached)");
                return;
            }

            Console.WriteLine("\n  Carriages:");
            foreach (Carriage c in train.Carriages)
                Console.WriteLine(c);
        }

        static void ViewOccupancyStats()
        {
            if (!HasTrains()) return;

            string number = Ask("Train number");
            Train? train  = FindTrain(number);
            if (train == null) return;

            if (train.Carriages.Count == 0)
            {
                Warn("This train has no carriages.");
                return;
            }

            Header($"Train #{train.TrainNumber} — occupancy");
            foreach (Carriage c in train.Carriages)
            {
                string bar = OccupancyBar(c.OccupancyPercent);
                Console.WriteLine($"  Carriage #{c.CarriageNumber,-3} [{bar}] {c.OccupancyPercent,5:F1}%  " +
                                  $"({c.Bookings.Count} booked / {c.TotalSeats - c.Bookings.Count} free)");
            }
        }
        static void AddCarriage()
        {
            if (!HasTrains()) return;

            string number = Ask("Train number");
            Train? train  = FindTrain(number);
            if (train == null) return;

            int? carriageNum = AskInt("Carriage number");
            if (carriageNum == null) return;

            int? seats = AskInt("Number of seats");
            if (seats == null || seats < 1) { Warn("Seat count must be at least 1."); return; }

            train.AddCarriage(new Carriage(carriageNum.Value, seats.Value));
            Ok($"Carriage #{carriageNum} ({seats} seats) added to train #{train.TrainNumber}.");
        }

        static void RemoveCarriage()
        {
            if (!HasTrains()) return;

            string number = Ask("Train number");
            Train? train  = FindTrain(number);
            if (train == null) return;

            int? carriageNum = AskInt("Carriage number to remove");
            if (carriageNum == null) return;

            if (train.RemoveCarriage(carriageNum.Value))
                Ok($"Carriage #{carriageNum} removed.");
            else
                Warn($"Cannot remove carriage #{carriageNum}: not found or it has active bookings.");
        }
        static void ViewCarriageSeats()
        {
            if (!HasTrains()) return;

            string   number   = Ask("Train number");
            Train?   train    = FindTrain(number);
            if (train == null) return;

            int?     carNum   = AskInt("Carriage number");
            if (carNum == null) return;

            Carriage? carriage = train.Carriages.FirstOrDefault(c => c.CarriageNumber == carNum);
            if (carriage == null) { Warn($"Carriage #{carNum} not found on train #{number}."); return; }

            Header($"Carriage #{carriage.CarriageNumber} — seats");
            Console.WriteLine($"  Total: {carriage.TotalSeats}  |  " +
                              $"Booked: {carriage.Bookings.Count}  |  " +
                              $"Free: {carriage.TotalSeats - carriage.Bookings.Count}");
            Console.WriteLine();

            HashSet<int> bookedSeats = new HashSet<int>(carriage.Bookings.Select(b => b.SeatNumber));
            for (int s = 1; s <= carriage.TotalSeats; s++)
            {
                if (bookedSeats.Contains(s))
                {
                    Booking? b = carriage.Bookings.First(x => x.SeatNumber == s);
                    Console.WriteLine($"  Seat {s,3}: [BOOKED] — {b.PassengerName}");
                }
                else
                {
                    Console.WriteLine($"  Seat {s,3}: [FREE  ]");
                }
            }
        }
        static void AddBooking()
        {
            if (!HasTrains()) return;

            string   number    = Ask("Train number");
            Train?   train     = FindTrain(number);
            if (train == null) return;

            int?     carNum    = AskInt("Carriage number");
            if (carNum == null) return;

            Carriage? carriage = train.Carriages.FirstOrDefault(c => c.CarriageNumber == carNum);
            if (carriage == null) { Warn($"Carriage #{carNum} not found."); return; }

            int? seatNum = AskInt("Seat number");
            if (seatNum == null) return;

            if (!carriage.IsSeatAvailable(seatNum.Value))
            {
                Warn($"Seat {seatNum} is not available (out of range or already booked).");
                return;
            }

            string passenger = Ask("Passenger name");
            if (string.IsNullOrWhiteSpace(passenger)) { Warn("Passenger name cannot be empty."); return; }

            Booking booking = new Booking(passenger.Trim(), seatNum.Value);
            carriage.Bookings.Add(booking);
            Ok($"Booking #{booking.Id} created — {passenger}, seat {seatNum}, carriage #{carNum}.");
        }

        static void CancelBooking()
        {
            int? bookingId = AskInt("Booking ID to cancel");
            if (bookingId == null) return;

            (Booking? booking, Carriage? carriage) = FindBookingById(bookingId.Value);
            if (booking == null || carriage == null) { Warn($"Booking #{bookingId} not found."); return; }

            carriage.Bookings.Remove(booking);
            Ok($"Booking #{bookingId} ({booking.PassengerName}, seat {booking.SeatNumber}) cancelled.");
        }

        static void ModifyBooking()
        {
            int? bookingId = AskInt("Booking ID to modify");
            if (bookingId == null) return;

            (Booking? booking, Carriage? carriage) = FindBookingById(bookingId.Value);
            if (booking == null || carriage == null) { Warn($"Booking #{bookingId} not found."); return; }

            Console.WriteLine($"\n  Current: {booking}");

            string newName = Ask($"New passenger name (leave blank to keep \"{booking.PassengerName}\")");
            if (!string.IsNullOrWhiteSpace(newName))
                booking.PassengerName = newName.Trim();

            string newSeatRaw = Ask($"New seat number (leave blank to keep {booking.SeatNumber})");
            if (!string.IsNullOrWhiteSpace(newSeatRaw))
            {
                if (!int.TryParse(newSeatRaw.Trim(), out int newSeat))
                {
                    Warn("Invalid seat number — seat unchanged.");
                }
                else if (newSeat == booking.SeatNumber)
                {
                    Console.WriteLine("  Seat unchanged.");
                }
                else if (!carriage.IsSeatAvailable(newSeat))
                {
                    Warn($"Seat {newSeat} is not available — seat unchanged.");
                }
                else
                {
                    booking.SeatNumber = newSeat;
                }
            }

            Ok($"Booking #{bookingId} updated.");
        }
        static void ViewBookings()
        {
            List<Booking> all = AllBookings();
            if (all.Count == 0) { Warn("No bookings in the system."); return; }

            Header("All Bookings");
            foreach (Booking b in all)
                Console.WriteLine(b);
        }

        static void SearchTrains()
        {
            string keyword = Ask("Keyword");
            if (string.IsNullOrWhiteSpace(keyword)) { Warn("Keyword cannot be empty."); return; }

            List<Train> results = Trains.Where(t => t.MatchesKeyword(keyword.Trim())).ToList();

            if (results.Count == 0) { Warn("No trains matched."); return; }

            Header($"Trains matching \"{keyword}\"");
            foreach (Train t in results)
                Console.WriteLine(t);
        }

        static void SearchByDate()
        {
            string raw = Ask("Date (dd.MM.yyyy)");
            if (!DateTime.TryParseExact(raw.Trim(), "dd.MM.yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime date))
            {
                Warn("Invalid date format. Expected dd.MM.yyyy");
                return;
            }

            List<Booking> results = AllBookings()
                .Where(b => b.BookingDate.Date == date.Date)
                .ToList();

            if (results.Count == 0) { Warn($"No bookings found for {date:dd.MM.yyyy}."); return; }

            Header($"Bookings for {date:dd.MM.yyyy}");
            foreach (Booking b in results)
                Console.WriteLine(b);
        }

         static List<Booking> AllBookings() =>
            Trains.SelectMany(t => t.Carriages)
                  .SelectMany(c => c.Bookings)
                  .ToList();


        static (Booking?, Carriage?) FindBookingById(int id)
        {
            foreach (Train t in Trains)
                foreach (Carriage c in t.Carriages)
                {
                    Booking? b = c.Bookings.FirstOrDefault(x => x.Id == id);
                    if (b != null) return (b, c);
                }
            return (null, null);
        }


        static Train? FindTrain(string number)
        {
            Train? t = Trains.FirstOrDefault(x =>
                x.TrainNumber.Equals(number.Trim(), StringComparison.OrdinalIgnoreCase));
            if (t == null) Warn($"Train #{number} not found.");
            return t;
        }


        static bool HasTrains()
        {
            if (Trains.Count == 0) { Warn("No trains in the system yet."); return false; }
            return true;
        }


        static int? AskInt(string prompt)
        {
            string raw = Ask(prompt);
            if (int.TryParse(raw.Trim(), out int value)) return value;
            Warn("Expected a whole number.");
            return null;
        }


        static string Ask(string prompt)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"  >> {prompt}: ");
            Console.ResetColor();
            return Console.ReadLine() ?? string.Empty;
        }


        static string OccupancyBar(double percent)
        {
            int filled = (int)Math.Round(percent / 10.0);
            return new string('█', filled) + new string('░', 10 - filled);
        }



        static void PrintMenu()
        {
            Console.Clear();
            Separator('═');
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  Railway Ticket Booking System");
            Console.ResetColor();
            Separator('═');

            Section("Train management");
            Opt(" 1", "Add train");
            Opt(" 2", "Remove train");
            Opt(" 3", "View all trains");
            Opt(" 4", "View train details");
            Opt(" 5", "View carriage occupancy stats");

            Section("Carriage management");
            Opt(" 6", "Add carriage to train");
            Opt(" 7", "Remove carriage  (only if no bookings)");
            Opt(" 8", "View carriage seats  (occupied / free)");

            Section("Booking management");
            Opt(" 9", "Add booking");
            Opt("10", "Cancel booking");
            Opt("11", "Modify booking");
            Opt("12", "View all bookings");

            Section("Search");
            Opt("13", "Search trains by keyword");
            Opt("14", "Search bookings by date");

            Separator('-');
            Opt(" 0", "Exit");
            Separator('-');
        }

        static void Section(string label)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"\n  {label}");
            Console.ResetColor();
        }

        static void Opt(string key, string label) =>
            Console.WriteLine($"  [{key}]  {label}");

        static void Header(string title)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  --- {title} ---");
            Console.ResetColor();
        }

        static void Ok(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  OK  {msg}");
            Console.ResetColor();
        }

        static void Warn(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  !   {msg}");
            Console.ResetColor();
        }

        static void Separator(char ch) =>
            Console.WriteLine(new string(ch, 52));
    }
}
