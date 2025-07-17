using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PetApp.Common.Migrations;

public static class MigrationApplyService
{
    public static async Task ApplyMigrations(this DatabaseFacade database, string assemblyName,
        CancellationToken token = default)
    {
        var migrator = database.GetService<IMigrator>();
        var migrations = await database.GetPendingMigrationsAsync(token);
        var connection = database.GetConnectionString()!;

        foreach (var migration in migrations)
        {
            var script = GetScript(assemblyName, migration.RemovePrefix(), connection);
            if (script is null)
            {
                await migrator.MigrateAsync(migration, token);
                continue;
            }

            script.BeforeMigrationScript();
            await migrator.MigrateAsync(migration, token);
            script.AfterMigrationScript();
        }
    }

    private static MigrationScript? GetScript(string assemblyName, string scriptName, string connectionString)
    {
        try
        {
            var handle = Activator.CreateInstance(assemblyName, $"{assemblyName}.MigrationScripts.{scriptName}", false,
                System.Reflection.BindingFlags.Default, default, [connectionString], default, default);
            if (handle is null) return null;
            var script = (MigrationScript)handle.Unwrap()!;
            return script;
        }
        catch (TypeLoadException)
        {
            return null;
        }
    }

    private static string RemovePrefix(this string migrationName)
    {
        return migrationName[(migrationName.IndexOf('_') + 1)..];
    }
}