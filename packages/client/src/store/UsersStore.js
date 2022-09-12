import { makeAutoObservable, runInAction } from "mobx";
import api from "@docspace/common/api";
import {
  EmployeeStatus,
  EmployeeActivationStatus,
} from "@docspace/common/constants";
const { Filter } = api;

class UsersStore {
  peopleStore = null;
  authStore = null;

  users = [];
  providers = [];
  accountsIsIsLoading = false;

  constructor(peopleStore, authStore) {
    this.peopleStore = peopleStore;
    this.authStore = authStore;
    makeAutoObservable(this);
  }

  getUsersList = async (filter) => {
    const filterData = filter ? filter.clone() : Filter.getDefault();

    if (!this.authStore.settingsStore.withPaging) {
      filterData.page = 0;
      filterData.pageCount = 100;
    }

    if (filterData.employeeStatus === EmployeeStatus.Active) {
      filterData.employeeStatus = null;
    }

    if (filterData.group && filterData.group === "root")
      filterData.group = undefined;

    const res = await api.people.getUserList(filterData);
    filterData.total = res.total;

    this.peopleStore.filterStore.setFilterParams(filterData);
    this.peopleStore.selectedGroupStore.setSelectedGroup(
      filterData.group || "root"
    );

    this.setUsers(res.items);
  };

  setUsers = (users) => {
    this.users = users;
  };

  setProviders = (providers) => {
    this.providers = providers;
  };

  employeeWrapperToMemberModel = (profile) => {
    const comment = profile.notes;
    const department = profile.groups
      ? profile.groups.map((group) => group.id)
      : [];
    const worksFrom = profile.workFrom;

    return { ...profile, comment, department, worksFrom };
  };

  createUser = async (user) => {
    const filter = this.peopleStore.filterStore.filter;
    const member = this.employeeWrapperToMemberModel(user);
    let result;
    const res = await api.people.createUser(member);
    result = res;

    await this.peopleStore.targetUserStore.getTargetUser(result.userName);
    await this.getUsersList(filter);
    return Promise.resolve(result);
  };

  removeUser = async (userId, filter) => {
    await api.people.deleteUsers(userId);
    await this.getUsersList(filter);
  };

  updateUserStatus = async (status, userIds) => {
    const user = await api.people.updateUserStatus(status, userIds);

    if (user) {
      const userIndex = this.users.findIndex((x) => x.id === user[0].id);
      if (userIndex !== -1) this.users[userIndex] = user[0];
    }
  };

  updateUserType = async (type, userIds, filter) => {
    await api.people.updateUserType(type, userIds);
    await this.getUsersList(filter);
  };

  updateProfileInUsers = async (updatedProfile) => {
    if (!this.users) {
      return this.getUsersList();
    }
    if (!updatedProfile) {
      updatedProfile = this.peopleStore.targetUserStore.targetUser;
    }
    const { userName } = updatedProfile;
    const oldProfileArr = this.users.filter((u) => u.userName === userName);
    const oldProfile = oldProfileArr[0];
    const newProfile = {};

    for (let key in oldProfile) {
      if (
        updatedProfile.hasOwnProperty(key) &&
        updatedProfile[key] !== oldProfile[key]
      ) {
        newProfile[key] = updatedProfile[key];
      } else {
        newProfile[key] = oldProfile[key];
      }
    }

    const updatedUsers = this.users.map((user) =>
      user.id === newProfile.id ? newProfile : user
    );

    this.setUsers(updatedUsers);
  };

  getStatusType = (user) => {
    if (
      user.status === EmployeeStatus.Active &&
      user.activationStatus === EmployeeActivationStatus.Activated
    ) {
      return "normal";
    } else if (
      user.status === EmployeeStatus.Active &&
      user.activationStatus === EmployeeActivationStatus.Pending
    ) {
      return "pending";
    } else if (user.status === EmployeeStatus.Disabled) {
      return "disabled";
    } else {
      return "unknown";
    }
  };

  getUserRole = (user) => {
    if (user.isOwner) return "owner";
    else if (user.isAdmin) return "admin";
    //TODO: Need refactoring
    else if (user.isVisitor) return "guest";
    else return "user";
  };

  getUserContextOptions = (
    isMySelf,
    isOwner,
    statusType,
    status,
    hasMobileNumber
  ) => {
    const options = [];

    switch (statusType) {
      case "normal":
      case "unknown":
        if (isMySelf) {
          options.push("profile");
        } else {
          options.push("details");
        }

        options.push("separator-1");

        if (isMySelf) {
          options.push("change-name");
        }

        options.push("change-email");
        options.push("change-password");

        if (!isMySelf) {
          options.push("reset-auth");
        }

        options.push("separator-2");

        if (isMySelf && isOwner) {
          options.push("change-owner");
        } else {
          options.push("disable");
        }

        break;
      case "disabled":
        options.push("enable");
        options.push("details");
        options.push("separator-1");
        options.push("reassign-data");
        options.push("delete-personal-data");
        options.push("separator-2");
        options.push("delete-user");
        break;
      case "pending":
        // options.push("edit");
        options.push("invite-again");
        options.push("details");

        options.push("separator-1");

        if (status === EmployeeStatus.Active) {
          options.push("disable");
        } else {
          options.push("enable");
        }

        break;
      default:
        break;
    }

    return options;
  };

  isUserSelected = (id) => {
    return this.peopleStore.selectionStore.selection.some((el) => el.id === id);
  };

  setAccountsIsIsLoading = (accountsIsIsLoading) => {
    this.accountsIsIsLoading = accountsIsIsLoading;
  };

  fetchMoreAccounts = async () => {
    if (!this.hasMoreAccounts || this.accountsIsIsLoading) return;
    // console.log("fetchMoreAccounts");

    this.setAccountsIsIsLoading(true);

    const { filter, setFilterParams } = this.peopleStore.filterStore;

    const newFilter = filter.clone();
    newFilter.page += 1;
    setFilterParams(newFilter);

    const res = await api.people.getUserList(newFilter);

    runInAction(() => {
      this.setUsers([...this.users, ...res.items]);
      this.setAccountsIsIsLoading(false);
    });
  };

  get hasMoreAccounts() {
    return this.peopleList.length < this.peopleStore.filterStore.filterTotal;
  }

  get peopleList() {
    const list = this.users.map((user) => {
      const {
        id,
        displayName,
        avatar,
        email,
        isOwner,
        isAdmin: isAdministrator,
        isVisitor,
        mobilePhone,
        userName,
        activationStatus,
        status,
        groups,
        title,
        firstName,
        lastName,
      } = user;
      const statusType = this.getStatusType(user);
      const role = this.getUserRole(user);
      const isMySelf =
        this.peopleStore.authStore.userStore.user &&
        user.userName === this.peopleStore.authStore.userStore.user.userName;
      //const isViewerAdmin = this.peopleStore.authStore.isAdmin;

      const options = this.getUserContextOptions(
        isMySelf,
        isOwner,
        statusType,
        status,
        !!mobilePhone
      );

      return {
        id,
        status,
        activationStatus,
        statusType,
        role,
        isOwner,
        isAdmin: isAdministrator,
        isVisitor,
        displayName,
        avatar,
        email,
        userName,
        mobilePhone,
        options,
        groups,
        position: title,
        firstName,
        lastName,
      };
    });

    return list;
  }
}

export default UsersStore;
