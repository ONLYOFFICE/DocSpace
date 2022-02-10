namespace ASC.Common.Web
{
    [Serializable]
    public class ItemNotFoundException : HttpException
    {

        public ItemNotFoundException() : base(404, "Not found")
        {
        }

        public ItemNotFoundException(string message) : base(404, message)
        {
        }

        public ItemNotFoundException(string message, Exception inner) : base(404, message, inner)
        {
        }
    }
}
