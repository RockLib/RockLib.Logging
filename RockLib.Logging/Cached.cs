﻿using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace RockLib.Logging;

internal static class Cached
{
    private static readonly Lazy<string> _ipAddress = new Lazy<string>(GetMachineIpAddress);
    private static readonly Lazy<string> _machineName = new Lazy<string>(() => Environment.MachineName);
    private static readonly Lazy<string> _userName = new Lazy<string>(() => Environment.UserName);

    public static string IpAddress => _ipAddress.Value;

    public static string MachineName => _machineName.Value;

    public static string UserName => _userName.Value;

    private static string GetMachineIpAddress()
    {
        try
        {
            var ipAddresses =
                from i in NetworkInterface.GetAllNetworkInterfaces()
                where IsValidNetworkInterface(i)
                let p = i.GetIPProperties()
                where p.GatewayAddresses.Any()
                from a in p.UnicastAddresses
                where IsDuplicateAddressDetectionStatePreferred(a) && IsDnsEligible(a)
                select a.Address.ToString();
            return string.Join("\n", ipAddresses);
        }
        catch
        {
            return null;
        }
    }

    private static bool IsValidNetworkInterface(NetworkInterface networkInterface) => networkInterface.OperationalStatus == OperationalStatus.Up
            && (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet || networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211);

    private static bool IsDuplicateAddressDetectionStatePreferred(UnicastIPAddressInformation addressInfo)
    {
        try { return addressInfo.DuplicateAddressDetectionState == DuplicateAddressDetectionState.Preferred; }
        catch { return true; }
    }

    private static bool IsDnsEligible(UnicastIPAddressInformation addressInfo)
    {
        try { return addressInfo.IsDnsEligible; }
        catch { return true; }
    }
}
