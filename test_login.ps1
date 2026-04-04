$body = @{
    email = "Admin@gmail.com"
    password = "Admin@123"
} | ConvertTo-Json

try {
    $res = Invoke-RestMethod -Uri 'https://donghuongsonglamserver-hzd6fkenhtgghhbr.southeastasia-01.azurewebsites.net/api/Auth/login' -Method Post -Body $body -ContentType 'application/json'
    Write-Output "SUCCESS:"
    $res | ConvertTo-Json -Depth 5
} catch {
    Write-Output "ERROR:"
    $stream = $_.Exception.Response.GetResponseStream()
    if ($stream) {
        $reader = New-Object System.IO.StreamReader($stream)
        $reader.ReadToEnd()
    } else {
        Write-Output $_.Exception.Message
    }
}
