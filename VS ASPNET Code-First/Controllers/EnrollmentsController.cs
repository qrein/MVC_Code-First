using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity;

namespace VS_ASPNET_Code_First.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly SchoolContext _context;

        public EnrollmentsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Enrollments
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["GradeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "grade_desc" : "";
            ViewData["CourseSortParm"] = sortOrder == "Course" ? "course_desc" : "Course";
            ViewData["StudentsSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Student" : "";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var enrollments = from s in _context.Enrollments.Include(f=>f.Course).Include(f=>f.Student)
                              select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                enrollments = enrollments.Where(s => s.Grade.GetValueOrDefault().ToString() == searchString
                                       || s.Course.Title.Contains(searchString) 
                                       || s.Student.FirstMidName.Contains(searchString)
                                       || s.Student.LastName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "grade_desc":
                    enrollments = enrollments.OrderByDescending(s => s.Grade);
                    break;
                case "Course":
                    enrollments = enrollments.OrderBy(s => s.Course);
                    break;
                case "Student":
                    enrollments = enrollments.OrderBy(s => s.Student);
                    break;
                case "course_desc":
                    enrollments = enrollments.OrderByDescending(s => s.Course);
                    break;
                case "student_desc":
                    enrollments = enrollments.OrderBy(s => s.Student);
                    break;
                default:
                    enrollments = enrollments.OrderBy(s => s.Grade);
                    break;
            }
            int pageSize = 5;
            return View(await PaginatedList<Enrollment>.CreateAsync(enrollments.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Enrollments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.EnrollmentID == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: Enrollments/Create
        public IActionResult Create()
        {
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "CourseID");
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "ID");
            return View();
        }

        // POST: Enrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnrollmentID,CourseID,StudentID,Grade")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "CourseID", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "ID", enrollment.StudentID);
            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "CourseID", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "ID", enrollment.StudentID);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EnrollmentID,CourseID,StudentID,Grade")] Enrollment enrollment)
        {
            if (id != enrollment.EnrollmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.EnrollmentID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "CourseID", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "ID", enrollment.StudentID);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.EnrollmentID == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollments.Any(e => e.EnrollmentID == id);
        }
    }
}
