namespace ASC.Web.Studio.Utility
{
    public static class TransferResourceHelper
    {
        public static string GetRegionDescription(string region)
        {
            region = region.ToLower().Trim();

            return region switch
            {
                "eu" => Resource.EuServerRegion,
                "us" => Resource.UsServerRegion,
                "sg" => Resource.SgServerRegion,
                "org" => Resource.OrgServerRegion,

                _ => string.Empty,
            };
        }
    }
}
