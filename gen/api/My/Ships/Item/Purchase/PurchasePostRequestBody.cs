// <auto-generated/>
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using SpaceTradersApi.Client.Models;
using System.Collections.Generic;
using System.IO;
using System;
namespace SpaceTradersApi.Client.My.Ships.Item.Purchase
{
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.16.0")]
    #pragma warning disable CS1591
    public partial class PurchasePostRequestBody : IAdditionalDataHolder, IParsable
    #pragma warning restore CS1591
    {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The good&apos;s symbol.</summary>
        public global::SpaceTradersApi.Client.Models.TradeSymbol? Symbol { get; set; }
        /// <summary>Amounts of units to purchase.</summary>
        public int? Units { get; set; }
        /// <summary>
        /// Instantiates a new <see cref="global::SpaceTradersApi.Client.My.Ships.Item.Purchase.PurchasePostRequestBody"/> and sets the default values.
        /// </summary>
        public PurchasePostRequestBody()
        {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::SpaceTradersApi.Client.My.Ships.Item.Purchase.PurchasePostRequestBody"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::SpaceTradersApi.Client.My.Ships.Item.Purchase.PurchasePostRequestBody CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::SpaceTradersApi.Client.My.Ships.Item.Purchase.PurchasePostRequestBody();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "symbol", n => { Symbol = n.GetEnumValue<global::SpaceTradersApi.Client.Models.TradeSymbol>(); } },
                { "units", n => { Units = n.GetIntValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteEnumValue<global::SpaceTradersApi.Client.Models.TradeSymbol>("symbol", Symbol);
            writer.WriteIntValue("units", Units);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
