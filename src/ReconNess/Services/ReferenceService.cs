using Microsoft.EntityFrameworkCore;
using ReconNess.Core;
using ReconNess.Core.Services;
using ReconNess.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Services
{
    public class ReferenceService : Service<Reference>, IReferenceService, IService<Reference>
    {
        public ReferenceService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public async Task<List<string>> GetAllCategoriesAsync(CancellationToken cancellationToken)
        {
            var categoriesDb = await (this.GetAllQueryable(cancellationToken)
                .Select(r => r.Categories))
                .ToListAsync();

            var categories = new List<string>();

            categoriesDb
                .ForEach(categoriesWithComma => categoriesWithComma.Split(',')
                .ToList()
                .ForEach(category =>
                {
                    if (!categories.Contains(category))
                    {
                        categories.Add(category);
                    }
                }
            ));

            return categories;
        }
    }
}
