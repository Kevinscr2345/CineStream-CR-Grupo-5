using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Net.Http;

namespace CineStreamCR.BLL.Services.Media;

public sealed class WikipediaMediaService(IHttpClientFactory httpClientFactory) : IMediaService
{
    private static readonly ConcurrentDictionary<string, string> ThumbnailCache =
        new(StringComparer.OrdinalIgnoreCase);

    public async Task<string> GetWikipediaThumbnailAsync(
        string? title,
        string? fallback,
        CancellationToken ct = default)
    {
        var isAllowedFallback = !string.IsNullOrWhiteSpace(fallback) &&
            (fallback.StartsWith("/images/people/", StringComparison.OrdinalIgnoreCase) ||
             fallback.StartsWith("/images/posters/", StringComparison.OrdinalIgnoreCase) ||
             fallback.StartsWith("/images/backdrops/", StringComparison.OrdinalIgnoreCase));

        var safeFallback = isAllowedFallback
            ? fallback!
            : "/images/people/person-fallback.jpg";

        if (string.IsNullOrWhiteSpace(title))
            return safeFallback;

        if (ThumbnailCache.TryGetValue(title, out var cached))
            return string.IsNullOrWhiteSpace(cached)
                ? safeFallback
                : cached;

        try
        {
            var client = httpClientFactory.CreateClient("Wikipedia");

            using var response = await client.GetAsync(Uri.EscapeDataString(title), ct);

            if (!response.IsSuccessStatusCode)
            {
                ThumbnailCache.TryAdd(title, string.Empty);
                return safeFallback;
            }

            await using var stream = await response.Content.ReadAsStreamAsync(ct);

            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

            if (document.RootElement.TryGetProperty("thumbnail", out var thumbnail) &&
                thumbnail.TryGetProperty("source", out var sourceElement))
            {
                var source = sourceElement.GetString();

                if (Uri.TryCreate(source, UriKind.Absolute, out var imageUri) &&
                    imageUri.Scheme == Uri.UriSchemeHttps)
                {
                    ThumbnailCache.TryAdd(title, imageUri.ToString());
                    return imageUri.AbsoluteUri; //cambio de imageUri.ToString(); a AbsoluteUri arregla el problema de caracteres especiales y devuelve formato valido para HTTP
                    
                }
            }
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
        }
        catch (HttpRequestException)
        {
        }
        catch (JsonException)
        {
        }

        ThumbnailCache.TryAdd(title, string.Empty);

        return safeFallback;
    }
}