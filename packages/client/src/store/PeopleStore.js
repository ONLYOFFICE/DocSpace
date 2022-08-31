import { makeAutoObservable } from "mobx";
import GroupsStore from "./GroupsStore";
import UsersStore from "./UsersStore";
import TargetUserStore from "./TargetUserStore";
import SelectedGroupStore from "./SelectedGroupStore";
import EditingFormStore from "./EditingFormStore";
import FilterStore from "./FilterStore";
import SelectionStore from "./SelectionPeopleStore";
import HeaderMenuStore from "./HeaderMenuStore";
import AvatarEditorStore from "./AvatarEditorStore";
import InviteLinksStore from "./InviteLinksStore";
import DialogStore from "./DialogStore";
import LoadingStore from "./LoadingStore";
import { isMobile } from "react-device-detect";

class PeopleStore {
  authStore = null;
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

  constructor(authStore) {
    this.authStore = authStore;
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
    return this.authStore.isAdmin;
  }

  init = async () => {
    if (this.isInit) return;
    this.isInit = true;

    //this.authStore.settingsStore.setModuleInfo(config.homepage, config.id);

    await this.authStore.settingsStore.getPortalPasswordSettings();

    this.loadingStore.setIsLoaded(true);
  };

  reset = () => {
    this.isInit = false;
    this.loadingStore.setIsLoaded(false);
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
    const {
      userCaption,
      guestCaption,
    } = this.authStore.settingsStore.customNames;
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
        iconUrl: "/static/images/change.to.employee.react.svg",
      },
      {
        label: t("ChangeToGuest", {
          guestCaption,
        }),
        disabled: !hasUsersToMakeGuests,
        onClick: () => setGuestDialogVisible(true),
        iconUrl: "/static/images/change.to.guest.react.svg",
      },
      {
        label: t("LblSetActive"),
        disabled: !hasUsersToActivate,
        onClick: () => setActiveDialogVisible(true),
        iconUrl: "/static/images/enable.react.svg",
      },
      {
        label: t("LblSetDisabled"),
        disabled: !hasUsersToDisable,
        onClick: () => setDisableDialogVisible(true),
        iconUrl: "/static/images/disable.react.svg",
      },
      {
        label: t("LblInviteAgain"),
        disabled: !hasUsersToInvite,
        onClick: () => setSendInviteDialogVisible(true),
        iconUrl: "/static/images/invite.again.react.svg",
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
        iconUrl: "/static/images/send.react.svg",
      },
      {
        label: t("Common:Delete"),
        disabled: !hasUsersToRemove,
        onClick: () => setDeleteDialogVisible(true),
        iconUrl: "/static/images/delete.react.svg",
      },
    ];

    return headerMenu;
  };

  setViewAs = (viewAs) => {
    this.viewAs = viewAs;
  };
}

export default PeopleStore;
