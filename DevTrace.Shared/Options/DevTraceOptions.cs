namespace DevTrace.Shared.Options;

public class DevTraceOptions
{
    /// <summary>
    /// Base path to the UI. Default: "/devtrace".
    /// </summary>
    public string UiPath { get; set; } = "/devtrace";

    /// <summary>
    /// Namespace to the embedded UI assets.
    /// Default: "DevTrace.UI.wwwroot".
    /// </summary>
    public string EmbeddedUiAssetsNamespace { get; set; } = "DevTrace.UI.wwwroot";

    /// <summary>
    /// Path to the UI host page.
    /// Default: "/_DevTraceHost".
    /// </summary>
    public string UiHostPagePath { get; set; } = "/_Host";

    /// <summary>
    /// Especifys the name of the authorization policy required to download logs.
    /// </summary>
    public string? DownloadLogsAuthorizationPolicyName { get; set; }

    /// <summary>
    /// Especifys the name of the authorization policy required to access the dashboard.
    /// </summary>
    public string? DashboardAuthorizationPolicyName { get; set; }

    /// <summary>
    /// Skip the registration of the Blazor Server UI services.
    /// </summary>
    public bool SkipBlazorServerUIServicesRegistration { get; set; } = false;
}