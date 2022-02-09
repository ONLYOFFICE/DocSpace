﻿namespace ASC.Core.Common.EF.Model
{
    public class MessageEvent
    {
        public int Id { get; set; }
        public string Ip { get; set; }
        public string Browser { get; set; }
        public string Platform { get; set; }
        public DateTime Date { get; set; }
        public int TenantId { get; set; }
        public Guid UserId { get; set; }
        public string Page { get; set; }
        public int Action { get; set; }
        public string Description { get; set; }
    }
}
