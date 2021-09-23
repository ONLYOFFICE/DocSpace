import { makeAutoObservable } from "mobx";
import GroupsStore from "./GroupsStore";
import UsersStore from "./UsersStore";
import config from "../../package.json";
import TargetUserStore from "./TargetUserStore";
import SelectedGroupStore from "./SelectedGroupStore";
import EditingFormStore from "./EditingFormStore";
import FilterStore from "./FilterStore";
import SelectionStore from "./SelectionStore";
import HeaderMenuStore from "./HeaderMenuStore";
import AvatarEditorStore from "./AvatarEditorStore";
import InviteLinksStore from "./InviteLinksStore";
import store from "studio/store";
import DialogStore from "./DialogStore";
import LoadingStore from "./LoadingStore";
import { isMobile } from "react-device-detect";
const { auth: authStore } = store;

class PeopleStore {
  groupsStore = null;
  usersStore = null;
  targetUserStore = null;
  selectedGroupStore = null;
  editingFormStore = null;
  filterStore = null;
  selectionStore = null;
  headerMenuStore = null;
  avatarEditorStore = null;
  inviteLinksStore = null;
  dialogStore = null;
  loadingStore = null;
  isInit = false;
  viewAs = isMobile ? "row" : "table";

  constructor() {
    this.groupsStore = new GroupsStore(this);
    this.usersStore = new UsersStore(this);
    this.targetUserStore = new TargetUserStore(this);
    this.selectedGroupStore = new SelectedGroupStore(this);
    this.editingFormStore = new EditingFormStore(this);
    this.filterStore = new FilterStore();
    this.selectionStore = new SelectionStore(this);
    this.headerMenuStore = new HeaderMenuStore(this);
    this.avatarEditorStore = new AvatarEditorStore(this);
    this.inviteLinksStore = new InviteLinksStore(this);
    this.dialogStore = new DialogStore();
    this.loadingStore = new LoadingStore();

    makeAutoObservable(this);
  }

  get isPeoplesAdmin() {
    return (
      authStore.userStore.user &&
      authStore.userStore.user.listAdminModules.includes("people")
    );
  }

  init = async () => {
    if (this.isInit) return;
    this.isInit = true;

    authStore.settingsStore.setModuleInfo(config.homepage, config.id);

    await this.groupsStore.getGroupList();
    await authStore.settingsStore.getPortalPasswordSettings();

    this.loadingStore.setIsLoaded(true);
  };

  resetFilter = (withoutGroup = false) => {
    const { filter } = this.filterStore;
    const { getUsersList } = this.usersStore;
    let newFilter;

    if (withoutGroup) {
      const { group } = filter;
      newFilter = filter.reset(group);
    } else {
      newFilter = filter.clone(true);
    }

    return getUsersList(newFilter);
  };

  getHeaderMenu = (t) => {
    const { userCaption, guestCaption } = authStore.settingsStore.customNames;
    const {
      hasUsersToMakeEmployees,
      hasUsersToMakeGuests,
      hasUsersToActivate,
      hasUsersToDisable,
      hasUsersToInvite,
      hasAnybodySelected,
      hasUsersToRemove,
      selection,
    } = this.selectionStore;
    const {
      setEmployeeDialogVisible,
      setGuestDialogVisible,
      setActiveDialogVisible,
      setDisableDialogVisible,
      setSendInviteDialogVisible,
      setDeleteDialogVisible,
    } = this.dialogStore;

    const headerMenu = [
      {
        label: t("ChangeToUser", {
          userCaption,
        }),
        disabled: !hasUsersToMakeEmployees,
        onClick: () => setEmployeeDialogVisible(true),
      },
      {
        label: t("ChangeToGuest", {
          guestCaption,
        }),
        disabled: !hasUsersToMakeGuests,
        onClick: () => setGuestDialogVisible(true),
      },
      {
        label: t("LblSetActive"),
        disabled: !hasUsersToActivate,
        onClick: () => setActiveDialogVisible(true),
      },
      {
        label: t("LblSetDisabled"),
        disabled: !hasUsersToDisable,
        onClick: () => setDisableDialogVisible(true),
      },
      {
        label: t("LblInviteAgain"),
        disabled: !hasUsersToInvite,
        onClick: () => setSendInviteDialogVisible(true),
      },
      {
        label: t("LblSendEmail"),
        disabled: !hasAnybodySelected,
        onClick: () => {
          let str = "";
          for (let item of selection) {
            str += `${item.email},`;
          }
          window.open(`mailto: ${str}`, "_self");
        },
      },
      {
        label: t("Common:Delete"),
        disabled: !hasUsersToRemove,
        onClick: () => setDeleteDialogVisible(true),
      },
    ];

    return headerMenu;
  };

  setViewAs = (viewAs) => {
    this.viewAs = viewAs;
  };
}

export default PeopleStore;
