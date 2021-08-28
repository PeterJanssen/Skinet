using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.LogModels
{
    public class Log : BaseModel
    {
        [Column(name: "message")]
        public string Message { get; set; }
        [Column(name: "message_template")]
        public string MessageTemplate { get; set; }
        [Column(name: "level")]
        public string Level { get; set; }
        [Column(name: "timestamp")]
        public DateTime TimeStamp { get; set; }
        [Column(name: "exception")]
        public string Exception { get; set; }
        [Column(name: "log_event", TypeName = "jsonb")]
        public string LogEvent { get; set; }

    }
}