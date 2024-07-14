// <auto-generated/>
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using SpaceTradersApi.Client.Models;
using System.Collections.Generic;
using System.IO;
using System;
namespace SpaceTradersApi.Client.My.Contracts.Item.Accept
{
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.16.0")]
    #pragma warning disable CS1591
    public partial class AcceptPostResponse_data : IAdditionalDataHolder, IParsable
    #pragma warning restore CS1591
    {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>Agent details.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::SpaceTradersApi.Client.Models.Agent? Agent { get; set; }
#nullable restore
#else
        public global::SpaceTradersApi.Client.Models.Agent Agent { get; set; }
#endif
        /// <summary>Contract details.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public global::SpaceTradersApi.Client.Models.Contract? Contract { get; set; }
#nullable restore
#else
        public global::SpaceTradersApi.Client.Models.Contract Contract { get; set; }
#endif
        /// <summary>
        /// Instantiates a new <see cref="global::SpaceTradersApi.Client.My.Contracts.Item.Accept.AcceptPostResponse_data"/> and sets the default values.
        /// </summary>
        public AcceptPostResponse_data()
        {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::SpaceTradersApi.Client.My.Contracts.Item.Accept.AcceptPostResponse_data"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::SpaceTradersApi.Client.My.Contracts.Item.Accept.AcceptPostResponse_data CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::SpaceTradersApi.Client.My.Contracts.Item.Accept.AcceptPostResponse_data();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "agent", n => { Agent = n.GetObjectValue<global::SpaceTradersApi.Client.Models.Agent>(global::SpaceTradersApi.Client.Models.Agent.CreateFromDiscriminatorValue); } },
                { "contract", n => { Contract = n.GetObjectValue<global::SpaceTradersApi.Client.Models.Contract>(global::SpaceTradersApi.Client.Models.Contract.CreateFromDiscriminatorValue); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteObjectValue<global::SpaceTradersApi.Client.Models.Agent>("agent", Agent);
            writer.WriteObjectValue<global::SpaceTradersApi.Client.Models.Contract>("contract", Contract);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
