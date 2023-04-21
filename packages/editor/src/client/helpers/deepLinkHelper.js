export const DL_ANDROID = "com.onlyoffice.documents";
export const DL_IOS = "944896972";
export const DL_URL = "oodocuments://openfile";

export const getDeepLink = (location, email, file, url) => {
  const jsonData = {
    portal: location,
    email: email,
    file: {
      id: file.fileId,
    },
    folder: {
      id: file.folderId,
      parentId: file.rootFolderId,
      rootFolderType: file.rootFolderType,
    },
  };
  const deepLinkData = window.btoa(JSON.stringify(jsonData));

  return `${url}?data=${deepLinkData}`;
};
