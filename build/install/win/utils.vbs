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
            shell.Run """" & installDir & "bin\mysql"" -u root -p" & dbpass & " -e ""ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY " & "'" & dbpass & "';""", 0, true	
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

    APP_INDEX_DIR = Session.Property("APPDIR") & "Data\Index\v7.13.1\"  
   
    If Not fso.FolderExists(APP_INDEX_DIR) Then
        Session.Property("NEED_REINDEX_ELASTICSEARCH") = "TRUE"
    End If
    
    Call Shell.Run("%COMSPEC% /c mkdir """ & Session.Property("APPDIR") & "Data\Index\v7.13.1\""",0,true)
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
    fileContent = oRE.Replace(fileContent, "path.data: " & Session.Property("APPDIR") & "Data\Index\v7.13.1\")                           

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

    ShellInstallCommand = """C:\Program Files\Elastic\Elasticsearch\7.13.1\bin\elasticsearch-plugin""" & " install -b -s ingest-attachment"""
    ShellRemoveCommand = """C:\Program Files\Elastic\Elasticsearch\7.13.1\bin\elasticsearch-plugin""" & " remove -s ingest-attachment"""
     
    Call Shell.Run("cmd /C " & """" & ShellRemoveCommand  & """",0,true)
    Call Shell.Run("cmd /C " & """" & ShellInstallCommand  & """",0,true)

    Set Shell = Nothing
    
End Function