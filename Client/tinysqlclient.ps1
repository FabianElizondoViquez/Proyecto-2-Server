param (
    [Parameter(Mandatory = $true)]
    [string]$IP,
    [Parameter(Mandatory = $true)]
    [int]$Port,
    [Parameter(Mandatory = $true)]
    [string]$QueryFile
)

$ipEndPoint = [System.Net.IPEndPoint]::new([System.Net.IPAddress]::Parse($IP), $Port)
$currentDatabase = ""

function Send-Message {
    param (
        [Parameter(Mandatory=$true)]
        [pscustomobject]$message,
        [Parameter(Mandatory=$true)]
        [System.Net.Sockets.Socket]$client
    )

    $stream = New-Object System.Net.Sockets.NetworkStream($client)
    $writer = New-Object System.IO.StreamWriter($stream)
    try {
        $writer.WriteLine($message)
    }
    finally {
        $writer.Close()
        $stream.Close()
    }
}

function Receive-Message {
    param (
        [System.Net.Sockets.Socket]$client
    )
    $stream = New-Object System.Net.Sockets.NetworkStream($client)
    $reader = New-Object System.IO.StreamReader($stream)
    try {
        $line = $reader.ReadLine()
        if ($null -ne $line) {
            return $line
        } else {
            return ""
        }
    }
    finally {
        $reader.Close()
        $stream.Close()
    }
}

function Execute-MyQuery {
    param (
        [Parameter(Mandatory = $true)]
        [string]$QueryFile,
        [Parameter(Mandatory = $true)]
        [int]$Port,
        [Parameter(Mandatory = $true)]
        [string]$IP
    )

    $queries = Get-Content -Path $QueryFile -Raw -ErrorAction Stop
    $queryList = $queries -split ';'

    $ipEndPoint = [System.Net.IPEndPoint]::new([System.Net.IPAddress]::Parse($IP), $Port)

    foreach ($query in $queryList) {
        $query = $query.Trim()
        if ($query -ne "") {
            $client = New-Object System.Net.Sockets.Socket($ipEndPoint.AddressFamily, [System.Net.Sockets.SocketType]::Stream, [System.Net.Sockets.ProtocolType]::Tcp)
            $client.Connect($ipEndPoint)

            $requestObject = [PSCustomObject]@{
                RequestType = 0; 
                RequestBody = $query
            }

            $jsonMessage = ConvertTo-Json -InputObject $requestObject -Compress

            Send-Message -client $client -message $jsonMessage

            $response = Receive-Message -client $client

            $responseObject = ConvertFrom-Json -InputObject $response

            if ($query.StartsWith("SET DATABASE")) {
                if ($responseObject.Status -eq "Success") {
                    $currentDatabase = $query.Substring("SET DATABASE".Length).Trim()
                    Write-Host "Database context set to: $currentDatabase" -ForegroundColor Green
                } else {
                    Write-Host "Failed to set database context: $($responseObject.ResponseBody)" -ForegroundColor Red
                }
            } else {
                $responseObject | Format-Table -AutoSize
            }

            $client.Shutdown([System.Net.Sockets.SocketShutdown]::Both)
            $client.Close()
        }
    }
}

# Ejemplo de uso
# . .\tinysqlclient.ps1
# Execute-MyQuery -QueryFile ".C:\script.tinysql" -Port 8000 -IP "127.0.0.1"

function Send-SQLCommand {
    param (
        [string]$command
    )
    $client = New-Object System.Net.Sockets.Socket($ipEndPoint.AddressFamily, [System.Net.Sockets.SocketType]::Stream, [System.Net.Sockets.ProtocolType]::Tcp)
    $client.Connect($ipEndPoint)
    $requestObject = [PSCustomObject]@{
        RequestType = 0;
        RequestBody = $command
    }
    Write-Host -ForegroundColor Green "Sending command: $command"

    $jsonMessage = ConvertTo-Json -InputObject $requestObject -Compress
    Send-Message -client $client -message $jsonMessage
    $response = Receive-Message -client $client

    Write-Host -ForegroundColor Green "Response received: $response"
    
    $responseObject = ConvertFrom-Json -InputObject $response
    Write-Output $responseObject
    $client.Shutdown([System.Net.Sockets.SocketShutdown]::Both)
    $client.Close()
}

Send-SQLCommand -command "CREATE TABLE ESTUDIANTE"
Send-SQlCommand -command "SELECT * FROM ESTUDIANTE"
