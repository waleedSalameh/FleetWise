using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Enums
{
    public enum BillStatus
    {
        New = 1,         // جديدة
        InProgress = 2,  // قيد التنفيذ
        Completed = 3,   // مكتملة
        Cancelled = 4    // ملغاة
    }
}
