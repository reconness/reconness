using Microsoft.EntityFrameworkCore;
using ReconNess.Application.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Infrastructure.DataAccess.Helpers;

public static class QueryableExtension
{
    public static async Task<PagedResult<T>> GetPageAsync<T>(this IQueryable<T> query,
                                     int page, int pageSize, CancellationToken cancellationToken = default) where T : class
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = new PagedResult<T>();
        result.CurrentPage = page;
        result.PageSize = pageSize;
        result.RowCount = await query.CountAsync(cancellationToken);


        var pageCount = (double)result.RowCount / pageSize;
        result.PageCount = (int)Math.Ceiling(pageCount);

        var skip = (page - 1) * pageSize;
        result.Results = await query.Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return result;
    }
}
