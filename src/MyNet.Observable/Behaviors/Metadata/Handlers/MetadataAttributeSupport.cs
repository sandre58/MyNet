using MyNet.Utilities.Metadata;

namespace MyNet.Observable.Behaviors.Metadata.Handlers;

/// <summary>
/// Provides support for registering default metadata attribute handlers for the MyNet.Observable library. This class contains a static method that registers default handlers for specific metadata attributes, such as UpdateOnCultureChanged and IgnoreModificationTracking, which are used to configure behaviors related to culture changes and modification tracking in observable objects. By calling the RegisterDefaults method, developers can ensure that these default handlers are registered and available for use when working with metadata attributes in the MyNet.Observable library, allowing for consistent behavior and functionality based on the configured metadata attributes in an application.
/// </summary>
public static class MetadataAttributeSupport
{
    /// <summary>
    /// Registers the default metadata attribute handlers for the MyNet.Observable library. This method should be called to ensure that the default handlers for specific metadata attributes, such as UpdateOnCultureChanged and IgnoreModificationTracking, are registered and available for use when working with metadata attributes in the MyNet.Observable library. By calling this method, developers can ensure that the default behaviors associated with these metadata attributes are properly handled and applied in their applications, allowing for consistent behavior based on the configured metadata attributes in observable objects.
    /// </summary>
    public static void RegisterDefaults()
    {
        MetadataAttributeBootstrapper.Register(new UpdateOnCultureChangedAttributeHandler());
        MetadataAttributeBootstrapper.Register(new IgnoreModificationTrackingAttributeHandler());
    }
}
