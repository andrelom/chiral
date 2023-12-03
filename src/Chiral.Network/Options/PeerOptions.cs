using System.ComponentModel.DataAnnotations;
using Chiral.Core.Attributes;

namespace Chiral.Network.Options;

[Option("Peer")]
public class PeerOptions
{
    [Required]
    public string Host { get; set; } = "::1";

    [Required]
    public int Port { get; set; } = 1984;
}
