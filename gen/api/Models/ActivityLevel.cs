// <auto-generated/>
using System.Runtime.Serialization;
using System;
namespace SpaceTradersApi.Client.Models
{
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.16.0")]
    /// <summary>The activity level of a trade good. If the good is an import, this represents how strong consumption is. If the good is an export, this represents how strong the production is for the good. When activity is strong, consumption or production is near maximum capacity. When activity is weak, consumption or production is near minimum capacity.</summary>
    public enum ActivityLevel
    {
        [EnumMember(Value = "WEAK")]
        #pragma warning disable CS1591
        WEAK,
        #pragma warning restore CS1591
        [EnumMember(Value = "GROWING")]
        #pragma warning disable CS1591
        GROWING,
        #pragma warning restore CS1591
        [EnumMember(Value = "STRONG")]
        #pragma warning disable CS1591
        STRONG,
        #pragma warning restore CS1591
        [EnumMember(Value = "RESTRICTED")]
        #pragma warning disable CS1591
        RESTRICTED,
        #pragma warning restore CS1591
    }
}
