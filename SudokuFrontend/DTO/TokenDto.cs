using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects {
    public record TokenDto {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public bool IsAuthSuccessful { get; set; }
    }
}
