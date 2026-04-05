$headers = @{
    "Authorization" = "Bearer YOUR_GROQ_API_KEY_HERE"
    "Content-Type"  = "application/json"
}

$body = @{
    model = "llama-3.3-70b-versatile"
    messages = @(
        @{ role = "system"; content = "You are a helpful assistant." },
        @{ role = "user"; content = "Hello" }
    )
} | ConvertTo-Json -Depth 5

try {
    $response = Invoke-RestMethod -Uri "https://api.groq.com/openai/v1/chat/completions" -Method Post -Headers $headers -Body $body
    Write-Output "SUCCESS:"
    $response | ConvertTo-Json -Depth 5
} catch {
    Write-Output "ERROR:"
    Write-Output $_.Exception.Response
    $stream = $_.Exception.Response.GetResponseStream()
    $reader = New-Object System.IO.StreamReader($stream)
    Write-Output $reader.ReadToEnd()
}
