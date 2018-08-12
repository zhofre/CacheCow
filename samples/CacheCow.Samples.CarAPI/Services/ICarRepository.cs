using CacheCow.Samples.CarAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CacheCow.Samples.CarAPI.Services
{
    public interface ICarRepository
    {
        PagedList<Entities.Car> Get(RequestParameters requestParameters);
        Entities.Car Get(int id);
    }
}
