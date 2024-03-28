using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace TestTheApi
{
    public class HelloEndPointUnitTest
    {
        private readonly HttpClient client = new() { BaseAddress = new Uri("https://localhost:7232/hello") };

        [Fact]
        public async Task Get_No_Params_Should_Return_200_Hello()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get
            };

            using var response = await client.SendAsync(request);
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var test = await response.Content.ReadAsStringAsync();

                Assert.Equal("Hello!", test);
            }
        }

        [Fact]
        public async Task Get_With_Param_Should_Return_200_HelloParamValue()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(client.BaseAddress?.AbsoluteUri + "/SomeValue")
            };

            using var response = await client.SendAsync(request);
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var test = await response.Content.ReadAsStringAsync();

                Assert.Equal("Hello SomeValue!", test);
            }
        }

        [Fact]
        public async Task Post_No_Params_No_Name_In_From_Should_Return_200_Hello()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent([new KeyValuePair<string, string>("NotName", "MyName")])
            };

            using var response = await client.SendAsync(request);
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var test = await response.Content.ReadAsStringAsync();

                Assert.Equal("Hello!", test);
            }
        }

        [Fact]
        public async Task Post_No_Params_With_Name_In_From_Should_Return_200_HelloName()
        {
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("name", "MyName"));
            nvc.Add(new KeyValuePair<string, string>("NotName", "MyNameAlso"));

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(nvc)
            };

            using var response = await client.SendAsync(request);
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var test = await response.Content.ReadAsStringAsync();

                Assert.Equal("Hello MyName!", test);
            }
        }

        [Fact]
        public async Task Put_No_Params_Should_Return_200_Hello()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put
            };

            using var response = await client.SendAsync(request);
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var test = await response.Content.ReadAsStringAsync();

                Assert.Equal("Hello!", test);
            }
        }

        [Fact]
        public async Task Put_With_Params_Should_Return_200_HelloParamValue()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(client.BaseAddress?.AbsoluteUri + "/SomeValue")
            };

            using var response = await client.SendAsync(request);
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var test = await response.Content.ReadAsStringAsync();

                Assert.Equal("Hello SomeValue!", test);
            }
        }

        [Fact]
        public async Task Put_No_Params_With_Name_In_Query_Should_Return_200_HelloName()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(client.BaseAddress?.AbsoluteUri + "?name=Some+Value")
            };

            using var response = await client.SendAsync(request);
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var test = await response.Content.ReadAsStringAsync();

                Assert.Equal("Hello Some Value!", test);
            }
        }

        [Fact]
        public async Task Put_No_Params_With_Name_In_Form_Should_Return_200_HelloName()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                Content = new FormUrlEncodedContent([new KeyValuePair<string, string>("name", "Some Name"), new KeyValuePair<string, string>("NotName", "MyName")])
            };

            using var response = await client.SendAsync(request);
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var test = await response.Content.ReadAsStringAsync();

                Assert.Equal("Hello Some Name!", test);
            }
        }
    }
}