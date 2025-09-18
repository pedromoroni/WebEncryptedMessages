using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMessages.Models.DTOs.UserDevice;

namespace WebMessages.Services.Interfaces;

public interface IDeviceService
{
    Task RegisterDeviceAsync(UserDeviceRequest userDeviceRequest);
}
