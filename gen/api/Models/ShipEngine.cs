// <auto-generated/>
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System;
namespace SpaceTradersApi.Client.Models
{
    /// <summary>
    /// The engine determines how quickly a ship travels between waypoints.
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.16.0")]
    public partial class ShipEngine : IAdditionalDataHolder, IParsable
    {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The repairable condition of a component. A value of 0 indicates the component needs significant repairs, while a value of 1 indicates the component is in near perfect condition. As the condition of a component is repaired, the overall integrity of the component decreases.</summary>
        public double? Condition { get; set; }
        /// <summary>The description of the engine.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Description { get; set; }
#nullable restore
#else
        public string Description { get; set; }
#endif
        /// <summary>The overall integrity of the component, which determines the performance of the component. A value of 0 indicates that the component is almost completely degraded, while a value of 1 indicates that the component is in near perfect condition. The integrity of the component is non-repairable, and represents permanent wear over time.</summary>
        public double? Integrity { get; set; }
        /// <summary>The name of the engine.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Name { get; set; }
#nullable restore
#else
        public string Name { get; set; }
#endif
        /// <summary>The requirements for installation on a ship</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::SpaceTradersApi.Client.Models.ShipRequirements? Requirements { get; set; }
#nullable restore
#else
        public global::SpaceTradersApi.Client.Models.ShipRequirements Requirements { get; set; }
#endif
        /// <summary>The speed stat of this engine. The higher the speed, the faster a ship can travel from one point to another. Reduces the time of arrival when navigating the ship.</summary>
        public int? Speed { get; set; }
        /// <summary>The symbol of the engine.</summary>
        public global::SpaceTradersApi.Client.Models.ShipEngine_symbol? Symbol { get; set; }
        /// <summary>
        /// Instantiates a new <see cref="global::SpaceTradersApi.Client.Models.ShipEngine"/> and sets the default values.
        /// </summary>
        public ShipEngine()
        {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::SpaceTradersApi.Client.Models.ShipEngine"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::SpaceTradersApi.Client.Models.ShipEngine CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::SpaceTradersApi.Client.Models.ShipEngine();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "condition", n => { Condition = n.GetDoubleValue(); } },
                { "description", n => { Description = n.GetStringValue(); } },
                { "integrity", n => { Integrity = n.GetDoubleValue(); } },
                { "name", n => { Name = n.GetStringValue(); } },
                { "requirements", n => { Requirements = n.GetObjectValue<global::SpaceTradersApi.Client.Models.ShipRequirements>(global::SpaceTradersApi.Client.Models.ShipRequirements.CreateFromDiscriminatorValue); } },
                { "speed", n => { Speed = n.GetIntValue(); } },
                { "symbol", n => { Symbol = n.GetEnumValue<global::SpaceTradersApi.Client.Models.ShipEngine_symbol>(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteDoubleValue("condition", Condition);
            writer.WriteStringValue("description", Description);
            writer.WriteDoubleValue("integrity", Integrity);
            writer.WriteStringValue("name", Name);
            writer.WriteObjectValue<global::SpaceTradersApi.Client.Models.ShipRequirements>("requirements", Requirements);
            writer.WriteIntValue("speed", Speed);
            writer.WriteEnumValue<global::SpaceTradersApi.Client.Models.ShipEngine_symbol>("symbol", Symbol);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
