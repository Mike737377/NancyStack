using Nancy;
using Nancy.Validation;
using NancyStack.Handlers;
using NancyStack.ModelBinding;
using NancyStack.Modules;
using NancyStack.Routing;
using NancyStack.Tests.Handlers;
using NSubstitute;
using Ploeh.AutoFixture;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace NancyStack.Tests.Routing
{
    public abstract class RouteHandlerBuilderTests
    {
        public class OtherInput { }

        private readonly Fixture autoFixture = new Fixture();
        private readonly INancyRouteRegister nancyRouteRegister = Substitute.For<INancyRouteRegister>();
        //private readonly RouteHandlerBuilder<SampleHandlerInput> routeBuilder;
        private bool onScenarioResult = false;
        private bool onScenarioInvoked = false;
        private readonly string path = "/";
        private readonly RouteHandlerContext context;
        private dynamic result;

        public RouteHandlerBuilderTests()
        {
            context = new RouteHandlerContext()
            {
                ModelBinder = Substitute.For<IModelBinder>(),
                NancyModule = Substitute.For<INancyModule>(),
                HandlerRegistry = Substitute.For<IHandlerRegistry>()
            };

            context.ModelBinder.BindAndValidate<SampleHandlerInput>(context.NancyModule).Returns(new SampleHandlerInput());

            Func<dynamic, dynamic> invoker = null;
            nancyRouteRegister.Register(path, Arg.Do<Func<dynamic, dynamic>>(x => invoker = x));

            CreateRouteHandler<SampleHandlerInput>()
                .Returning<SampleHandlerReply>()
                .OnValidationError<OtherInput>()
                .On(x => onScenarioResult, x => { onScenarioInvoked = true; return null; })
                .OnSuccess(x => x.RenderView("myview"));


            Setup();

            result = invoker(new SampleHandlerInput());
        }

        protected virtual void Setup() { }

        protected virtual RouteHandlerBuilder<T> CreateRouteHandler<T>()
        {
            return new RouteHandlerBuilder<T>(context, path, nancyRouteRegister);
        }

        public abstract class WithOnSuccess : RouteHandlerBuilderTests
        {
            public class AndValidationPassed : WithOnSuccess
            {
                protected override void Setup()
                {
                    base.Setup();
                    context.NancyModule.ModelValidationResult = new ModelValidationResult();
                }

                public abstract class AndOnScenarioInvoked : AndValidationPassed
                {
                    protected override void Setup()
                    {
                        base.Setup();
                        onScenarioResult = true;
                    }

                    //public class WithReferenceToAnotherModel : AndOnScenarioInvoked
                    //{
                    //    [Fact]
                    //    public void ShouldReturnOtherView()
                    //    {
                    //        throw new NotImplementedException();
                    //    }

                    //    [Fact]
                    //    public void ShouldExecuteOtherHandler()
                    //    {
                    //        throw new NotImplementedException();
                    //    }
                    //}

                    public class WithCustomFunc : AndOnScenarioInvoked
                    {
                        [Fact]
                        public void ShouldReturnCustomResult()
                        {
                            onScenarioInvoked.ShouldBe(true);
                        }
                    }
                }

                public abstract class AndOnSuccessInvoked : AndValidationPassed
                {
                    [Fact]
                    public void ShouldReturnView()
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            public abstract class AndValidationFailed : WithOnSuccess
            {
                private dynamic result;

                protected override void Setup()
                {
                    base.Setup();
                    context.NancyModule.ModelValidationResult = new ModelValidationResult(autoFixture.CreateMany<ModelValidationError>());
                }

                public class WithReferenceToAnotherModel : AndValidationFailed
                {
                    [Fact]
                    public void ShouldReturnOtherView()
                    {
                        throw new NotImplementedException();
                    }

                    [Fact]
                    public void ShouldExecuteOtherHandler()
                    {
                        throw new NotImplementedException();
                    }
                }
            }
        }

    }
}
