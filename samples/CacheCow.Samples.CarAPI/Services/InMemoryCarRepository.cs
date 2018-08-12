using System;
using System.Collections.Generic;
using System.Linq;
using CacheCow.Samples.CarAPI.Entities;
using CacheCow.Samples.CarAPI.Helpers;

namespace CacheCow.Samples.CarAPI.Services
{
    public class InMemoryCarRepository : ICarRepository
    {
        private readonly List<Car> _cars = new List<Car>
        {
            new Car
            {
                Id = 1,
                LastModified = DateTimeOffset.Now,
                NumberPlate = "XYZ-123",
                Year = 2017,
                Brand = "Jaguar",
                Owner = "John Doe",
                Color = "DarkGreen"
            }
        };

        public PagedList<Car> Get(RequestParameters requestParameters)
        {
            if (requestParameters.PageNumber < 1 || requestParameters.PageSize < 1)
            {
                return null;
            }

            var cnt = _cars.Count;
            var items = _cars
                    .Skip((requestParameters.PageNumber - 1) * requestParameters.PageSize)
                    .Take(requestParameters.PageSize);
            return new PagedList<Car>(items, cnt, requestParameters.PageNumber, requestParameters.PageSize);
        }

        public Car Get(int id)
        {
            return _cars.FirstOrDefault(c => c.Id == id);
        }

        public Car Add(Car newCar)
        {
            newCar.Id = _cars.Select(c => c.Id).Max() + 1;
            newCar.LastModified = DateTimeOffset.Now;
            _cars.Add(newCar);
            return newCar;
        }

        public bool Exists(int id)
        {
            return _cars.Any(c => c.Id == id);
        }

        public Car Update(Car updatedCar)
        {
            updatedCar.LastModified = DateTimeOffset.Now;
            var index = _cars
                .Select((c, i) => (c, i))
                .First(t => t.Item1.Id == updatedCar.Id)
                .Item2;
            _cars[index] = updatedCar;
            return updatedCar;
        }

        public void Delete(int id)
        {
            _cars.RemoveAll(c => c.Id == id);
        }
    }
}
