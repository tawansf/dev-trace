using Microsoft.EntityFrameworkCore.Migrations;

namespace DevTrace.Persistence.EFCore.Migrations;

public class InitialSchemaWithLogsNamespace: Migration
{
    private const string TargetSchemaName = "devtrace_logs";

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
        {
            migrationBuilder.Sql($"CREATE SCHEMA IF NOT EXISTS \"{TargetSchemaName}\";");
        }
        else if (migrationBuilder.ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
        {
            migrationBuilder.Sql($@"
                    IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = '{TargetSchemaName}')
                    BEGIN
                        EXEC('CREATE SCHEMA [{TargetSchemaName}]');
                    END");
        }
        
        migrationBuilder.CreateTable(
            name: "TraceEvents",
            schema: TargetSchemaName,
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                Message = table.Column<string>(type: "text", nullable: false),
                ExceptionDetails = table.Column<string>(type: "text", nullable: true),
                Source = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                CorrelationId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                ClientIp = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                DurationMs = table.Column<long>(type: "bigint", nullable: false),
                HttpMethod = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                StatusCode = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_TraceEvents", x => x.Id); });
        
        migrationBuilder.CreateIndex(
            name: "IX_TraceEvents_Timestamp",
            schema: TargetSchemaName,
            table: "TraceEvents",
            column: "Timestamp");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "TraceEvents",
            schema: TargetSchemaName);

    }
}