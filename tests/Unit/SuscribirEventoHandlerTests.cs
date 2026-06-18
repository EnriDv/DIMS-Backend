using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;
using DIMS_Backend.Features.Eventos.SuscribirEvento;
using DIMS_Backend.Infrastructure.BackgroundServices;

namespace DIMS_Backend.Tests.Unit;

public class SuscribirEventoHandlerTests
{
    [Fact]
    public async Task Handle_Subscribes_Successfully_When_Spots_Available()
    {
        var options = new DbContextOptionsBuilder<UcbPortalContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Suscribir_Valid")
            .Options;

        using var context = new UcbPortalContext(options);
        var evento = new Evento
        {
            Titulo = "Evento Con Cupo",
            Tipo = "charla",
            Lugar = "Test Lugar",
            Capacidad = 10,
            Inscritos = 2,
            Publicado = true,
            Lugar = "Aula Test"
        };
        context.Eventos.Add(evento);
        await context.SaveChangesAsync();

        var handler = new SuscribirEventoHandler(context, new S3BackgroundQueue());
        var userId = Guid.NewGuid();
        var command = new SuscribirEventoCommand(evento.Id, userId);

        var result = await handler.Handle(command, default);

        Assert.True(result);
        var updated = await context.Eventos.FindAsync(evento.Id);
        Assert.NotNull(updated);
        Assert.Equal(3, updated.Inscritos);

        var suscripcion = await context.EventoSuscripciones
            .AnyAsync(s => s.EventoId == evento.Id && s.UserId == userId);
        Assert.True(suscripcion);
    }

    [Fact]
    public async Task Handle_Fails_When_User_Is_Already_Subscribed()
    {
        var options = new DbContextOptionsBuilder<UcbPortalContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Suscribir_Duplicate")
            .Options;

        using var context = new UcbPortalContext(options);
        var evento = new Evento
        {
            Titulo = "Evento Con Cupo 2",
            Tipo = "charla",
            Lugar = "Test Lugar",
            Capacidad = 10,
            Inscritos = 2,
            Publicado = true,
            Lugar = "Aula Test"
        };
        context.Eventos.Add(evento);

        var userId = Guid.NewGuid();
        context.EventoSuscripciones.Add(new EventoSuscripcione
        {
            EventoId = evento.Id,
            UserId = userId
        });
        await context.SaveChangesAsync();

        var handler = new SuscribirEventoHandler(context, new S3BackgroundQueue());
        var command = new SuscribirEventoCommand(evento.Id, userId);

        var result = await handler.Handle(command, default);

        Assert.False(result); // Prevent double subscription!
    }

    [Fact]
    public async Task Handle_Fails_When_No_Spots_Left()
    {
        var options = new DbContextOptionsBuilder<UcbPortalContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Suscribir_Full")
            .Options;

        using var context = new UcbPortalContext(options);
        var evento = new Evento
        {
            Titulo = "Evento Lleno",
            Tipo = "charla",
            Lugar = "Test Lugar",
            Capacidad = 5,
            Inscritos = 5,
            Publicado = true,
            Lugar = "Aula Test"
        };
        context.Eventos.Add(evento);
        await context.SaveChangesAsync();

        var handler = new SuscribirEventoHandler(context, new S3BackgroundQueue());
        var userId = Guid.NewGuid();
        var command = new SuscribirEventoCommand(evento.Id, userId);

        var result = await handler.Handle(command, default);

        Assert.False(result); // Failure because capacity is full
    }
}
