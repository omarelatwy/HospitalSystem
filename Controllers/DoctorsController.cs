using HospitalSystem.Data;
using HospitalSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalSystem.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 2. Doctors Page (Filter & Pagination)
        public async Task<IActionResult> Index(string? searchName, string? specialization, int page = 1)
        {
            int pageSize = 4;
            var query = _context.Doctors.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                query = query.Where(d => d.Name.Contains(searchName));
            }

            if (!string.IsNullOrWhiteSpace(specialization))
            {
                query = query.Where(d => d.Specialization == specialization);
            }

            int totalDoctors = await query.CountAsync();
            var doctors = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SearchName = searchName;
            ViewBag.Specialization = specialization;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalDoctors / pageSize);
            ViewBag.Specializations = await _context.Doctors.Select(d => d.Specialization).Distinct().ToListAsync();

            return View(doctors);
        }

        // 3. Book Appointment Page (GET)
        public async Task<IActionResult> Book(int doctorId)
        {
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null) return NotFound();

            ViewBag.Doctor = doctor;
            return View(new Appointment { DoctorId = doctorId });
        }

        // 3. Book Appointment Page (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(Appointment appointment)
        {
            // Optional Requirement 1: Date Restriction (Sunday to Thursday)
            var dayOfWeek = appointment.Date.DayOfWeek;
            if (dayOfWeek == DayOfWeek.Friday || dayOfWeek == DayOfWeek.Saturday)
            {
                ModelState.AddModelError("Date", "Appointments are only available from Sunday to Thursday.");
            }

            // Optional Requirement 2: Time Slots (9:00 AM - 4:30 PM, 30 min intervals)
            var startTime = new TimeOnly(9, 0);
            var endTime = new TimeOnly(16, 30);
            if (appointment.Time < startTime || appointment.Time > endTime || appointment.Time.Minute % 30 != 0)
            {
                ModelState.AddModelError("Time", "Select a valid 30-minute slot between 9:00 AM and 4:30 PM.");
            }

            // Optional Requirement 3: Prevent Double Booking
            bool exists = await _context.Appointments.AnyAsync(a => 
                a.DoctorId == appointment.DoctorId && 
                a.Date == appointment.Date && 
                a.Time == appointment.Time);

            if (exists)
            {
                ModelState.AddModelError(string.Empty, "This doctor is already booked at the selected date and time.");
            }

            if (ModelState.IsValid)
            {
                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Appointments");
            }

            ViewBag.Doctor = await _context.Doctors.FindAsync(appointment.DoctorId);
            return View(appointment);
        }
    }
}
