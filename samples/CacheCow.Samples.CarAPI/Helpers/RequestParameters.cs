using Microsoft.AspNetCore.Mvc;

namespace CacheCow.Samples.CarAPI.Helpers
{
    public class RequestParameters
    {
        private readonly int _maxPageSize;
        private int _pageSize = 10;

        public RequestParameters() : this(20) { }

        public RequestParameters(int maxPageSize)
        {
            _maxPageSize = maxPageSize;
        }

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > _maxPageSize ? _maxPageSize : value;
        }

        public string CreateResourceUri(
          ResourceUriType type,
          string name,
          IUrlHelper urlHelper,
          object additionalValues = null)
        {
            dynamic values = new
            {
                pageNumber = PageNumber,
                pageSize = PageSize
            };

            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    values.pageNumber = values.pageNumber - 1;
                    return urlHelper.Link(
                        name,
                        values);
                case ResourceUriType.NextPage:
                    values.pageNumber = values.pageNumber + 1;
                    return urlHelper.Link(
                        name,
                        values);
                default:
                    return urlHelper.Link(
                        name,
                        values);
            }
        }

    }
}
