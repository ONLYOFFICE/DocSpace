namespace ASC.Api.Settings.Smtp;

public class SmtpOperationStatusRequestsDto
{
    public bool Completed { get; set; }
    public string Id { get; set; }
    public string Status { get; set; }
    public string Error { get; set; }
    public int Percents { get; set; }
    public string Source { get; set; }

    public static SmtpOperationStatusRequestsDto GetSample()
    {
        return new SmtpOperationStatusRequestsDto
        {
            Id = "{some-random-guid}",
            Error = "",
            Percents = 0,
            Completed = true,
            Status = "",
            Source = ""
        };
    }
}