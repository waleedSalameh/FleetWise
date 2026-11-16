using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Enums
{
    public enum InventoryMovementType
    {
        Receipt = 0,       // استلام من المورد
        Issue = 1,         // صرف للمركبة أو الصيانة
        Return = 2,        // إرجاع إلى المستودع
        Adjustment = 3     // تسوية المخزون
    }
}
