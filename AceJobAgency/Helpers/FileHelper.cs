using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceJobAgency.Helpers
{
    public class FileHelper
    {
        private readonly IWebHostEnvironment _environment;

        public FileHelper(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public async Task<string> UploadFile(int userId, IFormFile file)
        {
            var fileName = String.Format("{1:yyyy_MM_dd_hh_mm_ss}_{0}", file.FileName, DateTime.Now);
            var directory = Path.Combine(_environment.ContentRootPath, "Resumes", userId.ToString());
            var filePath = Path.Combine(directory, fileName);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            else { 
                var files = Directory.GetFiles(directory);
                foreach (var item in files)
                {
                    File.Delete(item);
                }
            }
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return filePath;
        }
    }
}
