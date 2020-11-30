using ReconNess.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface ICategoryService
    /// </summary>
    public interface IAgentCategoryService : IService<Category>
    {
        /// <summary>
        /// Obtain the list of categories from database, if does not exist create the category
        /// </summary>
        /// <param name="myCategories">The list of my categories</param>
        /// <param name="newCategories">The list of string categories</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of categories</returns>
        Task<ICollection<Category>> GetCategoriesAsync(ICollection<Category> myCategories, List<string> newCategories, CancellationToken cancellationToken = default);
    }
}
