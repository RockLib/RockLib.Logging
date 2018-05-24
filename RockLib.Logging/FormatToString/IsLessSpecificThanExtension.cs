using System;

namespace RockLib.Logging
{
    internal static class IsLessSpecificThanExtension
    {
        public static bool IsLessSpecificThan(this Type thisType, Type comparisonType)
        {
            thisType = Nullable.GetUnderlyingType(thisType) ?? thisType;
            comparisonType = Nullable.GetUnderlyingType(comparisonType) ?? comparisonType;

            if (thisType == comparisonType)
            {
                return false;
            }

            if (thisType.IsAssignableFrom(comparisonType))
            {
                return true;
            }

            if ((thisType == typeof(short)
                 && (comparisonType == typeof(sbyte)
                     || comparisonType == typeof(byte)))
                || (thisType == typeof(ushort)
                    && comparisonType == typeof(byte))
                || (thisType == typeof(int)
                    && (comparisonType == typeof(ushort)
                        || comparisonType == typeof(short)
                        || comparisonType == typeof(sbyte)
                        || comparisonType == typeof(byte)))
                || (thisType == typeof(uint)
                    && (comparisonType == typeof(ushort)
                        || comparisonType == typeof(byte)))
                || (thisType == typeof(long)
                    && (comparisonType == typeof(uint)
                        || comparisonType == typeof(int)
                        || comparisonType == typeof(ushort)
                        || comparisonType == typeof(short)
                        || comparisonType == typeof(sbyte)
                        || comparisonType == typeof(byte)))
                || (thisType == typeof(ulong)
                    && (comparisonType == typeof(uint)
                        || comparisonType == typeof(ushort)
                        || comparisonType == typeof(byte)))
                || (thisType == typeof(float)
                    && (comparisonType == typeof(ulong)
                        || comparisonType == typeof(long)
                        || comparisonType == typeof(uint)
                        || comparisonType == typeof(int)
                        || comparisonType == typeof(ushort)
                        || comparisonType == typeof(short)
                        || comparisonType == typeof(sbyte)
                        || comparisonType == typeof(byte)))
                || (thisType == typeof(double)
                    && (comparisonType == typeof(float)
                        || comparisonType == typeof(ulong)
                        || comparisonType == typeof(long)
                        || comparisonType == typeof(uint)
                        || comparisonType == typeof(int)
                        || comparisonType == typeof(ushort)
                        || comparisonType == typeof(short)
                        || comparisonType == typeof(sbyte)
                        || comparisonType == typeof(byte))))
            {
                return true;
            }

            return false;
        }
    }
}
