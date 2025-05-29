using DevTrace.Shared.Enums;

namespace DevTrace.Shared.Options;

public sealed class DevTraceOptions
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
    /// Default: "/_Host".
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

    /// <summary>
    /// Specifies the database provider to be used for persistence.
    /// Determines the type of database technology to interact with,
    /// such as PostgreSQL, SQL Server, or None for in-memory storage.
    /// Default: DatabaseProviderType.None.
    /// </summary>
    public DatabaseProviderType DatabaseProvider { get; set; } = DatabaseProviderType.None;

    /// <summary>
    /// The connection string used to configure the database connection for DevTrace.
    /// Applicable when a database provider other than "None" is specified.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Indicates whether the migrations should be automatically applied
    /// when using a database provider. Default is false.
    /// </summary>
    public bool AutoApplyMigrations { get; set; } = false;

    /// <summary>
    /// Specifies the name of the database schema used for storing logs. Default: "devtrace_logs".
    /// </summary>
    public string? DatabaseSchemaName { get; set; } = "devtrace_logs";
}