using NancyStack.Configuration;
using NancyStack.Handlers;
using NancyStack.Tests.Handlers;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace NancyStack.Tests.Configuration
{
    public class HandlerRegisterTests
    {
        private readonly HandlerRegister register;

        public HandlerRegisterTests()
        {
            register = new HandlerRegister();
        }

        public class WhenScanningAssemblyContainingType : HandlerRegisterTests
        {
            public WhenScanningAssemblyContainingType()
            {
                register.ScanAssemblyContainingType<SampleHandler>();
            }

            public class AndRetrievingRegistrationForSampleHandlerInput : WhenScanningAssemblyContainingType
            {
                private HandlerRegistration registeredHandler;

                public AndRetrievingRegistrationForSampleHandlerInput()
                {
                    registeredHandler = register.GetHandlerRegistration<SampleHandlerInput>();
                }

                [Fact]
                public void HandlerTypeShouldBeSampleHandler()
                {
                    registeredHandler.HandlerType.ShouldBe(typeof(SampleHandler));
                }

                [Fact]
                public void MessageTypeShouldBeSampleHandlerInput()
                {
                    registeredHandler.MessageType.ShouldBe(typeof(SampleHandlerInput));
                }

                [Fact]
                public void HandlerContextTypeShouldBeSampleHandlerContext()
                {
                    registeredHandler.HandlerContextType.ShouldBe(typeof(SampleHandlerContext));
                }
            }
        }
    }
}
