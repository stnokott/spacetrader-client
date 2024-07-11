// <auto-generated/>
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System;
namespace SpaceTradersApi.Client.Models
{
    /// <summary>
    /// The navigation information of the ship.
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.16.0")]
    public partial class ShipNav : IAdditionalDataHolder, IParsable
    {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The ship&apos;s set speed when traveling between waypoints or systems.</summary>
        public global::SpaceTradersApi.Client.Models.ShipNavFlightMode? FlightMode { get; set; }
        /// <summary>The routing information for the ship&apos;s most recent transit or current location.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::SpaceTradersApi.Client.Models.ShipNavRoute? Route { get; set; }
#nullable restore
#else
        public global::SpaceTradersApi.Client.Models.ShipNavRoute Route { get; set; }
#endif
        /// <summary>The current status of the ship</summary>
        public global::SpaceTradersApi.Client.Models.ShipNavStatus? Status { get; set; }
        /// <summary>The symbol of the system.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? SystemSymbol { get; set; }
#nullable restore
#else
        public string SystemSymbol { get; set; }
#endif
        /// <summary>The symbol of the waypoint.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? WaypointSymbol { get; set; }
#nullable restore
#else
        public string WaypointSymbol { get; set; }
#endif
        /// <summary>
        /// Instantiates a new <see cref="global::SpaceTradersApi.Client.Models.ShipNav"/> and sets the default values.
        /// </summary>
        public ShipNav()
        {
            AdditionalData = new Dictionary<string, object>();
            FlightMode = global::SpaceTradersApi.Client.Models.ShipNavFlightMode.CRUISE;
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::SpaceTradersApi.Client.Models.ShipNav"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::SpaceTradersApi.Client.Models.ShipNav CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::SpaceTradersApi.Client.Models.ShipNav();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "flightMode", n => { FlightMode = n.GetEnumValue<global::SpaceTradersApi.Client.Models.ShipNavFlightMode>(); } },
                { "route", n => { Route = n.GetObjectValue<global::SpaceTradersApi.Client.Models.ShipNavRoute>(global::SpaceTradersApi.Client.Models.ShipNavRoute.CreateFromDiscriminatorValue); } },
                { "status", n => { Status = n.GetEnumValue<global::SpaceTradersApi.Client.Models.ShipNavStatus>(); } },
                { "systemSymbol", n => { SystemSymbol = n.GetStringValue(); } },
                { "waypointSymbol", n => { WaypointSymbol = n.GetStringValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteEnumValue<global::SpaceTradersApi.Client.Models.ShipNavFlightMode>("flightMode", FlightMode);
            writer.WriteObjectValue<global::SpaceTradersApi.Client.Models.ShipNavRoute>("route", Route);
            writer.WriteEnumValue<global::SpaceTradersApi.Client.Models.ShipNavStatus>("status", Status);
            writer.WriteStringValue("systemSymbol", SystemSymbol);
            writer.WriteStringValue("waypointSymbol", WaypointSymbol);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
