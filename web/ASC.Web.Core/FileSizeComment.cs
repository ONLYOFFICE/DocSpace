namespace ASC.Web.Studio.Core
{
    [Scope]
    public class FileSizeComment
    {
        private TenantExtra TenantExtra { get; }
        private SetupInfo SetupInfo { get; }

        public FileSizeComment(TenantExtra tenantExtra, SetupInfo setupInfo)
        {
            TenantExtra = tenantExtra;
            SetupInfo = setupInfo;
        }

        public string FileSizeExceptionString
        {
            get { return GetFileSizeExceptionString(TenantExtra.MaxUploadSize); }
        }

        public string FileImageSizeExceptionString
        {
            get { return GetFileSizeExceptionString(SetupInfo.MaxImageUploadSize); }
        }

        public static string GetFileSizeExceptionString(long size)
        {
            return $"{Resource.FileSizeMaxExceed} ({FilesSizeToString(size)}).";
        }

        public static string GetPersonalFreeSpaceExceptionString(long size)
        {
            return $"{Resource.PersonalFreeSpaceException} ({FilesSizeToString(size)}).";
        }

        /// <summary>
        /// The maximum file size is exceeded (25 MB).
        /// </summary>
        public Exception FileSizeException
        {
            get { return new TenantQuotaException(FileSizeExceptionString); }
        }

        /// <summary>
        /// The maximum file size is exceeded (1 MB).
        /// </summary>
        public Exception FileImageSizeException
        {
            get { return new TenantQuotaException(FileImageSizeExceptionString); }
        }

        public static Exception GetFileSizeException(long size)
        {
            return new TenantQuotaException(GetFileSizeExceptionString(size));
        }

        public static Exception GetPersonalFreeSpaceException(long size)
        {
            return new TenantQuotaException(GetPersonalFreeSpaceExceptionString(size));
        }

        /// <summary>
        /// Get note about maximum file size
        /// </summary>
        /// <returns>Note: the file size cannot exceed 25 MB</returns>
        public string GetFileSizeNote()
        {
            return GetFileSizeNote(true);
        }

        /// <summary>
        /// Get note about maximum file size
        /// </summary>
        /// <param name="withHtmlStrong">Highlight a word about size</param>
        /// <returns>Note: the file size cannot exceed 25 MB</returns>
        public string GetFileSizeNote(bool withHtmlStrong)
        {
            return GetFileSizeNote(Resource.FileSizeNote, withHtmlStrong);
        }

        /// <summary>
        /// Get note about maximum file size
        /// </summary>
        /// <param name="note">Resource fromat of note</param>
        /// <param name="withHtmlStrong">Highlight a word about size</param>
        /// <returns>Note: the file size cannot exceed 25 MB</returns>
        public string GetFileSizeNote(string note, bool withHtmlStrong)
        {
            return
                string.Format(note,
                              FilesSizeToString(TenantExtra.MaxUploadSize),
                              withHtmlStrong ? "<strong>" : string.Empty,
                              withHtmlStrong ? "</strong>" : string.Empty);
        }

        /// <summary>
        /// Get note about maximum file size of image
        /// </summary>
        /// <param name="note">Resource fromat of note</param>
        /// <param name="withHtmlStrong">Highlight a word about size</param>
        /// <returns>Note: the file size cannot exceed 1 MB</returns>
        public string GetFileImageSizeNote(string note, bool withHtmlStrong)
        {
            return
                string.Format(note,
                              FilesSizeToString(SetupInfo.MaxImageUploadSize),
                              withHtmlStrong ? "<strong>" : string.Empty,
                              withHtmlStrong ? "</strong>" : string.Empty);
        }

        /// <summary>
        /// Generates a string the file size
        /// </summary>
        /// <param name="size">Size in bytes</param>
        /// <returns>10 b, 100 Kb, 25 Mb, 1 Gb</returns>
        public static string FilesSizeToString(long size)
        {
            var sizeNames = !string.IsNullOrEmpty(Resource.FileSizePostfix) ? Resource.FileSizePostfix.Split(',') : new[] { "bytes", "KB", "MB", "GB", "TB" };
            var power = 0;

            double resultSize = size;
            if (1024 <= resultSize)
            {
                power = (int)Math.Log(resultSize, 1024);
                power = power < sizeNames.Length ? power : sizeNames.Length - 1;
                resultSize /= Math.Pow(1024d, power);
            }
            return string.Format("{0:#,0.##} {1}", resultSize, sizeNames[power]);
        }
    }
}