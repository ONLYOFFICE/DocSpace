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

namespace ASC.Files.Core.EF;

[ElasticsearchType(RelationName = Tables.Tree)]
public class DbFolderTree : BaseEntity
{
    public int FolderId { get; set; }
    public int ParentId { get; set; }
    public int Level { get; set; }

    public DbFolder Folder { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { ParentId, FolderId };
    }
}

public static class DbFolderTreeExtension
{
    public static ModelBuilderWrapper AddDbFolderTree(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<DbFolderTree>().Navigation(e => e.Folder).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddDbFolderTree, Provider.MySql)
            .Add(PgSqlAddDbFolderTree, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddDbFolderTree(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbFolderTree>(entity =>
        {
            entity.HasKey(e => new { e.ParentId, e.FolderId })
                .HasName("PRIMARY");

            entity.ToTable("files_folder_tree")
                .HasCharSet("utf8");

            entity.HasIndex(e => e.FolderId)
                .HasDatabaseName("folder_id");

            entity.Property(e => e.ParentId).HasColumnName("parent_id");

            entity.Property(e => e.FolderId).HasColumnName("folder_id");

            entity.Property(e => e.Level).HasColumnName("level");
        });
    }

    public static void PgSqlAddDbFolderTree(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbFolderTree>(entity =>
        {
            entity.HasKey(e => new { e.ParentId, e.FolderId })
                .HasName("files_folder_tree_pkey");

            entity.ToTable("files_folder_tree", "onlyoffice");

            entity.HasIndex(e => e.FolderId)
                .HasDatabaseName("folder_id_files_folder_tree");

            entity.Property(e => e.ParentId).HasColumnName("parent_id");

            entity.Property(e => e.FolderId).HasColumnName("folder_id");

            entity.Property(e => e.Level).HasColumnName("level");
        });

    }
}
