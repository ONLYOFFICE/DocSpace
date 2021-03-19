import { makeObservable, action, observable } from "mobx";

class MediaViewerDataStore {
  id = null;
  visible = false;

  constructor() {
    makeObservable(this, {
      id: observable,
      visible: observable,

      setMediaViewerData: action,
    });
  }

  setMediaViewerData = (mediaData) => {
    this.id = mediaData.id;
    this.visible = mediaData.visible;
  };
}

export default new MediaViewerDataStore();
