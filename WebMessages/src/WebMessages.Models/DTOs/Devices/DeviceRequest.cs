using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMessages.Models.DTOs.Devices;

public class DeviceRequest
{
    public string? Name { get; set; }
    public byte[] PublicKey { get; set; }
}
