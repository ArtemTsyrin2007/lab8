using System;
using System.Collections.Generic;
using System.Linq;

namespace RailwaySystem
{

    public class Booking
    {
        private static int _nextId = 1;

        public int      Id            { get; }
        public string   PassengerName { get; set; }
        public int      SeatNumber    { get; set; }
        public DateTime BookingDate   { get; }

        public Booking(string passengerName, int seatNumber)
        {
            Id            = _nextId++;
            PassengerName = passengerName;
            SeatNumber    = seatNumber;
            BookingDate   = DateTime.Now;
        }

        public override string ToString() =>
            $"  Booking #{Id,-3} | Passenger: {PassengerName,-20} " +
            $"| Seat: {SeatNumber,-4} | Date: {BookingDate:dd.MM.yyyy HH:mm}";
    }


    public class Carriage
    {
        public int           CarriageNumber { get; }
        public int           TotalSeats     { get; }
        public List<Booking> Bookings       { get; } = new List<Booking>();

        public bool   HasBookings      => Bookings.Count > 0;
        public double OccupancyPercent => TotalSeats == 0
                                            ? 0.0
                                            : (double)Bookings.Count / TotalSeats * 100.0;

        public Carriage(int carriageNumber, int totalSeats)
        {
            CarriageNumber = carriageNumber;
            TotalSeats     = totalSeats;
        }
        public bool IsSeatAvailable(int seatNumber) =>
            seatNumber >= 1 &&
            seatNumber <= TotalSeats &&
            Bookings.All(b => b.SeatNumber != seatNumber);

        public override string ToString() =>
            $"  Carriage #{CarriageNumber,-3} | Seats: {TotalSeats,-4} " +
            $"| Booked: {Bookings.Count,-4} | Free: {TotalSeats - Bookings.Count,-4} " +
            $"| Occupancy: {OccupancyPercent:F1}%";
    }


    public class Train
    {
        public string        TrainNumber { get; }
        public List<Carriage> Carriages  { get; } = new List<Carriage>();

        public Train(string trainNumber) => TrainNumber = trainNumber;

        public void AddCarriage(Carriage carriage) => Carriages.Add(carriage);


        public bool RemoveCarriage(int carriageNumber)
        {
            Carriage? target = Carriages.FirstOrDefault(c => c.CarriageNumber == carriageNumber);
            if (target == null || target.HasBookings)
                return false;
            Carriages.Remove(target);
            return true;
        }
        public bool MatchesKeyword(string keyword) =>
            TrainNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase);

        public override string ToString() =>
            $"  Train #{TrainNumber,-8} | Carriages: {Carriages.Count}";
    }
}
