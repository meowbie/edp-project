using GrowGreenWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GrowGreenWeb.Pages.Lecturer.Courses.Manage
{
    public class CreateVideoModel : PageModel
    {
        [BindProperty, Required, DisplayName("Video File")]
        public IFormFile VideoFile { get; set; } = null!;

        [BindProperty, Required, DisplayName("Name")]
        public string Title { get; set; } = string.Empty;

        [BindProperty, Required, DisplayName("Description")]
        public string Description { get; set; } = string.Empty;

        public Course Course { get; set; } = null!;
        public Lecture Lecture { get; set; } = null!;

        private readonly GrowGreenContext _context;
        private readonly IWebHostEnvironment _environment;

        public CreateVideoModel(GrowGreenContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult OnGet(int id, int lectureId)
        {
            // todo: add account system support
            int lecturerId = TemporaryConstants.LecturerId;

            Course? course = _context.Courses
                .Include(c => c.Lectures)
                .SingleOrDefault(c => c.Id == id);
            if (course is null)
                return NotFound();

            if (course.LecturerId != lecturerId)
                return Forbid();

            Course = course;
            Lecture? lecture = _context.Lectures
                .Include(l => l.Videos)
                .SingleOrDefault(l => l.Id == lectureId);

            if (lecture is null)
                return NotFound();
            if (lecture.CourseId != course.Id)
                return Forbid();

            Lecture = lecture;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, int lectureId)
        {
            // todo: add account system support
            int lecturerId = TemporaryConstants.LecturerId;

            Course? course = _context.Courses
                .Include(c => c.Lectures)
                .SingleOrDefault(c => c.Id == id);
            if (course is null)
                return NotFound();

            if (course.LecturerId != lecturerId)
                return Forbid();

            Course = course;
            Lecture? lecture = _context.Lectures
                .Include(l => l.Videos)
                .SingleOrDefault(l => l.Id == lectureId);

            if (lecture is null)
                return NotFound();
            if (lecture.CourseId != course.Id)
                return Forbid();

            Lecture = lecture;

            if (VideoFile is null)
            {
                TempData["FlashMessage.Type"] = "danger";
                TempData["FlashMessage.Text"] = "Error uploading image";
                return OnGet(id, lectureId);
            }

            if (!Constants.AllowedImageExtensions.Contains(Path.GetExtension(VideoFile.FileName)))
            {
                TempData["FlashMessage.Type"] = "danger";
                TempData["FlashMessage.Text"] = "Image file type not allowed!";
                return OnGet(id, lectureId);
            }

            string random = Guid.NewGuid().ToString();
            string webRootPath = "/uploads/course/" + Course.Id + "/lecture/" + Lecture.Id + "/" + random + "-" + VideoFile.FileName;
            var file = Path.Combine(
                _environment.WebRootPath, "uploads", "course", Course.Id.ToString(), "lecture", Lecture.Id.ToString(), 
                random + "-" + VideoFile.FileName);

            await using (var fileStream = new FileStream(file, FileMode.Create))
            {
                await VideoFile.CopyToAsync(fileStream);
            }

            //ImageUrl = webRootPath;

            // upload new video into db
            Video video = new Video
            {
                Name = Title,
            }

            await _context.SaveChangesAsync();

            TempData["FlashMessage.Type"] = "success";
            TempData["FlashMessage.Text"] = "Successfully updated image";

            return RedirectToPage("Contents", new { id, lectureId });
        }
    }
}