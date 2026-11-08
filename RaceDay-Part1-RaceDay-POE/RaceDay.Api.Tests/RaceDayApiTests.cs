using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using RaceDay.Api.Filters;
using Xunit;

namespace RaceDay.Api.Tests
{
    public class RaceDayApiTests
    {
        [Fact]
        public void UnauthenticatedUser_ShouldReceive401()
        {
            var context = CreateContext();


        var filter = new RoleAuthorizeAttribute("Organiser");

            filter.OnAuthorization(context);

            var result = Assert.IsType<UnauthorizedObjectResult>(
                context.Result);

            Assert.Equal(401, result.StatusCode);
        }

        [Fact]
        public void ParticipantAccessingOrganiserFunction_ShouldReceive403()
        {
            var context = CreateContext();

            context.HttpContext.Session.SetInt32("UserId", 1);
            context.HttpContext.Session.SetString("Role", "Participant");

            var filter = new RoleAuthorizeAttribute("Organiser");

            filter.OnAuthorization(context);

            var result = Assert.IsType<ObjectResult>(
                context.Result);

            Assert.Equal(403, result.StatusCode);
        }

        [Fact]
        public void OrganiserAccessingOrganiserFunction_ShouldBeAllowed()
        {
            var context = CreateContext();

            context.HttpContext.Session.SetInt32("UserId", 1);
            context.HttpContext.Session.SetString("Role", "Organiser");

            var filter = new RoleAuthorizeAttribute("Organiser");

            filter.OnAuthorization(context);

            Assert.Null(context.Result);
        }

        private static AuthorizationFilterContext CreateContext()
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Features.Set<ISessionFeature>(
                new TestSessionFeature
                {
                    Session = new TestSession()
                });

            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor());

            return new AuthorizationFilterContext(
                actionContext,
                new List<IFilterMetadata>());
        }

        private sealed class TestSessionFeature : ISessionFeature
        {
            public ISession Session { get; set; } = null!;
        }

        private sealed class TestSession : ISession
        {
            private readonly Dictionary<string, byte[]> _values = new();

            public bool IsAvailable => true;

            public string Id => "TestSession";

            public IEnumerable<string> Keys => _values.Keys;

            public void Clear()
            {
                _values.Clear();
            }

            public void Remove(string key)
            {
                _values.Remove(key);
            }

            public void Set(string key, byte[] value)
            {
                _values[key] = value;
            }

            public bool TryGetValue(string key, out byte[] value)
            {
                if (_values.TryGetValue(key, out var storedValue))
                {
                    value = storedValue;
                    return true;
                }

                value = Array.Empty<byte>();
                return false;
            }

            public System.Threading.Tasks.Task LoadAsync(
                System.Threading.CancellationToken cancellationToken = default)
            {
                return System.Threading.Tasks.Task.CompletedTask;
            }

            public System.Threading.Tasks.Task CommitAsync(
                System.Threading.CancellationToken cancellationToken = default)
            {
                return System.Threading.Tasks.Task.CompletedTask;
            }
        }
    }


}
