﻿using System.Collections.Generic;
using System.Net;
using Antix.Http.Services.Filters;
using Antix.Http.Services.Models;
using Antix.Services.Models;
using Xunit;

namespace Antix.Tests.Http.Filters.Responses
{
    public class ResponseGlobalFilterTest
    {
        [Fact]
        public void when_response_is_empty_returns_null()
        {
            var serviceResponse = ServiceResponse.Empty;

            var processResponse =
                ServiceResponseGlobalFilter
                    .Process(serviceResponse);

            Assert.Equal(null, processResponse);
        }

        [Fact]
        public void when_response_has_errors_they_are_returned()
        {
            var errors = new[] {"Eek"};
            var serviceResponse = ServiceResponse
                .Empty
                .WithErrors(errors);

            var processResponse =
                ServiceResponseGlobalFilter
                    .Process(serviceResponse);

            Assert.Equal(HttpStatusCode.BadRequest, processResponse.StatusCode);
            Assert.Equal(errors, processResponse.Content);
        }

        [Fact]
        public void when_response_has_data_and_no_errors_data_is_returned()
        {
            var data = new {frog = true};
            var serviceResponse = ServiceResponse.Empty
                .WithData(data);

            var processResponse =
                ServiceResponseGlobalFilter
                    .Process(serviceResponse);

            Assert.Equal(null, processResponse.StatusCode);
            Assert.Equal(data, processResponse.Content);
        }

        [Fact]
        public void no_change_when_not_IServiceResponse()
        {
            var responseValue = new {frog = true};

            var processResponse =
                ServiceResponseGlobalFilter
                    .Process(responseValue);

            Assert.Equal(null, processResponse);
        }

        [Fact]
        public void using_created_http_response()
        {
            var data = new {frog = true};
            var url = "/somewhere";

            var serviceResponse = ServiceResponse.Empty
                .AsHttpCreated(url)
                .WithData(data);

            var processResponse =
                ServiceResponseGlobalFilter
                    .Process(serviceResponse);

            Assert.Equal(HttpStatusCode.Created, processResponse.StatusCode);
            Assert.Equal(data, processResponse.Content);
            Assert.Equal(url, processResponse.Headers["location"]);
        }

        [Fact]
        public void using_created_http_response_set_data_first()
        {
            var data = new { frog = true };
            var url = "/somewhere";

            var serviceResponse = ServiceResponse.Empty
                .WithData(data)
                .AsHttpCreated(url);

            var processResponse =
                ServiceResponseGlobalFilter
                    .Process(serviceResponse);

            Assert.Equal(HttpStatusCode.Created, processResponse.StatusCode);
            Assert.Equal(data, processResponse.Content);
            Assert.Equal(url, processResponse.Headers["location"]);
        }

        [Fact]
        public void using_created_http_response_merges_data()
        {
            var data = new { frogName = "Bob" };
            var placeHolder = "{frogName}";
            var url = "/somewhere/" + placeHolder;

            var serviceResponse = ServiceResponse.Empty
                .WithData(data)
                .AsHttpCreated(url);

            var processResponse =
                ServiceResponseGlobalFilter
                    .Process(serviceResponse);

            Assert.Equal(HttpStatusCode.Created, processResponse.StatusCode);
            Assert.Equal(data, processResponse.Content);
            Assert.Equal(url.Replace(placeHolder, data.frogName), processResponse.Headers["location"]);
        }

        [Fact]
        public void using_created_http_response_merges_data_override()
        {
            var data = new { frogId = "Bob" };
            var placeHolder = "{frogName}";
            var url = "/somewhere/" + placeHolder;

            var serviceResponse = ServiceResponse.Empty
                .WithData(data)
                .AsHttpCreated(url, m => new {frogName = m.frogId});

            var processResponse =
                ServiceResponseGlobalFilter
                    .Process(serviceResponse);

            Assert.Equal(HttpStatusCode.Created, processResponse.StatusCode);
            Assert.Equal(data, processResponse.Content);
            Assert.Equal(url.Replace(placeHolder, data.frogId), processResponse.Headers["location"]);
        }

        [Fact]
        public void using_created_http_response_only_on_success()
        {
            //var data = new {frog = true};
            const string url = "/somewhere";
            const string error = "OH NO!";

            var serviceResponse = ServiceResponse.Empty
                //.WithData(data)
                .WithErrors(error)
                .AsHttpCreated(url);

            var processResponse =
                ServiceResponseGlobalFilter
                    .Process(serviceResponse);

            Assert.NotEqual(HttpStatusCode.Created, processResponse.StatusCode);
            Assert.Equal(new[] {error}, processResponse.Content);
            Assert.False(processResponse.Headers.ContainsKey("location"));
        }

        [Fact]
        public void when_http_response_has_errors_default_status_code_is_sent()
        {
            var errors = new[] {"Eek"};
            var serviceResponse = ServiceResponse
                .Empty
                .WithErrors(errors)
                .AsHttp();

            var processResponse =
                ServiceResponseGlobalFilter
                    .Process(serviceResponse);

            Assert.Equal(HttpStatusCode.BadRequest, processResponse.StatusCode);
            Assert.Equal(errors, processResponse.Content);
        }

        [Fact]
        public void when_http_response_has_errors_set_status_code_is_sent()
        {
            var errors = new[] {"Eek"};
            var headers = new Dictionary<string, string>
            {
                {"One", "1"}
            };

            var serviceResponse = ServiceResponse
                .Empty
                .WithErrors(errors)
                .AsHttp(HttpStatusCode.Forbidden)
                .WithData("some data")
                .WithHeaders(headers);

            var processResponse =
                ServiceResponseGlobalFilter
                    .Process(serviceResponse);

            Assert.Equal(HttpStatusCode.Forbidden, processResponse.StatusCode);
            Assert.Equal(errors, processResponse.Content);
            Assert.Equal(headers.Values, processResponse.Headers.Values);
        }
    }
}