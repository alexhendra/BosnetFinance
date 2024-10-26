using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BosnetFinance.Core
{
    public enum ErrorCode
    {
        WARNING = 400,
        ERROR = 500,
        CREATED = 201,
        SUCCEEDED = 200,
        NOTFOUND = 404,
    }
}