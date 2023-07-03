﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects {
    public record HighscoreDto {
        public string Nickname { get; init; }
        public uint Score { get; init; }
        public DateTime LastPlayedDate { get; init; }
    }
}
