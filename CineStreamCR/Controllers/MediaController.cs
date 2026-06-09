using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace CineStreamCR.Controllers;

[ApiController]
[Route("api/media")]
public sealed class MediaController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    private static readonly ConcurrentDictionary<string, string> ThumbnailCache = new(StringComparer.OrdinalIgnoreCase);

    [HttpGet("wiki-thumbnail")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> WikiThumbnail([FromQuery] string? title, [FromQuery] string? fallback, CancellationToken ct)
    {
        var isAllowedFallback = !string.IsNullOrWhiteSpace(fallback) &&
            (fallback.StartsWith("/images/people/", StringComparison.OrdinalIgnoreCase) ||
             fallback.StartsWith("/images/posters/", StringComparison.OrdinalIgnoreCase) ||
             fallback.StartsWith("/images/backdrops/", StringComparison.OrdinalIgnoreCase));
        var safeFallback = isAllowedFallback ? fallback! : "/images/people/person-fallback.jpg";

        if (string.IsNullOrWhiteSpace(title)) return Redirect(safeFallback);
        if (ThumbnailCache.TryGetValue(title, out var cached))
            return Redirect(string.IsNullOrWhiteSpace(cached) ? safeFallback : cached);

        try
        {
            var client = httpClientFactory.CreateClient("Wikipedia");
            using var response = await client.GetAsync(Uri.EscapeDataString(title), ct);
            if (!response.IsSuccessStatusCode)
            {
                ThumbnailCache.TryAdd(title, string.Empty);
                return Redirect(safeFallback);
            }

            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
            if (document.RootElement.TryGetProperty("thumbnail", out var thumbnail) &&
                thumbnail.TryGetProperty("source", out var sourceElement))
            {
                var source = sourceElement.GetString();
                if (Uri.TryCreate(source, UriKind.Absolute, out var imageUri) && imageUri.Scheme == Uri.UriSchemeHttps)
                {
                    ThumbnailCache.TryAdd(title, imageUri.ToString());
                    return Redirect(imageUri.ToString());
                }
            }
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            // El servicio externo superó el tiempo de espera; se usa la imagen local.
        }
        catch (HttpRequestException)
        {
            // Sin conexión a Internet o servicio no disponible; se usa la imagen local.
        }
        catch (JsonException)
        {
            // Respuesta inesperada; se usa la imagen local.
        }

        ThumbnailCache.TryAdd(title, string.Empty);
        return Redirect(safeFallback);
    }
}

