namespace ASC.CRM.Core.EF
{
    public interface IDbCrm
    {
        public int TenantId { get; set; }
    }

    public interface IDbSearch
    {
        public string Title { get; set; }
    }
}
