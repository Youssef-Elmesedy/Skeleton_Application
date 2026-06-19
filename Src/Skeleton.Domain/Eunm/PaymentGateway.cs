namespace Skeleton.Domain.Eunm;

public enum PaymentGateway
{
    Cash        = 0,   // Cash in store / hand
    Stripe      = 1,   // Card online
    PayPal      = 2,   // PayPal
    Fawry       = 3,   // Fawry (Egypt)
    Vodafone    = 4,   // Vodafone Cash
    InstaPay    = 5,   // InstaPay (Egypt)
    BankTransfer= 6,   // Direct bank
    Installment = 7    // Deferred installment plan
}
