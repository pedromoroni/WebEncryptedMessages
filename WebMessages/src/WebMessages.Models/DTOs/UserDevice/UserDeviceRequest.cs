using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMessages.Models.DTOs.Users;
using WebMessages.Models.Entities;
using WebMessages.Models.DTOs.Devices;

namespace WebMessages.Models.DTOs.UserDevice;

public class UserDeviceRequest
{
    public UserCredentialsRequest User { get; set; }
    public DeviceRequest Device { get; set; } 
}
