namespace ASC.Security.Cryptography;

[Singletone]
public class PasswordHasher
{
    public int Size { get; private set; }
    public int Iterations { get; private set; }
    public string Salt { get; private set; }

    public PasswordHasher(IConfiguration configuration, MachinePseudoKeys machinePseudoKeys)
    {
        if (!int.TryParse(configuration["core:password:size"], out var size))
        {
            size = 256;
        }

        Size = size;

        if (!int.TryParse(configuration["core.password.iterations"], out var iterations))
        {
            iterations = 100000;
        }

        Iterations = iterations;

        Salt = (configuration["core:password:salt"] ?? "").Trim();
        if (string.IsNullOrEmpty(Salt))
        {
            var salt = Hasher.Hash("{9450BEF7-7D9F-4E4F-A18A-971D8681722D}", HashAlg.SHA256);

            var PasswordHashSaltBytes = KeyDerivation.Pbkdf2(
                                               Encoding.UTF8.GetString(machinePseudoKeys.GetMachineConstant()),
                                               salt,
                                               KeyDerivationPrf.HMACSHA256,
                                               Iterations,
                                               Size / 8);
            Salt = BitConverter.ToString(PasswordHashSaltBytes).Replace("-", string.Empty).ToLower();
        }
    }

    public string GetClientPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            password = Guid.NewGuid().ToString();
        }

        var salt = new UTF8Encoding(false).GetBytes(Salt);

        var hashBytes = KeyDerivation.Pbkdf2(
                           password,
                           salt,
                           KeyDerivationPrf.HMACSHA256,
                           Iterations,
                           Size / 8);

        var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();

        return hash;
    }
}