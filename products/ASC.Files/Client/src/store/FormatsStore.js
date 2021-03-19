import { makeObservable, observable } from "mobx";
import IconFormatsStore from "./IconFormatsStore";
import MediaViewersFormatsStore from "./MediaViewersFormatsStore";
import DocserviceStore from "./DocserviceStore";

class FormatsStore {
  iconFormatsStore = null;
  mediaViewersFormatsStore = null;
  docserviceStore = null;

  constructor() {
    makeObservable(this, {
      iconFormatsStore: observable,
      mediaViewersFormatsStore: observable,
      docserviceStore: observable,
    });

    this.iconFormatsStore = new IconFormatsStore();
    this.mediaViewersFormatsStore = new MediaViewersFormatsStore();
    this.docserviceStore = new DocserviceStore();
  }
}

export default new FormatsStore();
