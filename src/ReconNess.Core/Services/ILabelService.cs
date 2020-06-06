using ReconNess.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface ILabelService
    /// </summary>
    public interface ILabelService : IService<Label>
    {
        /// <summary>
        /// Obtain the list of labels from database, if does not exist create the label
        /// </summary>
        /// <param name="myLabels">The list of my labels</param>
        /// <param name="newLabels">The list of string new labels</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of labels</returns>
        Task<ICollection<SubdomainLabel>> GetLabelsAsync(ICollection<SubdomainLabel> myLabels, List<string> newLabels, CancellationToken cancellationToken = default);
    }
}
