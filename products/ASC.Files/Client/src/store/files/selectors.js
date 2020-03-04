export const getRootFolders = files => {
  const { my, share, common, project, trash } = files;

  const data = [
    {
      id: my.id,
      key: "0-0",
      title: my.title,
      foldersCount: my.folders.length
    },
    {
      id: share.id,
      key: "0-1",
      title: share.title,
      foldersCount: share.folders.length
    },
    {
      id: common.id,
      key: "0-2",
      title: common.title,
      foldersCount: common.folders.length
    },
    {
      id: project.id,
      key: "0-3",
      title: project.title,
      foldersCount: project.folders.length
    },
    {
      id: trash.id,
      key: "0-4",
      title: trash.title,
      foldersCount: null
    }
  ];

  return data;
};

export const setTreeFilter = (filter, rootFolders) => {
  let newFilter = filter.clone();

  if (newFilter.treeFolders.length === 0) {
    newFilter.treeFolders = [
      { id: rootFolders.my.id, key: "0-0" },
      { id: rootFolders.share.id, key: "0-1" },
      { id: rootFolders.common.id, key: "0-2" },
      { id: rootFolders.project.id, key: "0-3" },
      { id: rootFolders.trash.id, key: "0-4" }
    ];
  }
  return newFilter;
};
