Function RedisSetup
    On Error Resume Next
 
    Dim Shell

    Set Shell = CreateObject("WScript.Shell")
        
    Shell.Run "redis-cli config set save """"", 0, True
    Shell.Run "redis-cli config rewrite", 0, True
    
    Set Shell = Nothing
End Function

Function TestPostgreSqlConnection
    On Error Resume Next

    Dim ErrorText
    Dim Pos, postgreSqlDriver

    postgreSqlDriver = "PostgreSQL Unicode(x64)"

    Session.Property("PostgreSqlConnectionError") = ""

    Set ConnectionObject = CreateObject("ADODB.Connection")
    ConnectionObject.Open "Driver={" & postgreSqlDriver & "};" & _
                          "Server=" & Session.Property("PS_DB_HOST") & ";" & _
                          "Port=" & Session.Property("PS_DB_PORT")  & ";" & _
                          "Database=" & Session.Property("PS_DB_NAME") & ";" & _
                          "Uid=" & Session.Property("PS_DB_USER") & ";" & _
                          "Pwd=" & Session.Property("PS_DB_PWD")
    
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

Function PostgreSqlConfigure
    On Error Resume Next
    
   If (StrComp(Session.Property("POSTGRE_SQL_PATH"),"FALSE") = 0) Then
            Wscript.Quit
    End If

    Dim ErrorText
    Dim Pos, postgreSqlDriver
    Dim databaseUserName
    Dim databaseUserPwd
    Dim databaseName
    Dim databasePort
    Dim databaseHost

    databaseUserName = Session.Property("PS_DB_USER")
    databaseUserPwd = Session.Property("PS_DB_PWD")
    databaseName = Session.Property("PS_DB_NAME")
    databasePort = Session.Property("PS_DB_PORT")
    databaseHost = Session.Property("PS_DB_HOST")

    Call WriteToLog("PostgreSqlConfig: databaseUserName is " & databaseUserName)
    Call WriteToLog("PostgreSqlConfig: databaseUserPwd is " & databaseUserPwd)
    Call WriteToLog("PostgreSqlConfig: databaseName is " & databaseName)
    Call WriteToLog("PostgreSqlConfig: databasePort is " & databasePort)
    Call WriteToLog("PostgreSqlConfig: databaseHost is " & databaseHost)

    postgreSqlDriver = "PostgreSQL Unicode(x64)"

    Set ConnectionObject = CreateObject("ADODB.Connection")
    ConnectionObject.Open "Driver={" & postgreSqlDriver & "};Server=" & databaseHost & ";Port=" & databasePort & ";Database=" & "postgres" & ";Uid=" & "postgres" & ";Pwd=" & "postgres"
            
    ConnectionObject.Execute "CREATE DATABASE " & databaseName
    ConnectionObject.Execute "create user " & databaseUserName & " with encrypted password '" & databaseUserPwd & "'" 
    ConnectionObject.Execute "grant all privileges on database " & databaseName & " to " & databaseUserName
    
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

    Session.Property("JWT_SECRET") = RandomString( 30 )

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
    dbname = Session.Property("DB_NAME")
    dbpass = Session.Property("DB_PWD")
	
    Err.Clear
    installDir = shell.RegRead("HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\MySQL AB\MySQL Server 8.0\Location")
    dataDir = shell.RegRead("HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\MySQL AB\MySQL Server 8.0\DataLocation")

    Call WriteToLog("MySQLConfigure: installDir " & installDir)
    Call WriteToLog("MySQLConfigure: dataDir " & dataDir)

	    
	If Err.Number <> 0 Then
        Err.Clear
        installDir = shell.RegRead("HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\MySQL AB\MySQL Server 8.0\Location")
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
            shell.Run """" & installDir & "bin\mysql"" -u root -p" & dbpass & " -e ""ALTER USER 'root'@'localhost' IDENTIFIED BY " & "'" & dbpass & "';""", 0, true	
        End If        
		
        Set filesys = CreateObject("Scripting.FileSystemObject")

		WriteIni filesys.BuildPath(dataDir, "my.ini"), "mysqld", "sql-mode", "NO_ENGINE_SUBSTITUTION"
		WriteIni filesys.BuildPath(dataDir, "my.ini"), "mysqld", "max_connections", "1000"
		WriteIni filesys.BuildPath(dataDir, "my.ini"), "mysqld", "max_allowed_packet", "1048576000"
        WriteIni filesys.BuildPath(dataDir, "my.ini"), "mysqld", "group_concat_max_len", "2048"
        WriteIni filesys.BuildPath(dataDir, "my.ini"), "mysqld", "character_set_server", "utf8"
        WriteIni filesys.BuildPath(dataDir, "my.ini"), "mysqld", "collation_server", "utf8_general_ci"
		
	    Call WriteToLog("MySQLConfigure: WriteIni Path" & filesys.BuildPath(dataDir, "my.ini"))

    End If
End Function

Function WriteToLog(ByVal var)

    Const MsgType = &H04000000
    Set rec = Installer.CreateRecord(1)

    rec.StringData(1) = CStr(var)
    Session.Message MsgType, rec
    WriteToLog = 0

End Function

Function ElasticSearchSetup
    On Error Resume Next
    
    Dim ShellCommand
    Dim APP_INDEX_DIR
    
    Const ForReading = 1
    Const ForWriting = 2
    
    Set Shell = CreateObject("WScript.Shell")
    Set objFSO = CreateObject("Scripting.FileSystemObject")

    APP_INDEX_DIR = Session.Property("APPDIR") & "Data\Index\v7.10.0\"
   
    If Not fso.FolderExists(APP_INDEX_DIR) Then
        Session.Property("NEED_REINDEX_ELASTICSEARCH") = "TRUE"
    End If
    
    Call Shell.Run("%COMSPEC% /c mkdir """ & Session.Property("APPDIR") & "Data\Index\v7.10.0\""",0,true)
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
    fileContent = oRE.Replace(fileContent, "path.data: " & Session.Property("APPDIR") & "Data\Index\v7.10.0\")

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
    
    If InStrRev(fileContent, "-Xms") <> 0 Then
       oRE.Pattern = "-Xms.*"
       fileContent = oRE.Replace(fileContent, "-Xms4g")                           
    ElseIf InStrRev(fileContent, "-Xms4g") <> 0 Then
       fileContent = fileContent & Chr(13) & Chr(10) & "-Xms4g"
    End if

    If InStrRev(fileContent, "-Xmx") <> 0 Then
       oRE.Pattern = "-Xmx.*"
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

    ShellInstallCommand = """C:\Program Files\Elastic\Elasticsearch\7.10.0\bin\elasticsearch-plugin""" & " install -b -s ingest-attachment"""
    ShellRemoveCommand = """C:\Program Files\Elastic\Elasticsearch\7.10.0\bin\elasticsearch-plugin""" & " remove -s ingest-attachment"""
     
    Call Shell.Run("cmd /C " & """" & ShellRemoveCommand  & """",0,true)
    Call Shell.Run("cmd /C " & """" & ShellInstallCommand  & """",0,true)

    Set Shell = Nothing
    
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
    ConnectionObject.Open "Driver={" & mysqlDriver & "};Server=" & Session.Property("DB_HOST") & ";Port=" & Session.Property("DB_PORT") & ";Uid=" & Session.Property("DB_USER") & ";Pwd=" & Session.Property("DB_PWD")
    
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
