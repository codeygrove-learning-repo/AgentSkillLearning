using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using GoogleCalendarMcpServer;
using ModelContextProtocol;

var builder = WebApplication.CreateBuilder(args);

// Configure logging to stderr
builder.Logging.AddConsole(consoleLogOptions =>
{
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Add MCP server with HTTP transport and tools from assembly
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

// Add HttpClient for API calls
builder.Services.AddHttpClient();

var app = builder.Build();

app.MapMcp();

await app.RunAsync();

/// <summary>
/// Google Calendar tools for querying calendar information
/// </summary>
[McpServerToolType]
public static class GoogleCalendarTools
{
    /// <summary>
    /// Gets calendar events for the next one month from the current date
    /// Returns calendar events as natural language string
    /// </summary>
    /// <param name="httpClient">HTTP client for making API calls</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Calendar events formatted as human readable string</returns>
    [McpServerTool(Name = "GetMyGoogleCalendarForNextOneMonth")]
    [Description("Gets calendar events for the next one month from the current date. Returns events in natural language format.")]
    public static async Task<string> GetMyGoogleCalendarForNextOneMonth(
        HttpClient httpClient,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Authentication placeholder - engineers need to implement OAuth 2.0 authentication
            // This should use Google Calendar API OAuth 2.0 flow to get access token
            var accessToken = await GetGoogleCalendarAccessToken();

            if (string.IsNullOrEmpty(accessToken))
            {
                return "Authentication not configured. Please set up Google Calendar API OAuth 2.0 credentials.";
            }

            // Calculate date range: today to one month from now
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddMonths(1);

            // Fetch calendar events from Google Calendar API
            var events = await GetCalendarEvents(httpClient, accessToken, startDate, endDate, cancellationToken);

            if (events == null || events.Count == 0)
            {
                return "No calendar events found for the next month.";
            }

            // Format events as natural language
            return FormatEventsAsNaturalLanguage(events);
        }
        catch (Exception ex)
        {
            return $"Error retrieving calendar events: {ex.Message}";
        }
    }

    /// <summary>
    /// Placeholder method for Google Calendar authentication
    /// Engineers should implement OAuth 2.0 authentication here
    /// </summary>
    /// <returns>Access token for Google Calendar API</returns>
    private static async Task<string> GetGoogleCalendarAccessToken()
    {
        // TODO: Implement OAuth 2.0 authentication flow
        // Steps:
        // 1. Set up Google Cloud Project and enable Google Calendar API
        // 2. Create OAuth 2.0 credentials (Client ID and Client Secret)
        // 3. Implement OAuth 2.0 authorization flow
        // 4. Get and refresh access tokens
        // 5. Return the access token

        // For now, return empty string to indicate authentication is not configured
        await Task.CompletedTask;
        return string.Empty;
    }

    /// <summary>
    /// Fetches calendar events from Google Calendar API
    /// </summary>
    /// <param name="httpClient">HTTP client for making API calls</param>
    /// <param name="accessToken">OAuth 2.0 access token</param>
    /// <param name="startDate">Start date for event range</param>
    /// <param name="endDate">End date for event range</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of calendar events</returns>
    private static async Task<List<CalendarEvent>?> GetCalendarEvents(
        HttpClient httpClient,
        string accessToken,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken)
    {
        try
        {
            // Google Calendar API endpoint for primary calendar
            // Using 'primary' as the calendar ID for the authenticated user's primary calendar
            var calendarId = "primary";
            var timeMin = Uri.EscapeDataString(startDate.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            var timeMax = Uri.EscapeDataString(endDate.ToString("yyyy-MM-ddTHH:mm:ssZ"));

            var url = $"https://www.googleapis.com/calendar/v3/calendars/{calendarId}/events?timeMin={timeMin}&timeMax={timeMax}&singleEvents=true&orderBy=startTime";

            // Add authorization header
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            var response = await httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var json = JsonDocument.Parse(content);

            var events = new List<CalendarEvent>();

            if (json.RootElement.TryGetProperty("items", out var items))
            {
                foreach (var item in items.EnumerateArray())
                {
                    var calendarEvent = new CalendarEvent();

                    if (item.TryGetProperty("summary", out var summary))
                    {
                        calendarEvent.Summary = summary.GetString();
                    }

                    if (item.TryGetProperty("location", out var location))
                    {
                        calendarEvent.Location = location.GetString();
                    }

                    if (item.TryGetProperty("description", out var description))
                    {
                        calendarEvent.Description = description.GetString();
                    }

                    // Parse start time
                    if (item.TryGetProperty("start", out var start))
                    {
                        if (start.TryGetProperty("dateTime", out var startDateTime))
                        {
                            if (DateTime.TryParse(startDateTime.GetString(), out var parsedStart))
                            {
                                calendarEvent.Start = parsedStart;
                            }
                        }
                        else if (start.TryGetProperty("date", out var startDate_))
                        {
                            if (DateTime.TryParse(startDate_.GetString(), out var parsedStart))
                            {
                                calendarEvent.Start = parsedStart;
                            }
                        }
                    }

                    // Parse end time
                    if (item.TryGetProperty("end", out var end))
                    {
                        if (end.TryGetProperty("dateTime", out var endDateTime))
                        {
                            if (DateTime.TryParse(endDateTime.GetString(), out var parsedEnd))
                            {
                                calendarEvent.End = parsedEnd;
                            }
                        }
                        else if (end.TryGetProperty("date", out var endDate_))
                        {
                            if (DateTime.TryParse(endDate_.GetString(), out var parsedEnd))
                            {
                                calendarEvent.End = parsedEnd;
                            }
                        }
                    }

                    events.Add(calendarEvent);
                }
            }

            return events;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Formats calendar events as natural language
    /// </summary>
    /// <param name="events">List of calendar events</param>
    /// <returns>Formatted string with calendar events</returns>
    private static string FormatEventsAsNaturalLanguage(List<CalendarEvent> events)
    {
        var result = new System.Text.StringBuilder();
        result.AppendLine("Your calendar for the next month:");
        result.AppendLine();

        foreach (var evt in events)
        {
            if (evt.Start.HasValue && evt.End.HasValue)
            {
                var date = evt.Start.Value.ToString("dd MMM yyyy");
                var startTime = evt.Start.Value.ToString("HH:mm");
                var endTime = evt.End.Value.ToString("HH:mm");

                result.Append($"{date} from {startTime} - {endTime}");

                if (!string.IsNullOrEmpty(evt.Summary))
                {
                    result.Append($" - {evt.Summary}");
                }

                if (!string.IsNullOrEmpty(evt.Location))
                {
                    result.Append($" ({evt.Location})");
                }

                result.AppendLine();
            }
        }

        return result.ToString();
    }
}
