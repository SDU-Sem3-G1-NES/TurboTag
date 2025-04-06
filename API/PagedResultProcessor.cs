using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

public class PagedResultProcessor : IDocumentProcessor
{
    public void Process(DocumentProcessorContext context)
    {
        foreach (var schema in context.Document.Components.Schemas.Values)
            if (schema.IsObject && schema.Properties.ContainsKey("items"))
            {
                var itemsSchema = schema.Properties["items"];

                if (itemsSchema.Item?.Reference?.Title != null)
                {
                    var itemTypeName = itemsSchema.Item.Reference.Title;
                    schema.Title = $"PagedResult<{itemTypeName}>";
                }
            }
    }
}