using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TWebApp.Data;

namespace TWebApp.Controllers;

[Route("api/images")]
[ApiController]
public class ImageController : ControllerBase
{
    private readonly ImageDbContext _context;

    public ImageController(ImageDbContext context) => _context = context;

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Image>>> GetImages() =>
        await _context.Images.ToListAsync();

    [HttpPost("add")]
    public async Task<IActionResult> AddImage([FromBody] ImageDto dto)
    {
        var image = new Image
        {
            Name = dto.Name,
            Data = dto.Data
        };

        _context.Images.Add(image);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetImages), new { id = image.Id }, image);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateImage(int id, [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Файл не загружен.");

        var image = await _context.Images.FindAsync(id);
        if (image == null)
            return NotFound("Изображение не найдено.");

        await using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        image.Data = ms.ToArray();
        image.Name = file.FileName;

        await _context.SaveChangesAsync();
        return Ok("Изображение обновлено.");
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteImage(int id)
    {
        var image = await _context.Images.FindAsync(id);
        if (image == null)
            return NotFound("Изображение не найдено.");

        _context.Images.Remove(image);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
