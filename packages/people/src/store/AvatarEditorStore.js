import { action, makeObservable, observable } from "mobx";

class AvatarEditorStore {
  visible = false;
  avatarMax = null;
  createdAvatar = {
    tmpFile: "",
    image: null,
    defaultWidth: 0,
    defaultHeight: 0,
    x: 0,
    y: 0,
    width: 0,
    height: 0,
  };
  croppedAvatar = null;

  constructor() {
    makeObservable(this, {
      visible: observable,
      avatarMax: observable,
      createdAvatar: observable,
      croppedAvatar: observable,
      toggleAvatarEditor: action,
      setAvatarMax: action,
      setCreatedAvatar: action,
      setCroppedAvatar: action,
    });
  }

  toggleAvatarEditor = (isVisible) => {
    return (this.visible = isVisible);
  };

  setAvatarMax = (avatarMax) => {
    return (this.avatarMax = avatarMax);
  };

  setCreatedAvatar = (avatar) => {
    return (this.createdAvatar = avatar);
  };

  setCroppedAvatar = (croppedAvatar) => {
    return (this.croppedAvatar = croppedAvatar);
  };
}

export default AvatarEditorStore;
