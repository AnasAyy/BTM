using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace BTMBackend.Utilities
{
    public class PagedList2<T>
    {
        public Pagination2 Pagination { get; set; } = new Pagination2();
        public List<T> Data { get; set; } = new List<T>();

        public PagedList2(IEnumerable<T> items, HttpResponseHeaders responseHeaders)
        {
            try
            {
                responseHeaders.TryGetValues("X-Pagination", out var pagination);

                if (pagination != null && pagination.Any())
                {
                    var json = pagination.FirstOrDefault();
                    if (json != null)
                    {
                        var p = JsonConvert.DeserializeObject<Pagination2>(json);
                        if (p != null)
                            Pagination = p;
                    }
                }

                Pagination ??= new Pagination2();
            }
            catch
            {
            }

            Data.AddRange(items);
        }

        public PagedList2(List<T> items, int count, int pageNumber, int pageSize)
        {
            Pagination.TotalCount = count;
            Pagination.PageSize = pageSize;
            Pagination.CurrentPage = pageNumber;
            Pagination.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Data.AddRange(items);
        }

        public static PagedList2<T> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            if (pageNumber == 0)
            {
                pageNumber = 1;
            }

            var count = source.Count();
            var items = pageSize < 0 || pageNumber < 0 ? source.ToList() : source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList2<T>(items, count, pageNumber, pageSize);
        }
    }

    public class Pagination2
    {
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }

        [JsonProperty("hasPrevious")]
        public bool HasPrevious => CurrentPage > 1;

        [JsonProperty("hasNext")]
        public bool HasNext => CurrentPage < TotalPages;
    }
}