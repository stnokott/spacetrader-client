// <auto-generated/>
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions;
using SpaceTradersApi.Client.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
namespace SpaceTradersApi.Client.My.Ships.Item.Extract.Survey
{
    /// <summary>
    /// Builds and executes requests for operations under \my\ships\{shipSymbol}\extract\survey
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.16.0")]
    public partial class SurveyRequestBuilder : BaseRequestBuilder
    {
        /// <summary>
        /// Instantiates a new <see cref="global::SpaceTradersApi.Client.My.Ships.Item.Extract.Survey.SurveyRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public SurveyRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/my/ships/{shipSymbol}/extract/survey", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::SpaceTradersApi.Client.My.Ships.Item.Extract.Survey.SurveyRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public SurveyRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/my/ships/{shipSymbol}/extract/survey", rawUrl)
        {
        }
        /// <summary>
        /// Use a survey when extracting resources from a waypoint. This endpoint requires a survey as the payload, which allows your ship to extract specific yields.Send the full survey object as the payload which will be validated according to the signature. If the signature is invalid, or any properties of the survey are changed, the request will fail.
        /// </summary>
        /// <returns>A <see cref="global::SpaceTradersApi.Client.My.Ships.Item.Extract.Survey.SurveyPostResponse"/></returns>
        /// <param name="body">A resource survey of a waypoint, detailing a specific extraction location and the types of resources that can be found there.</param>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<global::SpaceTradersApi.Client.My.Ships.Item.Extract.Survey.SurveyPostResponse?> PostAsync(global::SpaceTradersApi.Client.Models.Survey body, Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#nullable restore
#else
        public async Task<global::SpaceTradersApi.Client.My.Ships.Item.Extract.Survey.SurveyPostResponse> PostAsync(global::SpaceTradersApi.Client.Models.Survey body, Action<RequestConfiguration<DefaultQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#endif
            _ = body ?? throw new ArgumentNullException(nameof(body));
            var requestInfo = ToPostRequestInformation(body, requestConfiguration);
            return await RequestAdapter.SendAsync<global::SpaceTradersApi.Client.My.Ships.Item.Extract.Survey.SurveyPostResponse>(requestInfo, global::SpaceTradersApi.Client.My.Ships.Item.Extract.Survey.SurveyPostResponse.CreateFromDiscriminatorValue, default, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Use a survey when extracting resources from a waypoint. This endpoint requires a survey as the payload, which allows your ship to extract specific yields.Send the full survey object as the payload which will be validated according to the signature. If the signature is invalid, or any properties of the survey are changed, the request will fail.
        /// </summary>
        /// <returns>A <see cref="RequestInformation"/></returns>
        /// <param name="body">A resource survey of a waypoint, detailing a specific extraction location and the types of resources that can be found there.</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RequestInformation ToPostRequestInformation(global::SpaceTradersApi.Client.Models.Survey body, Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default)
        {
#nullable restore
#else
        public RequestInformation ToPostRequestInformation(global::SpaceTradersApi.Client.Models.Survey body, Action<RequestConfiguration<DefaultQueryParameters>> requestConfiguration = default)
        {
#endif
            _ = body ?? throw new ArgumentNullException(nameof(body));
            var requestInfo = new RequestInformation(Method.POST, UrlTemplate, PathParameters);
            requestInfo.Configure(requestConfiguration);
            requestInfo.Headers.TryAdd("Accept", "application/json");
            requestInfo.SetContentFromParsable(RequestAdapter, "application/json", body);
            return requestInfo;
        }
        /// <summary>
        /// Returns a request builder with the provided arbitrary URL. Using this method means any other path or query parameters are ignored.
        /// </summary>
        /// <returns>A <see cref="global::SpaceTradersApi.Client.My.Ships.Item.Extract.Survey.SurveyRequestBuilder"/></returns>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        public global::SpaceTradersApi.Client.My.Ships.Item.Extract.Survey.SurveyRequestBuilder WithUrl(string rawUrl)
        {
            return new global::SpaceTradersApi.Client.My.Ships.Item.Extract.Survey.SurveyRequestBuilder(rawUrl, RequestAdapter);
        }
    }
}
