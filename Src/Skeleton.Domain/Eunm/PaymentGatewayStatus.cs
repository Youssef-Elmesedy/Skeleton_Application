namespace Skeleton.Domain.Eunm;

public enum PaymentGatewayStatus
{
    Initiated  = 0,   // Gateway call started
    Pending    = 1,   // Awaiting user action / webhook
    Authorized = 2,   // Authorized but not captured
    Captured   = 3,   // Money captured (success)
    Declined   = 4,   // Gateway declined
    TimedOut   = 5,   // No response
    Cancelled  = 6,   // Cancelled by user
    Refunded   = 7
}
