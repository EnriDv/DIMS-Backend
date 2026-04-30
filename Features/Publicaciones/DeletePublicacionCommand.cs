namespace DIMS_Backend.Features.Publicaciones;

using MediatR;

public class DeletePublicacionCommand : IRequest<bool>
{
    public int Id { get; set; }
    public Guid RequestUserId { get; set; }
    public string RequestUserRole { get; set; } = string.Empty;

    public DeletePublicacionCommand(int id, Guid userId, string role)
    {
        Id = id;
        RequestUserId = userId;
        RequestUserRole = role;
    }
}