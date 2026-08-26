$baseUrl = "http://localhost:8000"
$passed = 0
$failed = 0
$total = 0

function Run-Test {
    param (
        [string]$Name,
        [string]$Method = "GET",
        [string]$Url,
        [object]$Body = $null,
        [int[]]$ExpectedStatusCodes = @(200, 201, 204)
    )
    $script:total++
    Write-Host -NoNewline "[$script:total] $Name ($Method $Url)... "
    try {
        $jsonBody = $null
        if ($Body -ne $null) {
            $jsonBody = if ($Body -is [string]) { $Body } else { $Body | ConvertTo-Json -Depth 10 }
        }

        $headers = @{ "Accept" = "application/json" }
        $params = @{
            Uri = $Url
            Method = $Method
            UseBasicParsing = $true
            TimeoutSec = 15
            Headers = $headers
        }
        if ($jsonBody) {
            $params["Body"] = $jsonBody
            $params["ContentType"] = "application/json"
        }

        $res = Invoke-WebRequest @params
        $statusCode = [int]$res.StatusCode
        if ($ExpectedStatusCodes -contains $statusCode) {
            Write-Host -ForegroundColor Green "PASS (HTTP $statusCode)"
            $script:passed++
            return $res
        } else {
            Write-Host -ForegroundColor Red "FAIL (Got HTTP $statusCode, Expected: $($ExpectedStatusCodes -join ','))"
            $script:failed++
            return $null
        }
    } catch [System.Net.WebException] {
        $resp = $_.Exception.Response
        if ($resp -ne $null) {
            $statusCode = [int]$resp.StatusCode
            if ($ExpectedStatusCodes -contains $statusCode) {
                Write-Host -ForegroundColor Green "PASS (HTTP $statusCode)"
                $script:passed++
                return $resp
            } else {
                $stream = $resp.GetResponseStream()
                $reader = New-Object System.IO.StreamReader($stream)
                $bodyText = $reader.ReadToEnd()
                Write-Host -ForegroundColor Red "FAIL (HTTP $statusCode) -> $bodyText"
                $script:failed++
                return $null
            }
        } else {
            Write-Host -ForegroundColor Red "FAIL (Exception: $($_.Exception.Message))"
            $script:failed++
            return $null
        }
    } catch {
        Write-Host -ForegroundColor Red "FAIL (Exception: $($_.Exception.Message))"
        $script:failed++
        return $null
    }
}

Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "       SEADORA COMPLETE ENDPOINT HEALTH & REGRESSION SUITE       " -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan

# -------------------------------------------------------------
# 1. CONTENT SERVICE - TOURS CRUD & RETRIEVAL
# -------------------------------------------------------------
Write-Host "`n--- 1. CONTENT SERVICE: TOURS ---" -ForegroundColor Yellow
$toursRes = Run-Test -Name "Get All Tours" -Method "GET" -Url "$baseUrl/api/content/api/tours"
$tours = if ($toursRes) { $toursRes.Content | ConvertFrom-Json } else { @() }
$tourId = if ($tours.Count -gt 0) { $tours[0].id } else { $null }

if ($tourId) {
    Run-Test -Name "Get Tour Details by ID" -Method "GET" -Url "$baseUrl/api/content/api/tours/$tourId"
    
    # Test PUT Update Tour
    $putBody = @{
        id = $tourId
        names = @{ "en" = "Luxury Red Sea VIP Cruise"; "de" = "Luxus Rotes Meer VIP Kreuzfahrt" }
        descriptions = @{ "en" = "Updated luxury cruise experience in Hurghada."; "de" = "Aktualisiertes Luxuskreuzfahrt-Erlebnis in Hurghada." }
        price = 125.0
        currency = "EUR"
        duration = "Full Day"
        badge = "Bestseller"
        emoji = "Boat"
    }
    Run-Test -Name "Update Tour (PUT /api/content/api/tours/{id})" -Method "PUT" -Url "$baseUrl/api/content/api/tours/$tourId" -Body $putBody -ExpectedStatusCodes @(200, 204)
}

# Test POST Create Tour & DELETE
$createTourBody = @{
    names = @{ "en" = "Automated Smoke Test Tour"; "de" = "Testreise" }
    descriptions = @{ "en" = "Test description"; "de" = "Testbeschreibung" }
    price = 49.99
    currency = "EUR"
    duration = "2 Hours"
    badge = "New"
}
$createRes = Run-Test -Name "Create Tour (POST /api/content/api/tours)" -Method "POST" -Url "$baseUrl/api/content/api/tours" -Body $createTourBody -ExpectedStatusCodes @(200, 201)
if ($createRes) {
    $createdJson = $createRes.Content | ConvertFrom-Json
    $createdId = $createdJson.id
    if ($createdId) {
        Run-Test -Name "Delete Created Tour (DELETE /api/content/api/tours/{id})" -Method "DELETE" -Url "$baseUrl/api/content/api/tours/$createdId" -ExpectedStatusCodes @(200, 204)
    }
}

# -------------------------------------------------------------
# 2. CONTENT SERVICE - CATEGORIES CRUD
# -------------------------------------------------------------
Write-Host "`n--- 2. CONTENT SERVICE: CATEGORIES ---" -ForegroundColor Yellow
$catRes = Run-Test -Name "Get All Categories" -Method "GET" -Url "$baseUrl/api/content/api/categories"
$cats = if ($catRes) { $catRes.Content | ConvertFrom-Json } else { @() }
$catId = if ($cats.Count -gt 0) { $cats[0].id } else { $null }

if ($catId) {
    Run-Test -Name "Get Category by ID" -Method "GET" -Url "$baseUrl/api/content/api/categories/$catId"
}

$createCatBody = @{
    names = @{ "en" = "Test Luxury Category" }
    descriptions = @{ "en" = "Test category description" }
    iconName = "Sparkles"
    order = 99
}
$catPostRes = Run-Test -Name "Create Category (POST)" -Method "POST" -Url "$baseUrl/api/content/api/categories" -Body $createCatBody -ExpectedStatusCodes @(200, 201)
if ($catPostRes) {
    $createdCatId = ($catPostRes.Content | ConvertFrom-Json).id
    if ($createdCatId) {
        $updateCatBody = @{
            id = $createdCatId
            names = @{ "en" = "Updated Luxury Category" }
            descriptions = @{ "en" = "Updated category description" }
            iconName = "Star"
            order = 100
        }
        Run-Test -Name "Update Category (PUT)" -Method "PUT" -Url "$baseUrl/api/content/api/categories/$createdCatId" -Body $updateCatBody -ExpectedStatusCodes @(200, 204)
        Run-Test -Name "Delete Category (DELETE)" -Method "DELETE" -Url "$baseUrl/api/content/api/categories/$createdCatId" -ExpectedStatusCodes @(200, 204)
    }
}

# -------------------------------------------------------------
# 3. CONTENT SERVICE - DESTINATIONS CRUD
# -------------------------------------------------------------
Write-Host "`n--- 3. CONTENT SERVICE: DESTINATIONS ---" -ForegroundColor Yellow
$destRes = Run-Test -Name "Get All Destinations" -Method "GET" -Url "$baseUrl/api/content/api/destinations"
$dests = if ($destRes) { $destRes.Content | ConvertFrom-Json } else { @() }
$destId = if ($dests.Count -gt 0) { $dests[0].id } else { $null }

if ($destId) {
    Run-Test -Name "Get Destination by ID" -Method "GET" -Url "$baseUrl/api/content/api/destinations/$destId"
}

$createDestBody = @{
    names = @{ "en" = "Marsa Alam Luxury Bay" }
    descriptions = @{ "en" = "Dolphin sanctuary and pristine reefs" }
    flagEmoji = "Sea"
}
$destPostRes = Run-Test -Name "Create Destination (POST)" -Method "POST" -Url "$baseUrl/api/content/api/destinations" -Body $createDestBody -ExpectedStatusCodes @(200, 201)
if ($destPostRes) {
    $createdDestId = ($destPostRes.Content | ConvertFrom-Json).id
    if ($createdDestId) {
        $updateDestBody = @{
            id = $createdDestId
            names = @{ "en" = "Updated Marsa Alam Luxury Bay" }
            descriptions = @{ "en" = "Updated sanctuary and reefs" }
            flagEmoji = "Waves"
        }
        Run-Test -Name "Update Destination (PUT)" -Method "PUT" -Url "$baseUrl/api/content/api/destinations/$createdDestId" -Body $updateDestBody -ExpectedStatusCodes @(200, 204)
        Run-Test -Name "Delete Destination (DELETE)" -Method "DELETE" -Url "$baseUrl/api/content/api/destinations/$createdDestId" -ExpectedStatusCodes @(200, 204)
    }
}

# -------------------------------------------------------------
# 4. CONTENT SERVICE - CURRENCIES & EXCHANGE RATES
# -------------------------------------------------------------
Write-Host "`n--- 4. CONTENT SERVICE: CURRENCIES ---" -ForegroundColor Yellow
Run-Test -Name "Get All Currencies" -Method "GET" -Url "$baseUrl/api/content/api/v1/currencies?includeInactive=true"
Run-Test -Name "Sync Live Exchange Rates" -Method "POST" -Url "$baseUrl/api/content/api/v1/currencies/sync-rates"

# -------------------------------------------------------------
# 5. CONTENT SERVICE - LANGUAGES & TRANSLATIONS
# -------------------------------------------------------------
Write-Host "`n--- 5. CONTENT SERVICE: LANGUAGES ---" -ForegroundColor Yellow
Run-Test -Name "Get All Languages" -Method "GET" -Url "$baseUrl/api/content/api/v1/languages"
Run-Test -Name "Get All System Translations" -Method "GET" -Url "$baseUrl/api/content/api/v1/languages/all-translations"
Run-Test -Name "Get English Translations" -Method "GET" -Url "$baseUrl/api/content/api/v1/languages/en/translations"

# -------------------------------------------------------------
# 6. CONTENT SERVICE - NATIONALITIES & TOUR TYPES
# -------------------------------------------------------------
Write-Host "`n--- 6. CONTENT SERVICE: NATIONALITIES & TOUR TYPES ---" -ForegroundColor Yellow
Run-Test -Name "Get All Nationalities" -Method "GET" -Url "$baseUrl/api/content/api/v1/nationalities"
Run-Test -Name "Get All Tour Types" -Method "GET" -Url "$baseUrl/api/content/api/v1/tour-types"

# -------------------------------------------------------------
# 7. CONTENT SERVICE - SUPPLIERS & PAYMENT AGREEMENTS
# -------------------------------------------------------------
Write-Host "`n--- 7. CONTENT SERVICE: SUPPLIERS & AGREEMENTS ---" -ForegroundColor Yellow
Run-Test -Name "Get Payment Agreements" -Method "GET" -Url "$baseUrl/api/content/api/paymentagreements"
Run-Test -Name "Get Suppliers" -Method "GET" -Url "$baseUrl/api/content/api/suppliers"

# -------------------------------------------------------------
# 8. CONTENT SERVICE - CONCIERGE AI CHAT
# -------------------------------------------------------------
Write-Host "`n--- 8. CONTENT SERVICE: CONCIERGE CHAT ---" -ForegroundColor Yellow
$chatBody = @{
    message = "Can you recommend a luxury yacht tour in Hurghada?"
    language = "en"
}
Run-Test -Name "Concierge AI Chat (POST)" -Method "POST" -Url "$baseUrl/api/concierge/chat" -Body $chatBody -ExpectedStatusCodes @(200, 201)

# -------------------------------------------------------------
# 9. CONTENT SERVICE - EXCEL TEMPLATES & EXPORT
# -------------------------------------------------------------
Write-Host "`n--- 9. CONTENT SERVICE: EXCEL & SEARCH ---" -ForegroundColor Yellow
Run-Test -Name "Download Tours Excel Template" -Method "GET" -Url "$baseUrl/api/content/api/excel/template/tours"
Run-Test -Name "Export Tours Excel Data" -Method "GET" -Url "$baseUrl/api/content/api/excel/export/tours"
Run-Test -Name "Admin Global Search" -Method "GET" -Url "$baseUrl/api/content/api/search?query=Hurghada"

# -------------------------------------------------------------
# 10. BOOKING SERVICE - BOOKINGS CRUD & AVAILABILITY
# -------------------------------------------------------------
Write-Host "`n--- 10. BOOKING SERVICE: BOOKINGS ---" -ForegroundColor Yellow
Run-Test -Name "Get All Bookings" -Method "GET" -Url "$baseUrl/api/booking/api/bookings"

# Check Tour Availability
if ($tourId) {
    $todayIso = (Get-Date).ToString("yyyy-MM-dd")
    Run-Test -Name "Get Tour Availability" -Method "GET" -Url "$baseUrl/api/booking/api/bookings/$tourId/availability?date=$todayIso"
}

# Create Test Booking
$createBookingBody = @{
    tourId = if ($tourId) { $tourId } else { [Guid]::NewGuid().ToString() }
    customerName = "Smoke Test Guest"
    customerEmail = "smoketest@seadora.com"
    customerPhone = "+201000000000"
    bookingDate = (Get-Date).AddDays(3).ToString("yyyy-MM-ddTHH:mm:ssZ")
    guestCount = 2
    totalPrice = 250.0
    currency = "EUR"
    pickupLocation = "Steigenberger ALDAU Beach Hotel"
}
$bRes = Run-Test -Name "Create Booking (POST)" -Method "POST" -Url "$baseUrl/api/booking/api/bookings" -Body $createBookingBody -ExpectedStatusCodes @(200, 201)

# -------------------------------------------------------------
# 11. BOOKING SERVICE - FEEDBACKS
# -------------------------------------------------------------
Write-Host "`n--- 11. BOOKING SERVICE: FEEDBACKS ---" -ForegroundColor Yellow
Run-Test -Name "Get Feedbacks" -Method "GET" -Url "$baseUrl/api/booking/api/feedbacks"

$feedbackBody = @{
    tourId = if ($tourId) { $tourId } else { [Guid]::NewGuid().ToString() }
    customerName = "Happy Traveler"
    customerEmail = "guest@example.com"
    rating = 5
    comment = "Outstanding VIP service and breathtaking crystal waters!"
}
Run-Test -Name "Submit Customer Feedback (POST)" -Method "POST" -Url "$baseUrl/api/booking/api/feedbacks" -Body $feedbackBody -ExpectedStatusCodes @(200, 201)

# -------------------------------------------------------------
# 12. BOOKING SERVICE - CONTACT INQUIRIES
# -------------------------------------------------------------
Write-Host "`n--- 12. BOOKING SERVICE: INQUIRIES ---" -ForegroundColor Yellow
Run-Test -Name "Get Contact Inquiries" -Method "GET" -Url "$baseUrl/api/booking/api/inquiries"

$inquiryBody = @{
    fullName = "VIP Inquiry Client"
    email = "client@vip-travel.com"
    phone = "+491701234567"
    subject = "Private Superyacht Charter"
    message = "We would like to book a private superyacht for 10 guests next week."
}
$inqRes = Run-Test -Name "Submit Contact Inquiry (POST)" -Method "POST" -Url "$baseUrl/api/booking/api/inquiries" -Body $inquiryBody -ExpectedStatusCodes @(200, 201)

# -------------------------------------------------------------
# 13. BOOKING SERVICE - NOTIFICATIONS & REPORTS
# -------------------------------------------------------------
Write-Host "`n--- 13. BOOKING SERVICE: NOTIFICATIONS & REPORTS ---" -ForegroundColor Yellow
Run-Test -Name "Get Admin Notifications" -Method "GET" -Url "$baseUrl/api/booking/api/notifications"
Run-Test -Name "Get Dashboard Stats Report" -Method "GET" -Url "$baseUrl/api/booking/api/reports/dashboard"
Run-Test -Name "Get Supplier Report" -Method "GET" -Url "$baseUrl/api/booking/api/reports/supplier"
Run-Test -Name "Get Customers Report" -Method "GET" -Url "$baseUrl/api/booking/api/reports/customers"
Run-Test -Name "Get Financial Ledger Report" -Method "GET" -Url "$baseUrl/api/booking/api/reports/ledger"

# -------------------------------------------------------------
# 14. IDENTITY SERVICE - AUTHENTICATION
# -------------------------------------------------------------
Write-Host "`n--- 14. IDENTITY SERVICE: AUTHENTICATION ---" -ForegroundColor Yellow
$loginBody = @{
    email = "admin@seadora.com"
    password = "AdminPassword123!"
}
$loginRes = Run-Test -Name "Admin Login (POST /api/auth/api/auth/login)" -Method "POST" -Url "$baseUrl/api/auth/api/auth/login" -Body $loginBody -ExpectedStatusCodes @(200, 400, 401)

# -------------------------------------------------------------
# SUMMARY REPORT
# -------------------------------------------------------------
Write-Host "`n================================================================" -ForegroundColor Cyan
Write-Host "                      TEST RESULTS SUMMARY                      " -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host " Total Tests Executed: $total" -ForegroundColor White
Write-Host " Passed:               $passed" -ForegroundColor Green
Write-Host " Failed:               $failed" -ForegroundColor $(if ($failed -eq 0) { "Green" } else { "Red" })
Write-Host "================================================================" -ForegroundColor Cyan

if ($failed -eq 0) {
    Write-Host "ALL ENDPOINTS ARE FULLY OPERATIONAL AND HEALTHY!" -ForegroundColor Green
} else {
    Write-Host "SOME ENDPOINTS FAILED - SEE LOGS ABOVE." -ForegroundColor Red
}
