namespace Skeleton.Domain.Eunm;

public enum NotificationType
{
    OrderCreated     = 0,
    OrderConfirmed   = 1,
    OrderShipped     = 2,
    OrderDelivered   = 3,
    OrderCancelled   = 4,
    PaymentReceived  = 5,
    PaymentFailed    = 6,
    PaymentRefunded  = 7,
    AccountDebit     = 8,
    AccountPayment   = 9,
    PasswordChanged  = 10,
    EmailVerified    = 11,
    LowStock         = 12,
    SystemAlert      = 13
}
