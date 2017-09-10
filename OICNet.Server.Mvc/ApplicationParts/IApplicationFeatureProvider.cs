using System.Collections.Generic;

namespace OICNet.Server.Mvc.ApplicationParts
{
    public interface IApplicationFeatureProvider
    {
        
    }

    public interface IApplicationFeatureProvider<TFeature> : IApplicationFeatureProvider
    {
        /// <summary>
        /// Updates the <paramref name="feature"/> instance.
        /// </summary>
        /// <param name="parts">The list of <see cref="ApplicationPart"/>s of the
        /// application.
        /// </param>
        /// <param name="feature">The feature instance to populate.</param>
        void PopulateFeature(IEnumerable<ApplicationPart> parts, TFeature feature);
    }
}