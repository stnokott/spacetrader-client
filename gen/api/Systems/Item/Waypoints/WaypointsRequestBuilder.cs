// <auto-generated/>
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions;
using SpaceTradersApi.Client.Models;
using SpaceTradersApi.Client.Systems.Item.Waypoints.Item;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
namespace SpaceTradersApi.Client.Systems.Item.Waypoints
{
    /// <summary>
    /// Builds and executes requests for operations under \systems\{systemSymbol}\waypoints
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.16.0")]
    public partial class WaypointsRequestBuilder : BaseRequestBuilder
    {
        /// <summary>Gets an item from the SpaceTradersApi.Client.systems.item.waypoints.item collection</summary>
        /// <param name="position">The waypoint symbol</param>
        /// <returns>A <see cref="global::SpaceTradersApi.Client.Systems.Item.Waypoints.Item.WithWaypointSymbolItemRequestBuilder"/></returns>
        public global::SpaceTradersApi.Client.Systems.Item.Waypoints.Item.WithWaypointSymbolItemRequestBuilder this[string position]
        {
            get
            {
                var urlTplParams = new Dictionary<string, object>(PathParameters);
                urlTplParams.Add("waypointSymbol", position);
                return new global::SpaceTradersApi.Client.Systems.Item.Waypoints.Item.WithWaypointSymbolItemRequestBuilder(urlTplParams, RequestAdapter);
            }
        }
        /// <summary>
        /// Instantiates a new <see cref="global::SpaceTradersApi.Client.Systems.Item.Waypoints.WaypointsRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public WaypointsRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/systems/{systemSymbol}/waypoints{?limit*,page*,traits*,type*}", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::SpaceTradersApi.Client.Systems.Item.Waypoints.WaypointsRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public WaypointsRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/systems/{systemSymbol}/waypoints{?limit*,page*,traits*,type*}", rawUrl)
        {
        }
        /// <summary>
        /// Return a paginated list of all of the waypoints for a given system.If a waypoint is uncharted, it will return the `Uncharted` trait instead of its actual traits.
        /// </summary>
        /// <returns>A <see cref="global::SpaceTradersApi.Client.Systems.Item.Waypoints.WaypointsGetResponse"/></returns>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<global::SpaceTradersApi.Client.Systems.Item.Waypoints.WaypointsGetResponse?> GetAsync(Action<RequestConfiguration<global::SpaceTradersApi.Client.Systems.Item.Waypoints.WaypointsRequestBuilder.WaypointsRequestBuilderGetQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#nullable restore
#else
        public async Task<global::SpaceTradersApi.Client.Systems.Item.Waypoints.WaypointsGetResponse> GetAsync(Action<RequestConfiguration<global::SpaceTradersApi.Client.Systems.Item.Waypoints.WaypointsRequestBuilder.WaypointsRequestBuilderGetQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#endif
            var requestInfo = ToGetRequestInformation(requestConfiguration);
            return await RequestAdapter.SendAsync<global::SpaceTradersApi.Client.Systems.Item.Waypoints.WaypointsGetResponse>(requestInfo, global::SpaceTradersApi.Client.Systems.Item.Waypoints.WaypointsGetResponse.CreateFromDiscriminatorValue, default, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Return a paginated list of all of the waypoints for a given system.If a waypoint is uncharted, it will return the `Uncharted` trait instead of its actual traits.
        /// </summary>
        /// <returns>A <see cref="RequestInformation"/></returns>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<global::SpaceTradersApi.Client.Systems.Item.Waypoints.WaypointsRequestBuilder.WaypointsRequestBuilderGetQueryParameters>>? requestConfiguration = default)
        {
#nullable restore
#else
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<global::SpaceTradersApi.Client.Systems.Item.Waypoints.WaypointsRequestBuilder.WaypointsRequestBuilderGetQueryParameters>> requestConfiguration = default)
        {
#endif
            var requestInfo = new RequestInformation(Method.GET, UrlTemplate, PathParameters);
            requestInfo.Configure(requestConfiguration);
            requestInfo.Headers.TryAdd("Accept", "application/json");
            return requestInfo;
        }
        /// <summary>
        /// Returns a request builder with the provided arbitrary URL. Using this method means any other path or query parameters are ignored.
        /// </summary>
        /// <returns>A <see cref="global::SpaceTradersApi.Client.Systems.Item.Waypoints.WaypointsRequestBuilder"/></returns>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        public global::SpaceTradersApi.Client.Systems.Item.Waypoints.WaypointsRequestBuilder WithUrl(string rawUrl)
        {
            return new global::SpaceTradersApi.Client.Systems.Item.Waypoints.WaypointsRequestBuilder(rawUrl, RequestAdapter);
        }
        /// <summary>
        /// Return a paginated list of all of the waypoints for a given system.If a waypoint is uncharted, it will return the `Uncharted` trait instead of its actual traits.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.16.0")]
        public partial class WaypointsRequestBuilderGetQueryParameters 
        {
            /// <summary>How many entries to return per page</summary>
            [QueryParameter("limit")]
            public int? Limit { get; set; }
            /// <summary>What entry offset to request</summary>
            [QueryParameter("page")]
            public int? Page { get; set; }
            /// <summary>Filter waypoints by one or more traits.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("traits")]
            public string? Traits { get; set; }
#nullable restore
#else
            [QueryParameter("traits")]
            public string Traits { get; set; }
#endif
            /// <summary>Filter waypoints by type.</summary>
            [QueryParameter("type")]
            public global::SpaceTradersApi.Client.Models.WaypointType? Type { get; set; }
        }
    }
}
