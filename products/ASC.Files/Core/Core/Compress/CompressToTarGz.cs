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
    /// Archives the data stream into the format .tar.gz
    /// </summary>
    [Scope]
    public class CompressToTarGz : ICompress
    {
        private GZipOutputStream gzoStream;
        private TarOutputStream gzip;
        private TarEntry tarEntry;
        private TempStream TempStream { get; }

        public CompressToTarGz(TempStream tempStream)
        {
            TempStream = tempStream;
        }

        /// <summary></summary>
        /// <param name="stream">Accepts a new stream, it will contain an archive upon completion of work</param>
        public void SetStream(Stream stream)
        {
            gzoStream = new GZipOutputStream(stream) { IsStreamOwner = false };
            gzip = new TarOutputStream(gzoStream, Encoding.UTF8);
            gzoStream.IsStreamOwner = false;
        }

        /// <summary>
        /// The record name is created (the name of a separate file in the archive)
        /// </summary>
        /// <param name="title">File name with extension, this name will have the file in the archive</param>
        public void CreateEntry(string title)
        {
            tarEntry = TarEntry.CreateTarEntry(title);
        }

        /// <summary>
        /// Transfer the file itself to the archive
        /// </summary>
        /// <param name="readStream">File data</param>
        public void PutStream(Stream readStream)
        {
            using (var buffered = TempStream.GetBuffered(readStream))
            {
                tarEntry.Size = buffered.Length;
                gzip.PutNextEntry(tarEntry);
                buffered.CopyTo(gzip);
            }
        }

        /// <summary>
        /// Put an entry on the output stream.
        /// </summary>
        public void PutNextEntry()
        {
            gzip.PutNextEntry(tarEntry);
        }

        /// <summary>
        /// Closes the current entry.
        /// </summary>
        public void CloseEntry()
        {
            gzip.CloseEntry();
        }

        /// <summary>
        /// Resource title (does not affect the work of the class)
        /// </summary>
        /// <returns></returns>
        public string Title => FilesUCResource.FilesWillBeCompressedTarGz;

        /// <summary>
        /// Extension the archive (does not affect the work of the class)
        /// </summary>
        /// <returns></returns>
        public string ArchiveExtension => CompressToArchive.TarExt;

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            gzip?.Dispose();
            gzoStream?.Dispose();
        }
    }
}