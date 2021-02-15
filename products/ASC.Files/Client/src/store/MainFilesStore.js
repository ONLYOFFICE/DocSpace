import { makeObservable, action, observable, computed } from "mobx";
import { store, utils } from "asc-web-common";
import FilesStore from "./FilesStore";
import config from "../../package.json";

const { authStore } = store;
const { isAdmin } = authStore;

class MainFilesStore {
  isLoaded = false;
  isLoading = false;
  viewAs = "row";
  dragging = false;
  dragItem = null;

  filesStore = null;
  privacyInstructions = "https://www.onlyoffice.com/private-rooms.aspx";

  constructor() {
    this.filesStore = new FilesStore();

    const pathname = window.location.pathname.toLowerCase();
    this.isEditor = pathname.indexOf("doceditor") !== -1;

    makeObservable(this, {
      filesStore: observable,
      isLoaded: observable,
      isLoading: observable,
      viewAs: observable,
      dragging: observable,
      dragItem: observable,
      privacyInstructions: observable,

      tooltipValue: computed,

      initFiles: action,
      setIsLoaded: action,
      setIsLoading: action,
      setViewAs: action,
      setDragging: action,
      setDragItem: action,
    });
  }

  setIsLoaded = (isLoaded) => {
    this.isLoaded = isLoaded;
  };

  setViewAs = (viewAs) => {
    this.viewAs = viewAs;
  };

  setDragging = (dragging) => {
    this.dragging = dragging;
  };

  setIsLoading = (isLoading) => {
    this.isLoading = isLoading;
  };

  setDragItem = (dragItem) => {
    this.dragItem = dragItem;
  };

  get tooltipValue() {
    if (!this.dragging) return null;

    const selectionLength = this.filesStore.selection.length;
    const elementTitle = selectionLength && this.filesStore.selection[0].title;
    const singleElement = selectionLength === 1;
    const filesCount = singleElement ? elementTitle : selectionLength;
    const { isShareFolder, isCommonFolder } = this.filesStore.treeFoldersStore;

    let operationName;

    if (isAdmin && isShareFolder) {
      operationName = "copy";
    } else if (!isAdmin && (isShareFolder || isCommonFolder)) {
      operationName = "copy";
    } else {
      operationName = "move";
    }

    return operationName === "copy"
      ? singleElement
        ? { label: "TooltipElementCopyMessage", filesCount }
        : { label: "TooltipElementsCopyMessage", filesCount }
      : singleElement
      ? { label: "TooltipElementMoveMessage", filesCount }
      : { label: "TooltipElementsMoveMessage", filesCount };
  }

  initFiles = () => {
    const isAuthenticated = authStore.isAuthenticated;
    const {
      getPortalCultures,
      isDesktopClient,
      getIsEncryptionSupport,
      getEncryptionKeys,
      setModuleInfo,
    } = authStore.settingsStore;

    setModuleInfo(config.homepage, "e67be73d-f9ae-4ce1-8fec-1880cb518cb4");

    const requests = [];

    utils.updateTempContent();
    if (!isAuthenticated) {
      return this.setIsLoaded(true);
    } else {
      utils.updateTempContent(isAuthenticated);
    }

    if (!this.isEditor) {
      requests.push(
        getPortalCultures(),
        this.filesStore.treeFoldersStore.fetchTreeFolders()
      );

      if (isDesktopClient) {
        requests.push(getIsEncryptionSupport(), getEncryptionKeys());
      }
    }

    return Promise.all(requests);
  };
}

export default new MainFilesStore();
