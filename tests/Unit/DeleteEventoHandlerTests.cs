using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;
using DIMS_Backend.Features.Eventos;

namespace DIMS_Backend.Tests.Unit;

public class DeleteEventoHandlerTests
{
    [Fact]
    public async Task Handle_Deletes_Evento_Logically_In_Database()
    {
        var options = new DbContextOptionsBuilder<UcbPortalContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_DeleteEvento_Valid")
            .Options;

        using var context = new UcbPortalContext(options);
        var initialEvento = new Evento
        {
            Titulo = "Evento a Borrar",
            Descripcion = "Borrado lógico",
            Tipo = "charla",
            Publicado = true
        };
        context.Eventos.Add(initialEvento);
        await context.SaveChangesAsync();

        var handler = new DeleteEventoHandler(context);
        var command = new DeleteEventoCommand(initialEvento.Id);

        var result = await handler.Handle(command, default);

        Assert.True(result);
        var updated = await context.Eventos.FindAsync(initialEvento.Id);
        Assert.NotNull(updated);
        Assert.False(updated.Publicado);
    }

    [Fact]
    public async Task Handle_Returns_False_When_Evento_Does_Not_Exist()
    {
        var options = new DbContextOptionsBuilder<UcbPortalContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_DeleteEvento_NotFound")
            .Options;

        using var context = new UcbPortalContext(options);
        var handler = new DeleteEventoHandler(context);
        var command = new DeleteEventoCommand(999);

        var result = await handler.Handle(command, default);

        Assert.False(result);
    }
}
