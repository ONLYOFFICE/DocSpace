namespace ASC.VoipService;

public interface IVoipProvider
{
    IEnumerable<VoipPhone> GetExistingPhoneNumbers();

    IEnumerable<VoipPhone> GetAvailablePhoneNumbers(PhoneNumberType phoneNumberType, string isoCountryCode);

    VoipPhone BuyNumber(string phoneNumber);

    VoipPhone DeleteNumber(VoipPhone phone);

    VoipPhone GetPhone(VoipNumber r);

    VoipPhone GetPhone(string id);

    VoipCall GetCall(string callId);

    string GetToken(Agent agent, int seconds = 60 * 60 * 24);

    void UpdateSettings(VoipPhone phone);

    VoipRecord GetRecord(string callId, string recordId);

    void CreateQueue(VoipPhone newPhone);

    void DisablePhone(VoipPhone phone);
}
