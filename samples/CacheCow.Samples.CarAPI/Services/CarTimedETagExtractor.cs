using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CacheCow.Samples.CarAPI.Helpers;
using CacheCow.Server;

namespace CacheCow.Samples.CarAPI.Services
{
    public class CarTimedETagExtractor : ITimedETagExtractor<Dto.Car>
    {
        /// <inheritdoc />
        public TimedEntityTagHeaderValue Extract(Dto.Car viewModel)
        {
            if (viewModel == null)
            {
                return null;
            }

            return ExtractImpl(viewModel.LastModified, null);
        }

        /// <inheritdoc />
        public TimedEntityTagHeaderValue Extract(object viewModel)
        {
            if (!(viewModel is ExpandoObject))
            {
                return Extract(viewModel as Dto.Car);
            }

            var carExpando = (IDictionary<string, object>) viewModel;
            var lastModified = (DateTimeOffset) carExpando["LastModified"];

            var carProps = typeof(Dto.Car).GetProperties()
                .Select(p => p.Name)
                .ToList();
            var unmappedProps = carProps.Except(carExpando.Keys);
            var fields = unmappedProps.Any()
                ? carExpando.Keys.Aggregate((tot, nxt) => tot + "," + nxt)
                : null;
            return ExtractImpl(lastModified, fields);
        }

        private TimedEntityTagHeaderValue ExtractImpl(
            DateTimeOffset lastModified,
            string fields)
        {
            var dateTag = lastModified.ToETagString();
            if (string.IsNullOrEmpty(fields))
            {
                return new TimedEntityTagHeaderValue(dateTag);
            }

            var encodedFields = Convert.ToBase64String(GetHash(fields));
            return new TimedEntityTagHeaderValue(dateTag + encodedFields);
        }

        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create();  //or use SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

    }
}