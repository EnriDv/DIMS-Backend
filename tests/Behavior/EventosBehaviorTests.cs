using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using DIMS_Backend.Features.Eventos;
using DIMS_Backend.Features.Eventos.GetEventos;
using DIMS_Backend.Features.Eventos.CrearEvento;
using System.Collections.Generic;

namespace DIMS_Backend.Tests.Behavior;

public class EventosBehaviorTests : IClassFixture<BehaviorTestBase>
{
    private readonly HttpClient _client;
    private readonly BehaviorTestBase _factory;

    public EventosBehaviorTests(BehaviorTestBase factory)
    {
        _factory = factory;
        _client = factory.CreateTestClient();
    }

    [Fact]
    public async Task Event_Full_Behavior_Flow_Succeeds()
    {
        // 1. Generate Admin Token and Set Headers
        var adminId = Guid.NewGuid();
        var adminToken = _factory.GenerateTestToken(adminId, "admin@ucb.com", "admin");
        
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/Eventos");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var createCommand = new CreateEventoCommand(
            Titulo: "Conferencia IA Avanzada",
            Descripcion: "Charla de inteligencia artificial y agentes",
            FechaEvento: DateTime.Now.AddDays(10),
            Lugar: "Salón de Honor",
            Tipo: "conferencia",
            CarreraId: null,
            Capacidad: 100,
            ImagenUrl: "http://example.com/ia.png",
            CreatedBy: adminId
        );
        requestMessage.Content = JsonContent.Create(createCommand);

        // Send POST to create event
        var createResponse = await _client.SendAsync(requestMessage);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        var eventId = await createResponse.Content.ReadFromJsonAsync<int>();
        Assert.True(eventId > 0);

        // 2. Update Event
        var updateCommand = new UpdateEventoCommand
        {
            Id = eventId,
            Titulo = "Conferencia IA Avanzada Modificada",
            Descripcion = "Nueva descripción",
            FechaEvento = DateTime.Now.AddDays(11),
            Lugar = "Salón de Honor 2",
            Tipo = "conferencia",
            CarreraId = null,
            Capacidad = 150,
            ImagenUrl = "http://example.com/ia2.png",
            Publicado = true
        };

        var updateRequest = new HttpRequestMessage(HttpMethod.Put, $"api/Eventos/{eventId}");
        updateRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        updateRequest.Content = JsonContent.Create(updateCommand);

        var updateResponse = await _client.SendAsync(updateRequest);
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        // 3. Subscribe as Student
        var studentId = Guid.NewGuid();
        var studentToken = _factory.GenerateTestToken(studentId, "student@ucb.com", "estudiante");

        var subscribeRequest = new HttpRequestMessage(HttpMethod.Post, $"api/Eventos/{eventId}/suscribir");
        subscribeRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", studentToken);
        subscribeRequest.Content = new StringContent(string.Empty); // Empty body

        var subscribeResponse = await _client.SendAsync(subscribeRequest);
        Assert.Equal(HttpStatusCode.OK, subscribeResponse.StatusCode);

        // 4. Duplicate Subscription Check (Prevent double subscription)
        var duplicateSubscribeRequest = new HttpRequestMessage(HttpMethod.Post, $"api/Eventos/{eventId}/suscribir");
        duplicateSubscribeRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", studentToken);
        duplicateSubscribeRequest.Content = new StringContent(string.Empty);

        var duplicateResponse = await _client.SendAsync(duplicateSubscribeRequest);
        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);

        // 5. Get Subscribed Events List
        var listRequest = new HttpRequestMessage(HttpMethod.Get, "api/Eventos/suscritos");
        listRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", studentToken);

        var listResponse = await _client.SendAsync(listRequest);
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var subscribedEvents = await listResponse.Content.ReadFromJsonAsync<List<EventoListDto>>();
        Assert.NotNull(subscribedEvents);
        Assert.Single(subscribedEvents);
        Assert.Equal(eventId, subscribedEvents[0].Id);
        Assert.Equal("Conferencia IA Avanzada Modificada", subscribedEvents[0].Titulo);

        // 6. Delete (Hide) Event as Admin
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"api/Eventos/{eventId}");
        deleteRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var deleteResponse = await _client.SendAsync(deleteRequest);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }
}
