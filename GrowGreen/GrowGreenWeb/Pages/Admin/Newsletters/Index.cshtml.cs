using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GrowGreenWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GrowGreenWeb.Pages.Admin.Newsletters
{
    public class IndexModel : PageModel
    {
        [BindProperty, Required]
        public string Message { get; set; } = string.Empty;
        
        [BindProperty, DisplayName("New embedded image (jpg/jpeg/png/gif)")]
        public IFormFile? Upload { get; set; }
        
        public int RecipientCount { get; set; }
        
        private readonly GrowGreenContext _context;
        private readonly IWebHostEnvironment _environment;

        public IndexModel(GrowGreenContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        
        public async Task<IActionResult> OnGetAsync()
        {
            // todo: add account system support
            int adminId = TemporaryConstants.AdminId;
            
            // get recipient count
            RecipientCount = await _context.Emails.CountAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // todo: add account system support
            int adminId = TemporaryConstants.AdminId;
            
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(Message))
            {
                return Page();
            }

            Newsletter newsletter = new Newsletter
            {
                Timestamp = DateTime.Now
            };

            await _context.AddAsync(newsletter);
            await _context.SaveChangesAsync();

            NewsletterEditHistory history = new NewsletterEditHistory
            {
                Timestamp = newsletter.Timestamp,
                Content = Message,
                Description = null,
                NewsletterId = newsletter.Id,
                UserId = adminId
            };

            await _context.AddAsync(history);
            await _context.SaveChangesAsync();

            TempData["FlashMessage.Type"] = "success";
            TempData["FlashMessage.Text"] = "Successfully sent newsletter";
            
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            ModelState.Clear();

            if (Upload is null)
            {
                TempData["FlashMessage.Type"] = "danger";
                TempData["FlashMessage.Text"] = "Error uploading image";
                return await OnGetAsync();
            }

            if (!Constants.AllowedImageExtensions.Contains(Path.GetExtension(Upload.FileName)))
            {
                TempData["FlashMessage.Type"] = "danger";
                TempData["FlashMessage.Text"] = "Image file type not allowed!";
                return await OnGetAsync();
            }
            
            string random = Guid.NewGuid().ToString();
            string webRootPath = "/uploads/newsletter/" + random + "-" + Upload.FileName;
            var file = Path.Combine(_environment.WebRootPath, "uploads", "newsletter", random + "-" + Upload.FileName);
            await using (var fileStream = new FileStream(file, FileMode.Create))
            {
                await Upload.CopyToAsync(fileStream);
            }

            string html = $"<img src=\"{webRootPath}\" />";
            Message += html;
            
            TempData["FlashMessage.Type"] = "success";
            TempData["FlashMessage.Text"] = "Successfully uploaded image";
            
            return await OnGetAsync();
        }
    }
}