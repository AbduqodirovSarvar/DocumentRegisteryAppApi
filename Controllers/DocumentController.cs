using DocumentRegisteryAppApi.Data.EF;
using DocumentRegisteryAppApi.Data.Entities;
using DocumentRegisteryAppApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Net.Mime;

namespace DocumentRegisteryAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController(
        AppDbContext appDbContext,
        IWebHostEnvironment env
        ) : ControllerBase
    {
        private readonly AppDbContext _appDbContext = appDbContext;
        private IWebHostEnvironment _env = env;

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateDocumentModel model)
        {
            var newDoc = new DocumentEntity()
            {
                Number = model.Number,
                OutDate = model.OutDate,
                OutNumber = model.OutNumber,
                DeliveryMethod = model.DeliveryMethod,
                Correspondent = model.Correspondent,
                Subject = model.Subject,
                Description = model.Description,
                DueDate = model.DueDate,
                Access = model.Access,
                Control = model.Control,
                FileName = await SaveFileAsync(model.File)
            };

            await _appDbContext.Documents.AddAsync(newDoc);
            await _appDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UpdateDocumentModel model)
        {
            var doc = await _appDbContext.Documents.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (doc == null)
            {
                return NotFound();
            }

            doc.Number = model.Number ?? doc.Number;
            doc.OutDate = model.OutDate ?? doc.OutDate;
            doc.OutNumber = model.OutNumber ?? doc.OutNumber;
            doc.DeliveryMethod = model.DeliveryMethod ?? doc.DeliveryMethod;
            doc.Correspondent = model.Correspondent ?? doc.Correspondent;
            doc.Subject = model.Subject ?? doc.Subject;
            doc.Description = model.Description ?? doc.Description;
            doc.DueDate = model.DueDate ?? doc.DueDate;
            doc.Access = model.Access ?? doc.Access;
            doc.Control = model.Control ?? doc.Control;
            doc.FileName = model.File != null ? await SaveFileAsync(model.File) : doc.FileName;

            await _appDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var doc = await _appDbContext.Documents.FirstOrDefaultAsync(x => x.Id == Id);
            if (doc == null)
            {
                return NotFound();
            }

            _appDbContext.Documents.Remove(doc);
            await _appDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid Id)
        {
            var doc = await _appDbContext.Documents.FirstOrDefaultAsync(x => x.Id == Id);
            if (doc == null)
            {
                return NotFound();
            }

            return Ok(doc);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery] int size = 10, int pageIndex = 0)
        {
            try
            {
                var docs = await _appDbContext.Documents.Skip(pageIndex * size).Take(size).ToListAsync();
                var total = await _appDbContext.Documents.CountAsync();

                return Ok(new
                {
                    documents = docs,
                    total = total
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("file/download/{fileName}")]
        public IActionResult GetFile([FromRoute] string fileName)
        {
            var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            return PhysicalFile(filePath, "application/octet-stream");
        }

        [HttpGet("file/{fileName}")]
        public IActionResult GetFile2([FromRoute] string fileName)
        {
            var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);

            if (filePath == null || !System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            string contentType = GetContentType(fileName);

            return PhysicalFile(filePath, contentType);
        }



        private async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new Exception("File empty!");
            }

            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            string extension = Path.GetExtension(file.FileName);
            string fileName = Guid.NewGuid().ToString() + extension;

            string fileFullPath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(fileFullPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
                fileStream.Close();
            }

            return fileName;
        }

        private static string GetContentType(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extension switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => MediaTypeNames.Image.Jpeg,
                ".pdf" => MediaTypeNames.Application.Pdf,
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => MediaTypeNames.Application.Octet,
            };
        }
    }
}
