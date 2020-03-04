const loop = (folders, index) => {
  let i = 0;
  let newFolders = folders.my;
  while (i !== index.length) {
    console.log("newFolders", newFolders);
    newFolders = newFolders.folders[index[i]];
    i++;
  }
  //console.log("newFolders", newFolders);
};

export const getRootFolders = files => {
  const { my, share, common, project, trash } = files;

  const myIsLeaf = my.folders.length > 0;
  const shareIsLeaf = share.folders.length > 0;
  const commonIsLeaf = common.folders.length > 0;
  const projectIsLeaf = project.folders.length > 0;

  // my folder //path ["0", "14", "24"] - ID папок 1 значение рутовая папка
  // get index folder  "0", "2", "0" - Index папок 1 значение рутовая папка
  // my.folders[14].folders[24];

  //const index = ["0", "2", "0"];
  //loop(files, index);

  const data = [
    {
      //folders: my.folders,
      title: my.title,
      id: my.id,
      key: "0-0",
      isLeaf: !myIsLeaf
    },
    {
      //folders: share.folders,
      title: share.title,
      key: "0-1",
      isLeaf: !shareIsLeaf,
      id: share.id
    },
    {
      //folders: common.folders,
      title: common.title,
      key: "0-2",
      isLeaf: !commonIsLeaf,
      id: common.id
    },
    {
      //folders: project.folders,
      title: project.title,
      key: "0-3",
      isLeaf: !projectIsLeaf,
      id: project.id
    },
    { 
      //folders: [],
       title: trash.title, id: trash.id, key: "0-4", isLeaf: true }
  ];

  return data;
};
