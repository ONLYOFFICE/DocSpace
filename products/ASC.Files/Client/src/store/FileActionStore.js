import { makeObservable, action, observable } from "mobx";

class FileActionStore {
  id = null;
  type = null;
  extension = null;
  editingId = null;

  constructor() {
    makeObservable(this, {
      type: observable,
      extension: observable,
      id: observable,
      editingId: observable,

      setAction: action,
      setEditingId: action,
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

  setEditingId = (editingId) => (this.editingId = editingId);
}

export default FileActionStore;
