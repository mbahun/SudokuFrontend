using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects {
    public record ResponseDto {
        public bool Success { get; set; }
        public string? Error { get; set; }
    }
}
