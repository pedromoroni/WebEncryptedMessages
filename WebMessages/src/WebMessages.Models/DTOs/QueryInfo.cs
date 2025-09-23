using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMessages.Models.DTOs;

public class QueryInfo
{
    public int PageSize { get; set; } = 1;
    public int PageNumber { get; set; } = 1;
}

// veficar se sao positivos