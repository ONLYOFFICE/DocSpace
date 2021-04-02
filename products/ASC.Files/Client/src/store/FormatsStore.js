import { makeAutoObservable } from "mobx";
// import IconFormatsStore from "./IconFormatsStore";
// import MediaViewersFormatsStore from "./MediaViewersFormatsStore";
// import DocserviceStore from "./DocserviceStore";

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
