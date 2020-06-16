using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using RockLib.Dynamic;
using RockLib.Logging.DependencyInjection;
using RockLib.Logging.LogProcessing;
using System;
using System.Collections.Generic;
using Xunit;

namespace RockLib.Logging.Tests.DependencyInjection
{
    public class ReloadingLoggerTests
    {
        public static readonly Type ReloadingLogger = Type.GetType("RockLib.Logging.DependencyInjection.ReloadingLogger, RockLib.Logging", true);

        [Fact]
        public void ConstructorHappyPath()
        {

        }

        [Fact]
        public void ReloadHappyPath()
        {

        }
    }
}
