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

namespace ASC.Files.Thirdparty.Sharpbox;

[Scope]
internal class SharpBoxTagDao : SharpBoxDaoBase, IThirdPartyTagDao
{
    public SharpBoxTagDao(IServiceProvider serviceProvider,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        IDbContextFactory<FilesDbContext> dbContextManager,
        SetupInfo setupInfo,
        ILogger<SharpBoxDaoBase> monitor,
        FileUtility fileUtility,
        TempPath tempPath,
        AuthContext authContext,
        RegexDaoSelectorBase<ICloudFileSystemEntry, ICloudDirectoryEntry, ICloudFileSystemEntry> regexDaoSelectorBase)
        : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath, authContext, regexDaoSelectorBase)
    {
    }

    public async IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, Folder<string> parentFolder, bool deepSearch)
    {
        var folderId = DaoSelector.ConvertId(parentFolder.Id);

        var filesDbContext = _dbContextFactory.CreateDbContext();
        var entryIds = await Queries.HashIdsAsync(filesDbContext, PathPrefix).ToListAsync();

        if (!entryIds.Any())
        {
            yield break;
        }

        var qList = await Queries.TagLinkTagPairAsync(filesDbContext, _tenantId, entryIds, subject).ToListAsync();

        var tags = new List<Tag>();

        foreach (var r in qList)
        {
            tags.Add(new Tag
            {
                Name = r.Tag.Name,
                Type = r.Tag.Type,
                Owner = r.Tag.Owner,
                EntryId = await MappingIDAsync(r.TagLink.EntryId),
                EntryType = r.TagLink.EntryType,
                Count = r.TagLink.Count,
                Id = r.Tag.Id
            });
        }


        if (deepSearch)
        {
            foreach (var e in tags)
            {
                yield return e;
            }
            yield break;
        }

        var folderFileIds = new[] { parentFolder.Id }
            .Concat(await GetChildrenAsync(folderId));

        foreach (var e in tags.Where(tag => folderFileIds.Contains(tag.EntryId.ToString())))
        {
            yield return e;
        }
    }
}

file class TagLinkTagPair
{
    public DbFilesTag Tag { get; set; }
    public DbFilesTagLink TagLink { get; set; }
}

static file class Queries
{
    public static readonly Func<FilesDbContext, string, IAsyncEnumerable<string>> HashIdsAsync =
        EF.CompileAsyncQuery(
            (FilesDbContext ctx, string idStart) =>
                ctx.ThirdpartyIdMapping
                    .Where(r => r.Id.StartsWith(idStart))
                    .Select(r => r.HashId));

    public static readonly Func<FilesDbContext, int, IEnumerable<string>, Guid, IAsyncEnumerable<TagLinkTagPair>>
        TagLinkTagPairAsync =
            EF.CompileAsyncQuery(
                (FilesDbContext ctx, int tenantId, IEnumerable<string> entryIds, Guid owner) =>
                    (from r in ctx.Tag
                     from l in ctx.TagLink.Where(a => a.TenantId == r.TenantId && a.TagId == r.Id).DefaultIfEmpty()
                     where r.TenantId == tenantId && l.TenantId == tenantId && r.Type == TagType.New &&
                           entryIds.Contains(l.EntryId)
                     select new TagLinkTagPair { Tag = r, TagLink = l })
                    .Where(r => owner == Guid.Empty || r.Tag.Owner == owner)
                    .Distinct());
}
