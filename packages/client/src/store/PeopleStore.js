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
import AccountsContextOptionsStore from "./AccountsContextOptionsStore";
import {
  isMobile,
  isTablet,
  isDesktop,
} from "@docspace/components/utils/device";
import { isMobileRDD } from "react-device-detect";

import toastr from "@docspace/components/toast/toastr";
import { EmployeeStatus } from "@docspace/common/constants";

class PeopleStore {
  contextOptionsStore = null;
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
  infoPanelStore = null;
  setupStore = null;
  isInit = false;
  viewAs = isMobileRDD ? "row" : "table";

  constructor(authStore, infoPanelStore, setupStore) {
    this.authStore = authStore;
    this.groupsStore = new GroupsStore(this);
    this.usersStore = new UsersStore(this, authStore);
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
    this.setupStore = setupStore;

    this.contextOptionsStore = new AccountsContextOptionsStore(this);

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

  onChangeType = (e) => {
    const action = e?.action ? e.action : e?.target?.dataset?.action;

    const { getUsersToMakeEmployees } = this.selectionStore;

    this.changeType(action, getUsersToMakeEmployees);
  };

  changeType = (type, users) => {
    const { setChangeUserTypeDialogVisible, setDialogData } = this.dialogStore;

    let fromType =
      users.length === 1 ? [users[0].role] : users.map((u) => u.role);

    if (users.length > 1) {
      fromType = fromType.filter(
        (item, index) => fromType.indexOf(item) === index && item !== type
      );

      if (fromType.length === 0) fromType = [fromType[0]];
    }

    if (fromType.length === 1 && fromType[0] === type) return;

    const userIDs = users
      .filter((u) => u.role !== type)
      .map((user) => {
        return user?.id ? user.id : user;
      });

    setDialogData({ toType: type, fromType, userIDs });

    setChangeUserTypeDialogVisible(true);
  };

  onChangeStatus = (status) => {
    const users = [];

    if (status === EmployeeStatus.Active) {
      const { getUsersToActivate } = this.selectionStore;

      users.push(...getUsersToActivate);
    } else {
      const { getUsersToDisable } = this.selectionStore;

      users.push(...getUsersToDisable);
    }

    this.changeStatus(status, users);
  };

  changeStatus = (status, users) => {
    const {
      setChangeUserStatusDialogVisible,
      setDialogData,
    } = this.dialogStore;

    const userIDs = users.map((user) => {
      return user?.id ? user.id : user;
    });

    setDialogData({ status, userIDs });

    setChangeUserStatusDialogVisible(true);
  };

  onOpenInfoPanel = () => {
    const { setIsVisible } = this.infoPanelStore;
    setIsVisible(true);
  };

  getHeaderMenu = (t) => {
    const {
      hasUsersToMakeEmployees,
      hasUsersToActivate,
      hasUsersToDisable,
      hasUsersToInvite,
      hasUsersToRemove,
      hasFreeUsers,
    } = this.selectionStore;
    const {
      setSendInviteDialogVisible,
      setDeleteDialogVisible,
    } = this.dialogStore;

    const { isOwner } = this.authStore.userStore.user;

    const { isVisible } = this.infoPanelStore;

    const options = [];

    const adminOption = {
      id: "group-menu_administrator",
      className: "group-menu_drop-down",
      label: t("Common:DocSpaceAdmin"),
      title: t("Common:DocSpaceAdmin"),
      onClick: (e) => this.onChangeType(e),
      "data-action": "admin",
      key: "administrator",
    };
    const managerOption = {
      id: "group-menu_manager",
      className: "group-menu_drop-down",
      label: t("Common:RoomAdmin"),
      title: t("Common:RoomAdmin"),
      onClick: (e) => this.onChangeType(e),
      "data-action": "manager",
      key: "manager",
    };
    const userOption = {
      id: "group-menu_user",
      className: "group-menu_drop-down",
      label: t("Common:User"),
      title: t("Common:User"),
      onClick: (e) => this.onChangeType(e),
      "data-action": "user",
      key: "user",
    };

    isOwner && options.push(adminOption);

    options.push(managerOption);

    hasFreeUsers && options.push(userOption);

    const headerMenu = [
      {
        key: "change-user",
        label: t("ChangeUserTypeDialog:ChangeUserTypeButton"),
        disabled: !hasUsersToMakeEmployees,
        iconUrl: "/static/images/change.to.employee.react.svg",
        withDropDown: true,
        options: options,
      },
      {
        key: "info",
        label: t("Common:Info"),
        disabled:
          isVisible ||
          !(isTablet() || isMobile() || isMobileRDD || !isDesktop()),
        onClick: (item) => this.onOpenInfoPanel(item),
        iconUrl: "images/info.react.svg",
      },
      {
        key: "invite",
        label: t("Common:Invite"),
        disabled: !hasUsersToInvite,
        onClick: () => setSendInviteDialogVisible(true),
        iconUrl: "/static/images/invite.again.react.svg",
      },
      {
        key: "enable",
        label: t("Common:Enable"),
        disabled: !hasUsersToActivate,
        onClick: () => this.onChangeStatus(EmployeeStatus.Active),
        iconUrl: "images/enable.react.svg",
        "data-status": EmployeeStatus.Active,
      },
      {
        key: "disable",
        label: t("PeopleTranslations:DisableUserButton"),
        disabled: !hasUsersToDisable,
        onClick: () => this.onChangeStatus(EmployeeStatus.Disabled),
        iconUrl: "images/disable.react.svg",
        "data-status": EmployeeStatus.Disabled,
      },
      {
        key: "delete",
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
