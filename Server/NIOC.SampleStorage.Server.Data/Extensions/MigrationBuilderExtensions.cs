using Microsoft.EntityFrameworkCore.Migrations;

namespace NIOC.SampleStorage.Server.Data.Extensions
{
    public static class MigrationBuilderExtensions
    {
        #region AppUsersView
        public static void CreateOrAlterAppUsersView(this MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.Sql(@$"
            //    Create Or Alter View [dbo].[{DbViewNames.vAppUsers}]
            //    As
            //    SELECT Id, FirstName, LastName, NationalCode, Mobile, Email, IsRegistered, ATANumber, PassportNumber , (SELECT Case WHEN SUm(Score) Is NULL THEN 0 ELSE SUm(Score) END as ATAMile FROM ATABooking.dbo.UserAtaMiles WHERE UserId = ATABooking.dbo.Users.Id) as ATAMile
            //    FROM     ATABooking.dbo.Users
            //    GO
            //");
        }

        public static void DropIfExistsAppUsersView(this MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.Sql(@$"
            //    Drop View If Exists dbo.{DbViewNames.ViewName}
            //");
        }
        #endregion
    }
}