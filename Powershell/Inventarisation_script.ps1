class Computer {

    [string]$Name
    [string]$IPAddress
    [string]$Manufacturer
    [string]$Serialnumber
    [System.Collections.ArrayList]$Monitors = @()

    [void] setName([string]$name) {
        $this.Name = $name
    }

    [void] setIPAdress([string]$ipAdress) {
        $this.IPAddress = $ipAdress
    }

    [void] setManufacture([string]$manufacture) {
        $this.Manufacturer = $manufacture
    }

    [void] setSerialnumber([string]$serialnumber) {
        $this.Serialnumber = $serialnumber
    }

    [void] addMonitor([Monitor]$monitor) {
        $this.Monitors.Add($monitor)
    }

    [string] getName() {
        return $this.Name
    }

    [string] getIPAddress() {
        return $this.IPAddress
    }

    [string] getManufacture() {
        return $this.Manufacturer
    }

    [string] getSerialnumber() {
        return $this.Serialnumber
    }

    
}

class Monitor {

    [string]$Manufacture
    [string]$Serialnumber

    [void]setManufacture([string]$manufacture) {
        $this.Manufacture = $manufacture
    }

    [void]setSerialnumber([string]$serialnumber) {
        $this.Serialnumber = $serialnumber
    }

    [string] getManufacture() {

        return $this.Manufacture
    }

    [string] getSerialnumber() {

        return $this.Serialnumber
    }
}

function Decode {
    If ($args[0] -is [System.Array]) {
        [System.Text.Encoding]::ASCII.GetString($args[0])
    }
    Else {
        "Not Found"
    }
}
$Computer = [Computer]::new()
$jsonBase = @{}
$date = Get-Date -Format "dddd MM/dd/yyyy HH:mm"

$array = @{}


$computerName = hostname
$computerManufacture = (Get-CimInstance -ClassName Win32_ComputerSystem).Manufacturer
$computerSerial = Get-WmiObject win32_bios | select Serialnumber
$ipv4 = (Get-NetIPAddress | Where-Object {$_.AddressState -eq "Preferred" -and $_.ValidLifetime -lt "24:00:00"}).IPAddress

$Computer.setName($computerName)
$Computer.setIPAdress($ipv4)
$Computer.setManufacture($computerManufacture)
$Computer.setSerialnumber($computerSerial)

$array.Add("Computer", $Computer)



$Monitors = Get-WmiObject WmiMonitorID -Namespace root\wmi
    

    
ForEach ($Monitor in $Monitors) {  
    $Serial = Decode $Monitor.SerialNumberID -notmatch 0

    $monitor = [Monitor]::new()
    $monitor.setSerialnumber($Serial)


    if($Serial -eq 0) {
        
        $Computer.addMonitor($monitor)
        continue;
    } else {

        $Computer.addMonitor($monitor)
    }
}
$array | ConvertTo-Json -Depth 10 | Out-File "C:\Users\dcif01\Documents\LKQ_INVENTORY\Invetory\write-array.json"
