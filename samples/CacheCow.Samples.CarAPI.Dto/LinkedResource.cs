using System;
using System.Collections.Generic;
using System.Text;

namespace CacheCow.Samples.CarAPI.Dto
{
    public abstract class LinkedResource
    {
        public List<Link> Links { get; }
            = new List<Link>();
    }
}
