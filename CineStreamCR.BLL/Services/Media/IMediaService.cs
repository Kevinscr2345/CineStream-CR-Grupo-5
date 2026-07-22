using System;
using System.Collections.Generic;
using System.Text;

namespace CineStreamCR.BLL.Services.Media
{
    public interface IMediaService
    {
        Task<string> GetWikipediaThumbnailAsync(string? title, string? fallback, CancellationToken cancellationToken);
    }
}
