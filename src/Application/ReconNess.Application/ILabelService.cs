using ReconNess.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Application;

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
    Task<ICollection<Label>> GetLabelsAsync(ICollection<Label> myLabels, List<string> newLabels, CancellationToken cancellationToken = default);
}
