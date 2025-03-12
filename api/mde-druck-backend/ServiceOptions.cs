using System.ComponentModel.DataAnnotations;

namespace Denner.MDEDruck
{
    public class ServiceOptions
    {
        [Required]
        public required string PrintBaseAddress { get; set; }
        [Required]
        public required string PrintUserName { get; set; }
        [Required]
        public required string PrintPassword { get; set; }
        [Required]
        public required string TCPOSBaseAddress { get; set; }
        [Required]
        public required string TCPOSUserName { get; set; }
        [Required]
        public required string TCPOSPassword { get; set; }

    }
}
