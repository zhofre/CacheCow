using System;

namespace CacheCow.Samples.CarAPI.Dto
{
    public class Car : LinkedResource
    {
        public int Year { get; set; }

        public string NumberPlate { get; set; }

        public int Id { get; set; }

        public DateTimeOffset LastModified { get; set; }
    }
}
