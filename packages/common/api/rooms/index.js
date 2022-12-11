import { request } from "../client";
import { decodeDisplayName } from "../../utils";
import { FolderType } from "../../constants";

export function getRooms(filter) {
  const options = {
    method: "get",
    url: `/files/rooms?${filter.toApiUrlParams()}`,
  };

  return request(options).then((res) => {
    res.files = decodeDisplayName(res.files);
    res.folders = decodeDisplayName(res.folders);

    if (res.current.rootFolderType === FolderType.Archive) {
      res.folders.forEach((room) => {
        room.isArchive = true;
        room.isLogoLoading = false;
        for (let key in room.logo) {
          room.logo[key] = "";
        }
      });
    } else {
      res.folders.forEach((f, index) => {
        res.folders[index].isLogoLoading = true;
        res.folders[index].logoHandlers = f.logo;

        const newLogos = {};

        for (let key in f.logo) {
          newLogos[key] = "";
        }

        res.folders[index].logo = newLogos;
      });
    }

    return res;
  });
}

export function getLogoIcon(url) {
  const options = {
    // baseURL: combineUrl(AppServerConfig.proxyURL, config.homepage),
    method: "get",
    url: `/products/files/httphandlers${url}`,
    responseType: "text",
  };

  return request(options);
}

export function getRoomInfo(id) {
  const options = {
    method: "get",
    url: `/files/rooms/${id}`,
  };

  return request(options).then(async (res) => {
    return new Promise((resolve, reject) => {
      if (res.rootFolderType === FolderType.Archive) {
        res.isLogoLoading = false;
        for (let key in room.logo) {
          room.logo[key] = "";
        }

        return resolve(res);
      }

      res.isLogoLoading = false;
      res.logoHandlers = res.logo;

      const newLogos = {};

      const actions = [];

      const getLogo = async (key) => {
        const logo = await getLogoIcon(res.logo[key]);

        newLogos[key] = logo;
      };

      for (let key in res.logo) {
        actions.push(getLogo(key));
      }

      return Promise.all(actions).then(() => {
        res.logo = newLogos;

        resolve(res);
      });
    });
  });
}

export function getRoomMembers(id) {
  const options = {
    method: "get",
    url: `/files/rooms/${id}/share`,
  };

  return request(options).then((res) => {
    return res;
  });
}

export function updateRoomMemberRole(id, data) {
  const options = {
    method: "put",
    url: `/files/rooms/${id}/share`,
    data,
  };

  return request(options).then((res) => {
    return res;
  });
}

export function getHistory(module, id) {
  const options = {
    method: "get",
    url: `/feed/filter?module=${module}&withRelated=true&id=${id}`,
  };

  return request(options).then((res) => {
    return res;
  });
}

export function getRoomHistory(id) {
  const options = {
    method: "get",
    url: `/feed/filter?module=rooms&withRelated=true&id=${id}`,
  };

  return request(options).then((res) => {
    return res;
  });
}

export function getFileHistory(id) {
  const options = {
    method: "get",
    url: `/feed/filter?module=files&withRelated=true&id=${id}`,
  };

  return request(options).then((res) => {
    return res;
  });
}

export function createRoom(data) {
  const options = { method: "post", url: `/files/rooms`, data };

  return request(options).then((res) => {
    return res;
  });
}

export function createRoomInThirdpary(id, data) {
  const options = {
    method: "post",
    url: `/files/rooms/thirdparty/${id}`,
    data,
  };

  return request(options).then((res) => {
    return res;
  });
}

export function editRoom(id, data) {
  const options = { method: "put", url: `/files/rooms/${id}`, data };

  return request(options).then((res) => {
    return res;
  });
}

export function pinRoom(id) {
  const options = { method: "put", url: `/files/rooms/${id}/pin` };

  return request(options).then((res) => {
    return res;
  });
}

export function unpinRoom(id) {
  const options = { method: "put", url: `/files/rooms/${id}/unpin` };

  return request(options).then((res) => {
    return res;
  });
}

export function deleteRoom(id, deleteAfter = true) {
  const data = { deleteAfter };

  const options = {
    method: "delete",
    url: `/files/rooms/${id}`,
    data,
  };

  return request(options).then((res) => {
    return res;
  });
}

export function archiveRoom(id, deleteAfter = false) {
  const data = { deleteAfter };

  const options = {
    method: "put",
    url: `/files/rooms/${id}/archive`,
    data,
  };

  return request(options).then((res) => {
    return res;
  });
}

export function unarchiveRoom(id) {
  const data = { deleteAfter: false };
  const options = {
    method: "put",
    url: `/files/rooms/${id}/unarchive`,
    data,
  };

  return request(options).then((res) => {
    return res;
  });
}

export function createTag(name) {
  const data = { name };
  const options = {
    method: "post",
    url: "/files/tags",
    data,
  };

  return request(options).then((res) => {
    return res;
  });
}

export function addTagsToRoom(id, tagArray) {
  const data = { names: tagArray };
  const options = {
    method: "put",
    url: `/files/rooms/${id}/tags`,
    data,
  };

  return request(options).then((res) => {
    return res;
  });
}

export function removeTagsFromRoom(id, tagArray) {
  const data = { names: tagArray };
  const options = {
    method: "delete",
    url: `/files/rooms/${id}/tags`,
    data,
  };

  return request(options).then((res) => {
    return res;
  });
}

export function getTags() {
  const options = {
    method: "get",
    url: "/files/tags",
  };

  return request(options).then((res) => {
    return res;
  });
}

export function uploadRoomLogo(data) {
  const options = {
    method: "post",
    url: `/files/logos`,
    data,
  };

  return request(options).then((res) => {
    return res;
  });
}

export function addLogoToRoom(id, data) {
  const options = {
    method: "post",
    url: `/files/rooms/${id}/logo`,
    data,
  };

  return request(options).then((res) => {
    return res;
  });
}

export function removeLogoFromRoom(id) {
  const options = {
    method: "delete",
    url: `/files/rooms/${id}/logo`,
  };

  return request(options).then((res) => {
    return res;
  });
}

export const setInvitationLinks = async (roomId, linkId, title, access) => {
  const options = {
    method: "put",
    url: `/files/rooms/${roomId}/links`,
    data: {
      linkId,
      title,
      access,
    },
  };

  const res = await request(options);

  return res;
};

export const resendEmailInvitations = async (id, usersIds) => {
  const options = {
    method: "post",
    url: `/files/rooms/${id}/resend`,
    data: {
      usersIds,
    },
  };

  const res = await request(options);

  return res;
};

export const getRoomSecurityInfo = async (id) => {
  const options = {
    method: "get",
    url: `/files/rooms/${id}/share`,
  };

  const res = await request(options);

  return res;
};

export const setRoomSecurity = async (id, data) => {
  const options = {
    method: "put",
    url: `/files/rooms/${id}/share`,
    data,
  };

  const res = await request(options);

  return res;
};

export const acceptInvitationByLink = async () => {
  const options = {
    method: "post",
    url: `/files/rooms/accept`,
  };

  return await request(options);
};
