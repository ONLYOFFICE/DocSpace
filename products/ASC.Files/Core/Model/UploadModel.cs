using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;

using ASC.Files.Core.Model;

using Microsoft.AspNetCore.Http;

namespace ASC.Files.Model
{
    public class UploadModel : IModelWithFile, IDisposable
    {
        public IFormFile File { get; set; }
        public ContentType ContentType { get; set; }
        public ContentDisposition ContentDisposition { get; set; }
        public IEnumerable<IFormFile> Files { get; set; }
        public bool? CreateNewIfExist { get; set; }
        public bool? StoreOriginalFileFlag { get; set; }
        public bool KeepConvertStatus { get; set; }

        private Stream stream;
        private bool disposedValue;

        public Stream Stream
        {
            get => File?.OpenReadStream() ?? stream;
            set => stream = value;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing && stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                    stream = null;
                }

                disposedValue = true;
            }
        }

        ~UploadModel()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
