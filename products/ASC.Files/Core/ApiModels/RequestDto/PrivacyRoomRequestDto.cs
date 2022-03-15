namespace ASC.Files.Core.ApiModels.RequestDto;

public class PrivacyRoomRequestDto
{
    public string PublicKey { get; set; }
    public string PrivateKeyEnc { get; set; }
    public bool Enable { get; set; }
    public bool Update { get; set; }
}
