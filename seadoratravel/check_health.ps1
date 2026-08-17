$endpoints = @(
    @{ Name = "Customer Website"; Url = "http://localhost:3000" },
    @{ Name = "Admin Dashboard"; Url = "http://localhost:3001" },
    @{ Name = "API Gateway"; Url = "http://localhost:8000" },
    @{ Name = "Content API (Tours)"; Url = "http://localhost:8000/api/content/api/tours" },
    @{ Name = "Content API (Categories)"; Url = "http://localhost:8000/api/content/api/categories" },
    @{ Name = "Content API (Destinations)"; Url = "http://localhost:8000/api/content/api/destinations" },
    @{ Name = "Booking API (Bookings)"; Url = "http://localhost:8000/api/booking/api/bookings" }
)

Write-Host "=================================================="
Write-Host "         SEADORA HEALTH VALIDATION REPORT         "
Write-Host "=================================================="

foreach ($ep in $endpoints) {
    try {
        $res = Invoke-WebRequest -Uri $ep.Url -UseBasicParsing -TimeoutSec 6
        Write-Host " [PASS] $($ep.Name) -> HTTP $($res.StatusCode)"
    } catch {
        Write-Host " [FAIL] $($ep.Name) -> $($_.Exception.Message)"
    }
}
Write-Host "=================================================="
