const getTreeGroups = (groups, departments, key) => {
  const treeData = [
    {
      key: key,
      title: departments,
      root: true,
      children:
        groups.map(g => {
          return {
            key: g.id,
            title: g.title || g.name,
            root: false
          };
        }) || []
    }
  ];

  return treeData;
};

const getFakeFolders = count =>
  Array.from(Array(count), (x, index) => {
    return {
      id: `00000000-0000-0000-0000-00000000000${index}`,
      name: `fakeFolder${index}`,
      manager: null
    };
  });

export const getRootFolders = files => {
  const { folders, rootFolders } = files;
  const { my, share, common, project, trash } = rootFolders;

  const myDocumentsFolder = getTreeGroups(folders, my.title, my.id);
  const sharedWithMeFolder = getTreeGroups(
    getFakeFolders(share.foldersCount),
    share.title,
    share.id
  );
  const commonDocumentsFolder = getTreeGroups(
    getFakeFolders(common.foldersCount),
    common.title,
    common.id
  );
  const projectDocumentsFolder = getTreeGroups(
    getFakeFolders(project.foldersCount),
    project.title,
    project.id
  );
  const recycleBinFolder = getTreeGroups([], trash.title, trash.id);

  const data = [
    myDocumentsFolder,
    sharedWithMeFolder,
    commonDocumentsFolder,
    projectDocumentsFolder,
    recycleBinFolder
  ];
  return data;
};
