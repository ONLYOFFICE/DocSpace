
using System;
using System.IO;
using System.Linq;

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

        public static InsertFileModel FromQuery(HttpContext httpContext, InsertFileModel model)
        {
            if (model.File != null) return model;

            var result = new InsertFileModel();
            var query = httpContext.Request.Query;

            var Title = query["Title"];
            if (Title.Any())
            {
                result.Title = Title.First();
            }

            var CreateNewIfExist = query["CreateNewIfExist"];
            if (CreateNewIfExist.Any())
            {
                if (bool.TryParse(CreateNewIfExist.First(), out var d))
                {
                    result.CreateNewIfExist = d;
                }
            }

            var KeepConvertStatus = query["KeepConvertStatus"];
            if (KeepConvertStatus.Any())
            {
                if (bool.TryParse(KeepConvertStatus.First(), out var d))
                {
                    result.KeepConvertStatus = d;
                }
            }

            httpContext.Request.EnableBuffering();

            httpContext.Request.Body.Position = 0;

            result.stream = new MemoryStream();
            httpContext.Request.Body.CopyToAsync(result.stream).Wait();
            result.stream.Position = 0;

            return result;
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
