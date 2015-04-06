using NancyStack.Modules;
using NancyStack.Routing;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace NancyStack.Tests.Modules
{
    public class TestModule : NancyStackModule
    {
        public TestModule()
        {
            AddRoute.Get<GetQuery>("/get").Returning<GetResponse>().OnSuccess(x => new GetViewModel());
            AddRoute.Delete<DeleteInput>("/delete").Returning<DeleteResponse>().OnSuccess(x => new DeleteViewModel());
            AddRoute.Post<PostInput>("/post").Returning<PostResponse>().OnSuccess(x => new PostViewModel());
            AddRoute.Put<PutInput>("/put").Returning<PutResponse>().OnSuccess(x => new PutViewModel());
        }
    }

    public class GetQuery { }
    public class GetResponse { }
    public class GetViewModel { }

    public class DeleteInput { }
    public class DeleteResponse { }
    public class DeleteViewModel { }

    public class PutInput { }
    public class PutResponse { }
    public class PutViewModel { }

    public class PostInput { }
    public class PostResponse { }
    public class PostViewModel { }

    public class NancyStackModuleTest
    {
        private readonly TestModule module;

        public NancyStackModuleTest()
        {
            module = new TestModule();
        }

        public class WhenRegisteringGetRoute : NancyStackModuleTest
        {
            [Fact]
            public void ShouldRegisterCorrectUrl()
            {
                UrlRoute.Instance.For(new GetQuery()).ShouldBe("/get");
            }
        }
    }
}

