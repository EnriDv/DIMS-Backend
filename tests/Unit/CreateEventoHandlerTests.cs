using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;
using DIMS_Backend.Features.Eventos.CrearEvento;

namespace DIMS_Backend.Tests.Unit;

public class CreateEventoHandlerTests
{
    [Fact]
    public async Task Handle_Creates_Evento_In_Database_When_Data_Is_Valid()
    {
        var options = new DbContextOptionsBuilder<UcbPortalContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_CreateEvento_Valid")
            .Options;

        using var context = new UcbPortalContext(options);
        var handler = new CreateEventoHandler(context);

        var command = new CreateEventoCommand(
            Titulo: "Nuevo Taller de IA",
            Descripcion: "Aprende sobre transformers",
            FechaEvento: DateTime.Now.AddDays(5),
            Lugar: "Aula Magna",
            Tipo: "workshop",
            CarreraId: 1,
            Capacidad: 50,
            ImagenUrl: "http://example.com/image.png",
            CreatedBy: Guid.NewGuid()
        );

        var result = await handler.Handle(command, default);

        Assert.True(result > 0);
        var evento = await context.Eventos.FindAsync(result);
        Assert.NotNull(evento);
        Assert.Equal("Nuevo Taller de IA", evento.Titulo);
        Assert.Equal("workshop", evento.Tipo);
    }

    [Fact]
    public async Task Handle_Throws_ArgumentException_When_Tipo_Is_Invalid()
    {
        var options = new DbContextOptionsBuilder<UcbPortalContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_CreateEvento_Invalid")
            .Options;

        using var context = new UcbPortalContext(options);
        var handler = new CreateEventoHandler(context);

        var command = new CreateEventoCommand(
            Titulo: "Nuevo Taller de IA",
            Descripcion: "Aprende sobre transformers",
            FechaEvento: DateTime.Now.AddDays(5),
            Lugar: "Aula Magna",
            Tipo: "invalid_type",
            CarreraId: 1,
            Capacidad: 50,
            ImagenUrl: "http://example.com/image.png",
            CreatedBy: Guid.NewGuid()
        );

        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, default));
    }
}
