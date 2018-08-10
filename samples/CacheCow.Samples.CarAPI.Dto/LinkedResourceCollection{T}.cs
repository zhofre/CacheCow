using System.Collections.Generic;

namespace CacheCow.Samples.CarAPI.Dto
{
    public class LinkedResourceCollection<T> : LinkedResource
        where T : LinkedResource
    {
        public IEnumerable<T> Content { get; set; }
    }
}
