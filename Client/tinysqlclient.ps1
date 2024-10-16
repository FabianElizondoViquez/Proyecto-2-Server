param (
    [Parameter(Mandatory = $true)]
    [string]$IP,
    [Parameter(Mandatory = $true)]
    [int]$Port
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
        return $null -ne $reader.ReadLine ? $reader.ReadLine() : ""
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

    # Leer el contenido del archivo de consulta y dividir por punto y coma
    $queries = Get-Content -Path $QueryFile -Raw -ErrorAction Stop
    $queryList = $queries -split ';'

    # Configurar el punto de conexión
    $ipEndPoint = [System.Net.IPEndPoint]::new([System.Net.IPAddress]::Parse($IP), $Port)

    # Procesar cada consulta en el archivo
    foreach ($query in $queryList) {
        $query = $query.Trim()
        if ($query -ne "") {
            # Crear un cliente de socket
            $client = New-Object System.Net.Sockets.Socket($ipEndPoint.AddressFamily, [System.Net.Sockets.SocketType]::Stream, [System.Net.Sockets.ProtocolType]::Tcp)
            $client.Connect($ipEndPoint)

            # Crear el objeto de solicitud
            $requestObject = [PSCustomObject]@{
                RequestType = 0;  # Asumiendo que 0 es para SQLSentence
                RequestBody = $query
            }

            # Convertir el objeto de solicitud a JSON
            $jsonMessage = ConvertTo-Json -InputObject $requestObject -Compress

            # Enviar el mensaje
            Send-Message -client $client -message $jsonMessage

            # Recibir la respuesta
            $response = Receive-Message -client $client

            # Convertir la respuesta de JSON a objeto
            $responseObject = ConvertFrom-Json -InputObject $response

            # Verificar si es una sentencia SET DATABASE y actualizar el contexto
            if ($query.StartsWith("SET DATABASE")) {
                if ($responseObject.Status -eq "Success") {
                    $currentDatabase = $query.Substring("SET DATABASE".Length).Trim()
                    Write-Host "Database context set to: $currentDatabase" -ForegroundColor Green
                } else {
                    Write-Host "Failed to set database context: $($responseObject.ResponseBody)" -ForegroundColor Red
                }
            } else {
                # Mostrar la respuesta en formato de tabla
                $responseObject | Format-Table -AutoSize
            }

            # Cerrar el cliente
            $client.Shutdown([System.Net.Sockets.SocketShutdown]::Both)
            $client.Close()
        }
    }
}

# Ejemplo de uso
# Execute-MyQuery -QueryFile ".\Script.tinysql" -Port 8000 -IP "10.0.0.2"

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

# This is an example, should not be called here
Send-SQLCommand -command "CREATE TABLE ESTUDIANTE"
Send-SQlCommand -command "SELECT * FROM ESTUDIANTE"