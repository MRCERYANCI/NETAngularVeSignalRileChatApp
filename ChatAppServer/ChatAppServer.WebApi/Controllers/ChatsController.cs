using ChatAppServer.WebApi.Context;
using ChatAppServer.WebApi.Dtos;
using ChatAppServer.WebApi.Hubs;
using ChatAppServer.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatAppServer.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public sealed class ChatsController(ApplicationDbContext _applicationDbContext, IHubContext<ChatHub> _hubContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            List<User> users = await _applicationDbContext.Users.OrderByDescending(x=>x.Name).ToListAsync();
            return Ok(users);
        }

        [HttpGet]
        public async Task<IActionResult> GetChtas(Guid userId, Guid toUserId, CancellationToken cancellationToken)
        {
            List<Chat> chats = await _applicationDbContext.Chats.Where(x => x.UserId == userId && x.ToUserId == toUserId || x.ToUserId == userId && x.UserId == toUserId).OrderBy(x => x.Date).ToListAsync(cancellationToken);

            return Ok(chats);
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage(SendMessageDto sendMessageDto, CancellationToken cancellationToken)
        {
            Chat chat = new Chat()
            {
                UserId = sendMessageDto.userId,
                ToUserId = sendMessageDto.toUserId,
                Message = sendMessageDto.message,
                Date = DateTime.Now
            };

            await _applicationDbContext.AddAsync(chat,cancellationToken);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            string connectionId = ChatHub.Users.First(x => x.Value == chat.UserId).Key;
            await _hubContext.Clients.Client(connectionId).SendAsync("Messages", chat);

            return Ok(chat);
        }
    }
}
