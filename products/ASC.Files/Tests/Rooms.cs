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

using ASC.Core.Users;

namespace ASC.Files.Tests;

[TestFixture]
public partial class BaseFilesTests
{
    [TestCase(DataTests.RoomTitle, DataTests.CustomRoomId)]
    [Category("Room")]
    [Order(1)]
    [Description("post - rooms - create room")]
    public async Task CreateRoom(string title, int roomType)
    {
        var room = await PostAsync<FolderDto<int>>("rooms", new { Title = title, RoomType = roomType });
        Assert.IsNotNull(room);
        Assert.AreEqual(title, room.Title);
    }

    [TestCase(DataTests.RoomId, DataTests.NewRoomTitle)]
    [Category("Room")]
    [Order(2)]
    [Description("put - rooms/{id} - rename room")]
    public async Task RenameRoom(int id, string newTitle)
    {
        var room = await PutAsync<FolderDto<int>>($"rooms/{id}", new { Title = newTitle });
        Assert.IsNotNull(room);
        Assert.AreEqual(newTitle, room.Title);
    }

    [TestCase(DataTests.RoomIdForDelete, DataTests.DeleteAfter)]
    [Category("Room")]
    [Order(3)]
    [Description("delete - rooms/{id} - delete room")]
    public async Task DeleteRoom(int id, bool deleteAfter)
    {
        await DeleteAsync<FileOperationDto>($"rooms/{id}", new { DeleteAfter = deleteAfter });
        var statuses = await WaitLongOperation();
        CheckStatuses(statuses);
    }

    [TestCase(DataTests.RoomIdForArchive, DataTests.DeleteAfter)]
    [Category("Room")]
    [Order(4)]
    [Description("put - rooms/{id}/archive - archive a room")]
    public async Task ArchiveRoom(int id, bool deleteAfter)
    {
        await PutAsync<FileOperationDto>($"rooms/{id}/archive", new { DeleteAfter = deleteAfter });
        var statuses = await WaitLongOperation();
        CheckStatuses(statuses);
    }

    [TestCase(DataTests.RoomIdForUnarchive, DataTests.DeleteAfter)]
    [Category("Room")]
    [Order(5)]
    [Description("put - rooms/{id}/archive - unarchive a room")]
    public async Task UnarchiveRoom(int id, bool deleteAfter)
    {
        await PutAsync<FileOperationDto>($"rooms/{id}/unarchive", new { DeleteAfter = deleteAfter });
        var statuses = await WaitLongOperation();
        CheckStatuses(statuses);
    }

    [TestCase(DataTests.RoomId, DataTests.Notify, DataTests.Message)]
    [Category("Room")]
    [Order(6)]
    [Description("put - rooms/{id}/share - share a room")]
    public async Task ShareRoom(int id, bool notify, string message)
    {
        var newUser = _userManager.GetUsers(Guid.Parse("005bb3ff-7de3-47d2-9b3d-61b9ec8a76a5"));
        var testRoomParamRead = new List<FileShareParams> { new FileShareParams { Access = Core.Security.FileShare.Read, ShareTo = newUser.Id } };

        var share = await PutAsync<IEnumerable<FileShareDto>>($"rooms/{id}/share", new { Share = testRoomParamRead, Notify = notify, SharingMessage = message });
        Assert.IsNotNull(share);
    }

    [TestCase]
    [Category("Room")]
    [Order(7)]
    [Description("get - rooms - get all rooms")]
    public async Task GetAllRooms()
    {
        var rooms = await GetAsync<FolderContentDto<int>>($"rooms");
        Assert.IsNotNull(rooms);
    }

    [TestCase(DataTests.RoomId)]
    [Category("Room")]
    [Order(8)]
    [Description("get - rooms/{id} - get room by id")]
    public async Task GetRoomById(int id)
    {
        var room = await GetAsync<FolderContentDto<int>>($"rooms/{id}");
        Assert.IsNotNull(room);
    }

    [TestCase(DataTests.RoomId, DataTests.TagNames)]
    [Category("Room")]
    [Order(9)]
    [Description("put - rooms/{id}/tags - add tags by id")]
    public async Task AddTagsById(int id, string tagNames)
    {
        var folder = await PutAsync<FolderDto<int>>($"rooms/{id}/tags", new { Names = tagNames.Split(',') });
        Assert.IsTrue(folder.Tags.Count() == 2);
    }

    [TestCase(DataTests.RoomIdWithTags, DataTests.TagNames)]
    [Category("Room")]
    [Order(10)]
    [Description("delete - rooms/{id}/tags - delete tags by id")]
    public async Task DeleteTagsById(int id, string tagNames)
    {
        var folder = await DeleteAsync<FolderDto<int>>($"rooms/{id}/tags", new { Names = tagNames.Split(',') });
        Assert.IsTrue(folder.Tags.Count() == 0);
    }

    [TestCase(DataTests.RoomId)]
    [Category("Room")]
    [Order(11)]
    [Description("put - rooms/{id}/pin - pin a room")]
    public async Task PinRoom(int id)
    {
        var folder = await PutAsync<FolderDto<int>>($"rooms/{id}/pin");
        Assert.IsTrue(folder.Pinned);
    }

    [TestCase(DataTests.RoomIdForUnpin)]
    [Category("Room")]
    [Order(12)]
    [Description("put - rooms/{id}/unpin - unpin a room")]
    public async Task UnpinRoom(int id)
    {
        var folder = await PutAsync<FolderDto<int>>($"rooms/{id}/unpin");
        Assert.IsFalse(folder.Pinned);
    }

    //[TestCase(DataTests.RoomId, DataTests.Image)]
    //[Category("Room")]
    //[Order(16)]
    //[Description("post - rooms/{id}/logo - add logo/ delete - rooms/{id}/logo - delete logo")]
    //public async Task AddAndDeleteLogo(int id, string image)
    //{
    //    CopyImage(image);
    //    var room = await PostAsync<FolderDto<int>>($"rooms/{id}/logo", JsonContent.Create(new { TmpFile = image, X = 0, Y = 0, Width = 180, Height = 180 }));
    //    Assert.IsNotEmpty(room.Logo.Original);

    //    room = await DeleteAsync<FolderDto<int>>($"rooms/{id}/logo", null);

    //    Assert.IsEmpty(room.Logo.Original);
    //}

    //private void CopyImage(string image)
    //{
    //    var imgPath = Path.Combine("..", "..", "..", "Infrastructure", "images", image);
    //    var destPath = Path.Combine("..", "..", "..", "..", "..", "..", "Data.Test", "Products\\Files\\logos\\00/00/01\\temp");
    //    Directory.CreateDirectory(destPath);
    //    File.Copy(imgPath, Path.Combine(destPath, image), true);
    //}
}
