Function SetPermissionsControlPanel
	On Error Resume Next

	Dim SitePath
	Dim User
    Dim PermissionsStr
    Dim Shell

    SitePath = Session.Property("CustomActionData")
	User = "IIS AppPool\ONLYOFFICE Control Panel"
    Set Shell = CreateObject("WScript.Shell")

	Shell.Run  "cacls """ & SitePath & "Data"" /e /g """ & User & """:c", 0, True
	Shell.Run "icacls """ & SitePath & "Data"" /grant """ & User & """:(OI)(CI)(M)", 0, True

  	Shell.Run  "cacls """ & SitePath & "Logs"" /e /g """ & User & """:c", 0, True
	Shell.Run "icacls """ & SitePath & "Logs"" /grant """ & User & """:(OI)(CI)(M)", 0, True

    Set Shell = Nothing
	SetPermissionsControlPanel = 0
End Function



Function RandomString( ByVal strLen )
    Dim str, min, max

    Const LETTERS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLOMNOPQRSTUVWXYZ0123456789"
    min = 1
    max = Len(LETTERS)

    Randomize
    For i = 1 to strLen
        str = str & Mid( LETTERS, Int((max-min+1)*Rnd+min), 1 )
    Next
    RandomString = str  
End Function

Function SetDocumentServerJWTSecretProp
    On Error Resume Next

    Session.Property("DOCUMENT_SERVER_JWT_SECRET") = RandomString( 30 )

End Function

Function SetIISPermissions
	On Error Resume Next

	Dim SitePath
	Dim User
    Dim PermissionsStr
    Dim Shell
	Dim customActionData
	Dim iisSiteName
    Dim iisApiSystemSiteName
	
	Call WriteToLog("Start SetIISPermissions CustomAction")
		
    Set Shell = CreateObject("WScript.Shell")
	   	
	customActionData = Split(Session.Property("CustomActionData"), ",")
	
	SitePath = customActionData(0)
	iisSiteName = customActionData(1)
	iisApiSystemSiteName = customActionData(2)
	
	Call WriteToLog("SitePath is " & SitePath)
	Call WriteToLog("iisSiteName is " & iisSiteName)
	Call WriteToLog("iisApiSystemSiteName is " & iisApiSystemSiteName)
	
	User = "IIS AppPool\" & iisApiSystemSiteName
   
	Shell.Run  "cacls """ & SitePath & "Data"" /e /g """ & User & """:c", 0, True
	Shell.Run "icacls """ & SitePath & "Data"" /grant """ & User & """:(OI)(CI)(M)", 0, True
    Shell.Run  "cacls """ & SitePath & "Logs"" /e /g """ & User & """:c", 0, True
	Shell.Run "icacls """ & SitePath & "Logs"" /grant """ & User & """:(OI)(CI)(M)", 0, True
   
  	User = "IIS AppPool\" & iisSiteName
   
	Shell.Run  "cacls """ & SitePath & "Data"" /e /g """ & User & """:c", 0, True
	Shell.Run "icacls """ & SitePath & "Data"" /grant """ & User & """:(OI)(CI)(M)", 0, True
    Shell.Run  "cacls """ & SitePath & "Logs"" /e /g """ & User & """:c", 0, True
	Shell.Run "icacls """ & SitePath & "Logs"" /grant """ & User & """:(OI)(CI)(M)", 0, True
'    Shell.Run  "cacls """ & SitePath & "WebStudio\web.appsettings.config"" /e /g """ & User & """:c", 0, True
'	Shell.Run "icacls """ & SitePath & "WebStudio\web.appsettings.config"" /grant """ & User & """:(OI)(CI)(M)", 0, True
    Shell.Run  "cacls """ & SitePath & "WebStudio"" /e /g """ & User & """:c", 0, True
	Shell.Run "icacls """ & SitePath & "WebStudio"" /grant """ & User & """:(OI)(CI)(M)", 0, True
    
    Set Shell = Nothing
	SetIISPermissions = 0
	
	Call WriteToLog("End SetIISPermissions CustomAction")
	
End Function

Function SetServicesPermissions
	On Error Resume Next

	Dim SitePath
	Dim User
    Dim PermissionsStr
    Dim Shell

    Set Shell = CreateObject("WScript.Shell")

    SitePath = Session.Property("CustomActionData")
      
    User = "NT AUTHORITY\LOCAL SERVICE"
	
    Shell.Run  "cacls """ & SitePath & "Data"" /e /g """ & User & """:c", 0, True
	Shell.Run "icacls """ & SitePath & "Data"" /grant """ & User & """:(OI)(CI)(M)", 0, True
    Shell.Run  "cacls """ & SitePath & "Logs"" /e /g """ & User & """:c", 0, True
	Shell.Run "icacls """ & SitePath & "Logs"" /grant """ & User & """:(OI)(CI)(M)", 0, True
    Shell.Run  "cacls """ & SitePath & "Services\TeamLabSvc\sphinx-min.conf.in"" /e /g """ & User & """:c", 0, True
	Shell.Run "icacls """ & SitePath & "Services\TeamLabSvc\sphinx-min.conf.in"" /grant """ & User & """:(OI)(CI)(M)", 0, True
    Shell.Run  "cacls """ & SitePath & "Services\TeamLabSvc\searchd.pid"" /e /g """ & User & """:c", 0, True
	Shell.Run "icacls """ & SitePath & "Services\TeamLabSvc\searchd.pid"" /grant """ & User & """:(OI)(CI)(M)", 0, True

	Shell.Run "netsh http add urlacl url=http://*:5280/http-poll/ user=""" & User & """", 0, True
    
    Set Shell = Nothing
	SetServicesPermissions = 0
End Function

Function ElasticSearchSetup
	On Error Resume Next
	
	Dim ShellCommand
	Dim APP_INDEX_DIR
	
	Const ForReading = 1
	Const ForWriting = 2
	
	Set Shell = CreateObject("WScript.Shell")
	Set objFSO = CreateObject("Scripting.FileSystemObject")

	APP_INDEX_DIR = Session.Property("APPDIR") & "Data\Index\v7.9.0\"  
   
	If Not fso.FolderExists(APP_INDEX_DIR) Then
		Session.Property("NEED_REINDEX_ELASTICSEARCH") = "TRUE"
	End If
	
	Call Shell.Run("%COMSPEC% /c mkdir """ & Session.Property("APPDIR") & "Data\Index\v7.9.0\""",0,true)
	Call Shell.Run("%COMSPEC% /c mkdir """ & Session.Property("APPDIR") & "Logs\""",0,true)
	
	Set objFile = objFSO.OpenTextFile(Session.Property("CommonAppDataFolder") & "Elastic\Elasticsearch\config\elasticsearch.yml", ForReading)

	fileContent = objFile.ReadAll

	objFile.Close

    Set oRE = New RegExp
    oRE.Global = True
    
    If InStrRev(fileContent, "indices.fielddata.cache.size") = 0 Then
       fileContent = fileContent & Chr(13) & Chr(10) & "indices.fielddata.cache.size: 30%"
    Else
       oRE.Pattern = "indices.fielddata.cache.size:.*"
       fileContent = oRE.Replace(fileContent, "indices.fielddata.cache.size: 30%")                           
    End if

    If InStrRev(fileContent, "indices.memory.index_buffer_size") = 0 Then
       fileContent = fileContent & Chr(13) & Chr(10) & "indices.memory.index_buffer_size: 30%"
    Else
       oRE.Pattern = "indices.memory.index_buffer_size:.*"
       fileContent = oRE.Replace(fileContent, "indices.memory.index_buffer_size: 30%")                           
    End if
		
    If InStrRev(fileContent, "http.max_content_length") <> 0 Then    
       oRE.Pattern = "http.max_content_length:.*"
       fileContent = oRE.Replace(fileContent, " ")                           
    End if

    If InStrRev(fileContent, "thread_pool.index.queue_size") <> 0 Then    
       oRE.Pattern = "thread_pool.index.queue_size:.*"
       fileContent = oRE.Replace(fileContent, " ")                           
    End if

    If InStrRev(fileContent, "thread_pool.index.size") <> 0 Then    
       oRE.Pattern = "thread_pool.index.size:.*"
       fileContent = oRE.Replace(fileContent, " ")                           
    End if

    If InStrRev(fileContent, "thread_pool.write.queue_size") <> 0 Then    
       oRE.Pattern = "thread_pool.write.queue_size:.*"
       fileContent = oRE.Replace(fileContent, " ")                           
    End if

    If InStrRev(fileContent, "thread_pool.write.size") <> 0 Then    
       oRE.Pattern = "thread_pool.write.size:.*"
       fileContent = oRE.Replace(fileContent, " ")                           
    End if

    oRE.Pattern = "path.data:.*"
    fileContent = oRE.Replace(fileContent, "path.data: " & Session.Property("APPDIR") & "Data\Index\v7.9.0\")                           

    oRE.Pattern = "path.logs:.*"
    fileContent = oRE.Replace(fileContent, "path.logs: " & Session.Property("APPDIR") & "Logs\")                           
	
	Call WriteToLog("ElasticSearchSetup: New config:" & fileContent)	
	Call WriteToLog("ElasticSearchSetup:  CommonAppDataFolder :" & Session.Property("CommonAppDataFolder") & "Elastic\Elasticsearch\data")
		
	Set objFile = objFSO.OpenTextFile(Session.Property("CommonAppDataFolder") & "Elastic\Elasticsearch\config\elasticsearch.yml", ForWriting)

	objFile.WriteLine fileContent

	objFile.Close

    Set objFile = objFSO.OpenTextFile(Session.Property("CommonAppDataFolder") & "Elastic\Elasticsearch\config\jvm.options", ForReading)

	fileContent = objFile.ReadAll

	objFile.Close

    If InStrRev(fileContent, "-XX:+HeapDumpOnOutOfMemoryError") <> 0 Then    
       oRE.Pattern = "-XX:+HeapDumpOnOutOfMemoryError"
       fileContent = oRE.Replace(fileContent, " ")                           
    End if
	
	If InStrRev(fileContent, "-Xms1g") <> 0 Then
       oRE.Pattern = "-Xms1g"
       fileContent = oRE.Replace(fileContent, "-Xms4g")                           
    ElseIf InStrRev(fileContent, "-Xms4g") <> 0 Then
       fileContent = fileContent & Chr(13) & Chr(10) & "-Xms4g"
    End if

	If InStrRev(fileContent, "-Xmx1g") <> 0 Then
       oRE.Pattern = "-Xmx1g"
       fileContent = oRE.Replace(fileContent, "-Xmx4g")                           
    ElseIf InStrRev(fileContent, "-Xmx4g") <> 0 Then
       fileContent = fileContent & Chr(13) & Chr(10) & "-Xmx4g"
    End if

    Set objFile = objFSO.OpenTextFile(Session.Property("CommonAppDataFolder") & "Elastic\Elasticsearch\config\jvm.options", ForWriting)

    objFile.WriteLine fileContent

    objFile.Close
	
    Set Shell = Nothing
	
End Function

Function ElasticSearchInstallPlugin
    On Error Resume Next

    Dim Shell

    Set Shell = CreateObject("WScript.Shell")

    ElasticSearchInstallPluginDir = Session.Property("CustomActionData")

    ShellCommand = """C:\Program Files\Elastic\Elasticsearch\7.9.0\bin\elasticsearch-plugin""" & " install -b file:///" & """" & ElasticSearchInstallPluginDir & "ingest-attachment-7.9.0.zip""" 
     
    Call Shell.Run("cmd /C " & """" & ShellCommand  & """",0,true)

    Set Shell = Nothing

End Function

Function RedisSetup
	On Error Resume Next
 
    Dim Shell

    Set Shell = CreateObject("WScript.Shell")
		
	Shell.Run "redis-cli config set save """"", 0, True
	Shell.Run "redis-cli config rewrite", 0, True
	
    Set Shell = Nothing
End Function

Function PostgreSqlConfig
	On Error Resume Next
    
   If (StrComp(Session.Property("POSTGRE_SQL_PATH"),"FALSE") = 0) Then
            Wscript.Quit
    End If

  	Dim ErrorText
	Dim Pos, postgreSqlDriver
	Dim databaseUserName
	Dim databaseUserPwd
	Dim databaseName

	databaseUserName = Session.Property("POSTGRESQL_USERNAME_PROP")
	databaseUserPwd = Session.Property("POSTGRESQL_PASSWORD_PROP")
	databaseName = Session.Property("POSTGRESQL_DATABASE_PROP")

    Call WriteToLog("PostgreSqlConfig: databaseUserName is " & databaseUserName)
    Call WriteToLog("PostgreSqlConfig: databaseUserPwd is " & databaseUserPwd)
    Call WriteToLog("PostgreSqlConfig: databaseName is " & databaseName)

    postgreSqlDriver = "PostgreSQL Unicode(x64)"

    Set ConnectionObject = CreateObject("ADODB.Connection")
	ConnectionObject.Open "Driver={" & postgreSqlDriver & "};Server=" & "localhost" & ";Port=" & "5432" & ";Database=" & "postgres" & ";Uid=" & "postgres" & ";Pwd=" & "postgres"
			
	ConnectionObject.Execute "CREATE DATABASE " & Session.Property("POSTGRESQL_DATABASE_PROP") 
	ConnectionObject.Execute "create user " & Session.Property("POSTGRESQL_USERNAME_PROP") & " with encrypted password '" & Session.Property("POSTGRESQL_PASSWORD_PROP") & "'" 
	ConnectionObject.Execute "grant all privileges on database " & Session.Property("POSTGRESQL_DATABASE_PROP") & " to " & Session.Property("POSTGRESQL_USERNAME_PROP") 
	
	If Err.Number <> 0 Then
    	ErrorText = Err.Description
		Pos = InStrRev( ErrorText, "]" )
		If 0 < Pos Then
            Call WriteToLog("PostgreSqlConfig: error is " & ErrorText)
			ErrorText = Right( ErrorText, Len( ErrorText ) - Pos )
        	Session.Property("PostgreSqlConnectionError") = ErrorText

		End If
    End If
	
    ConnectionObject.Close
    
    Set ConnectionObject = Nothing

End Function


Function TestPostgreSqlConnection
	On Error Resume Next

	Dim ErrorText
	Dim Pos, postgreSqlDriver

    postgreSqlDriver = "PostgreSQL Unicode(x64)"

   	Session.Property("PostgreSqlConnectionError") = ""

    Set ConnectionObject = CreateObject("ADODB.Connection")
	ConnectionObject.Open "Driver={" & postgreSqlDriver & "};Server=" & Session.Property("POSTGRESQL_SERVER_PROP") & ";Port=" & Session.Property("POSTGRESQL_PORT_PROP")  & ";Database=" & Session.Property("POSTGRESQL_DATABASE_PROP") & ";Uid=" & Session.Property("POSTGRESQL_USERNAME_PROP") & ";Pwd=" & Session.Property("POSTGRESQL_PASSWORD_PROP")
	
	If Err.Number <> 0 Then
    	ErrorText = Err.Description
		Pos = InStrRev( ErrorText, "]" )
		If 0 < Pos Then
			ErrorText = Right( ErrorText, Len( ErrorText ) - Pos )
		End If
		Session.Property("PostgreSqlConnectionError") = ErrorText
	End If

    ConnectionObject.Close
    
    Set ConnectionObject = Nothing
   
End Function


Function TestSqlConnection
	On Error Resume Next

    Const HKLM = &H80000002	
	Dim ErrorText
	Dim Pos, keys, mysqlDriver
    Dim registry
    
	TestSqlConnection = 0
	Session.Property("SqlConnectionError") = ""

    Set registry = GetObject("winmgmts:{impersonationLevel=impersonate}!\\.\root\default:StdRegProv")
    registry.EnumKey HKLM, "SOFTWARE\ODBC\ODBCINST.INI", keys
    If Not IsNull(keys) Then
        For Each key In keys
            If InStr(1, key, "MySQL ODBC", 1) <> 0 And InStr(1, key, "ANSI", 1) = 0 Then
                mysqlDriver = key
            End If
        Next
    End If

    If mysqlDriver = "" Then
        registry.EnumKey HKLM, "SOFTWARE\WOW6432Node\ODBC\ODBCINST.INI", keys
        If Not IsNull(keys) Then
            For Each key In keys
                If InStr(1, key, "MySQL ODBC", 1) <> 0 And InStr(1, key, "ANSI", 1) = 0 Then
                    mysqlDriver = key
                End If
            Next
        End If
    End If
    
    Session.Property("MYSQLODBCDRIVER") = mysqlDriver

	Set ConnectionObject = CreateObject("ADODB.Connection")
	ConnectionObject.Open "Driver={" & mysqlDriver & "};Server=" & Session.Property("SERVER_PROP") & ";Port=" & Session.Property("PORT_PROP") & ";Uid=" & Session.Property("USERNAME_PROP") & ";Pwd=" & Session.Property("PASSWORD_PROP")
	
	If Err.Number <> 0 Then
		ErrorText = Err.Description
		Pos = InStrRev( ErrorText, "]" )
		If 0 < Pos Then
			ErrorText = Right( ErrorText, Len( ErrorText ) - Pos )
		End If
		Session.Property("SqlConnectionError") = ErrorText
	End If
	
	Set ConnectionObject = Nothing
End Function

Function WarmUp()
	Dim XmlHttpObject
	Dim ShellObject
	Dim Url
	
	Set XmlHttpObject = CreateObject("MSXML2.XMLHTTP")
	Url = Session.Property("CustomActionData")
	
	If Url = "" Then
		Set ShellObject = CreateObject( "WScript.Shell" )
		Url = "http://" & ShellObject.ExpandEnvironmentStrings( "%COMPUTERNAME%" )
	End If

	XmlHttpObject.Open "GET", Url, False
	XmlHttpObject.Send
	XmlHttpObject.Open "GET", Url & "/products/community/", False
	XmlHttpObject.Send
	XmlHttpObject.Open "GET", Url & "/products/projects/", False
	XmlHttpObject.Send
	XmlHttpObject.Open "GET", Url & "/products/crm/", False
	XmlHttpObject.Send
	XmlHttpObject.Open "GET", Url & "/products/files/", False
	XmlHttpObject.Send
	XmlHttpObject.Open "GET", Url & "/addons/mail/", False
	XmlHttpObject.Send
End Function


Function ParseConnectionString()
	Dim ConnectionString
	Dim Parts
	Dim i
	
	ConnectionString = Session.Property("CustomActionData")
	Parts = Split( ConnectionString, ";" )
	Session.Property( "PORT_PROP" ) = "3306"
	For i = 0 to UBound(Parts) - 1
		If InStr( Parts(i), "Server=" ) Then
			Session.Property( "SERVER_PROP" ) = Right( Parts(i), Len( Parts(i) ) - InStr( Parts(i), "=" ) )
		End If
		If InStr( Parts(i), "Database=" ) Then
			Session.Property( "DATABASE_PROP" ) = Right( Parts(i), Len( Parts(i) ) - InStr( Parts(i), "=" ) )
		End If
		If InStr( Parts(i), "UserID=" ) Or InStr( Parts(i), "User ID=" ) Then
			Session.Property( "USERNAME_PROP" ) = Right( Parts(i), Len( Parts(i) ) - InStr( Parts(i), "=" ) )
		End If
		If InStr( Parts(i), "Port=" ) Then
			Session.Property( "PORT_PROP" ) = Right( Parts(i), Len( Parts(i) ) - InStr( Parts(i), "=" ) )
		End If
		If InStr( Parts(i), "Pwd=" ) Or InStr( Parts(i), "Password=" ) Then
			Session.Property( "PASSWORD_PROP" ) = Right( Parts(i), Len( Parts(i) ) - InStr( Parts(i), "=" ) )
		End If
	Next
End Function


Function CopyLicenseFile()
	Dim DestinationFolder
	Dim SourceFile
	Dim Data
	
	Data = Session.Property("CustomActionData")
	SourceFile = Split( Data, "|" )(0)
	DestinationFolder = Split( Data, "|" )(1)
	
	Set fso = CreateObject("Scripting.FileSystemObject")
   	fso.CopyFile SourceFile, DestinationFolder, True
	Set fso = Nothing
End Function


Function MySQLConfigure
    On Error Resume Next

    Dim installed, service

    Const HKLM = &H80000002
    Set registry = GetObject("winmgmts:{impersonationLevel=impersonate}!\\.\root\default:StdRegProv")
    registry.EnumKey HKLM, "SOFTWARE\ODBC\ODBCINST.INI", keys
    If Not IsNull(keys) Then
        For Each key In keys
            If InStr(1, key, "MySQL ODBC", 1) <> 0 And InStr(1, key, "ANSI", 1) = 0 Then
                mysqlDriver = key
            End If
        Next
    End If

    If mysqlDriver = "" Then
        registry.EnumKey HKLM, "SOFTWARE\WOW6432Node\ODBC\ODBCINST.INI", keys
        If Not IsNull(keys) Then
            For Each key In keys
                If InStr(1, key, "MySQL ODBC", 1) <> 0 And InStr(1, key, "ANSI", 1) = 0 Then
                    mysqlDriver = key
                End If
            Next
        End If
    End If
    
    Session.Property("MYSQLODBCDRIVER") = mysqlDriver

    Set shell = CreateObject("WScript.Shell")
    dbname = Session.Property("DATABASE_PROP")
    dbpass = Session.Property("PASSWORD_PROP")
	
    Err.Clear
    installDir = shell.RegRead("HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\MySQL AB\MySQL Server 8.0\Location")
    dataDir = shell.RegRead("HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\MySQL AB\MySQL Server 8.0\DataLocation")

    Call WriteToLog("MySQLConfigure: installDir " & installDir)
    Call WriteToLog("MySQLConfigure: dataDir " & dataDir)

	    
	If Err.Number <> 0 Then
        Err.Clear
        installDir = shell.RegRead("HKEY_LOCAL_MACHINE\SOFTWARE\MySQL AB\MySQL Server 8.0\Location")
 	    dataDir = shell.RegRead("HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\MySQL AB\MySQL Server 8.0\DataLocation") 
    End If

    Call WriteToLog("MySQLConfigure: installDir " & installDir)
    Call WriteToLog("MySQLConfigure: dataDir " & dataDir)
	

    If Err.Number = 0 Then
		Set wmiService = GetObject("winmgmts:{impersonationLevel=impersonate}!\\.\root\cimv2")
        Set service = wmiService.Get("Win32_Service.Name='MySQL80'")

		If Err.Number <> 0 Then
			WScript.Echo "MySQL80 service doesn't exists."
			Wscript.Quit 1
		End If 

		If service.Started Then			
			shell.Run """" & installDir & "bin\mysqladmin"" -u root password " & dbpass, 0, true		
        End If        
		
        Set filesys = CreateObject("Scripting.FileSystemObject")

		WriteIni filesys.BuildPath(dataDir, "my.ini"), "mysqld", "sql-mode", "NO_ENGINE_SUBSTITUTION"
		WriteIni filesys.BuildPath(dataDir, "my.ini"), "mysqld", "max_connections", "1000"
		WriteIni filesys.BuildPath(dataDir, "my.ini"), "mysqld", "max_allowed_packet", "100M"
        WriteIni filesys.BuildPath(dataDir, "my.ini"), "mysqld", "group_concat_max_len", "16M"
        WriteIni filesys.BuildPath(dataDir, "my.ini"), "mysqld", "character_set_server", "utf8"
        WriteIni filesys.BuildPath(dataDir, "my.ini"), "mysqld", "collation_server", "utf8_general_ci"
		
	    Call WriteToLog("MySQLConfigure: WriteIni Path" & filesys.BuildPath(dataDir, "my.ini"))

    End If
End Function

Sub CopyDirIfNotExists( source, dest )
	On Error Resume Next

	Set fso = CreateObject("Scripting.FileSystemObject")

	If Not fso.FolderExists( Replace( source, "\*", "" ) ) Or fso.FolderExists( dest ) Then
		Exit Sub
	End If
	
	dirs = Split( dest, "\" )
	path = dirs( 0 )
	For i = 1 To UBound( dirs )
		path = path & "\" & dirs( i )
		If Not fso.FolderExists( path ) Then
			fso.CreateFolder( path )
		End If
	Next
	
	fso.CopyFolder source, dest, False
	Set fso = Nothing
End Sub

Function ReadIni( myFilePath, mySection, myKey )
    ' This function returns a value read from an INI file
    '
    ' Arguments:
    ' myFilePath  [string]  the (path and) file name of the INI file
    ' mySection   [string]  the section in the INI file to be searched
    ' myKey       [string]  the key whose value is to be returned
    '
    ' Returns:
    ' the [string] value for the specified key in the specified section
    '
    ' CAVEAT:     Will return a space if key exists but value is blank
    '
    ' Written by Keith Lacelle
    ' Modified by Denis St-Pierre and Rob van der Woude

    Const ForReading   = 1
    Const ForWriting   = 2
    Const ForAppending = 8

    Dim intEqualPos
    Dim objFSO, objIniFile
    Dim strFilePath, strKey, strLeftString, strLine, strSection

    Set objFSO = CreateObject( "Scripting.FileSystemObject" )

    ReadIni     = ""
    strFilePath = Trim( myFilePath )
    strSection  = Trim( mySection )
    strKey      = Trim( myKey )

    If objFSO.FileExists( strFilePath ) Then
        Set objIniFile = objFSO.OpenTextFile( strFilePath, ForReading, False )
        Do While objIniFile.AtEndOfStream = False
            strLine = Trim( objIniFile.ReadLine )

            ' Check if section is found in the current line
            If LCase( strLine ) = "[" & LCase( strSection ) & "]" Then
                strLine = Trim( objIniFile.ReadLine )

                ' Parse lines until the next section is reached
                Do While Left( strLine, 1 ) <> "["
                    ' Find position of equal sign in the line
                    intEqualPos = InStr( 1, strLine, "=", 1 )
                    If intEqualPos > 0 Then
                        strLeftString = Trim( Left( strLine, intEqualPos - 1 ) )
                        ' Check if item is found in the current line
                        If LCase( strLeftString ) = LCase( strKey ) Then
                            ReadIni = Trim( Mid( strLine, intEqualPos + 1 ) )
                            ' In case the item exists but value is blank
                            If ReadIni = "" Then
                                ReadIni = " "
                            End If
                            ' Abort loop when item is found
                            Exit Do
                        End If
                    End If

                    ' Abort if the end of the INI file is reached
                    If objIniFile.AtEndOfStream Then Exit Do

                    ' Continue with next line
                    strLine = Trim( objIniFile.ReadLine )
                Loop
            Exit Do
            End If
        Loop
        objIniFile.Close
    Else
        WScript.Echo strFilePath & " doesn't exists. Exiting..."
        Wscript.Quit 1
    End If
End Function

Function WriteToLog(ByVal var)

    Const MsgType = &H04000000
    Set rec = Installer.CreateRecord(1)

    rec.StringData(1) = CStr(var)
    Session.Message MsgType, rec
    WriteToLog = 0

End Function

Function IsBlank(Value)
'returns True if Empty or NULL or Zero
If IsEmpty(Value) or IsNull(Value) Then
 IsBlank = True
ElseIf VarType(Value) = vbString Then
 If Value = "" Then
  IsBlank = True
 End If
ElseIf IsObject(Value) Then
 If Value Is Nothing Then
  IsBlank = True
 End If
ElseIf IsNumeric(Value) Then
 If Value = 0 Then
  wscript.echo " Zero value found"
  IsBlank = True
 End If
Else
 IsBlank = False
End If
End Function

Function CheckDocumentServerVersion()
    On Error Resume Next
	Dim http,URL

    Call WriteToLog("Start CheckDocumentServerVersion....")


	URL = "http://localhost:8000/info/info.json"

	Set http = CreateObject("Msxml2.XMLHTTP")
	http.open "GET",URL,False
	http.send
	
	If Err.Number <> 0 Then
		Session.Property("DOCUMENT_SERVER_VERSION_IS_PAID") = "FALSE"	

        Exit Function
	End If

	Dim oRE, oMatches
	Set oRE = New RegExp
	oRE.IgnoreCase = True
	oRE.Global = True
	oRE.Pattern = "(\x22packageType\x22:\s*0)"
	strJson = http.responseText

	If oRE.Test(strJson) then
		Session.Property("DOCUMENT_SERVER_VERSION_IS_PAID") = "FALSE"
		
		Exit Function
	End if

	Session.Property("DOCUMENT_SERVER_VERSION_IS_PAID") = "TRUE"
	Session.Property("DOCUMENT_SERVER_INSTALLED") = "TRUE"

    Call WriteToLog("End CheckDocumentServerVersion....")

End Function

Function editXML(XMLPath, Query, ChangedValue)
	Dim objXML, objError, objNodes, node  
	set objXML = CreateObject("MSXML2.DomDocument")
	objXML.load XMLPath
	objXML.async = False
	objXML.setProperty "SelectionLanguage", "XPath"
	set objError = objXML.parseError
	Set objNodes = objXML.selectNodes(Query)
		for each node in objNodes
			node.text = ChangedValue
		next
	objXML.save XMLPath
	set objXML = Nothing
End Function

Function waitForServiceState(serviceName, serviceState)
	Dim wmi, query
	
	Set wmi = GetObject("winmgmts://./root/cimv2")

	query = "SELECT * FROM Win32_Service WHERE Name='" & serviceName & "'"

	Do Until wmi.ExecQuery(query & " AND State='" & ServiceState & "'").Count > 0
		WScript.Sleep 100
	Loop

End Function

Function EnterpriseConfigure()
   	On Error Resume Next

    Dim appcmd 
    Dim rulesTemp
    Dim appSettingsTemp
	Dim iisSiteName
	Dim teamlabSvcCfg

    appcmd = Session.Property("SystemFolder") & "inetsrv\appcmd.exe"
   
    iisSiteName = Session.Property("IIS_SITE_NAME")	
   
    Set objShell = CreateObject("WScript.Shell")

	objShell.Run appcmd & " set config """ & iisSiteName & """ -section:system.webServer/rewrite/allowedServerVariables /+""[name='HTTP_X_SCRIPT_NAME']"" /commit:apphost",0,True
    objShell.Run appcmd & " set config """ & iisSiteName & """ -section:system.webServer/rewrite/allowedServerVariables /+""[name='HTTP_X_REWRITER_URL']"" /commit:apphost",0,True
    objShell.Run appcmd & " set config """ & iisSiteName & """ -section:system.webServer/rewrite/allowedServerVariables /+""[name='HTTP_X_FORWARDED_PROTO']"" /commit:apphost",0,True
    objShell.Run appcmd & " set config """ & iisSiteName & """ -section:system.webServer/rewrite/allowedServerVariables /+""[name='HTTP_X_FORWARDED_HOST']"" /commit:apphost",0,True
    objShell.Run appcmd & " set config """ & iisSiteName & """ -section:system.webServer/rewrite/allowedServerVariables /+""[name='HTTP_THE_SCHEME']"" /commit:apphost",0,True
    objShell.Run appcmd & " set config """ & iisSiteName & """ -section:system.webServer/rewrite/allowedServerVariables /+""[name='HTTP_THE_HOST']"" /commit:apphost",0,True

	rulesTemp = "/+""[name='SsoAuthRewrite',enabled='True',stopProcessing='True']"" " &_
			"/[name='SsoAuthRewrite'].match.url:""^sso\/(.*)"" " &_
			"/+[name='SsoAuthRewrite'].conditions.""[input='{HTTPS}s',pattern='on(s)|offs']"" " &_
			"/[name='SsoAuthRewrite'].conditions.trackAllCaptures:true " &_
			"/+[name='SsoAuthRewrite'].serverVariables.""[name='HTTP_X_REWRITER_URL',value='http{C:1}://{HTTP_HOST}',replace='false']"" " &_
			"/[name='SsoAuthRewrite'].action.url:http://localhost:" & Session.Property("SSO_AUTH_PORT") & "/sso/{R:1} " &_
			"/[name='SsoAuthRewrite'].action.type:Rewrite"

	objShell.Run appcmd & " set config """ & iisSiteName & """ /section:system.webServer/rewrite/rules " & rulesTemp,0,True
	
	Call WriteToLog("EnterpriseConfigure: Begin")
	Call WriteToLog("EnterpriseConfigure: APPDIR is " & Session.Property("APPDIR"))

    If (StrComp(Session.Property("DOCUMENT_SERVER_INSTALLED"),"FALSE") <> 0) Then
		    Call WriteToLog("EnterpriseConfigure: Property: DOCUMENT_SERVER_INSTALLED is " & Session.Property("DOCUMENT_SERVER_INSTALLED"))
		    Call WriteToLog("EnterpriseConfigure: Start setup document server....")
	
			Call WriteToLog("EnterpriseConfigure: Setup document server URLs")
			appSettingsTemp = "/[key='files.docservice.url.public'].value:/ds-vpath/ " &_
							  "/[key='files.docservice.url.internal'].value:http://localhost:" & Session.Property("DOCUMENT_SERVER_PORT") & " " &_
							  "/[key='files.docservice.url.portal'].value:http://localhost"

			objShell.Run appcmd & " set config """ & iisSiteName & """ /section:appSettings " & appSettingsTemp,0,True

			Call WriteToLog("EnterpriseConfigure: Setup document server rewrite rules")
			rulesTemp = "/+""[name='DocumentServerRewrite',enabled='True',stopProcessing='True']"" " &_
						"/[name='DocumentServerRewrite'].match.url:""^ds-vpath/(.*)"" " &_
						"/+[name='DocumentServerRewrite'].conditions.""[input='{HTTPS}s',pattern='on(s)|offs']"" " &_
						"/[name='DocumentServerRewrite'].conditions.trackAllCaptures:true " &_
						"/+[name='DocumentServerRewrite'].serverVariables.""[name='HTTP_X_FORWARDED_PROTO',value='{HTTP_THE_SCHEME}',replace='true']"" " &_
						"/+[name='DocumentServerRewrite'].serverVariables.""[name='HTTP_X_FORWARDED_HOST',value='{HTTP_THE_HOST}/ds-vpath',replace='true']"" " &_
						"/[name='DocumentServerRewrite'].action.url:http://localhost:" & Session.Property("DOCUMENT_SERVER_PORT") & "/{R:1} " &_
						"/[name='DocumentServerRewrite'].action.type:Rewrite"
						
			objShell.Run appcmd & " set config """ & iisSiteName & """ /section:system.webServer/rewrite/rules " & rulesTemp,0,True

			Call WriteToLog("EnterpriseConfigure: Setup document server for TeamLabSvc")
			teamlabSvcCfg = Session.Property("APPDIR") & "..\CommunityServer\Services\TeamLabSvc\TeamLabSvc.exe.config"

			editXML teamlabSvcCfg, "//appSettings/add[@key = 'files.docservice.url.public']    /@value", "/ds-vpath/"  
			editXML teamlabSvcCfg, "//appSettings/add[@key = 'files.docservice.url.internal']  /@value", "http://localhost:" & Session.Property("DOCUMENT_SERVER_PORT")
			editXML teamlabSvcCfg, "//appSettings/add[@key = 'files.docservice.url.portal']    /@value", "http://localhost"
			
			objShell.Run "net stop OnlyOfficeThumb"
			waitForServiceState "OnlyOfficeThumb", "Stopped"

			objShell.Run "net stop OnlyOfficeThumbnailBuilder"
			waitForServiceState "OnlyOfficeThumbnailBuilder", "Stopped"

			objShell.Run "net start OnlyOfficeThumb"
			waitForServiceState "OnlyOfficeThumb", "Running"

			objShell.Run "net start OnlyOfficeThumbnailBuilder"
			waitForServiceState "OnlyOfficeThumbnailBuilder", "Running"

			Call WriteToLog("EnterpriseConfigure: End setup document server....")	
    End If   
                
    If (StrComp(Session.Property("CONTROL_PANEL_INSTALLED"),"FALSE") <> 0) Then
	    Call WriteToLog("EnterpriseConfigure: CONTROL_PANEL_INSTALLED is " & Session.Property("CONTROL_PANEL_INSTALLED"))
	    Call WriteToLog("EnterpriseConfigure: Start setup control panel....")
			
		rulesTemp = "/+""[name='ControlPanelRewrite',enabled='True',stopProcessing='True']"" " &_
					"/[name='ControlPanelRewrite'].match.url:""^controlpanel(.*)"" " &_
					"/+[name='ControlPanelRewrite'].conditions.""[input='{HTTPS}s',pattern='on(s)|offs']"" " &_
					"/[name='ControlPanelRewrite'].conditions.trackAllCaptures:true " &_
					"/+[name='ControlPanelRewrite'].serverVariables.""[name='HTTP_X_REWRITER_URL',value='http{C:1}://{HTTP_HOST}',replace='false']"" " &_
					"/[name='ControlPanelRewrite'].action.url:http://localhost:" & Session.Property("CONTROL_PANEL_PORT") & "/controlpanel{R:1} " &_
					"/[name='ControlPanelRewrite'].action.type:Rewrite"

		objShell.Run appcmd & " set config """ & iisSiteName & """ /section:system.webServer/rewrite/rules " & rulesTemp,0,True
		
	    Call WriteToLog("EnterpriseConfigure: End setup control panel....")			
    End If
    
    If ((StrComp(Session.Property("XMPPSERVER_INSTALLED"),"FALSE") <> 0) Or (StrComp(Session.Property("INSTALLATION_TYPE"),"Groups") <> 0)) Then
	    Call WriteToLog("EnterpriseConfigure: Start setup xmpp server....")

        Call WriteToLog("EnterpriseConfigure: XMPPSERVER_INSTALLED is " & Session.Property("XMPPSERVER_INSTALLED"))
        Call WriteToLog("EnterpriseConfigure: INSTALLATION_TYPE is " & Session.Property("INSTALLATION_TYPE"))
		
		appSettingsTemp = "/[key='web.talk'].value:true"

		objShell.Run appcmd & " set config """ & iisSiteName & """ /section:appSettings " & appSettingsTemp,0,True
		
		Call WriteToLog("EnterpriseConfigure: End setup xmpp server....")			
    End If   

	Call WriteToLog("EnterpriseConfigure: End")
End Function


Sub WriteIni( myFilePath, mySection, myKey, myValue )
    ' This subroutine writes a value to an INI file
    '
    ' Arguments:
    ' myFilePath  [string]  the (path and) file name of the INI file
    ' mySection   [string]  the section in the INI file to be searched
    ' myKey       [string]  the key whose value is to be written
    ' myValue     [string]  the value to be written (myKey will be
    '                       deleted if myValue is <DELETE_THIS_VALUE>)
    '
    ' Returns:
    ' N/A
    '
    ' CAVEAT:     WriteIni function needs ReadIni function to run
    '
    ' Written by Keith Lacelle
    ' Modified by Denis St-Pierre, Johan Pol and Rob van der Woude

    Const ForReading   = 1
    Const ForWriting   = 2
    Const ForAppending = 8

    Dim blnInSection, blnKeyExists, blnSectionExists, blnWritten
    Dim intEqualPos
    Dim objFSO, objNewIni, objOrgIni, wshShell
    Dim strFilePath, strFolderPath, strKey, strLeftString
    Dim strLine, strSection, strTempDir, strTempFile, strValue

    strFilePath = Trim( myFilePath )
    strSection  = Trim( mySection )
    strKey      = Trim( myKey )
    strValue    = Trim( myValue )

    Set objFSO   = CreateObject( "Scripting.FileSystemObject" )
    Set wshShell = CreateObject( "WScript.Shell" )

    strTempDir  = wshShell.ExpandEnvironmentStrings( "%TEMP%" )
    strTempFile = objFSO.BuildPath( strTempDir, objFSO.GetTempName )

    Set objOrgIni = objFSO.OpenTextFile( strFilePath, ForReading, True )
    Set objNewIni = objFSO.CreateTextFile( strTempFile, False, False )

    blnInSection     = False
    blnSectionExists = False
    ' Check if the specified key already exists
    blnKeyExists     = ( ReadIni( strFilePath, strSection, strKey ) <> "" )
    blnWritten       = False

    ' Check if path to INI file exists, quit if not
    strFolderPath = Mid( strFilePath, 1, InStrRev( strFilePath, "\" ) )
    If Not objFSO.FolderExists ( strFolderPath ) Then
        WScript.Echo "Error: WriteIni failed, folder path (" _
                   & strFolderPath & ") to ini file " _
                   & strFilePath & " not found!"
        Set objOrgIni = Nothing
        Set objNewIni = Nothing
        Set objFSO    = Nothing
        WScript.Quit 1
    End If

    While objOrgIni.AtEndOfStream = False
        strLine = Trim( objOrgIni.ReadLine )
        If blnWritten = False Then
            If LCase( strLine ) = "[" & LCase( strSection ) & "]" Then
                blnSectionExists = True
                blnInSection = True
            ElseIf InStr( strLine, "[" ) = 1 Then
                blnInSection = False
            End If
        End If

        If blnInSection Then
            If blnKeyExists Then
                intEqualPos = InStr( 1, strLine, "=", vbTextCompare )
                If intEqualPos > 0 Then
                    strLeftString = Trim( Left( strLine, intEqualPos - 1 ) )
                    If LCase( strLeftString ) = LCase( strKey ) Then
                        ' Only write the key if the value isn't empty
                        ' Modification by Johan Pol
                        If strValue <> "<DELETE_THIS_VALUE>" Then
                            objNewIni.WriteLine strKey & "=" & strValue
                        End If
                        blnWritten   = True
                        blnInSection = False
                    End If
                End If
                If Not blnWritten Then
                    objNewIni.WriteLine strLine
                End If
            Else
                objNewIni.WriteLine strLine
                    ' Only write the key if the value isn't empty
                    ' Modification by Johan Pol
                    If strValue <> "<DELETE_THIS_VALUE>" Then
                        objNewIni.WriteLine strKey & "=" & strValue
                    End If
                blnWritten   = True
                blnInSection = False
            End If
        Else
            objNewIni.WriteLine strLine
        End If
    Wend

    If blnSectionExists = False Then ' section doesn't exist
        objNewIni.WriteLine
        objNewIni.WriteLine "[" & strSection & "]"
            ' Only write the key if the value isn't empty
            ' Modification by Johan Pol
            If strValue <> "<DELETE_THIS_VALUE>" Then
                objNewIni.WriteLine strKey & "=" & strValue
            End If
    End If

    objOrgIni.Close
    objNewIni.Close

    ' Delete old INI file
    objFSO.DeleteFile strFilePath, True
    ' Rename new INI file
    objFSO.CopyFile strTempFile, strFilePath, True

    objFSO.DeleteFile strTempFile, True

    Set objOrgIni = Nothing
    Set objNewIni = Nothing
    Set objFSO    = Nothing
    Set wshShell  = Nothing
End Sub

