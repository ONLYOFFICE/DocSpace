﻿/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

namespace System.IO;

[Singletone]
public class TempPath
{
    private readonly string _tempFolder;

    public TempPath(IConfiguration configuration)
    {
        var rootFolder = AppContext.BaseDirectory;
        if (string.IsNullOrEmpty(rootFolder))
        {
            rootFolder = Assembly.GetEntryAssembly().Location;
        }

        _tempFolder = configuration["temp"] ?? Path.Combine("..", "Data", "temp");
        if (!Path.IsPathRooted(_tempFolder))
        {
            _tempFolder = Path.GetFullPath(Path.Combine(rootFolder, _tempFolder));
        }

        if (!Directory.Exists(_tempFolder))
        {
            Directory.CreateDirectory(_tempFolder);
        }
    }

    public string GetTempPath()
    {
        return _tempFolder;
    }

    public string GetTempFileName()
    {
        FileStream f = null;
        string path;
        var count = 0;

        do
        {
            path = Path.Combine(_tempFolder, Path.GetRandomFileName());

            try
            {
                using (f = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
                {
                    return path;
                }
            }
            catch (IOException ex)
            {
                if (ex.HResult != -2147024816 || count++ > 65536)
                {
                    throw;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                if (count++ > 65536)
                {
                    throw new IOException(ex.Message, ex);
                }
            }
        } while (f == null);

        return path;
    }
}