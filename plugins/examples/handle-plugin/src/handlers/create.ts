const createHandler = (e: any): void => {
  let fileType: string = "folder";
  if (e?.payload?.extension) {
    fileType = e?.payload?.extension;
  }
  alert(`User create ${fileType}`);
};

export default createHandler;
