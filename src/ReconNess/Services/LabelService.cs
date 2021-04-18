using NLog;
using ReconNess.Core;
using ReconNess.Core.Services;
using ReconNess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="ILabelService"/>
    /// </summary>
    public class LabelService : Service<Label>, IService<Label>, ILabelService
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="ILabelService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public LabelService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        /// <inheritdoc/>
        public async Task<ICollection<Label>> GetLabelsAsync(ICollection<Label> myLabels, List<string> newLabels, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var myLabelsName = this.GetIntersectionLabelsName(myLabels, newLabels);
            foreach (var newLabel in newLabels)
            {
                if (myLabelsName.Contains(newLabel))
                {
                    continue;
                }

                // avoid add new duplicate labels
                myLabelsName.Add(newLabel);

                var label = await GetNewOrExistLabel(newLabel, cancellationToken);
                myLabels.Add(label);
            }

            return myLabels;
        }

        /// <summary>
        /// Obtain the names of the labels that interset the old and the new Labels
        /// </summary>
        /// <param name="myLabels">The list of my Labels</param>
        /// <param name="newLabels">The list of string Labels</param>
        /// <returns>The names of the categorias that interset the old and the new Labels</returns>
        private List<string> GetIntersectionLabelsName(ICollection<Label> myLabels, List<string> newLabels)
        {
            var myLabelsName = myLabels.Select(c => c.Name).ToList();
            foreach (var myLabelName in myLabelsName)
            {
                if (!newLabels.Contains(myLabelName))
                {
                    myLabels.Remove(myLabels.First(c => c.Name == myLabelName));
                }
            }

            return myLabels.Select(c => c.Name).ToList();
        }

        /// <summary>
        /// Obtain a new label if does not exist on database or return a database label
        /// </summary>
        /// <param name="newLabel">The new labels assign to the subdomain</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A Label</returns>
        private async Task<Label> GetNewOrExistLabel(string newLabel, CancellationToken cancellationToken)
        {
            var label = await this.GetByCriteriaAsync(c => c.Name == newLabel, cancellationToken);
            if (label == null)
            {
                var random = new Random();

                label = new Label
                {
                    Name = newLabel,
                    Color = string.Format("#{0:X6}", random.Next(0x1000000))
                };

                label = await this.AddAsync(label, cancellationToken);
            }

            return label;
        }
    }
}
