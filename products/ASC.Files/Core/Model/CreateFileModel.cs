namespace ASC.Files.Core.Model
{
    public class CreateFileModel<T>
    {
        public string Title { get; set; }

        public T TemplateId { get; set; }
    }
}
