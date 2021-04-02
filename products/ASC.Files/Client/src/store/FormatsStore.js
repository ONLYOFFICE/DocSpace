import { makeAutoObservable } from "mobx";

class FormatsStore {
  iconFormatsStore;
  mediaViewersFormatsStore;
  docserviceStore;

  constructor(iconFormatsStore, mediaViewersFormatsStore, docserviceStore) {
    makeAutoObservable(this);
    this.iconFormatsStore = iconFormatsStore;
    this.mediaViewersFormatsStore = mediaViewersFormatsStore;
    this.docserviceStore = docserviceStore;
  }
}

export default FormatsStore;
