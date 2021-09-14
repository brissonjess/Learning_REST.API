using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Helpers
{
    public enum ResourceUriType
    {
        //enumeration allows us to determine whether we want to pass the previous or next page
        PreviousPage,
        NextPage
    }
}
