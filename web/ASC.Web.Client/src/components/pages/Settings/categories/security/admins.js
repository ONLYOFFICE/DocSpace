import React, { Component } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import styled from "styled-components";

import Text from "@appserver/components/text";
import Avatar from "@appserver/components/avatar";
import Row from "@appserver/components/row";
import RowContainer from "@appserver/components/row-container";
import Link from "@appserver/components/link";
import IconButton from "@appserver/components/icon-button";
import toastr from "@appserver/components/toast/toastr";
import SearchInput from "@appserver/components/search-input";
import RequestLoader from "@appserver/components/request-loader";
import Loader from "@appserver/components/loader";
import EmptyScreenContainer from "@appserver/components/empty-screen-container";

import { inject, observer } from "mobx-react";

import { getUserRole } from "@appserver/people/src/helpers/people-helpers";

import isEmpty from "lodash/isEmpty";

import {
  EmployeeStatus,
  EmployeeActivationStatus,
} from "@appserver/common/constants";

import { tablet } from "@appserver/components/utils/device";

const getUserStatus = (user) => {
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

const ToggleContentContainer = styled.div`
  .buttons_container {
    display: flex;
    @media (max-width: 1024px) {
      display: block;
    }
  }
  .toggle_content {
    margin-bottom: 24px;
  }

  .wrapper {
    margin-top: 16px;
  }

  .remove_icon {
    margin-left: 70px;
    @media (max-width: 576px) {
      margin-left: 0px;
    }
  }

  .people-admin_container {
    margin-right: 16px;
    position: relative;

    @media (max-width: 1024px) {
      margin-bottom: 8px;
    }
  }

  .full-admin_container {
    position: relative;
  }

  *,
  *::before,
  *::after {
    box-sizing: border-box;
  }
  .nameAndStatus {
    display: flex;
    align-items: center;

    .statusIcon {
      margin-left: 5px;
    }
  }

  .rowMainContainer {
    height: auto;
  }

  .userRole {
    text-transform: capitalize;
    font-size: 12px;
    color: #d0d5da;
  }

  .row_content {
    justify-content: space-between;
    align-items: center;

    .userData {
      max-width: calc(100% - 300px);
    }
  }

  .iconsWrapper {
    display: flex;
  }

  .actionIconsWrapper {
    display: flex;
    align-items: center;

    .fullAccessWrapper {
      margin-right: 20px;
      display: flex;
      align-items: center;
      padding: 10px 0px;
      cursor: pointer;

      &.owner {
        cursor: default;
      }

      .fullAccessIcon {
        margin-right: 4px;
        width: 16px;
      }
    }

    .hyphen {
      height: 1px;
      width: 8px;
      background-color: #d0d5da;
      margin-right: 20px;
    }

    .iconWrapper {
      display: inline-block;
      margin-right: 32px;

      &:last-child {
        margin-right: 0;
      }
    }
  }

  @media ${tablet} {
    .row_content {
      flex-direction: column;
      align-items: baseline;

      .userData {
        max-width: 100%;
      }
    }

    .actionIconsWrapper {
      .hyphen {
        margin-right: 10px;
      }

      .fullAccessWrapper {
        margin-right: 10px;
        p {
          display: none;
        }
      }

      .iconWrapper {
        margin-right: 22px;
      }
    }

    .wrapper {
      #rowContainer {
        & > div > div {
          align-items: normal;
          padding-top: 10px;

          & > div label {
            margin-top: 8px;
          }
        }
      }
    }
  }
`;

let adminsFromSessionStorage = null;
const fullAccessId = "00000000-0000-0000-0000-000000000000";

class PortalAdmins extends Component {
  constructor(props) {
    super(props);

    this.state = {
      showSelector: false,
      showFullAdminSelector: false,
      isLoading: false,
      showLoader: true,
      selectedOptions: [],
      admins: adminsFromSessionStorage || {},
      hasChanged: false,
      showReminder: false,
      searchValue: "",
    };
  }

  componentDidMount() {
    const { admins, fetchPeople } = this.props;
    const { showReminder } = this.state;

    if (adminsFromSessionStorage && !showReminder) {
      this.setState({
        showReminder: true,
      });
    }

    if (isEmpty(admins, true)) {
      const newFilter = this.onAdminsFilter();
      fetchPeople(newFilter)
        .catch((error) => {
          toastr.error(error);
        })
        .finally(() => {
          this.setState({
            showLoader: false,
          });

          this.checkChanges();

          if (!adminsFromSessionStorage && this.props.admins.length > 0) {
            this.setState({
              admins: this.props.admins,
            });
          }
        });
    } else {
      if (!adminsFromSessionStorage && this.props.admins.length > 0) {
        this.setState({
          admins: this.props.admins,
          showLoader: false,
        });
      } else {
        this.setState({
          admins: adminsFromSessionStorage || {},
          showLoader: false,
        });
      }
    }
  }

  onChangeAdmin = async (userIds, isAdmin, productId) => {
    this.onLoading(true);
    const { changeAdmins } = this.props;
    const newFilter = this.onAdminsFilter();

    await changeAdmins(userIds, productId, isAdmin, newFilter)
      .catch((error) => {
        toastr.error("accessRights onChangeAdmin", error);
      })
      .finally(() => {
        this.onLoading(false);
      });
  };

  onLoading = (status) => {
    this.setState({ isLoading: status });
  };

  onAdminsFilter = () => {
    const { filter } = this.props;

    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.role = "admin";

    return newFilter;
  };

  onModuleIconClick = (userIds, moduleName, isAdmin) => {
    const { admins } = this.state;

    if (admins.length < 1) return false;
    let adminIndex = null;

    for (let i = 0; i < admins.length; i++) {
      if (admins[i].id === userIds[0]) {
        adminIndex = i;
        break;
      }
    }

    this.changeAdminRights(adminIndex, moduleName, isAdmin);
  };

  onFullAccessClick = (admin) => {
    const admins = JSON.parse(JSON.stringify(this.state.admins));

    const adminIndex = admins.findIndex((adminState) => {
      if (admin.id === adminState.id) return true;
      return false;
    });

    if (adminIndex < 0) return false;

    admins[adminIndex].isAdmin = !admin.isAdmin;

    this.setState({
      admins,
    });

    this.checkChanges();
  };

  findAdminById = (admin) => {
    if (admin.id === this.id) return true;
  };

  filterNewAdmins = (admins, newAdmins) => {
    admins.forEach((admin) => {
      for (let t = 0; t < newAdmins.length; t++) {
        if (admin.id === newAdmins[t].id) {
          newAdmins.splice(t, 1);
          break;
        }
      }
    });
  };

  changeAdminRights = (adminIndex, moduleName, isAdmin) => {
    const admins = JSON.parse(JSON.stringify(this.state.admins));
    const listAdminModules = admins[adminIndex].listAdminModules;

    let newListAdminModules = this.createNewListAdminModules(
      isAdmin,
      listAdminModules,
      moduleName
    );
    newListAdminModules.sort();

    admins[adminIndex].listAdminModules = newListAdminModules;
    const newAdmins = [];

    for (const key in admins) {
      newAdmins.push(admins[key]);
    }
    this.setState({
      admins: newAdmins,
    });

    this.checkChanges();
  };

  createNewListAdminModules = (isAdmin, listAdminModules, moduleName) => {
    let newListAdminModules = listAdminModules ? listAdminModules.slice() : [];

    if (!isAdmin) {
      newListAdminModules.push(moduleName);
    } else {
      newListAdminModules = listAdminModules.filter((module) => {
        return module !== moduleName;
      });
    }
    return newListAdminModules;
  };

  onSaveButtonClick = () => {
    const { fetchPeople } = this.props;
    const changedAdmins = this.createChangedAdminsList();
    const changedFullAccessAdmins = this.createChangedFullAccessAdminsList();
    const deletedAdmins = this.createDeletedAdminsList();
    this.saveChanges(
      changedAdmins,
      deletedAdmins,
      changedFullAccessAdmins
    ).then(() => {
      const newFilter = this.onAdminsFilter();
      fetchPeople(newFilter)
        .catch((error) => {
          toastr.error(error);
        })
        .finally(() => {
          this.onCancelClick();
          this.setState({
            showLoader: false,
          });
        });
    });
  };

  onCancelClick = () => {
    adminsFromSessionStorage = "";

    this.setState({
      admins: this.props.admins,
      showReminder: false,
      hasChanged: false,
    });
  };

  onContentRowSelect = (checked, user) => {
    if (checked) {
      this.props.selectUser(user);
    } else {
      this.props.deselectUser(user);
    }
  };

  onSearchChange = (value) => {
    if (this.state.searchValue === value) return false;

    this.setState({
      searchValue: value,
    });
  };

  saveChanges = async (
    changedAdmins,
    deletedAdmins,
    changedFullAccessAdmins
  ) => {
    await this.saveChangedFullAccessAdmins(changedFullAccessAdmins);
    await this.saveChangedAdmins(changedAdmins);
    await this.saveDeletedAdmins(deletedAdmins);
  };

  saveChangedAdmins = async (changedAdmins) => {
    for (let i = 0; i < changedAdmins.length; i++) {
      const adminBeforeChanges = this.getAdminById(
        this.props.admins,
        changedAdmins[i].id
      );

      let changedAdminModules = adminBeforeChanges
        ? this.getChangedAdminModules(adminBeforeChanges, changedAdmins[i])
        : changedAdmins[i].listAdminModules &&
          changedAdmins[i].listAdminModules.slice();

      if (changedAdminModules && changedAdminModules.length > 0) {
        for (const key in changedAdminModules) {
          const currentModule = this.props.modules.find(
            (module) =>
              module.title.toLowerCase() ===
              changedAdminModules[key].toLowerCase()
          );
          if (currentModule)
            await this.onChangeAdmin(
              [changedAdmins[i].id],
              this.isModuleAdmin(changedAdmins[i], changedAdminModules[key]),
              currentModule.id
            );
        }
      }
    }
  };

  saveDeletedAdmins = async (deletedAdmins) => {
    for (let i = 0; i < deletedAdmins.length; i++) {
      await this.onChangeAdmin([deletedAdmins[i].id], false, fullAccessId);
    }
  };

  saveChangedFullAccessAdmins = async (changedAdmins) => {
    for (let i = 0; i < changedAdmins.length; i++) {
      const modulesList = changedAdmins[i].listAdminModules;

      await this.onChangeAdmin(
        [changedAdmins[i].id],
        changedAdmins[i].isAdmin,
        fullAccessId
      );

      if (modulesList && modulesList.length > 0) {
        for (const key in modulesList) {
          const currentModule = this.props.modules.find(
            (module) =>
              module.title.toLowerCase() === modulesList[key].toLowerCase()
          );
          if (currentModule)
            await this.onChangeAdmin(
              [changedAdmins[i].id],
              this.isModuleAdmin(changedAdmins[i], modulesList[key]),
              currentModule.id
            );
        }
      }
    }
  };

  getChangedAdminModules = (adminBeforeChanges, admin) => {
    const modulesListBeforeChanges = adminBeforeChanges.listAdminModules
      ? adminBeforeChanges.listAdminModules.slice()
      : [];
    const modulesList = admin.listAdminModules
      ? admin.listAdminModules.slice()
      : [];
    let newListAdminModules = [];

    newListAdminModules = modulesList.filter((module) => {
      let hasModule = false;

      for (let i = 0; i < modulesListBeforeChanges.length; i++) {
        if (modulesListBeforeChanges[i] === module) {
          hasModule = true;
          modulesListBeforeChanges.splice(i, 1);
          break;
        }
      }

      return !hasModule;
    });

    if (modulesListBeforeChanges.length > 0) {
      newListAdminModules = newListAdminModules
        .concat(modulesListBeforeChanges)
        .sort();
    }

    return newListAdminModules;
  };

  createChangedAdminsList = () => {
    const { admins } = this.state;
    let changedAdmins = [];

    for (let i = 0; i < admins.length; i++) {
      const adminBeforeChanges = this.getAdminById(
        this.props.admins,
        admins[i].id
      );

      if (adminBeforeChanges) {
        if (
          adminBeforeChanges.isAdmin === admins[i].isAdmin &&
          !this.compareObjects(admins[i], adminBeforeChanges)
        ) {
          changedAdmins.push(admins[i]);
        }
      } else if (!this.compareObjects(admins[i], adminBeforeChanges)) {
        changedAdmins.push(admins[i]);
      }
    }

    if (changedAdmins) return changedAdmins;
  };

  createDeletedAdminsList = () => {
    const { admins } = this.props;
    let deletedAdmins = [];

    if (!admins && admins.length < 1) return deletedAdmins;

    for (let i = 0; i < admins.length; i++) {
      const adminAfterChanges = this.getAdminById(
        this.state.admins,
        admins[i].id
      );
      if (!adminAfterChanges) deletedAdmins.push(admins[i]);
    }

    return deletedAdmins;
  };

  createChangedFullAccessAdminsList = () => {
    const { admins } = this.state;
    let changedAdmins = [];

    for (let i = 0; i < admins.length; i++) {
      const adminBeforeChanges = this.getAdminById(
        this.props.admins,
        admins[i].id
      );

      if (
        (!adminBeforeChanges && admins[i].isAdmin) ||
        (adminBeforeChanges && adminBeforeChanges.isAdmin !== admins[i].isAdmin)
      ) {
        changedAdmins.push(admins[i]);
      }
    }

    if (changedAdmins) return changedAdmins;
  };

  getAdminById = (admins, id) => {
    let currentAdmin;

    admins.findIndex((admin) => {
      for (let key in admin) {
        if (key === "id" && admin[key] === id) {
          currentAdmin = JSON.parse(JSON.stringify(admin));
          return true;
        }
      }
      return false;
    });

    if (currentAdmin) return currentAdmin;
  };

  isModuleAdmin = (user, moduleName) => {
    let isModuleAdmin = false;

    if (!user.listAdminModules) return false;

    for (let i = 0; i < user.listAdminModules.length; i++) {
      if (user.listAdminModules[i] === moduleName) {
        isModuleAdmin = true;
        break;
      }
    }

    return isModuleAdmin;
  };

  checkChanges = () => {
    let hasChanged =
      adminsFromSessionStorage &&
      !this.compareObjects(adminsFromSessionStorage, this.props.admins);

    if (hasChanged !== this.state.hasChanged) {
      this.setState({
        hasChanged: hasChanged,
      });
    }
  };

  compareObjects = (obj1, obj2) => {
    return JSON.stringify(obj1) === JSON.stringify(obj2);
  };

  getFilteredAdmins = (admins, searchValue) => {
    const filteredAdmins = admins.filter((admin) => {
      if (
        admin.displayName.toLowerCase().indexOf(searchValue.toLowerCase()) !==
        -1
      )
        return true;
      return false;
    });

    return filteredAdmins;
  };

  render() {
    const { t } = this.props;
    const {
      isLoading,
      showLoader,
      admins,
      hasChanged,
      showReminder,
      searchValue,
    } = this.state;

    const filteredAdmins = searchValue
      ? this.getFilteredAdmins(admins, searchValue)
      : admins;

    const checked = false;

    return (
      <>
        {showLoader ? (
          <Loader className="pageLoader" type="rombs" size="40px" />
        ) : (
          <>
            <RequestLoader
              visible={isLoading}
              zIndex={256}
              loaderSize="16px"
              loaderColor={"#999"}
              label={`${t("LoadingProcessing")} ${t("LoadingDescription")}`}
              fontSize="12px"
              fontColor={"#999"}
              className="page_loader"
            />

            <ToggleContentContainer>
              <SearchInput
                className="filter_container"
                placeholder="Search added employees"
                onChange={this.onSearchChange}
                onClearSearch={this.onSearchChange}
                value={searchValue}
              />

              {filteredAdmins.length > 0 ? (
                <>
                  <div className="wrapper">
                    <RowContainer useReactWindow={false}>
                      {filteredAdmins.map((user) => {
                        const element = (
                          <Avatar
                            size="small"
                            role={getUserRole(user)}
                            userName={user.displayName}
                            source={user.avatar}
                          />
                        );

                        const nameColor =
                          getUserStatus(user) === "pending"
                            ? "#A3A9AE"
                            : "#333333";

                        return (
                          <Row
                            key={user.id}
                            status={user.status}
                            onSelect={this.onContentRowSelect}
                            data={user}
                            element={element}
                            checkbox={true}
                            checked={checked}
                            contextButtonSpacerWidth={"0px"}
                          >
                            <>
                              <div className="userData">
                                <div className="nameAndStatus">
                                  <Link
                                    isTextOverflow={true}
                                    type="page"
                                    title={user.displayName}
                                    isBold={true}
                                    fontSize="15px"
                                    color={nameColor}
                                    href={user.profileUrl}
                                  >
                                    {user.displayName}
                                  </Link>
                                  {/*getUserStatus(user) === "pending" && (
                                    <Icons.SendClockIcon
                                      className="statusIcon"
                                      size="small"
                                      isfill={true}
                                      color="#3B72A7"
                                    />
                                  )*/}
                                  {/*getUserStatus(user) === "disabled" && (
                                    <Icons.CatalogSpamIcon
                                      className="statusIcon"
                                      size="small"
                                      isfill={true}
                                      color="#3B72A7"
                                    />
                                  )*/}
                                </div>
                                <div>
                                  <Text truncate={true} className="userRole">
                                    {getUserRole(user)}
                                  </Text>
                                </div>
                              </div>
                              <div className="actionIconsWrapper">
                                {getUserRole(user) === "owner" ? (
                                  <div className="fullAccessWrapper owner">
                                    <IconButton
                                      iconName="OwnerSettingsIcon"
                                      isClickable={false}
                                      className="fullAccessIcon"
                                      size="medium"
                                      isFill={true}
                                      color="#7A95B0"
                                    />
                                    <Text
                                      as="p"
                                      truncate={true}
                                      color="#7A95B0"
                                      font-size="11px"
                                      fontWeight={700}
                                    >
                                      Owner
                                    </Text>
                                  </div>
                                ) : (
                                  <div
                                    className="fullAccessWrapper"
                                    onClick={this.onFullAccessClick.bind(
                                      this,
                                      user
                                    )}
                                  >
                                    <IconButton
                                      iconName="ActionsFullAccessIcon"
                                      isClickable={false}
                                      className="fullAccessIcon"
                                      size="medium"
                                      isFill={true}
                                      color={
                                        user.isAdmin ? "#316DAA" : "#D0D5DA"
                                      }
                                    />
                                    <Text
                                      as="p"
                                      truncate={true}
                                      color={
                                        user.isAdmin ? "#316DAA" : "#D0D5DA"
                                      }
                                      font-size="11px"
                                      fontWeight={700}
                                    >
                                      Full access
                                    </Text>
                                  </div>
                                )}
                                <div className="hyphen"></div>
                                <div className="iconsWrapper">
                                  <div className="iconWrapper">
                                    <IconButton
                                      iconName="ActionsDocumentsSettingsIcon"
                                      size={14}
                                      color={
                                        getUserRole(user) === "owner" ||
                                        user.isAdmin
                                          ? "#7A95B0"
                                          : this.isModuleAdmin(
                                              user,
                                              "documents"
                                            )
                                          ? "#316DAA"
                                          : "#D0D5DA"
                                      }
                                      isfill={true}
                                      isClickable={false}
                                      isDisabled={
                                        getUserRole(user) === "owner" ||
                                        user.isAdmin
                                      }
                                      onClick={
                                        getUserRole(user) !== "owner" ||
                                        !user.isAdmin
                                          ? this.onModuleIconClick.bind(
                                              this,
                                              [user.id],
                                              "documents",
                                              this.isModuleAdmin(
                                                user,
                                                "documents"
                                              )
                                            )
                                          : null
                                      }
                                    />
                                  </div>
                                  <div className="iconWrapper">
                                    <IconButton
                                      iconName="MainMenuPeopleIcon"
                                      size={16}
                                      color={
                                        getUserRole(user) === "owner" ||
                                        user.isAdmin
                                          ? "#7A95B0"
                                          : this.isModuleAdmin(user, "people")
                                          ? "#316DAA"
                                          : "#D0D5DA"
                                      }
                                      isfill={true}
                                      isClickable={false}
                                      isDisabled={
                                        getUserRole(user) === "owner" ||
                                        user.isAdmin
                                      }
                                      onClick={
                                        getUserRole(user) !== "owner" ||
                                        !user.isAdmin
                                          ? this.onModuleIconClick.bind(
                                              this,
                                              [user.id],
                                              "people",
                                              this.isModuleAdmin(user, "people")
                                            )
                                          : null
                                      }
                                    />
                                  </div>
                                </div>
                              </div>
                            </>
                          </Row>
                        );
                      })}
                    </RowContainer>
                  </div>
                  {hasChanged && (
                    <SaveCancelButtons
                      onSaveClick={this.onSaveButtonClick}
                      onCancelClick={this.onCancelClick}
                      showReminder={showReminder}
                      reminderTest={t("YouHaveUnsavedChanges")}
                      saveButtonLabel={t("SaveButton")}
                      cancelButtonLabel={t("CancelButton")}
                    />
                  )}
                </>
              ) : (
                <EmptyScreenContainer
                  imageSrc="products/people/images/empty_screen_filter.png"
                  imageAlt="Empty Screen Filter image"
                  headerText={t("NotFoundTitle")}
                  descriptionText={t("NotFoundDescription")}
                  buttons={
                    <>
                      <Link
                        type="action"
                        isHovered={true}
                        onClick={this.onSearchChange.bind(this, "")}
                      >
                        {t("ClearButton")}
                      </Link>
                    </>
                  }
                />
              )}
            </ToggleContentContainer>
          </>
        )}
      </>
    );
  }
}

PortalAdmins.defaultProps = {
  admins: [],
  productId: "",
  owner: {},
};

PortalAdmins.propTypes = {
  admins: PropTypes.arrayOf(PropTypes.object),
  productId: PropTypes.string,
  owner: PropTypes.object,
};

export default inject(({ auth, setup }) => {
  const { admins, owner, filter } = setup.security.accessRight;
  const { user: me } = auth.userStore;

  return {
    groupsCaption: auth.settingsStore.customNames.groupsCaption,
    changeAdmins: setup.changeAdmins,
    fetchPeople: setup.fetchPeople,
    getUpdateListAdmin: setup.getUpdateListAdmin,
    admins,
    productId: auth.moduleStore.modules[0].id,
    owner,
    filter,
    me,
  };
})(withTranslation("Settings")(withRouter(observer(PortalAdmins))));
