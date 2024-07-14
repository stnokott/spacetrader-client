// <auto-generated/>
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using SpaceTradersApi.Client.Models;
using System.Collections.Generic;
using System.IO;
using System;
namespace SpaceTradersApi.Client.Systems.Item.Waypoints.Item.JumpGate
{
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.16.0")]
    #pragma warning disable CS1591
    public partial class JumpGateGetResponse : IAdditionalDataHolder, IParsable
    #pragma warning restore CS1591
    {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The data property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::SpaceTradersApi.Client.Models.JumpGate? Data { get; set; }
#nullable restore
#else
        public global::SpaceTradersApi.Client.Models.JumpGate Data { get; set; }
#endif
        /// <summary>
        /// Instantiates a new <see cref="global::SpaceTradersApi.Client.Systems.Item.Waypoints.Item.JumpGate.JumpGateGetResponse"/> and sets the default values.
        /// </summary>
        public JumpGateGetResponse()
        {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::SpaceTradersApi.Client.Systems.Item.Waypoints.Item.JumpGate.JumpGateGetResponse"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::SpaceTradersApi.Client.Systems.Item.Waypoints.Item.JumpGate.JumpGateGetResponse CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::SpaceTradersApi.Client.Systems.Item.Waypoints.Item.JumpGate.JumpGateGetResponse();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "data", n => { Data = n.GetObjectValue<global::SpaceTradersApi.Client.Models.JumpGate>(global::SpaceTradersApi.Client.Models.JumpGate.CreateFromDiscriminatorValue); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteObjectValue<global::SpaceTradersApi.Client.Models.JumpGate>("data", Data);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
