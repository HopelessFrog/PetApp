namespace PetApp.Common.Migrations;

public abstract class MigrationScript(string connectionString)
{
    protected readonly string ConnectionString = connectionString;

    public abstract void BeforeMigrationScript();
    public abstract void AfterMigrationScript();
}