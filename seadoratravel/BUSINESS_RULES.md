# Seadora Travel: Business Rules & Policies

## Status: DRAFT (Pending Activation)
*These rules are structurally prepared but not currently enforced by the backend systems.*

### 1. Payment Methods & Deadlines

#### 1.1 Cash Payments
*   **Rule**: Reservations marked for "Cash" payment must be confirmed/paid at least **48 hours** prior to the scheduled `BookingDate`.
*   **Consequence**: If confirmation is not received within the 48-hour window, the system should automatically transition the booking status to `Cancelled`.

#### 1.2 Online Payments (e.g., Stripe/PayPal)
*   **Rule**: Online payments require immediate capture at booking or via a secure link.

### 2. Cancellation & Refund Policy (Online Payments)

If a customer cancels an online-paid reservation, the refund amount is dictated by the time remaining until the scheduled `BookingDate`:

*   **Tier 1: Free Cancellation**
    *   **Window**: More than **72 hours** before the `BookingDate`.
    *   **Penalty**: 0% (Full refund).
*   **Tier 2: Late Cancellation**
    *   **Window**: Between **48 hours and 72 hours** before the `BookingDate`.
    *   **Penalty**: 25% of the total cost is retained.
*   **Tier 3: Last-Minute Cancellation**
    *   **Window**: Less than **24 hours** before the `BookingDate`.
    *   **Penalty**: 50% of the total cost is retained.

---
*To activate these rules: Implement the `ICancellationPolicyService` in the Booking.Service domain and configure a background worker (e.g., Hangfire/Quartz) to monitor and prune pending cash bookings.*
