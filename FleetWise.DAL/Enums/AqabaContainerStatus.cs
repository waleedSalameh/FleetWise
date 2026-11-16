using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Enums
{
    public enum AqabaContainerStatus
    {
        Released = 1,        // خروج
        Inspection = 2,      // معاينة
        AmmanCustoms = 3,    // جمرك عمان
        AqabaCustoms = 4,    // جمرك العقبة
        FreeZone = 5,            // حرة
        InspectionReport = 6 // كشف
    }
}
