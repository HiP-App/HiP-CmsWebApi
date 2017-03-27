namespace Api.Models.AnnotationTag
{
    public class TagResult
    {
        public TagResult(Entity.Annotation.AnnotationTag tag)
        {
            TagId = tag.Id;
            Name = tag.Name;
            ShortName = tag.ShortName;
            Layer = tag.Layer;
            Style = tag.Style;
            Description = tag.Description;
            IsDeleted = tag.IsDeleted;
            UsageCount = tag.UsageCounter();

        }

        public int TagId { get; private set; }
        public string Name { get; private set; }
        public string ShortName { get; private set; }
        public string Layer { get; private set; }
        public string Style { get; private set; }
        public string Description { get; private set; }
        public bool IsDeleted { get; private set; }
        public int UsageCount { get; private set; }
    }
}
