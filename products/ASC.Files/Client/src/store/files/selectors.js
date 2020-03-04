export const getRootFolders = files => {
  const { my, share, common, project, trash } = files;

  const myIsLeaf = my.folders.length > 0;
  const shareIsLeaf = share.folders.length > 0;
  const commonIsLeaf = common.folders.length > 0;
  const projectIsLeaf = project.folders.length > 0;

  const data = [
    {
      title: my.title,
      id: my.id,
      key: "0-0",
      isLeaf: !myIsLeaf
    },
    {
      title: share.title,
      key: "0-1",
      isLeaf: !shareIsLeaf,
      id: share.id
    },
    {
      title: common.title,
      key: "0-2",
      isLeaf: !commonIsLeaf,
      id: common.id
    },
    {
      title: project.title,
      key: "0-3",
      isLeaf: !projectIsLeaf,
      id: project.id
    },
    {
      title: trash.title,
      id: trash.id,
      key: "0-4",
      isLeaf: true
    }
  ];

  return data;
};
