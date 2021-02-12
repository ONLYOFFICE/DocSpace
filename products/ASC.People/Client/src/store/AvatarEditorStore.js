import { action, makeObservable, observable } from "mobx";

class AvatarEditorStore {
  visible = false;

  constructor() {
    makeObservable(this, {
      visible: observable,
      toggleAvatarEditor: action,
    });
  }

  toggleAvatarEditor = (isVisible) => {
    return (this.visible = isVisible);
  };
}

export default AvatarEditorStore;
