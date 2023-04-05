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

namespace ASC.Files.Core.Core.Thirdparty;

[Scope]
internal class ThirdPartyTagDao<TFile, TFolder, TItem> : IThirdPartyTagDao
    where TFile : class, TItem
    where TFolder : class, TItem
    where TItem : class
{
    public int TenantID { get; private set; }
    private readonly IDbContextFactory<FilesDbContext> _dbContextFactory;
    private readonly IDaoSelector<TFile, TFolder, TItem> _daoSelector;
    private readonly IDaoBase<TFile, TFolder, TItem> _dao;
    private string PathPrefix { get; set; }

    public ThirdPartyTagDao(
        IDbContextFactory<FilesDbContext> dbContextFactory,
        IDaoSelector<TFile, TFolder, TItem> daoSelector,
        IDaoBase<TFile, TFolder, TItem> dao,
        TenantManager tenantManager
        )
    {
        _dbContextFactory = dbContextFactory;
        _daoSelector = daoSelector;
        _dao = dao;
        TenantID = tenantManager.GetCurrentTenant().Id;
    }

    public void Init(string pathPrefix)
    {
        PathPrefix = pathPrefix;
    }

    public async IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, Folder<string> parentFolder, bool deepSearch)
    {
        var folderId = _daoSelector.ConvertId(parentFolder.Id);

        var filesDbContext = _dbContextFactory.CreateDbContext();
        var entryIDs = await filesDbContext.ThirdpartyIdMapping
                   .Where(r => r.Id.StartsWith(PathPrefix))
                   .Select(r => r.HashId)
                   .ToListAsync();

        if (!entryIDs.Any())
        {
            yield break;
        }

        var q = from r in filesDbContext.Tag
                from l in filesDbContext.TagLink.Where(a => a.TenantId == r.TenantId && a.TagId == r.Id).DefaultIfEmpty()
                where r.TenantId == TenantID && l.TenantId == TenantID && r.Type == TagType.New && entryIDs.Contains(l.EntryId)
                select new { tag = r, tagLink = l };

        if (subject != Guid.Empty)
        {
            q = q.Where(r => r.tag.Owner == subject);
        }

        var qList = await q
            .Distinct()
            .AsAsyncEnumerable()
            .ToListAsync();

        var tags = new List<Tag>();

        foreach (var r in qList)
        {
            tags.Add(new Tag
            {
                Name = r.tag.Name,
                Type = r.tag.Type,
                Owner = r.tag.Owner,
                EntryId = await _dao.MappingIDAsync(r.tagLink.EntryId),
                EntryType = r.tagLink.EntryType,
                Count = r.tagLink.Count,
                Id = r.tag.Id
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
            .Concat(await _dao.GetChildrenAsync(folderId));

        foreach (var e in tags.Where(tag => folderFileIds.Contains(tag.EntryId.ToString())))
        {
            yield return e;
        }
    }
}
