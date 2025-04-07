using System.Collections;
using Newtonsoft.Json;
using NJsonSchema.Annotations;

namespace API.DataAccess;

public abstract class PaginationFilter
{
    protected PaginationFilter(int? pageNumber, int? pageSize)
    {
    }

    protected PaginationFilter()
    {
    }
}

[JsonObject]
[JsonSchemaType(typeof(List<object>))]
public class PagedResult<T> : IEnumerable<T>
{
    private long _totalCount;

    [JsonProperty("items")] public List<T> Items { get; set; } = [];

    [JsonProperty("totalCount")]
    public int TotalCount
    {
        get => (int)_totalCount;
        set => _totalCount = value;
    }

    [JsonProperty("pageSize")] public int PageSize { get; set; }

    [JsonProperty("currentPage")] public int CurrentPage { get; set; }

    [JsonProperty("totalPages")] public int TotalPages { get; set; }

    public T this[int index]
    {
        get => Items[index];
        set => Items[index] = value;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(T item)
    {
        Items.Add(item);
        ++_totalCount;
    }
}