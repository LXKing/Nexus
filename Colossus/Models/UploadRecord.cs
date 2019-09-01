﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Colossus.Models
{
    [Obsolete]
    public class UploadRecord
    {
        public int Id { get; set; }
        public string UploaderId { get; set; }
        [ForeignKey(nameof(UploaderId))]
        public ColossusUser Uploader { get; set; }
        public string SourceFileName { get; set; }
        public int FileId { get; set; }
        public DateTime UploadTime { get; set; } = DateTime.UtcNow;
    }
}
