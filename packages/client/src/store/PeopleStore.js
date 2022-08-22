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
import store from "client/store";
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
  infoPanelStore = null;
  isInit = false;
  viewAs = isMobile ? "row" : "table";

  constructor(infoPanelStore) {
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
    this.infoPanelStore = infoPanelStore;

    makeAutoObservable(this);
  }

  get isPeoplesAdmin() {
    return authStore.isAdmin;
  }

  init = async () => {
    if (this.isInit) return;
    this.isInit = true;

    //authStore.settingsStore.setModuleInfo(config.homepage, config.id);

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

  onInvite = (e) => {
    console.log(e.target.dataset.action);
  };

  getHeaderMenu = (t) => {
    const {
      hasUsersToMakeEmployees,
      hasUsersToActivate,
      hasUsersToDisable,
      hasUsersToInvite,
      hasUsersToRemove,
      getUsersToRemoveIds,
      selection,
    } = this.selectionStore;
    const {
      setActiveDialogVisible,
      setDisableDialogVisible,
      setSendInviteDialogVisible,
      setDeleteDialogVisible,
    } = this.dialogStore;

    const { setVisible, isVisible } = this.infoPanelStore;

    const headerMenu = [
      {
        label: t("ChangeUserTypeDialog:ChangeUserTypeButton"),
        disabled: !hasUsersToMakeEmployees,
        iconUrl: "/static/images/change.to.employee.react.svg",
        withDropDown: true,
        options: [
          {
            id: "group-menu_administrator",
            className: "group-menu_drop-down",
            label: t("Administrator"),
            onClick: this.onInvite,
            "data-action": "administrator",
            key: "administrator",
          },
          {
            id: "group-menu_manager",
            className: "group-menu_drop-down",
            label: t("Manager"),
            onClick: this.onInvite,
            "data-action": "manager",
            key: "manager",
          },
          {
            id: "group-menu_user",
            className: "group-menu_drop-down",
            label: t("Common:User"),
            onClick: this.onInvite,
            "data-action": "user",
            key: "user",
          },
        ],
      },
      {
        label: t("PeopleTranslations:Details"),
        disabled: isVisible,
        onClick: setVisible,
        iconUrl: "images/info.react.svg",
      },
      {
        label: t("Common:Invite"),
        disabled: !hasUsersToInvite,
        onClick: () => setSendInviteDialogVisible(true),
        iconUrl: "/static/images/invite.again.react.svg",
      },
      {
        label: t("Common:Enable"),
        disabled: !hasUsersToActivate,
        onClick: () => setActiveDialogVisible(true),
        iconUrl: "images/enable.react.svg",
      },
      {
        label: t("PeopleTranslations:DisableUserButton"),
        disabled: !hasUsersToDisable,
        onClick: () => setDisableDialogVisible(true),
        iconUrl: "images/disable.react.svg",
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
