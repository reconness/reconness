using NLog;
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
    /// This class implement <see cref="IAgentCategoryService"/>
    /// </summary>
    public class AgentCategoryService : Service<Category>, IService<Category>, IAgentCategoryService
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="IAgentCategoryService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public AgentCategoryService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        /// <inheritdoc/>
        public async Task<ICollection<Category>> GetCategoriesAsync(ICollection<Category> myCategories, List<string> newCategories, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var myCategoriesName = this.GetIntersectionCategoriesName(myCategories, newCategories);
            foreach (var newCategory in newCategories)
            {
                if (myCategoriesName.Contains(newCategory))
                {
                    continue;
                }

                // avoid add new duplicate categories
                myCategoriesName.Add(newCategory);

                var category = await this.GetNewOrExistCategory(newCategory, cancellationToken);
                myCategories.Add(category);
            }

            return myCategories;
        }

        /// <summary>
        /// Obtain the names of the categories that interset the old and the new categories
        /// </summary>
        /// <param name="myCategories">The list of my categories</param>
        /// <param name="newCategories">The list of string categories</param>
        /// <returns>The names of the categorias that interset the old and the new categories</returns>
        private List<string> GetIntersectionCategoriesName(ICollection<Category> myCategories, List<string> newCategories)
        {
            var myCategoriesName = myCategories.Select(c => c.Name).ToList();
            foreach (var myCategoryName in myCategoriesName)
            {
                if (!newCategories.Contains(myCategoryName))
                {
                    myCategories.Remove(myCategories.First(c => c.Name == myCategoryName));
                }
            }

            return myCategories.Select(c => c.Name).ToList();
        }

        /// <summary>
        /// Obtain a new category if does not exist on database or return a database category
        /// </summary>
        /// <param name="newCategory">The new categories assign to the agent</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns></returns>
        private async Task<Category> GetNewOrExistCategory(string newCategory, CancellationToken cancellationToken)
        {
            var category = await this.GetByCriteriaAsync(c => c.Name == newCategory, cancellationToken);
            if (category == null)
            {
                category = new Category
                {
                    Name = newCategory
                };

                category = await this.AddAsync(category, cancellationToken);
            }

            return category;
        }
    }
}
