
using System;
using System.IO;

using Microsoft.AspNetCore.Http;

namespace ASC.Files.Core.Model
{
    public class InsertFileModel : IModelWithFile, IDisposable
    {
        public IFormFile File { get; set; }
        public string Title { get; set; }
        public bool? CreateNewIfExist { get; set; }
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

        ~InsertFileModel()
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
