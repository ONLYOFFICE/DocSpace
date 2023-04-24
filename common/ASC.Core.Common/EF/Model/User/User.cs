// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Core.Common.EF;

public class User : BaseEntity, IMapFrom<UserInfo>
{
    public int TenantId { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Guid Id { get; set; }
    public bool? Sex { get; set; }
    public DateTime? BirthDate { get; set; }
    public EmployeeStatus Status { get; set; }
    public EmployeeActivationStatus ActivationStatus { get; set; }
    public string Email { get; set; }
    public DateTime? WorkFromDate { get; set; }
    public DateTime? TerminatedDate { get; set; }
    public string Title { get; set; }
    public string CultureName { get; set; }
    public string Contacts { get; set; }
    public string MobilePhone { get; set; }
    public MobilePhoneActivationStatus MobilePhoneActivation { get; set; }
    public string Location { get; set; }
    public string Notes { get; set; }
    public string Sid { get; set; }
    public string SsoNameId { get; set; }
    public string SsoSessionId { get; set; }
    public bool Removed { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime LastModified { get; set; }

    public DbTenant Tenant { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { Id };
    }
}

public static class DbUserExtension
{
    public static ModelBuilderWrapper AddUser(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<User>().Navigation(e => e.Tenant).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddUser, Provider.MySql)
            .Add(PgSqlAddUser, Provider.PostgreSql)
            .HasData(
            new User
            {
                Id = Guid.Parse("66faa6e4-f133-11ea-b126-00ffeec8b4ef"),
                FirstName = "Administrator",
                LastName = "",
                UserName = "administrator",
                TenantId = 1,
                Email = "",
                Status = (EmployeeStatus)1,
                ActivationStatus = 0,
                WorkFromDate = new DateTime(2021, 3, 9, 9, 52, 55, 764, DateTimeKind.Utc).AddTicks(9157),
                LastModified = new DateTime(2021, 3, 9, 9, 52, 55, 765, DateTimeKind.Utc).AddTicks(1420),
                CreateDate = new DateTime(2022, 7, 8)
            });

        return modelBuilder;
    }

    private static void MySqlAddUser(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("core_user")
                .HasCharSet("utf8");

            entity.HasKey(e => e.Id)
                .HasName("PRIMARY");

            entity.HasIndex(e => e.Email)
                .HasDatabaseName("email");

            entity.HasIndex(e => e.LastModified)
                .HasDatabaseName("last_modified");

            entity.HasIndex(e => new { e.TenantId, e.UserName })
                .HasDatabaseName("username");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.ActivationStatus)
                .HasColumnName("activation_status")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.BirthDate)
                .HasColumnName("bithdate")
                .HasColumnType("datetime");

            entity.Property(e => e.Contacts)
                .HasColumnName("contacts")
                .HasColumnType("varchar(1024)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.CreateDate)
                .HasColumnName("create_on")
                .HasColumnType("timestamp");

            entity.Property(e => e.CultureName)
                .HasColumnName("culture")
                .HasColumnType("varchar(20)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");


            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasColumnName("firstname")
                .HasColumnType("varchar(64)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.LastModified)
                .HasColumnName("last_modified")
                .HasColumnType("datetime");

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasColumnName("lastname")
                .HasColumnType("varchar(64)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Location)
                .HasColumnName("location")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Notes)
                .HasColumnName("notes")
                .HasColumnType("varchar(512)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.MobilePhone)
                .HasColumnName("phone")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.MobilePhoneActivation)
                .HasColumnName("phone_activation")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.Removed)
                .HasColumnName("removed")
                .HasColumnType("tinyint(1)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.Sex)
                .HasColumnName("sex")
                .HasColumnType("tinyint(1)");

            entity.Property(e => e.Sid)
                .HasColumnName("sid")
                .HasColumnType("varchar(512)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.SsoNameId)
                .HasColumnName("sso_name_id")
                .HasColumnType("varchar(512)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.SsoSessionId)
                .HasColumnName("sso_session_id")
                .HasColumnType("varchar(512)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasDefaultValueSql("'1'");

            entity.Property(e => e.TenantId).HasColumnName("tenant");

            entity.Property(e => e.TerminatedDate)
                .HasColumnName("terminateddate")
                .HasColumnType("datetime");

            entity.Property(e => e.Title)
                .HasColumnName("title")
                .HasColumnType("varchar(64)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.UserName)
                .IsRequired()
                .HasColumnName("username")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.WorkFromDate)
                .HasColumnName("workfromdate")
                .HasColumnType("datetime");
        });
    }

    private static void PgSqlAddUser(this ModelBuilder modelBuilder)
    {
        modelBuilder.PgSqlAddUserGroup();
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("core_user", "onlyoffice");

            entity.HasIndex(e => e.Email)
                .HasDatabaseName("email");

            entity.HasIndex(e => e.LastModified)
                .HasDatabaseName("last_modified_core_user");

            entity.HasIndex(e => new { e.UserName, e.TenantId })
                .HasDatabaseName("username");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasMaxLength(38);

            entity.Property(e => e.ActivationStatus).HasColumnName("activation_status");

            entity.Property(e => e.BirthDate).HasColumnName("bithdate");

            entity.Property(e => e.Contacts)
                .HasColumnName("contacts")
                .HasMaxLength(1024)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.CreateDate)
                .HasColumnName("create_on")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.CultureName)
                .HasColumnName("culture")
                .HasMaxLength(20)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasColumnName("firstname")
                .HasMaxLength(64);

            entity.Property(e => e.LastModified).HasColumnName("last_modified");

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasColumnName("lastname")
                .HasMaxLength(64);

            entity.Property(e => e.Location)
                .HasColumnName("location")
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Notes)
                .HasColumnName("notes")
                .HasMaxLength(512)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.MobilePhone)
                .HasColumnName("phone")
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.MobilePhoneActivation).HasColumnName("phone_activation");

            entity.Property(e => e.Removed).HasColumnName("removed");

            entity.Property(e => e.Sex).HasColumnName("sex");

            entity.Property(e => e.Sid)
                .HasColumnName("sid")
                .HasMaxLength(512)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.SsoNameId)
                .HasColumnName("sso_name_id")
                .HasMaxLength(512)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.SsoSessionId)
                .HasColumnName("sso_session_id")
                .HasMaxLength(512)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasDefaultValueSql("1");

            entity.Property(e => e.TenantId).HasColumnName("tenant");

            entity.Property(e => e.TerminatedDate).HasColumnName("terminateddate");

            entity.Property(e => e.Title)
                .HasColumnName("title")
                .HasMaxLength(64)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.UserName)
                .IsRequired()
                .HasColumnName("username")
                .HasMaxLength(255);

            entity.Property(e => e.WorkFromDate).HasColumnName("workfromdate");
        });
    }
}
