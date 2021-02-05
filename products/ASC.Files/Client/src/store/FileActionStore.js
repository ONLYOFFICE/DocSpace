import { makeObservable, action, observable } from "mobx";

class FileActionStore {
  id = null;
  type = null;
  extension = null;

  constructor() {
    makeObservable(this, {
      type: observable,
      extension: observable,
      id: observable,

      setAction: action,
    });
  }

  setAction = (fileAction) => {
    const fileActionItems = Object.keys(fileAction);
    for (let key of fileActionItems) {
      if (key in this) {
        this[key] = fileAction[key];
      }
    }
  };
}

export default FileActionStore;
