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
    /// <summary>
    /// This class implement <see cref="IReferenceService"/>
    /// </summary>
    public class ReferenceService : Service<Reference>, IReferenceService, IService<Reference>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IReferenceService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public ReferenceService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        /// <summary>
        /// <see cref="IReferenceService.GetAllCategoriesAsync(CancellationToken)"/>
        /// </summary>
        public async Task<List<string>> GetAllCategoriesAsync(CancellationToken cancellationToken)
        {
            var entities = await (this.GetAllQueryable(cancellationToken).Select(r => r.Categories))
                .ToListAsync();

            var categories = new List<string>();
            entities.ForEach(c => c.Split(',').ToList()
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
