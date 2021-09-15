using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Services
{
    public class PropertyMapping<TSource, TDestination> : IPropertyMapping
    {
        public Dictionary<string, PropertyMappingValue> _mappingDictionary { get; private set; }
        /// <summary>
        /// Constructor to check for nulls in the mappingDictionary. Will throw ArgumentNullException if a null is found.
        /// </summary>
        /// <param name="mappingDictionary">Property Mapping Dictionary Object</param>
        public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            _mappingDictionary = mappingDictionary ?? throw new ArgumentNullException(nameof(mappingDictionary));
        }
    }
}
