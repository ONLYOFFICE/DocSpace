import { makeAutoObservable } from "mobx";

class FileActionStore {
  id = null;
  type = null;
  extension = null;
  title = "";
  templateId = null;

  constructor() {
    makeAutoObservable(this);
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

export default new FileActionStore();
