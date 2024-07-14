// <auto-generated/>
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System;
namespace SpaceTradersApi.Client.Models
{
    /// <summary>
    /// A cooldown is a period of time in which a ship cannot perform certain actions.
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.16.0")]
    public partial class Cooldown : IAdditionalDataHolder, IParsable
    {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The date and time when the cooldown expires in ISO 8601 format</summary>
        public DateTimeOffset? Expiration { get; set; }
        /// <summary>The remaining duration of the cooldown in seconds</summary>
        public int? RemainingSeconds { get; set; }
        /// <summary>The symbol of the ship that is on cooldown</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? ShipSymbol { get; set; }
#nullable restore
#else
        public string ShipSymbol { get; set; }
#endif
        /// <summary>The total duration of the cooldown in seconds</summary>
        public int? TotalSeconds { get; set; }
        /// <summary>
        /// Instantiates a new <see cref="global::SpaceTradersApi.Client.Models.Cooldown"/> and sets the default values.
        /// </summary>
        public Cooldown()
        {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::SpaceTradersApi.Client.Models.Cooldown"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::SpaceTradersApi.Client.Models.Cooldown CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::SpaceTradersApi.Client.Models.Cooldown();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "expiration", n => { Expiration = n.GetDateTimeOffsetValue(); } },
                { "remainingSeconds", n => { RemainingSeconds = n.GetIntValue(); } },
                { "shipSymbol", n => { ShipSymbol = n.GetStringValue(); } },
                { "totalSeconds", n => { TotalSeconds = n.GetIntValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteDateTimeOffsetValue("expiration", Expiration);
            writer.WriteIntValue("remainingSeconds", RemainingSeconds);
            writer.WriteStringValue("shipSymbol", ShipSymbol);
            writer.WriteIntValue("totalSeconds", TotalSeconds);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
