import { makeAutoObservable } from "mobx";
import toastr from "@docspace/components/toast/toastr";
import { isMobile } from "react-device-detect";
import FilesFilter from "@docspace/common/api/files/filter";
import { getCategoryUrl } from "SRC_DIR/helpers/utils";
import { CategoryType } from "SRC_DIR/helpers/constants";

class CreateEditRoomStore {
  roomParams = null;
  isLoading = null;
  confirmDialogIsLoading = false;
  onClose = null;

  filesStore = null;
  tagsStore = null;
  selectedFolderStore = null;
  filesActionsStore = null;
  thirdPartyStore = null;
  settingsStore = null;
  infoPanelStore = null;
  currentQuotaStore = null;

  constructor(
    filesStore,
    filesActionsStore,
    selectedFolderStore,
    tagsStore,
    thirdPartyStore,
    settingsStore,
    infoPanelStore,
    currentQuotaStore,
    clientLoadingStore
  ) {
    makeAutoObservable(this);

    this.filesStore = filesStore;
    this.tagsStore = tagsStore;
    this.selectedFolderStore = selectedFolderStore;
    this.filesActionsStore = filesActionsStore;
    this.thirdPartyStore = thirdPartyStore;
    this.settingsStore = settingsStore;
    this.infoPanelStore = infoPanelStore;
    this.currentQuotaStore = currentQuotaStore;
    this.clientLoadingStore = clientLoadingStore;
  }

  setRoomParams = (roomParams) => {
    this.roomParams = roomParams;
  };

  setIsLoading = (isLoading) => {
    this.isLoading = isLoading;
  };

  setConfirmDialogIsLoading = (confirmDialogIsLoading) => {
    this.confirmDialogIsLoading = confirmDialogIsLoading;
  };

  setOnClose = (onClose) => {
    this.onClose = onClose;
  };

  setRoomIsCreated = (onClose) => {
    this.onClose = onClose;
  };

  onCreateRoom = async (withConfirm = false) => {
    const roomParams = this.roomParams;

    const { createTag } = this.tagsStore;
    const { id: currentFolderId } = this.selectedFolderStore;
    const { updateCurrentFolder } = this.filesActionsStore;
    const { deleteThirdParty } = this.thirdPartyStore;
    const { withPaging } = this.settingsStore;
    const {
      createRoom,
      createRoomInThirdpary,
      addTagsToRoom,
      calculateRoomLogoParams,
      uploadRoomLogo,
      addLogoToRoom,
    } = this.filesStore;

    const createRoomData = {
      roomType: roomParams.type,
      title: roomParams.title || t("Files:NewRoom"),
    };

    const createTagsData = roomParams.tags
      .filter((t) => t.isNew)
      .map((t) => t.name);
    const addTagsData = roomParams.tags.map((tag) => tag.name);

    const isThirdparty = roomParams.storageLocation.isThirdparty;
    const storageFolderId = roomParams.storageLocation.storageFolderId;
    const thirdpartyAccount = roomParams.storageLocation.thirdpartyAccount;

    const uploadLogoData = new FormData();
    uploadLogoData.append(0, roomParams.icon.uploadedFile);

    try {
      this.setIsLoading(true);
      withConfirm && this.setConfirmDialogIsLoading(true);

      // create room
      let room =
        isThirdparty && storageFolderId
          ? await createRoomInThirdpary(storageFolderId, createRoomData)
          : await createRoom(createRoomData);

      room.isLogoLoading = true;

      // delete thirdparty account if not needed
      if (!isThirdparty && storageFolderId)
        await deleteThirdParty(thirdpartyAccount.providerId);

      // create new tags
      for (let i = 0; i < createTagsData.length; i++)
        await createTag(createTagsData[i]);

      // add new tags to room
      if (!!addTagsData.length)
        room = await addTagsToRoom(room.id, addTagsData);

      // calculate and upload logo to room
      if (roomParams.icon.uploadedFile) {
        await uploadRoomLogo(uploadLogoData).then(async (response) => {
          const url = URL.createObjectURL(roomParams.icon.uploadedFile);
          const img = new Image();
          img.onload = async () => {
            const { x, y, zoom } = roomParams.icon;
            try {
              room = await addLogoToRoom(room.id, {
                tmpFile: response.data,
                ...calculateRoomLogoParams(img, x, y, zoom),
              });
            } catch (e) {
              toastr.error(e);
              this.setIsLoading(false);
              this.setConfirmDialogIsLoading(false);
              this.onClose();
            }

            !withPaging && this.onOpenNewRoom(room);
            URL.revokeObjectURL(img.src);
          };
          img.src = url;
        });
      } else !withPaging && this.onOpenNewRoom(room);

      this.roomIsCreated = true;
    } catch (err) {
      toastr.error(err);
      console.log(err);
      this.setIsLoading(false);
      this.setConfirmDialogIsLoading(false);
      this.onClose();
      this.roomIsCreated = true;
    } finally {
      if (withPaging) await updateCurrentFolder(null, currentFolderId);
    }
  };

  onOpenNewRoom = async (room) => {
    const { setIsSectionFilterLoading } = this.clientLoadingStore;
    const { setView, setIsVisible } = this.infoPanelStore;

    const setIsLoading = (param) => {
      setIsSectionFilterLoading(param);
    };

    setView("info_members");

    const state = {
      isRoot: false,
      title: room.title,

      rootFolderType: room.rootFolderType,
    };

    const newFilter = FilesFilter.getDefault();
    newFilter.folder = room.id;
    setIsLoading(true);

    const path = getCategoryUrl(CategoryType.SharedRoom, room.id);

    window.DocSpace.navigate(`${path}?${newFilter.toUrlParams()}`, { state });

    !isMobile && setIsVisible(true);

    this.setIsLoading(false);
    this.setConfirmDialogIsLoading(false);
    this.onClose();
  };
}

export default CreateEditRoomStore;
