using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace RockLib.Logging;

internal static class Cached
{
    private static readonly Lazy<string?> _ipAddress = new(GetMachineIpAddress);
    private static readonly Lazy<string> _machineName = new(() => Environment.MachineName);
    private static readonly Lazy<string> _userName = new(() => Environment.UserName);

    public static string? IpAddress => _ipAddress.Value;

    public static string MachineName => _machineName.Value;

    public static string UserName => _userName.Value;

    private static string? GetMachineIpAddress()
    {
        try
        {
            var ipAddresses =
                from i in NetworkInterface.GetAllNetworkInterfaces()
                where IsValidNetworkInterface(i)
                let p = i.GetIPProperties()
                where p.GatewayAddresses.Count > 0
                from a in p.UnicastAddresses
                where IsDuplicateAddressDetectionStatePreferred(a) && IsDnsEligible(a)
                select a.Address.ToString();
            return string.Join("\n", ipAddresses);
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch
#pragma warning restore CA1031 // Do not catch general exception types
        {
            return null;
        }
    }

    private static bool IsValidNetworkInterface(NetworkInterface networkInterface) => networkInterface.OperationalStatus == OperationalStatus.Up
        && (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet || networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211);

#pragma warning disable CA1416 // Validate platform compatibility
    private static bool IsDuplicateAddressDetectionStatePreferred(UnicastIPAddressInformation addressInfo) => 
        addressInfo.DuplicateAddressDetectionState == DuplicateAddressDetectionState.Preferred;

    private static bool IsDnsEligible(UnicastIPAddressInformation addressInfo) => addressInfo.IsDnsEligible;
#pragma warning restore CA1416 // Validate platform compatibility
}
