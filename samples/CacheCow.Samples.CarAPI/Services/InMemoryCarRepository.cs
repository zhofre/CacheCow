using System.Collections.Generic;
using System.Linq;
using CacheCow.Samples.CarAPI.Entities;
using CacheCow.Samples.CarAPI.Helpers;

namespace CacheCow.Samples.CarAPI.Services
{
    public class InMemoryCarRepository : ICarRepository
    {
        private readonly List<Entities.Car> _cars = new List<Entities.Car>();

        public PagedList<Car> Get(RequestParameters requestParameters)
        {
            if (requestParameters.PageNumber < 1 || requestParameters.PageSize < 1)
                return null;

            var cnt = _cars.Count;
            var items = _cars
                    .Skip((requestParameters.PageNumber - 1) * requestParameters.PageSize)
                    .Take(requestParameters.PageSize);
            return new PagedList<Car>(items, cnt, requestParameters.PageNumber, requestParameters.PageSize);
        }
    }
}
