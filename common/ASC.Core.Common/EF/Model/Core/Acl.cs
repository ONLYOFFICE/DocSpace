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

public class Acl : BaseEntity, IMapFrom<AzRecord>
{
    public int TenantId { get; set; }
    public Guid Subject { get; set; }
    public Guid Action { get; set; }
    public string Object { get; set; }
    public AceType AceType { get; set; }

    public DbTenant Tenant { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { TenantId, Subject, Action, Object };
    }
}

public static class AclExtension
{
    public static ModelBuilderWrapper AddAcl(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<Acl>().Navigation(e => e.Tenant).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddAcl, Provider.MySql)
            .Add(PgSqlAddAcl, Provider.PostgreSql)
            .HasData(
                new Acl { TenantId = -1, Subject = Guid.Parse("5d5b7260-f7f7-49f1-a1c9-95fbb6a12604"), Action = Guid.Parse("ef5e6790-f346-4b6e-b662-722bc28cb0db"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("5d5b7260-f7f7-49f1-a1c9-95fbb6a12604"), Action = Guid.Parse("f11e8f3f-46e6-4e55-90e3-09c22ec565bd"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("088d5940-a80f-4403-9741-d610718ce95c"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("08d66144-e1c9-4065-9aa1-aa4bba0a7bc8"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("abef62db-11a8-4673-9d32-ef1d8af19dc0"), Action = Guid.Parse("08d75c97-cf3f-494b-90d1-751c941fe2dd"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("abef62db-11a8-4673-9d32-ef1d8af19dc0"), Action = Guid.Parse("0d1f72a8-63da-47ea-ae42-0900e4ac72a9"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("abef62db-11a8-4673-9d32-ef1d8af19dc0"), Action = Guid.Parse("13e30b51-5b4d-40a5-8575-cb561899eeb1"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("abef62db-11a8-4673-9d32-ef1d8af19dc0"), Action = Guid.Parse("19f658ae-722b-4cd8-8236-3ad150801d96"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("abef62db-11a8-4673-9d32-ef1d8af19dc0"), Action = Guid.Parse("2c6552b3-b2e0-4a00-b8fd-13c161e337b1"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("388c29d3-c662-4a61-bf47-fc2f7094224a"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("abef62db-11a8-4673-9d32-ef1d8af19dc0"), Action = Guid.Parse("40bf31f4-3132-4e76-8d5c-9828a89501a3"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("abef62db-11a8-4673-9d32-ef1d8af19dc0"), Action = Guid.Parse("49ae8915-2b30-4348-ab74-b152279364fb"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("63e9f35f-6bb5-4fb1-afaa-e4c2f4dec9bd"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("9018c001-24c2-44bf-a1db-d1121a570e74"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("abef62db-11a8-4673-9d32-ef1d8af19dc0"), Action = Guid.Parse("948ad738-434b-4a88-8e38-7569d332910a"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("abef62db-11a8-4673-9d32-ef1d8af19dc0"), Action = Guid.Parse("9d75a568-52aa-49d8-ad43-473756cd8903"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("a362fe79-684e-4d43-a599-65bc1f4e167f"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("c426c349-9ad4-47cd-9b8f-99fc30675951"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("d11ebcb9-0e6e-45e6-a6d0-99c41d687598"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("d1f3b53d-d9e2-4259-80e7-d24380978395"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("abef62db-11a8-4673-9d32-ef1d8af19dc0"), Action = Guid.Parse("d49f4e30-da10-4b39-bc6d-b41ef6e039d3"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("abef62db-11a8-4673-9d32-ef1d8af19dc0"), Action = Guid.Parse("d852b66f-6719-45e1-8657-18f0bb791690"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("e0759a42-47f0-4763-a26a-d5aa665bec35"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("e37239bd-c5b5-4f1e-a9f8-3ceeac209615"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("fbc37705-a04c-40ad-a68c-ce2f0423f397"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("fcac42b8-9386-48eb-a938-d19b3c576912"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("ba74ca02-873f-43dc-8470-8620c156bc67"), Action = Guid.Parse("13e30b51-5b4d-40a5-8575-cb561899eeb1"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("ba74ca02-873f-43dc-8470-8620c156bc67"), Action = Guid.Parse("49ae8915-2b30-4348-ab74-b152279364fb"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("ba74ca02-873f-43dc-8470-8620c156bc67"), Action = Guid.Parse("63e9f35f-6bb5-4fb1-afaa-e4c2f4dec9bd"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("ba74ca02-873f-43dc-8470-8620c156bc67"), Action = Guid.Parse("9018c001-24c2-44bf-a1db-d1121a570e74"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("ba74ca02-873f-43dc-8470-8620c156bc67"), Action = Guid.Parse("d1f3b53d-d9e2-4259-80e7-d24380978395"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("ba74ca02-873f-43dc-8470-8620c156bc67"), Action = Guid.Parse("e0759a42-47f0-4763-a26a-d5aa665bec35"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("ba74ca02-873f-43dc-8470-8620c156bc67"), Action = Guid.Parse("e37239bd-c5b5-4f1e-a9f8-3ceeac209615"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("ba74ca02-873f-43dc-8470-8620c156bc67"), Action = Guid.Parse("f11e88d7-f185-4372-927c-d88008d2c483"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("ba74ca02-873f-43dc-8470-8620c156bc67"), Action = Guid.Parse("f11e8f3f-46e6-4e55-90e3-09c22ec565bd"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), Action = Guid.Parse("00e7dfc5-ac49-4fd3-a1d6-98d84e877ac4"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), Action = Guid.Parse("14be970f-7af5-4590-8e81-ea32b5f7866d"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), Action = Guid.Parse("18ecc94d-6afa-4994-8406-aee9dff12ce2"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), Action = Guid.Parse("298530eb-435e-4dc6-a776-9abcd95c70e9"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), Action = Guid.Parse("430eaf70-1886-483c-a746-1a18e3e6bb63"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), Action = Guid.Parse("557d6503-633b-4490-a14c-6473147ce2b3"), Object = "", AceType = 0 }/*qwerty*/,
                new Acl { TenantId = -1, Subject = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), Action = Guid.Parse("724cbb75-d1c9-451e-bae0-4de0db96b1f7"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), Action = Guid.Parse("7cb5c0d1-d254-433f-abe3-ff23373ec631"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), Action = Guid.Parse("91b29dcd-9430-4403-b17a-27d09189be88"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), Action = Guid.Parse("a18480a4-6d18-4c71-84fa-789888791f45"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), Action = Guid.Parse("b630d29b-1844-4bda-bbbe-cf5542df3559"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), Action = Guid.Parse("c62a9e8d-b24c-4513-90aa-7ff0f8ba38eb"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), Action = Guid.Parse("d7cdb020-288b-41e5-a857-597347618533"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), Action = Guid.Parse("662f3db7-9bc8-42cf-84da-2765f563e9b0"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("712d9ec3-5d2b-4b13-824f-71f00191dcca"), Action = Guid.Parse("e0759a42-47f0-4763-a26a-d5aa665bec35"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), Action = Guid.Parse("0d68b142-e20a-446e-a832-0d6b0b65a164"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("6f05c382-8bca-4469-9424-c807a98c40d7"), Object = "", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|1e04460243b54d7982f3fd6208a11960", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|6743007c6f954d208c88a8601ce5e76d", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|e67be73df9ae4ce18fec1880cb518cb4", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|ea942538e68e49079394035336ee0ba8", AceType = (AceType)1 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|32d24cb57ece46069c9419216ba42086", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|bf88953e3c434850a3fbb1e43ad53a3e", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|2a9230378b2d487b9a225ac0918acf3f", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|f4d98afdd336433287783c6945c81ea0", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|28b10049dd204f54b986873bc14ccfc7", AceType = (AceType)1 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|3cfd481b46f24a4ab55cb8c0c9def02c", AceType = (AceType)1 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|6a598c7491ae437da5f4ad339bd11bb2", AceType = (AceType)1 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|742cf945cbbc4a5782d61600a12cf8ca", AceType = (AceType)1 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|853b6eb973ee438d9b098ffeedf36234", AceType = (AceType)1 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|46cfa73af32046cf8d5bcd82e1d67f26", AceType = 0 },
                new Acl { TenantId = -1, Subject = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), Action = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), Object = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|37620ae5c40b45ce855a39dd7d76a1fa", AceType = 0 });

        return modelBuilder;
    }

    public static void MySqlAddAcl(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Acl>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.Subject, e.Action, e.Object })
                .HasName("PRIMARY");

            entity.ToTable("core_acl")
                .HasCharSet("utf8");

            entity.Property(e => e.TenantId).HasColumnName("tenant");

            entity.Property(e => e.Subject)
                .HasColumnName("subject")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Action)
                .HasColumnName("action")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Object)
                .HasColumnName("object")
                .HasColumnType("varchar(255)")
                .HasDefaultValueSql("''")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.AceType).HasColumnName("acetype");
        });
    }
    public static void PgSqlAddAcl(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Acl>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.Subject, e.Action, e.Object })
                .HasName("core_acl_pkey");

            entity.ToTable("core_acl", "onlyoffice");

            entity.Property(e => e.TenantId).HasColumnName("tenant");

            entity.Property(e => e.Subject)
                .HasColumnName("subject")
                .HasMaxLength(38);

            entity.Property(e => e.Action)
                .HasColumnName("action")
                .HasMaxLength(38);

            entity.Property(e => e.Object)
                .HasColumnName("object")
                .HasMaxLength(255)
                .HasDefaultValueSql("''");

            entity.Property(e => e.AceType).HasColumnName("acetype");
        });
    }
}
