using CryptoBot.Data;
using CryptoBot.Models;
using CryptoBot.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CryptoBot.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController(
    AppDbContext db,
    IAnthropicService anthropic,
    IRagService rag) : ControllerBase
{
    // POST /api/chat/session  — create a new session
    [HttpPost("session")]
    public async Task<ActionResult<NewSessionResponse>> CreateSession()
    {
        var session = new ChatSession
        {
            Id = Guid.NewGuid().ToString(),
            Title = "New Chat",
            CreatedAt = DateTime.UtcNow
        };
        db.ChatSessions.Add(session);
        await db.SaveChangesAsync();
        return Ok(new NewSessionResponse(session.Id, session.Title));
    }

    // GET /api/chat/sessions  — list recent sessions
    [HttpGet("sessions")]
    public async Task<ActionResult<List<NewSessionResponse>>> GetSessions()
    {
        var sessions = await db.ChatSessions
            .OrderByDescending(s => s.CreatedAt)
            .Take(20)
            .Select(s => new NewSessionResponse(s.Id, s.Title))
            .ToListAsync();
        return Ok(sessions);
    }

    // GET /api/chat/history/{sessionId}  — get messages for a session
    [HttpGet("history/{sessionId}")]
    public async Task<ActionResult<List<object>>> GetHistory(string sessionId)
    {
        var messages = await db.ChatMessages
            .Where(m => m.SessionId == sessionId)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new { m.Role, m.Content, m.CreatedAt })
            .ToListAsync();
        return Ok(messages);
    }

    // POST /api/chat  — send a message
    [HttpPost]
    public async Task<ActionResult<ChatResponse>> SendMessage([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest("Message cannot be empty.");

        // Ensure session exists
        var session = await db.ChatSessions.FindAsync(request.SessionId);
        if (session == null)
        {
            session = new ChatSession
            {
                Id = request.SessionId,
                Title = TruncateTitle(request.Message),
                CreatedAt = DateTime.UtcNow
            };
            db.ChatSessions.Add(session);
        }
        else if (session.Title == "New Chat")
        {
            session.Title = TruncateTitle(request.Message);
        }

        // Save user message
        var userMessage = new ChatMessage
        {
            SessionId = session.Id,
            Role = "user",
            Content = request.Message,
            CreatedAt = DateTime.UtcNow
        };
        db.ChatMessages.Add(userMessage);
        await db.SaveChangesAsync();

        // Load conversation history (last 10 messages)
        var history = await db.ChatMessages
            .Where(m => m.SessionId == session.Id && m.Id != userMessage.Id)
            .OrderBy(m => m.CreatedAt)
            .TakeLast(10)
            .ToListAsync();

        // RAG: retrieve relevant context
        var ragContext = await rag.GetRelevantContextAsync(request.Message);

        // Call Claude
        var (assistantText, toolResults) = await anthropic.ChatAsync(request.Message, history, ragContext);

        // Save assistant message
        var assistantMessage = new ChatMessage
        {
            SessionId = session.Id,
            Role = "assistant",
            Content = assistantText,
            CreatedAt = DateTime.UtcNow
        };
        db.ChatMessages.Add(assistantMessage);
        await db.SaveChangesAsync();

        return Ok(new ChatResponse(session.Id, assistantText, "assistant", toolResults));
    }

    // DELETE /api/chat/session/{sessionId}
    [HttpDelete("session/{sessionId}")]
    public async Task<IActionResult> DeleteSession(string sessionId)
    {
        var messages = db.ChatMessages.Where(m => m.SessionId == sessionId);
        db.ChatMessages.RemoveRange(messages);
        var session = await db.ChatSessions.FindAsync(sessionId);
        if (session != null) db.ChatSessions.Remove(session);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static string TruncateTitle(string message) =>
        message.Length > 50 ? message[..47] + "..." : message;
}
