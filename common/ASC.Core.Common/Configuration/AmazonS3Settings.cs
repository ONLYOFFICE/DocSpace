namespace ASC.Core.Common.Configuration;

public class AmazonS3Settings
{
    public string AccessKeyId { get; set; }
    public string SecretAccessKey { get; set; }
    public string Bucket { get; set; }
    public string Region { get; set; }
}
