using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace BTMBackend.Utilities
{
    public class PagedList<T> : List<T>
    {
        public Paganation Paganation { get; set; } = new Paganation();

        public PagedList(IEnumerable<T> items, HttpResponseHeaders responseHeaders)
        {
            try
            {
                responseHeaders.TryGetValues("X-Pagination", out var paganation);

                if (paganation != null && paganation.Any())
                {
                    var f = paganation.FirstOrDefault();
                    if (f != null)
                    {
                        var p = JsonConvert.DeserializeObject<Paganation>(f);
                        if (p != null)
                            Paganation = p;
                    }
                }

                Paganation ??= new Paganation();
            }
            catch
            { }
            AddRange(items);
        }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            Paganation.TotalCount = count;
            Paganation.PageSize = pageSize;
            Paganation.CurrentPage = pageNumber;
            Paganation.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }
        public static PagedList<T> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            if (pageNumber == 0)
            {
                pageNumber = 1;
            }

            var count = source.Count();
            var items = pageSize < 0 || pageNumber < 0 ? source.ToList() : source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
    public class Paganation
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

    }
}
