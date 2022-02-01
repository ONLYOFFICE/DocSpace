using System;

namespace ASC.Files.Core.Model
{
    public class CheckFillFormDraftModel
    {
        public int Version { get; set; }
        public string Doc { get; set; }
        public string Action { get; set; }

        public bool RequestView
        {
            get { return (Action ?? "").Equals("view", StringComparison.InvariantCultureIgnoreCase); }
        }

        public bool RequestEmbedded
        {
            get
            {
                return
                    (Action ?? "").Equals("embedded", StringComparison.InvariantCultureIgnoreCase)
                    && !string.IsNullOrEmpty(Doc);
            }
        }
    }
}
