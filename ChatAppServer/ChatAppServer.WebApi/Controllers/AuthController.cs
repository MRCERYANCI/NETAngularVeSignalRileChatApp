using ChatAppServer.WebApi.Context;
using ChatAppServer.WebApi.Dtos;
using ChatAppServer.WebApi.Models;
using GenericFileService.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatAppServer.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public sealed class AuthController(ApplicationDbContext _applicationDbContext) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto, CancellationToken cancellationToken)
        {
            bool isNameExists = await _applicationDbContext.Users.AnyAsync(x => x.Name == registerDto.Name, cancellationToken);
            if (isNameExists)
            {
                return BadRequest(new
                {
                    Message = "Bu kullanıcı zaten kullanılıyor, lütfen başka kullanıcı adı giriniz"
                });
            }

            string avatar = FileService.FileSaveToServer(registerDto.file, "wwwroot/avatar/");

            User user = new User()
            {
                Name = registerDto.Name,
                Avatar = avatar     
            };

            await _applicationDbContext.AddAsync(user, cancellationToken);
            await _applicationDbContext.SaveChangesAsync();

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> Login(string name, CancellationToken cancellationToken)
        {
            User? user = await _applicationDbContext.Users.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
            if(user is null)
            {
                return BadRequest(new
                {
                    Message = "Böyle kullanıcı bulunamadı"
                });
            }

            return Ok(user); 
        }
    }
}
