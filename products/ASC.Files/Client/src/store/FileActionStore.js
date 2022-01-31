import { makeObservable, action, observable } from "mobx";

class FileActionStore {
  id = null;
  type = null;
  extension = null;
  title = "";
  templateId = null;

  constructor() {
    makeObservable(this, {
      type: observable,
      extension: observable,
      id: observable,
      title: observable,
      templateId: observable,

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

export default new FileActionStore();
