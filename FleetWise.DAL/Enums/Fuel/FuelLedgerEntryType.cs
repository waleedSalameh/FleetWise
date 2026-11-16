namespace FleetWise.DAL.Enums.Fuel;

public enum FuelLedgerEntryType
{
    OpeningMeasurement = 1,
    Fueling = 2,
    TransferIn = 3,
    TransferOut = 4,
    MonthlyClosing = 5,
    Correction = 6,
    Reversal = 7
}
