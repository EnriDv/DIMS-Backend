using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;
using DIMS_Backend.Features.Eventos;

namespace DIMS_Backend.Tests.Unit;

public class UpdateEventoHandlerTests
{
    [Fact]
    public async Task Handle_Updates_Evento_In_Database_When_Data_Is_Valid()
    {
        var options = new DbContextOptionsBuilder<UcbPortalContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_UpdateEvento_Valid")
            .Options;

        using var context = new UcbPortalContext(options);
        var initialEvento = new Evento
        {
            Titulo = "Evento Original",
            Descripcion = "Descripción Original",
            FechaEvento = DateTime.Now.AddDays(2),
            Lugar = "Laboratorio",
            Tipo = "charla",
            Capacidad = 10,
            Publicado = true
        };
        context.Eventos.Add(initialEvento);
        await context.SaveChangesAsync();

        var handler = new UpdateEventoHandler(context);

        var command = new UpdateEventoCommand
        {
            Id = initialEvento.Id,
            Titulo = "Evento Modificado",
            Descripcion = "Descripción Modificada",
            FechaEvento = DateTime.Now.AddDays(3),
            Lugar = "Laboratorio 2",
            Tipo = "conferencia",
            CarreraId = null,
            Capacidad = 20,
            ImagenUrl = null,
            Publicado = false
        };

        var result = await handler.Handle(command, default);

        Assert.True(result);
        var updated = await context.Eventos.FindAsync(initialEvento.Id);
        Assert.NotNull(updated);
        Assert.Equal("Evento Modificado", updated.Titulo);
        Assert.Equal("conferencia", updated.Tipo);
        Assert.False(updated.Publicado);
    }

    [Fact]
    public async Task Handle_Returns_False_When_Evento_Does_Not_Exist()
    {
        var options = new DbContextOptionsBuilder<UcbPortalContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_UpdateEvento_NotFound")
            .Options;

        using var context = new UcbPortalContext(options);
        var handler = new UpdateEventoHandler(context);

        var command = new UpdateEventoCommand
        {
            Id = 999, // Inexistente
            Titulo = "Evento Modificado",
            Descripcion = "Descripción Modificada",
            FechaEvento = DateTime.Now.AddDays(3),
            Lugar = "Laboratorio 2",
            Tipo = "conferencia",
            CarreraId = null,
            Capacidad = 20,
            ImagenUrl = null,
            Publicado = false
        };

        var result = await handler.Handle(command, default);

        Assert.False(result);
    }
}
