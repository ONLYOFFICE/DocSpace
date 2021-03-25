import { makeObservable, action, observable, computed } from "mobx";
import store from "studio/store";
import { updateTempContent } from "@appserver/common/utils";
import filesStore from "./FilesStore";
import treeFoldersStore from "./TreeFoldersStore";
import config from "../../package.json";

const { auth } = store;
const { isAdmin } = auth;

class InitFilesStore {
  isLoaded = false;
  isLoading = false;
  viewAs = "row";
  dragging = false;
  dragItem = null;
  privacyInstructions = "https://www.onlyoffice.com/private-rooms.aspx";
  isInit = false;

  isDrag = false;
  canDrag = true;

  constructor() {
    const pathname = window.location.pathname.toLowerCase();
    this.isEditor = pathname.indexOf("doceditor") !== -1;

    makeObservable(this, {
      isLoaded: observable,
      isLoading: observable,
      viewAs: observable,
      dragging: observable,
      dragItem: observable,
      privacyInstructions: observable,
      isDrag: observable,
      canDrag: observable,

      tooltipValue: computed,

      initFiles: action,
      setIsLoaded: action,
      setIsLoading: action,
      setViewAs: action,
      setDragging: action,
      setDragItem: action,
      setIsDrag: action,
      setCanDrag: action,
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

  setIsDrag = (isDrag) => {
    this.isDrag = isDrag;
  };

  setCanDrag = (canDrag) => {
    this.canDrag = canDrag;
  };

  get tooltipValue() {
    if (!this.dragging) return null;

    const selectionLength = filesStore.selection.length;
    const elementTitle = selectionLength && filesStore.selection[0].title;
    const singleElement = selectionLength === 1;
    const filesCount = singleElement ? elementTitle : selectionLength;
    const { isShareFolder, isCommonFolder } = treeFoldersStore;

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
    if (this.isInit) return;
    this.isInit = true;

    const isAuthenticated = auth.isAuthenticated;
    const {
      getPortalCultures,
      isDesktopClient,
      getIsEncryptionSupport,
      getEncryptionKeys,
      setModuleInfo,
    } = auth.settingsStore;

    setModuleInfo(config.homepage, config.id);

    const requests = [];

    updateTempContent();
    if (!isAuthenticated) {
      return this.setIsLoaded(true);
    } else {
      updateTempContent(isAuthenticated);
    }

    if (!this.isEditor) {
      requests.push(getPortalCultures(), treeFoldersStore.fetchTreeFolders());

      if (isDesktopClient) {
        requests.push(getIsEncryptionSupport(), getEncryptionKeys());
      }
    }

    return Promise.all(requests);
  };
}

export default new InitFilesStore();
