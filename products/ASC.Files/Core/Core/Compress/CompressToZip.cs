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

namespace ASC.Web.Files.Core.Compress
{
    /// <summary>
    /// Archives the data stream into the format .zip
    /// </summary>
    [Scope]
    public class CompressToZip : ICompress
    {
        private ZipOutputStream zipStream;
        private ZipEntry zipEntry;
        private TempStream TempStream { get; }

        public CompressToZip(TempStream tempStream)
        {
            TempStream = tempStream;
        }

        /// <summary> </summary>
        /// <param name="stream">Accepts a new stream, it will contain an archive upon completion of work</param>
        public void SetStream(Stream stream)
        {
            zipStream = new ZipOutputStream(stream) { UseZip64 = UseZip64.Dynamic };
            zipStream.IsStreamOwner = false;
        }

        /// <summary>
        /// The record name is created (the name of a separate file in the archive)
        /// </summary>
        /// <param name="title">File name with extension, this name will have the file in the archive</param>
        public void CreateEntry(string title)
        {
            zipEntry = new ZipEntry(title) { IsUnicodeText = true };
        }

        /// <summary>
        /// Transfer the file itself to the archive
        /// </summary>
        /// <param name="readStream">File data</param>
        public void PutStream(Stream readStream)
        {
            using (var buffered = TempStream.GetBuffered(readStream))
            {
                zipEntry.Size = buffered.Length;
                zipStream.PutNextEntry(zipEntry);
                buffered.CopyTo(zipStream);
            }
        }

        /// <summary>
        /// Put an entry on the output stream.
        /// </summary>
        public void PutNextEntry()
        {
            zipStream.PutNextEntry(zipEntry);
        }

        /// <summary>
        /// Closes the current entry.
        /// </summary>
        public void CloseEntry()
        {
            zipStream.CloseEntry();
        }

        /// <summary>
        /// Resource title (does not affect the work of the class)
        /// </summary>
        /// <returns></returns>
        public string Title => FilesUCResource.FilesWillBeCompressedZip;

        /// <summary>
        /// Extension the archive (does not affect the work of the class)
        /// </summary>
        /// <returns></returns>
        public string ArchiveExtension => CompressToArchive.ZipExt;

        public void Dispose()
        {
            zipStream?.Dispose();
        }
    }
}