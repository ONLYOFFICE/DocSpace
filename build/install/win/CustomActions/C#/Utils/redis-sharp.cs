//
// redis-sharp.cs: ECMA CLI Binding to the Redis key-value storage system
//
// Authors:
//   Miguel de Icaza (miguel@gnome.org)
//
// Copyright 2010 Novell, Inc.
//
// Licensed under the same terms of reddis: new BSD license.
//
#define DEBUG

using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.Linq;

public class Redis : IDisposable {
	Socket socket;
	BufferedStream bstream;

	public enum KeyType {
		None, String, List, Set
	}
	
	public class ResponseException : Exception {
		public ResponseException (string code) : base ("Response error")
		{
			Code = code;
		}

		public string Code { get; private set; }
	}
	
	public Redis (string host, int port)
	{
		if (host == null)
			throw new ArgumentNullException ("host");
		
		Host = host;
		Port = port;
		SendTimeout = -1;
	}
	
	public Redis (string host) : this (host, 6379)
	{
	}
	
	public Redis () : this ("localhost", 6379)
	{
	}

	public string Host { get; private set; }
	public int Port { get; private set; }
	public int RetryTimeout { get; set; }
	public int RetryCount { get; set; }
	public int SendTimeout { get; set; }
	public string Password { get; set; }
	
	int db;
	public int Db {
		get {
			return db;
		}

		set {
			db = value;
			SendExpectSuccess ("SELECT", db);
		}
	}

	public string this [string key] {
		get { return GetString (key); }
		set { Set (key, value); }
	}

	public void Set (string key, string value)
	{
		if (key == null)
			throw new ArgumentNullException ("key");
		if (value == null)
			throw new ArgumentNullException ("value");
		
		Set (key, Encoding.UTF8.GetBytes (value));
	}
	

	public void Set (string key, byte [] value)
	{
		if (key == null)
			throw new ArgumentNullException ("key");
		if (value == null)
			throw new ArgumentNullException ("value");

		if (value.Length > 1073741824)
			throw new ArgumentException ("value exceeds 1G", "value");

		if (!SendDataCommand (value, "SET", key))
			throw new Exception ("Unable to connect");
		ExpectSuccess ();
	}

	public bool SetNX (string key, string value)
	{
		if (key == null)
			throw new ArgumentNullException ("key");
		if (value == null)
			throw new ArgumentNullException ("value");
		
		return SetNX (key, Encoding.UTF8.GetBytes (value));
	}
	
	public bool SetNX (string key, byte [] value)
	{
		if (key == null)
			throw new ArgumentNullException ("key");
		if (value == null)
			throw new ArgumentNullException ("value");

		if (value.Length > 1073741824)
			throw new ArgumentException ("value exceeds 1G", "value");

		return SendDataExpectInt (value, "SETNX", key) > 0 ? true : false;
	}

	public void Set (IDictionary<string,string> dict)
	{
		if (dict == null)
			throw new ArgumentNullException ("dict");

		Set (dict.ToDictionary(k => k.Key, v => Encoding.UTF8.GetBytes(v.Value)));
	}

	public void Set (IDictionary<string,byte []> dict)
	{
		if (dict == null)
			throw new ArgumentNullException ("dict");

		MSet (dict.Keys.ToArray (), dict.Values.ToArray ());
	}

	public void MSet (string [] keys, byte [][] values)
	{
		if (keys.Length != values.Length)
			throw new ArgumentException ("keys and values must have the same size");

		byte [] nl = Encoding.UTF8.GetBytes ("\r\n");
		MemoryStream ms = new MemoryStream ();

		for (int i = 0; i < keys.Length; i++) {
			byte [] key = Encoding.UTF8.GetBytes(keys[i]);
			byte [] val = values[i];
			byte [] kLength = Encoding.UTF8.GetBytes ("$" + key.Length + "\r\n");
			byte [] k = Encoding.UTF8.GetBytes (keys[i] + "\r\n");
			byte [] vLength = Encoding.UTF8.GetBytes ("$" + val.Length + "\r\n");
			ms.Write (kLength, 0, kLength.Length);
			ms.Write (k, 0, k.Length);
			ms.Write (vLength, 0, vLength.Length);
			ms.Write (val, 0, val.Length);
			ms.Write (nl, 0, nl.Length);
		}
		
		SendDataRESP (ms.ToArray (), "*" + (keys.Length * 2 + 1) + "\r\n$4\r\nMSET\r\n");
		ExpectSuccess ();
	}

	public byte [] Get (string key)
	{
		if (key == null)
			throw new ArgumentNullException ("key");
		return SendExpectData ("GET", key);
	}

    public string Ping(String key)
    {
        return Encoding.UTF8.GetString (SendExpectData("PING", key));
    }

	public string GetString (string key)
	{
		if (key == null)
			throw new ArgumentNullException ("key");
		return Encoding.UTF8.GetString (Get (key));
	}

	public byte [][] Sort (SortOptions options)
	{
		return Sort (options.Key, options.StoreInKey, options.ToArgs());
	}

	public byte [][] Sort (string key, string destination, params object [] options)
	{
		if (key == null)
			throw new ArgumentNullException ("key");

		int offset = string.IsNullOrEmpty (destination) ? 1 : 3;
		object [] args = new object [offset + options.Length];

		args [0] = key;
		Array.Copy (options, 0, args, offset, options.Length);
		if (offset == 1) {
			return SendExpectDataArray ("SORT", args);
		}
		else {
			args [1] = "STORE";
			args [2] = destination;
			int n = SendExpectInt ("SORT", args);
			return new byte [n][];
		}
	}
	
	public byte [] GetSet (string key, byte [] value)
	{
		if (key == null)
			throw new ArgumentNullException ("key");
		if (value == null)
			throw new ArgumentNullException ("value");
		
		if (value.Length > 1073741824)
			throw new ArgumentException ("value exceeds 1G", "value");

		if (!SendDataCommand (value, "GETSET", key))
			throw new Exception ("Unable to connect");

		return ReadData ();
	}

	public string GetSet (string key, string value)
	{
		if (key == null)
			throw new ArgumentNullException ("key");
		if (value == null)
			throw new ArgumentNullException ("value");
		return Encoding.UTF8.GetString (GetSet (key, Encoding.UTF8.GetBytes (value)));
	}
	
	string ReadLine ()
	{
		StringBuilder sb = new StringBuilder ();
		int c;
		
		while ((c = bstream.ReadByte ()) != -1){
			if (c == '\r')
				continue;
			if (c == '\n')
				break;
			sb.Append ((char) c);
		}
		return sb.ToString ();
	}
	
	void Connect ()
	{
		socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		socket.NoDelay = true;
		socket.SendTimeout = SendTimeout;
		socket.Connect (Host, Port);
		if (!socket.Connected){
			socket.Close ();
			socket = null;
			return;
		}
		bstream = new BufferedStream (new NetworkStream (socket), 16*1024);
		
		if (Password != null)
			SendExpectSuccess ("AUTH", Password);
	}

	byte [] end_data = new byte [] { (byte) '\r', (byte) '\n' };

	bool SendDataCommand (byte [] data, string cmd, params object [] args)
	{
		string resp = "*" + (1 + args.Length + 1).ToString () + "\r\n";
		resp += "$" + cmd.Length + "\r\n" + cmd + "\r\n";
		foreach (object arg in args) {
			string argStr = arg.ToString ();
			int argStrLength = Encoding.UTF8.GetByteCount(argStr);
			resp += "$" + argStrLength + "\r\n" + argStr + "\r\n";
		}
		resp +=	"$" + data.Length + "\r\n";

		return SendDataRESP (data, resp);
	}

	bool SendDataRESP (byte [] data, string resp)
	{
		if (socket == null)
			Connect ();
		if (socket == null)
			return false;

		byte [] r = Encoding.UTF8.GetBytes (resp);
		try {
			Log ("C", resp);
			socket.Send (r);
			if (data != null){
				socket.Send (data);
				socket.Send (end_data);
			}
		} catch (SocketException){
			// timeout;
			socket.Close ();
			socket = null;

			return false;
		}
		return true;
	}

	bool SendCommand (string cmd, params object [] args)
	{
		if (socket == null)
			Connect ();
		if (socket == null)
			return false;

		string resp = "*" + (1 + args.Length).ToString () + "\r\n";
		resp += "$" + cmd.Length + "\r\n" + cmd + "\r\n";
		foreach (object arg in args) {
			string argStr = arg.ToString ();
			int argStrLength = Encoding.UTF8.GetByteCount(argStr);
			resp += "$" + argStrLength + "\r\n" + argStr + "\r\n";
		}

		byte [] r = Encoding.UTF8.GetBytes (resp);
		try {
			Log ("C", resp);
			socket.Send (r);
		} catch (SocketException){
			// timeout;
			socket.Close ();
			socket = null;

			return false;
		}
		return true;
	}
	
	[Conditional ("DEBUG")]
	void Log (string id, string message)
	{
		Console.WriteLine(id + ": " + message.Trim().Replace("\r\n", " "));
	}

	void ExpectSuccess ()
	{
		int c = bstream.ReadByte ();
		if (c == -1)
			throw new ResponseException ("No more data");

		string s = ReadLine ();
		Log ("S", (char)c + s);
		if (c == '-')
			throw new ResponseException (s.StartsWith ("ERR ") ? s.Substring (4) : s);
	}
	
	void SendExpectSuccess (string cmd, params object [] args)
	{
		if (!SendCommand (cmd, args))
			throw new Exception ("Unable to connect");

		ExpectSuccess ();
	}	

	int SendDataExpectInt (byte[] data, string cmd, params object [] args)
	{
		if (!SendDataCommand (data, cmd, args))
			throw new Exception ("Unable to connect");

		int c = bstream.ReadByte ();
		if (c == -1)
			throw new ResponseException ("No more data");

		string s = ReadLine ();
		Log ("S", (char)c + s);
		if (c == '-')
			throw new ResponseException (s.StartsWith ("ERR ") ? s.Substring (4) : s);
		if (c == ':'){
			int i;
			if (int.TryParse (s, out i))
				return i;
		}
		throw new ResponseException ("Unknown reply on integer request: " + c + s);
	}	

	int SendExpectInt (string cmd, params object [] args)
	{
		if (!SendCommand (cmd, args))
			throw new Exception ("Unable to connect");

		int c = bstream.ReadByte ();
		if (c == -1)
			throw new ResponseException ("No more data");

		string s = ReadLine ();
		Log ("S", (char)c + s);
		if (c == '-')
			throw new ResponseException (s.StartsWith ("ERR ") ? s.Substring (4) : s);
		if (c == ':'){
			int i;
			if (int.TryParse (s, out i))
				return i;
		}
		throw new ResponseException ("Unknown reply on integer request: " + c + s);
	}	

	string SendExpectString (string cmd, params object [] args)
	{
		if (!SendCommand (cmd, args))
			throw new Exception ("Unable to connect");

		int c = bstream.ReadByte ();
		if (c == -1)
			throw new ResponseException ("No more data");

		string s = ReadLine ();
		Log ("S", (char)c + s);
		if (c == '-')
			throw new ResponseException (s.StartsWith ("ERR ") ? s.Substring (4) : s);
		if (c == '+')
			return s;
		
		throw new ResponseException ("Unknown reply on integer request: " + c + s);
	}	

	//
	// This one does not throw errors
	//
	string SendGetString (string cmd, params object [] args)
	{
		if (!SendCommand (cmd, args))
			throw new Exception ("Unable to connect");

		return ReadLine ();
	}	
	
	byte [] SendExpectData (string cmd, params object [] args)
	{
		if (!SendCommand (cmd, args))
			throw new Exception ("Unable to connect");

		return ReadData ();
	}

	byte [] ReadData ()
	{
		string s = ReadLine ();
		Log ("S", s);
		if (s.Length == 0)
			throw new ResponseException ("Zero length respose");
		
		char c = s [0];
		if (c == '-')
			throw new ResponseException (s.StartsWith ("-ERR ") ? s.Substring (5) : s.Substring (1));

		if (c == '$'){
			if (s == "$-1")
				return null;
			int n;
			
			if (Int32.TryParse (s.Substring (1), out n)){
				byte [] retbuf = new byte [n];

				int bytesRead = 0;
				do {
					int read = bstream.Read (retbuf, bytesRead, n - bytesRead);
					if (read < 1)
						throw new ResponseException("Invalid termination mid stream");
					bytesRead += read; 
				}
				while (bytesRead < n);
				if (bstream.ReadByte () != '\r' || bstream.ReadByte () != '\n')
					throw new ResponseException ("Invalid termination");
				return retbuf;
			}
			throw new ResponseException ("Invalid length");
		}

		/* don't treat arrays here because only one element works -- use DataArray!
		//returns the number of matches
		if (c == '*') {
			int n;
			if (Int32.TryParse(s.Substring(1), out n)) 
				return n <= 0 ? new byte [0] : ReadData();
			
			throw new ResponseException ("Unexpected length parameter" + r);
		}
		*/

		throw new ResponseException ("Unexpected reply: " + s);
	}	

	public bool ContainsKey (string key)
	{
		if (key == null)
			throw new ArgumentNullException ("key");
		return SendExpectInt ("EXISTS", key) == 1;
	}

	public bool Remove (string key)
	{
		if (key == null)
			throw new ArgumentNullException ("key");
		return SendExpectInt ("DEL", key) == 1;
	}

	public int Remove (params string [] args)
	{
		if (args == null)
			throw new ArgumentNullException ("args");
		return SendExpectInt ("DEL", args);
	}

	public int Increment (string key)
	{
		if (key == null)
			throw new ArgumentNullException ("key");
		return SendExpectInt ("INCR", key);
	}

	public int Increment (string key, int count)
	{
		if (key == null)
			throw new ArgumentNullException ("key");
		return SendExpectInt ("INCRBY", key, count);
	}

	public int Decrement (string key)
	{
		if (key == null)
			throw new ArgumentNullException ("key");
		return SendExpectInt ("DECR", key);
	}

	public int Decrement (string key, int count)
	{
		if (key == null)
			throw new ArgumentNullException ("key");
		return SendExpectInt ("DECRBY", key, count);
	}

	public KeyType TypeOf (string key)
	{
		if (key == null)
			throw new ArgumentNullException ("key");
		switch (SendExpectString ("TYPE", key)) {
		case "none":
			return KeyType.None;
		case "string":
			return KeyType.String;
		case "set":
			return KeyType.Set;
		case "list":
			return KeyType.List;
		}
		throw new ResponseException ("Invalid value");
	}

	public string RandomKey ()
	{
		return SendExpectString ("RANDOMKEY");
	}

	public bool Rename (string oldKeyname, string newKeyname)
	{
		if (oldKeyname == null)
			throw new ArgumentNullException ("oldKeyname");
		if (newKeyname == null)
			throw new ArgumentNullException ("newKeyname");
		return SendGetString ("RENAME", oldKeyname, newKeyname) [0] == '+';
	}

	public bool Expire (string key, int seconds)
	{
		if (key == null)
			throw new ArgumentNullException ("key");
		return SendExpectInt ("EXPIRE", key, seconds) == 1;
	}

	public bool ExpireAt (string key, int time)
	{
		if (key == null)
			throw new ArgumentNullException ("key");
		return SendExpectInt ("EXPIREAT", key, time) == 1;
	}

	public int TimeToLive (string key)
	{
		if (key == null)
			throw new ArgumentNullException ("key");
		return SendExpectInt ("TTL", key);
	}
	
	public int DbSize {
		get {
			return SendExpectInt ("DBSIZE");
		}
	}

	public void Save ()
	{
		SendExpectSuccess ("SAVE");
	}

	public void BackgroundSave ()
	{
		SendExpectSuccess ("BGSAVE");
	}

	public void Shutdown ()
	{
		SendCommand ("SHUTDOWN");
		try {
			// the server may return an error
			string s = ReadLine ();
			Log ("S", s);
			if (s.Length == 0)
				throw new ResponseException ("Zero length respose");
			throw new ResponseException (s.StartsWith ("-ERR ") ? s.Substring (5) : s.Substring (1));
		} catch (IOException) {
			// this is the expected good result
			socket.Close ();
			socket = null;
		}
	}

	public void FlushAll ()
	{
		SendExpectSuccess ("FLUSHALL");
	}
	
	public void FlushDb ()
	{
		SendExpectSuccess ("FLUSHDB");
	}

	const long UnixEpoch = 621355968000000000L;
	
	public DateTime LastSave {
		get {
			int t = SendExpectInt ("LASTSAVE");
			
			return new DateTime (UnixEpoch) + TimeSpan.FromSeconds (t);
		}
	}
	
	public Dictionary<string,string> GetInfo ()
	{
		byte [] r = SendExpectData ("INFO");
		var dict = new Dictionary<string,string>();
		
		foreach (var line in Encoding.UTF8.GetString (r).Split ('\n')){
			int p = line.IndexOf (':');
			if (p == -1)
				continue;
			dict.Add (line.Substring (0, p), line.Substring (p+1));
		}
		return dict;
	}

	public string [] Keys {
		get {
			return GetKeys("*");
		}
	}

	public string [] GetKeys (string pattern)
	{
		if (pattern == null)
			throw new ArgumentNullException ("pattern");

		return SendExpectStringArray ("KEYS", pattern);
	}

	public byte [][] MGet (params string [] keys)
	{
		if (keys == null)
			throw new ArgumentNullException ("keys");
		if (keys.Length == 0)
			throw new ArgumentException ("keys");
		
		return SendExpectDataArray ("MGET", keys);
	}


	public string [] SendExpectStringArray (string cmd, params object [] args)
	{
		byte [][] reply = SendExpectDataArray (cmd, args);
		string [] keys = new string [reply.Length];
		for (int i = 0; i < reply.Length; i++)
			keys[i] = Encoding.UTF8.GetString (reply[i]);
		return keys;
	}

	public byte[][] SendExpectDataArray (string cmd, params object [] args)
	{
		if (!SendCommand (cmd, args))
			throw new Exception("Unable to connect");
		int c = bstream.ReadByte();
		if (c == -1)
			throw new ResponseException("No more data");
		
		string s = ReadLine();
		Log("S", (char)c + s);
		if (c == '-')
			throw new ResponseException(s.StartsWith("ERR ") ? s.Substring(4) : s);
		if (c == '*') {
			int count;
			if (int.TryParse (s, out count)) {
				byte [][] result = new byte [count][];
				
				for (int i = 0; i < count; i++)
					result[i] = ReadData();
				
				return result;
			}
		}
		throw new ResponseException("Unknown reply on multi-request: " + c + s);
	}
	
	#region List commands
	public byte[][] ListRange(string key, int start, int end)
	{
		return SendExpectDataArray ("LRANGE", key, start, end);
	}

	public void LeftPush(string key, string value)
	{
		LeftPush(key, Encoding.UTF8.GetBytes (value));
	}

	public void LeftPush(string key, byte [] value)
	{
		SendDataCommand (value, "LPUSH", key);
		ExpectSuccess();
	}

	public void RightPush(string key, string value)
	{
		RightPush(key, Encoding.UTF8.GetBytes (value));
	}

	public void RightPush(string key, byte [] value)
	{
		SendDataCommand (value, "RPUSH", key);
		ExpectSuccess();
	}

	public int ListLength (string key)
	{
		return SendExpectInt ("LLEN", key);
	}

	public byte[] ListIndex (string key, int index)
	{
		SendCommand ("LINDEX", key, index);
		return ReadData ();
	}

	public byte[] LeftPop(string key)
	{
		SendCommand ("LPOP", key);
		return ReadData ();
	}

	public byte[] RightPop(string key)
	{
		SendCommand ("RPOP", key);
		return ReadData ();
	}
	#endregion

	#region Set commands
	public bool AddToSet (string key, byte[] member)
	{
		return SendDataExpectInt(member, "SADD", key) > 0;
	}

	public bool AddToSet (string key, string member)
	{
		return AddToSet (key, Encoding.UTF8.GetBytes(member));
	}
	
	public int CardinalityOfSet (string key)
	{
		return SendExpectInt ("SCARD", key);
	}

	public bool IsMemberOfSet (string key, byte[] member)
	{
		return SendDataExpectInt (member, "SISMEMBER", key) > 0;
	}

	public bool IsMemberOfSet(string key, string member)
	{
		return IsMemberOfSet(key, Encoding.UTF8.GetBytes(member));
	}
	
	public byte[][] GetMembersOfSet (string key)
	{
		return SendExpectDataArray ("SMEMBERS", key);
	}
	
	public byte[] GetRandomMemberOfSet (string key)
	{
		return SendExpectData ("SRANDMEMBER", key);
	}
	
	public byte[] PopRandomMemberOfSet (string key)
	{
		return SendExpectData ("SPOP", key);
	}

	public bool RemoveFromSet (string key, byte[] member)
	{
		return SendDataExpectInt (member, "SREM", key) > 0;
	}

	public bool RemoveFromSet (string key, string member)
	{
		return RemoveFromSet (key, Encoding.UTF8.GetBytes(member));
	}
		
	public byte[][] GetUnionOfSets (params string[] keys)
	{
		if (keys == null)
			throw new ArgumentNullException();
		
		return SendExpectDataArray ("SUNION", keys);
		
	}
	
	void StoreSetCommands (string cmd, params string[] keys)
	{
		if (String.IsNullOrEmpty(cmd))
			throw new ArgumentNullException ("cmd");

		if (keys == null)
			throw new ArgumentNullException ("keys");

		SendExpectSuccess (cmd, keys);
	}
	
	public void StoreUnionOfSets (params string[] keys)
	{
		StoreSetCommands ("SUNIONSTORE", keys);
	}
	
	public byte[][] GetIntersectionOfSets (params string[] keys)
	{
		if (keys == null)
			throw new ArgumentNullException();
		
		return SendExpectDataArray ("SINTER", keys);
	}
	
	public void StoreIntersectionOfSets (params string[] keys)
	{
		StoreSetCommands ("SINTERSTORE", keys);
	}
	
	public byte[][] GetDifferenceOfSets (params string[] keys)
	{
		if (keys == null)
			throw new ArgumentNullException();
		
		return SendExpectDataArray ("SDIFF", keys);
	}
	
	public void StoreDifferenceOfSets (params string[] keys)
	{
		StoreSetCommands ("SDIFFSTORE", keys);
	}
	
	public bool MoveMemberToSet (string srcKey, string destKey, byte[] member)
	{
		return SendDataExpectInt (member, "SMOVE", srcKey, destKey) > 0;
	}
	#endregion

	public void Dispose ()
	{
		Dispose (true);
		GC.SuppressFinalize (this);
	}

	~Redis ()
	{
		Dispose (false);
	}
	
	protected virtual void Dispose (bool disposing)
	{
		if (disposing){
			SendCommand ("QUIT");
			ExpectSuccess ();
			socket.Close ();
			socket = null;
		}
	}
}

public class SortOptions {
	public string Key { get; set; }
	public bool Descending { get; set; }
	public bool Lexographically { get; set; }
	public Int32 LowerLimit { get; set; }
	public Int32 UpperLimit { get; set; }
	public string By { get; set; }
	public string StoreInKey { get; set; }
	public string Get { get; set; }
	
	public object [] ToArgs ()
	{
		System.Collections.ArrayList args = new System.Collections.ArrayList();

		if (LowerLimit != 0 || UpperLimit != 0) {
			args.Add ("LIMIT");
			args.Add (LowerLimit);
			args.Add (UpperLimit);
		}
		if (Lexographically)
			args.Add("ALPHA");
		if (!string.IsNullOrEmpty (By)) {
			args.Add("BY");
			args.Add(By);
		}
		if (!string.IsNullOrEmpty (Get)) {
			args.Add("GET");
			args.Add(Get);
		}
		return args.ToArray ();
	}
}
