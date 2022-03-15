namespace ASC.Security.Cryptography;

[Singletone]
public class MachinePseudoKeys
{
    private readonly byte[] _confKey = null;

    public MachinePseudoKeys(IConfiguration configuration)
    {
        var key = configuration["core:machinekey"];

        if (string.IsNullOrEmpty(key))
        {
            key = configuration["asc:common.machinekey"];
        }

        if (!string.IsNullOrEmpty(key))
        {
            _confKey = Encoding.UTF8.GetBytes(key);
        }
    }


    public byte[] GetMachineConstant()
    {
        if (_confKey != null)
        {
            return _confKey;
        }

        var path = typeof(MachinePseudoKeys).Assembly.Location;
        var fi = new FileInfo(path);

        return BitConverter.GetBytes(fi.CreationTime.ToOADate());
    }

    public byte[] GetMachineConstant(int bytesCount)
    {
        var cnst = Enumerable.Repeat<byte>(0, sizeof(int)).Concat(GetMachineConstant()).ToArray();
        var icnst = BitConverter.ToInt32(cnst, cnst.Length - sizeof(int));
        var rnd = new AscRandom(icnst);
        var buff = new byte[bytesCount];
        rnd.NextBytes(buff);

        return buff;
    }
}
