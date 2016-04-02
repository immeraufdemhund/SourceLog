using System;
using System.Data.SqlTypes;

namespace SourceLog.Core
{
    public static class Extensions
    {
        public static DateTime PrecisionFix(this DateTime dateTime)
        {
            return new SqlDateTime(dateTime).Value;
        }
    }
}
