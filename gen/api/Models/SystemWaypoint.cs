// <auto-generated/>
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System;
namespace SpaceTradersApi.Client.Models
{
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.16.0")]
    #pragma warning disable CS1591
    public partial class SystemWaypoint : IAdditionalDataHolder, IParsable
    #pragma warning restore CS1591
    {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>Waypoints that orbit this waypoint.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<global::SpaceTradersApi.Client.Models.WaypointOrbital>? Orbitals { get; set; }
#nullable restore
#else
        public List<global::SpaceTradersApi.Client.Models.WaypointOrbital> Orbitals { get; set; }
#endif
        /// <summary>The symbol of the parent waypoint, if this waypoint is in orbit around another waypoint. Otherwise this value is undefined.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Orbits { get; set; }
#nullable restore
#else
        public string Orbits { get; set; }
#endif
        /// <summary>The symbol of the waypoint.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Symbol { get; set; }
#nullable restore
#else
        public string Symbol { get; set; }
#endif
        /// <summary>The type of waypoint.</summary>
        public global::SpaceTradersApi.Client.Models.WaypointType? Type { get; set; }
        /// <summary>Relative position of the waypoint on the system&apos;s x axis. This is not an absolute position in the universe.</summary>
        public int? X { get; set; }
        /// <summary>Relative position of the waypoint on the system&apos;s y axis. This is not an absolute position in the universe.</summary>
        public int? Y { get; set; }
        /// <summary>
        /// Instantiates a new <see cref="global::SpaceTradersApi.Client.Models.SystemWaypoint"/> and sets the default values.
        /// </summary>
        public SystemWaypoint()
        {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::SpaceTradersApi.Client.Models.SystemWaypoint"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::SpaceTradersApi.Client.Models.SystemWaypoint CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::SpaceTradersApi.Client.Models.SystemWaypoint();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "orbitals", n => { Orbitals = n.GetCollectionOfObjectValues<global::SpaceTradersApi.Client.Models.WaypointOrbital>(global::SpaceTradersApi.Client.Models.WaypointOrbital.CreateFromDiscriminatorValue)?.AsList(); } },
                { "orbits", n => { Orbits = n.GetStringValue(); } },
                { "symbol", n => { Symbol = n.GetStringValue(); } },
                { "type", n => { Type = n.GetEnumValue<global::SpaceTradersApi.Client.Models.WaypointType>(); } },
                { "x", n => { X = n.GetIntValue(); } },
                { "y", n => { Y = n.GetIntValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteCollectionOfObjectValues<global::SpaceTradersApi.Client.Models.WaypointOrbital>("orbitals", Orbitals);
            writer.WriteStringValue("orbits", Orbits);
            writer.WriteStringValue("symbol", Symbol);
            writer.WriteEnumValue<global::SpaceTradersApi.Client.Models.WaypointType>("type", Type);
            writer.WriteIntValue("x", X);
            writer.WriteIntValue("y", Y);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
