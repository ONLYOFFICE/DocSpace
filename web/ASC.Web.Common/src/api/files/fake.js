//import Filter from "./filter";
import { random, system, lorem, internet, name, date } from "faker";

const generateFiles = (count) => {
  return Array.from({ length: count }, () => {
    const file = {
      folderId: random.number(30),
      version: random.number(3),
      versionGroup: random.number(3),
      contentLength: `${random.number(99)}.${random.number(99)} KB`,
      pureContentLength: random.number(9999),
      fileStatus: 0,
      viewUrl: "",
      webUrl: "",
      fileType: 7,
      fileExst: `.${system.commonFileExt()}`,
      comment: lorem.word(),
      id: random.number(500),
      title: lorem.sentence(random.number({ min: 2, max: 10 })).split('.').slice(0, -1).join('.'),
      access: 0,
      shared: false,
      rootFolderType: 5,
      updatedBy: {
        id: random.uuid(),
        displayName: name.findName(),
        title: name.title(),
        avatarSmall: internet.avatar(),
        profileUrl: ""
      },
      created: date.past().toLocaleString("en-US"),
      createdBy: {
        id: random.uuid(),
        displayName: name.findName(),
        title: name.title(),
        avatarSmall: internet.avatar(),
        profileUrl: ""
      },
      updated: date.past().toLocaleString("en-US"),
    };

    return file;
  });
};

const generateFolders = (count) => {
  return Array.from({ length: count }, () => {
    const folder = {
      parentId: random.number(20),
      filesCount: random.number(50),
      foldersCount: random.number(10),
      id: random.number(100000),
      title: lorem.sentence(random.number({ min: 2, max: 10 })),
      access: 0,
      shared: false,
      rootFolderType: 5,
      updatedBy: {
        id: random.uuid(),
        displayName: name.findName(),
        title: name.title(),
        avatarSmall: internet.avatar(),
        profileUrl: ""
      },
      created: date.past().toLocaleString("en-US"),
      createdBy: {
        id: random.uuid(),
        displayName: name.findName(),
        title: name.title(),
        avatarSmall: internet.avatar(),
        profileUrl: ""
      },
      updated: date.past().toLocaleString("en-US"),
    };

    return folder;
  });
};

const current = (rootTitle) => {
  return {
    parentId: 0,
    filesCount: 4,
    foldersCount: 2,
    isShareable: true,
    id: random.uuid(),
    title: rootTitle,
    access: 0,
    shared: false,
    rootFolderType: 5,
    updatedBy: {
      id: random.uuid(),
      displayName: name.findName(),
      title: name.title(),
      avatarSmall: internet.avatar(),
      profileUrl: ""
    },
    created: date.past().toLocaleString("en-US"),
    createdBy: {
      id: random.uuid(),
      displayName: name.findName(),
      title: name.title(),
      avatarSmall: internet.avatar(),
      profileUrl: ""
    },
    updated: date.past().toLocaleString("en-US")
  }
};


export function getFakeElements(filter, rootTitle = "My Documents") {
  const files = random.number({ min: 1, max: 30 });
  const folders = random.number({ min: 1, max: 15 });
  const total = files + folders;
  const fakeFiles = generateFiles(files)
  const fakeFolders = generateFolders(folders);
  return Promise.resolve({
    files: fakeFiles,
    folders: fakeFolders,
    current: current(rootTitle),
    pathParts: [17],
    startIndex: 0,
    count: total,
    total: total
  });
}