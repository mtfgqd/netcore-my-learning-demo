using System;
using System.Collections.Generic;

namespace CoreDemo
{
    public partial class MessageQueue
    {
        public Guid MessageId { get; set; }
        public string QueueName { get; set; }
        public int Priority { get; set; }
        public DateTime DateActive { get; set; }
        public bool IsActive { get; set; }
        public byte[] MessageBody { get; set; }
        public DateTime DateCreated { get; set; }
        public long Sequence { get; set; }
        public Guid? RequestId { get; set; }
        public DateTime? DateRequestExpires { get; set; }
        public Guid? CorrelationId { get; set; }
        public string LookupField1 { get; set; }
        public string LookupField2 { get; set; }
        public string LookupField3 { get; set; }
    }
}
