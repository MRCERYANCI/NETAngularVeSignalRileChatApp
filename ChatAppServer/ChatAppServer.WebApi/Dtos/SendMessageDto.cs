namespace ChatAppServer.WebApi.Dtos;

public sealed record SendMessageDto(Guid userId, Guid toUserId,string message);




