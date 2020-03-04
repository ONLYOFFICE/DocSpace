export const getRootFolders = files => {
  const { my, share, common, project, trash } = files;

  const myIsLeaf = my.folders.length > 0;
  const shareIsLeaf = share.folders.length > 0;
  const commonIsLeaf = common.folders.length > 0;
  const projectIsLeaf = project.folders.length > 0;

  const data = [
    {
      id: my.id,
      key: "0-0",
      title: my.title,
      isLeaf: !myIsLeaf
    },
    {
      id: share.id,
      key: "0-1",
      title: share.title,
      isLeaf: !shareIsLeaf
    },
    {
      id: common.id,
      key: "0-2",
      title: common.title,
      isLeaf: !commonIsLeaf
    },
    {
      id: project.id,
      key: "0-3",
      title: project.title,
      isLeaf: !projectIsLeaf
    },
    {
      id: trash.id,
      key: "0-4",
      title: trash.title,
      isLeaf: true
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
