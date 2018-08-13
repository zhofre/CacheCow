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
        Entities.Car Add(Entities.Car newCar);
        bool Exists(int id);
        Entities.Car Update(Entities.Car updatedCar);
        void Delete(int id);
        Entities.Car Last();
    }
}
