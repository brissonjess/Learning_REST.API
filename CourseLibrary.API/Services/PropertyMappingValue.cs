using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Services
{
    public class PropertyMappingValue
    {
        public IEnumerable<string> DestinationProperties { get; private set; }
        public bool Revert { get; private set; }

        /// <summary>
        /// Constructor making it easier to create a property mapping object
        /// </summary>
        /// <param name="destinationProperties">The destination properties (w/ type ienumerable of string) that one resource propertry will map to.</param>
        /// <param name="revert">Allows us to revert the sort order if needed.</param>
        public PropertyMappingValue(IEnumerable<string> destinationProperties, bool revert = false)
        {
            DestinationProperties = destinationProperties ?? throw new ArgumentException(nameof(destinationProperties));
            Revert = revert;
        }
    }
}
