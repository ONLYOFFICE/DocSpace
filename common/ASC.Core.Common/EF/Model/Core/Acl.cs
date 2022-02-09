namespace ASC.Core.Common.EF
{
    public class Acl : BaseEntity, IMapFrom<AzRecord>
    {
        public Guid SubjectId { get; set; }
        public Guid ActionId { get; set; }
        public string ObjectId { get; set; }
        public AceType Reaction { get; set; }
        public int Tenant { get; set; }

        public override object[] GetKeys() => new object[] { Tenant, SubjectId, ActionId, ObjectId };
    }

    public static class AclExtension
    {
        public static ModelBuilderWrapper AddAcl(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddAcl, Provider.MySql)
                .Add(PgSqlAddAcl, Provider.PostgreSql)
                .HasData(
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("5d5b7260-f7f7-49f1-a1c9-95fbb6a12604"), ActionId = Guid.Parse("ef5e6790-f346-4b6e-b662-722bc28cb0db"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("5d5b7260-f7f7-49f1-a1c9-95fbb6a12604"), ActionId = Guid.Parse("f11e8f3f-46e6-4e55-90e3-09c22ec565bd"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("088d5940-a80f-4403-9741-d610718ce95c"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("08d66144-e1c9-4065-9aa1-aa4bba0a7bc8"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("abef62db-11a8-4673-9d32-ef1d8af19dc0"), ActionId = Guid.Parse("08d75c97-cf3f-494b-90d1-751c941fe2dd"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("abef62db-11a8-4673-9d32-ef1d8af19dc0"), ActionId = Guid.Parse("0d1f72a8-63da-47ea-ae42-0900e4ac72a9"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("abef62db-11a8-4673-9d32-ef1d8af19dc0"), ActionId = Guid.Parse("13e30b51-5b4d-40a5-8575-cb561899eeb1"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("abef62db-11a8-4673-9d32-ef1d8af19dc0"), ActionId = Guid.Parse("19f658ae-722b-4cd8-8236-3ad150801d96"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("abef62db-11a8-4673-9d32-ef1d8af19dc0"), ActionId = Guid.Parse("2c6552b3-b2e0-4a00-b8fd-13c161e337b1"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("388c29d3-c662-4a61-bf47-fc2f7094224a"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("abef62db-11a8-4673-9d32-ef1d8af19dc0"), ActionId = Guid.Parse("40bf31f4-3132-4e76-8d5c-9828a89501a3"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("abef62db-11a8-4673-9d32-ef1d8af19dc0"), ActionId = Guid.Parse("49ae8915-2b30-4348-ab74-b152279364fb"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("63e9f35f-6bb5-4fb1-afaa-e4c2f4dec9bd"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("9018c001-24c2-44bf-a1db-d1121a570e74"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("abef62db-11a8-4673-9d32-ef1d8af19dc0"), ActionId = Guid.Parse("948ad738-434b-4a88-8e38-7569d332910a"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("abef62db-11a8-4673-9d32-ef1d8af19dc0"), ActionId = Guid.Parse("9d75a568-52aa-49d8-ad43-473756cd8903"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("a362fe79-684e-4d43-a599-65bc1f4e167f"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("c426c349-9ad4-47cd-9b8f-99fc30675951"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("d11ebcb9-0e6e-45e6-a6d0-99c41d687598"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("d1f3b53d-d9e2-4259-80e7-d24380978395"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("abef62db-11a8-4673-9d32-ef1d8af19dc0"), ActionId = Guid.Parse("d49f4e30-da10-4b39-bc6d-b41ef6e039d3"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("abef62db-11a8-4673-9d32-ef1d8af19dc0"), ActionId = Guid.Parse("d852b66f-6719-45e1-8657-18f0bb791690"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("e0759a42-47f0-4763-a26a-d5aa665bec35"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("e37239bd-c5b5-4f1e-a9f8-3ceeac209615"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("fbc37705-a04c-40ad-a68c-ce2f0423f397"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("fcac42b8-9386-48eb-a938-d19b3c576912"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("ba74ca02-873f-43dc-8470-8620c156bc67"), ActionId = Guid.Parse("13e30b51-5b4d-40a5-8575-cb561899eeb1"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("ba74ca02-873f-43dc-8470-8620c156bc67"), ActionId = Guid.Parse("49ae8915-2b30-4348-ab74-b152279364fb"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("ba74ca02-873f-43dc-8470-8620c156bc67"), ActionId = Guid.Parse("63e9f35f-6bb5-4fb1-afaa-e4c2f4dec9bd"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("ba74ca02-873f-43dc-8470-8620c156bc67"), ActionId = Guid.Parse("9018c001-24c2-44bf-a1db-d1121a570e74"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("ba74ca02-873f-43dc-8470-8620c156bc67"), ActionId = Guid.Parse("d1f3b53d-d9e2-4259-80e7-d24380978395"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("ba74ca02-873f-43dc-8470-8620c156bc67"), ActionId = Guid.Parse("e0759a42-47f0-4763-a26a-d5aa665bec35"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("ba74ca02-873f-43dc-8470-8620c156bc67"), ActionId = Guid.Parse("e37239bd-c5b5-4f1e-a9f8-3ceeac209615"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("ba74ca02-873f-43dc-8470-8620c156bc67"), ActionId = Guid.Parse("f11e88d7-f185-4372-927c-d88008d2c483"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("ba74ca02-873f-43dc-8470-8620c156bc67"), ActionId = Guid.Parse("f11e8f3f-46e6-4e55-90e3-09c22ec565bd"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), ActionId = Guid.Parse("00e7dfc5-ac49-4fd3-a1d6-98d84e877ac4"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), ActionId = Guid.Parse("14be970f-7af5-4590-8e81-ea32b5f7866d"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), ActionId = Guid.Parse("18ecc94d-6afa-4994-8406-aee9dff12ce2"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), ActionId = Guid.Parse("298530eb-435e-4dc6-a776-9abcd95c70e9"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), ActionId = Guid.Parse("430eaf70-1886-483c-a746-1a18e3e6bb63"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), ActionId = Guid.Parse("557d6503-633b-4490-a14c-6473147ce2b3"), ObjectId = "", Reaction = 0 }/*qwerty*/,
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), ActionId = Guid.Parse("724cbb75-d1c9-451e-bae0-4de0db96b1f7"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), ActionId = Guid.Parse("7cb5c0d1-d254-433f-abe3-ff23373ec631"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), ActionId = Guid.Parse("91b29dcd-9430-4403-b17a-27d09189be88"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), ActionId = Guid.Parse("a18480a4-6d18-4c71-84fa-789888791f45"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), ActionId = Guid.Parse("b630d29b-1844-4bda-bbbe-cf5542df3559"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), ActionId = Guid.Parse("c62a9e8d-b24c-4513-90aa-7ff0f8ba38eb"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), ActionId = Guid.Parse("d7cdb020-288b-41e5-a857-597347618533"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), ActionId = Guid.Parse("662f3db7-9bc8-42cf-84da-2765f563e9b0"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("712d9ec3-5d2b-4b13-824f-71f00191dcca"), ActionId = Guid.Parse("e0759a42-47f0-4763-a26a-d5aa665bec35"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), ActionId = Guid.Parse("0d68b142-e20a-446e-a832-0d6b0b65a164"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("6f05c382-8bca-4469-9424-c807a98c40d7"), ObjectId = "", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), ObjectId = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|1e04460243b54d7982f3fd6208a11960", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), ObjectId = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|6743007c6f954d208c88a8601ce5e76d", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), ObjectId = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|e67be73df9ae4ce18fec1880cb518cb4", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), ObjectId = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|ea942538e68e49079394035336ee0ba8", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), ObjectId = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|32d24cb57ece46069c9419216ba42086", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), ObjectId = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|bf88953e3c434850a3fbb1e43ad53a3e", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), ObjectId = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|2a9230378b2d487b9a225ac0918acf3f", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), ObjectId = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|f4d98afdd336433287783c6945c81ea0", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), ObjectId = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|28b10049dd204f54b986873bc14ccfc7", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), ObjectId = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|3cfd481b46f24a4ab55cb8c0c9def02c", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), ObjectId = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|6a598c7491ae437da5f4ad339bd11bb2", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), ObjectId = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|742cf945cbbc4a5782d61600a12cf8ca", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), ObjectId = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|853b6eb973ee438d9b098ffeedf36234", Reaction = 0 },
                    new Acl { Tenant = -1, SubjectId = Guid.Parse("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), ActionId = Guid.Parse("77777777-32ae-425f-99b5-83176061d1ae"), ObjectId = "ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|46cfa73af32046cf8d5bcd82e1d67f26", Reaction = 0 }
                );

            return modelBuilder;
        }

        public static void MySqlAddAcl(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Acl>(entity =>
            {
                entity.HasKey(e => new { e.Tenant, e.SubjectId, e.ActionId, e.ObjectId })
                    .HasName("PRIMARY");

                entity.ToTable("core_acl");

                entity.Property(e => e.Tenant).HasColumnName("tenant");

                entity.Property(e => e.SubjectId)
                    .HasColumnName("subject")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ActionId)
                    .HasColumnName("action")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ObjectId)
                    .HasColumnName("object")
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Reaction).HasColumnName("acetype");
            });
        }
        public static void PgSqlAddAcl(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Acl>(entity =>
            {
                entity.HasKey(e => new { e.Tenant, e.SubjectId, e.ActionId, e.ObjectId })
                    .HasName("core_acl_pkey");

                entity.ToTable("core_acl", "onlyoffice");

                entity.Property(e => e.Tenant).HasColumnName("tenant");

                entity.Property(e => e.SubjectId)
                    .HasColumnName("subject")
                    .HasMaxLength(38);

                entity.Property(e => e.ActionId)
                    .HasColumnName("action")
                    .HasMaxLength(38);

                entity.Property(e => e.ObjectId)
                    .HasColumnName("object")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Reaction).HasColumnName("acetype");
            });
        }
    }
}
