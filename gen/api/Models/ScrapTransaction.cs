// <auto-generated/>
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System;
namespace SpaceTradersApi.Client.Models
{
    /// <summary>
    /// Result of a scrap transaction.
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.16.0")]
    public partial class ScrapTransaction : IAdditionalDataHolder, IParsable
    {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The symbol of the ship.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? ShipSymbol { get; set; }
#nullable restore
#else
        public string ShipSymbol { get; set; }
#endif
        /// <summary>The timestamp of the transaction.</summary>
        public DateTimeOffset? Timestamp { get; set; }
        /// <summary>The total price of the transaction.</summary>
        public int? TotalPrice { get; set; }
        /// <summary>The symbol of the waypoint.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? WaypointSymbol { get; set; }
#nullable restore
#else
        public string WaypointSymbol { get; set; }
#endif
        /// <summary>
        /// Instantiates a new <see cref="global::SpaceTradersApi.Client.Models.ScrapTransaction"/> and sets the default values.
        /// </summary>
        public ScrapTransaction()
        {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::SpaceTradersApi.Client.Models.ScrapTransaction"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::SpaceTradersApi.Client.Models.ScrapTransaction CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::SpaceTradersApi.Client.Models.ScrapTransaction();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "shipSymbol", n => { ShipSymbol = n.GetStringValue(); } },
                { "timestamp", n => { Timestamp = n.GetDateTimeOffsetValue(); } },
                { "totalPrice", n => { TotalPrice = n.GetIntValue(); } },
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
            writer.WriteStringValue("shipSymbol", ShipSymbol);
            writer.WriteDateTimeOffsetValue("timestamp", Timestamp);
            writer.WriteIntValue("totalPrice", TotalPrice);
            writer.WriteStringValue("waypointSymbol", WaypointSymbol);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
