namespace CarAuction.Business.Core
{
    // This could be transformed to a table, if we want to be dynamic
    // Since there was a requirement for a very specific subset of vehicle types, a enum was the best approach
    public enum VehicleType
    {
        Sedan,
        SUV,
        Hatchback,
        Truck
    }
}