namespace DIMS_Backend.Features.Noticias;

using MediatR;

public class DeleteNoticiaCommand : IRequest<bool>
{
    public int Id { get; set; }
    public Guid RequestUserId { get; set; }
    public string RequestUserRole { get; set; } = string.Empty;

    public DeleteNoticiaCommand(int id, Guid userId, string role)
    {
        Id = id;
        RequestUserId = userId;
        RequestUserRole = role;
    }
}