import React, { Component } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import styled from "styled-components";
import { ReactSVG } from "react-svg";

import ToggleButton from "@appserver/components/toggle-button";
import ModalDialog from "@appserver/components/modal-dialog";
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
import PeopleSelector from "people/PeopleSelector";

import { inject, observer } from "mobx-react";

import { getUserRole } from "@appserver/people/src/helpers/people-helpers";

import isEmpty from "lodash/isEmpty";

import {
  EmployeeStatus,
  EmployeeActivationStatus,
} from "@appserver/common/constants";

import api from "@appserver/common/api";

import { tablet } from "@appserver/components/utils/device";
import { ConsoleView } from "react-device-detect";

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

const StyledModalBody = styled.div`
  display: flex;
  flex-direction: column;

  .avatar {
    margin-right: 16px;
  }

  .toggle-btn {
    position: relative;
  }

  .user-info {
    display: flex;
    align-items: center;
    margin-bottom: 32px;
  }

  .full-access-wrapper {
    display: flex;
    justify-content: space-between;
  }

  .setting-wrapper {
    display: flex;
    justify-content: space-between;
    margin-top: 12px;
  }

  .setting-heading {
    margin-top: 12px;
  }
`;

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
    height: 100%;

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
      padding: 0px 4px;
      background-color: #2da7db;
      border-radius: 5px;
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
      flex-direction: row;
      align-items: center;
      height: 48px;

      .userData {
        max-width: 100%;
      }
    }

    .actionIconsWrapper {
      .fullAccessWrapper {
        margin-right: 0;
      }

      .iconWrapper {
        margin-right: 22px;
      }
    }

    .wrapper {
      #rowContainer {
        .styled-element {
          margin-right: 8px;
        }
      }
    }
  }
`;

const fullAccessId = "00000000-0000-0000-0000-000000000000";

class PortalAdmins extends Component {
  constructor(props) {
    super(props);

    this.state = {
      showFullAdminSelector: false,
      isLoading: false,
      showLoader: false,
      selectedOptions: [],
      searchValue: "",
      selectedUser: null,
    };
  }

  async componentDidMount() {
    const {
      admins,
      setAddUsers,
      setRemoveAdmins,
      updateListAdmins,
    } = this.props;

    setAddUsers(this.addUsers);
    setRemoveAdmins(this.removeAdmins);

    if (isEmpty(admins, true)) {
      try {
        await updateListAdmins(null, true);
      } catch (error) {
        toastr.error(error);
      }
    }
  }

  componentWillUnmount() {
    const { setAddUsers, setRemoveAdmins } = this.props;
    setAddUsers("");
    setRemoveAdmins("");
  }

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

  onCancelSelector = () => {
    const { toggleSelector } = this.props;

    toggleSelector(false);
  };

  onSelect = (items) => {
    const { toggleSelector } = this.props;

    toggleSelector(false);
    this.addUsers(items);
  };

  findAdminById = (admin) => {
    if (admin.id === this.id) return true;
  };

  addUsers = (users) => {
    const { t, admins, setAdmins, getUsersByIds, changeAdmins } = this.props;

    if (!users && users.length === 0) return;

    const userIds = [];

    users.forEach((user) => {
      let isNewAdmin = admins.every((admin) => {
        return admin.id !== user.key;
      });

      if (user.isAdmin) isNewAdmin = false;

      if (isNewAdmin) userIds.push(user.key);
    });

    if (!userIds.length > 0) return;

    changeAdmins(userIds, fullAccessId, true).then(async () => {
      const updatedAdmins = admins.slice();
      const addedUsers = await getUsersByIds(userIds);
      updatedAdmins.push(...addedUsers);
      setAdmins(updatedAdmins);
      toastr.success(t("administratorsAddedSuccessfully"));
    });
  };

  removeAdmins = () => {
    const {
      selection,
      setSelected,
      t,
      setAdmins,
      admins,
      changeAdmins,
    } = this.props;

    if (!selection && selection.length === 0) return;
    const userIds = selection.map((user) => {
      return user.id;
    });

    changeAdmins(userIds, fullAccessId, false).then(() => {
      const newAdmins = admins.filter((admin) => {
        return selection.every((selectedAdmin) => {
          if (selectedAdmin.id === admin.id) {
            return false;
          }
          return true;
        });
      });

      setAdmins(newAdmins);
      setSelected("none");
      toastr.success(t("administratorsRemovedSuccessfully"));
    });
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

  onContentRowSelect = (checked, user) => {
    const { selectUser, deselectUser } = this.props;

    if (checked) {
      selectUser(user);
    } else {
      deselectUser(user);
    }
  };

  onSearchChange = (value) => {
    if (this.state.searchValue === value) return false;

    this.setState({
      searchValue: value,
    });
  };

  onRowClick = (user) => {
    this.setState({
      selectedUser: user,
    });
    this.openModal();
  };

  closeModal = () => {
    this.setState({
      modalIsVisible: false,
    });
  };

  openModal = () => {
    this.setState({
      modalIsVisible: true,
    });
  };

  onFullAccessToggle = () => {
    const { selectedUser } = this.state;
    const { changeAdmins, admins, setAdmins } = this.props;

    changeAdmins([selectedUser.id], fullAccessId, !selectedUser.isAdmin)
      .then(async () => {
        const updatedUser = await api.people.getUserById([selectedUser.id]);
        const updatedAdmins = admins.map((admin) => {
          if (admin.id === selectedUser.id) return updatedUser;
          return admin;
        });

        setAdmins(updatedAdmins);
        this.setState({
          selectedUser: updatedUser,
        });
      })
      .catch((e) => {
        console.log(e);
      });
  };

  onDocumentToggle = () => {};

  onPeopleToggle = () => {};

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
    const {
      t,
      admins,
      isUserSelected,
      selectorIsOpen,
      groupsCaption,
      modules,
    } = this.props;
    const {
      isLoading,
      showLoader,
      searchValue,
      modalIsVisible,
      selectedUser,
    } = this.state;

    const filteredAdmins = searchValue
      ? this.getFilteredAdmins(admins, searchValue)
      : admins;

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
              <PeopleSelector
                isMultiSelect={true}
                displayType="aside"
                isOpen={!!selectorIsOpen}
                onSelect={this.onSelect}
                groupsCaption={groupsCaption}
                onCancel={this.onCancelSelector}
              />
              {selectedUser && (
                <ModalDialog
                  visible={modalIsVisible}
                  zIndex={310}
                  onClose={this.closeModal}
                >
                  <ModalDialog.Header>{t("AccessSettings")}</ModalDialog.Header>
                  <ModalDialog.Body>
                    <StyledModalBody>
                      <div className="user-info">
                        <Avatar
                          size="medium"
                          role={getUserRole(selectedUser)}
                          userName={selectedUser.displayName}
                          source={selectedUser.avatar}
                          className="avatar"
                        />
                        <div>
                          <Text
                            color="#316DAA"
                            fontWeight={600}
                            fontSize="19px"
                          >
                            {selectedUser.displayName}
                          </Text>
                          {selectedUser.department && (
                            <Text
                              color="#A3A9AE"
                              fontWeight={400}
                              fontSize="13px"
                            >
                              {selectedUser.department}
                            </Text>
                          )}
                        </div>
                      </div>
                      <div>
                        <div className="full-access-wrapper">
                          <Text fontWeight={600} fontSize="15px">
                            {t("FullAccess")}
                          </Text>
                          <ToggleButton
                            className="toggle-btn"
                            isChecked={selectedUser.isAdmin}
                            onChange={this.onFullAccessToggle}
                            isDisabled={false}
                          />
                        </div>
                        {modules && modules.length > 0 && (
                          <div>
                            <Text
                              className="setting-heading"
                              fontWeight={600}
                              fontSize="15px"
                            >
                              {t("AdminInModules")}
                            </Text>
                            <div className="module-settings">
                              {modules.map((module) => {
                                return (
                                  <div
                                    key={module.appName}
                                    className="setting-wrapper"
                                  >
                                    <Text fontWeight={400} fontSize="13px">
                                      {module.title}
                                    </Text>
                                    <ToggleButton
                                      className="toggle-btn"
                                      isChecked={this.isModuleAdmin(
                                        selectedUser,
                                        module.appName
                                      )}
                                      onChange={this.onDocumentToggle}
                                      isDisabled={selectedUser.isAdmin}
                                    />
                                  </div>
                                );
                              })}
                            </div>
                          </div>
                        )}
                      </div>
                    </StyledModalBody>
                  </ModalDialog.Body>
                </ModalDialog>
              )}

              {filteredAdmins.length > 0 ? (
                <>
                  <div className="wrapper">
                    <RowContainer useReactWindow={false}>
                      {filteredAdmins.map((user) => {
                        const userRole = getUserRole(user);

                        if (userRole === "owner") return;
                        const element = (
                          <Avatar
                            size="small"
                            role={userRole}
                            userName={user.displayName}
                            source={user.avatar}
                          />
                        );

                        const nameColor =
                          getUserStatus(user) === "pending"
                            ? "#A3A9AE"
                            : "#333333";

                        const checked = isUserSelected(user.id);

                        return (
                          <Row
                            key={user.id}
                            status={user.status}
                            onSelect={this.onContentRowSelect}
                            data={user}
                            data-letter="test"
                            element={element}
                            checkbox={true}
                            checked={checked}
                            contextButtonSpacerWidth={"0px"}
                            onRowClick={() => this.onRowClick(user)}
                          >
                            <>
                              <div className="userData">
                                <div className="nameAndStatus">
                                  <Text
                                    isBold={true}
                                    fontSize="15px"
                                    color={nameColor}
                                  >
                                    {user.displayName}
                                  </Text>
                                </div>
                              </div>
                              <div className="actionIconsWrapper">
                                {user.isAdmin ? (
                                  <div className="fullAccessWrapper">
                                    <Text
                                      truncate={true}
                                      color="#FFFFFF"
                                      font-size="9px"
                                      fontWeight={700}
                                    >
                                      Full access
                                    </Text>
                                  </div>
                                ) : (
                                  <div className="iconsWrapper">
                                    {this.isModuleAdmin(user, "documents") && (
                                      <div className="iconWrapper">
                                        <IconButton
                                          iconName="/static/images/files.menu.svg"
                                          size={14}
                                          color="#2DA7DB"
                                          isfill={true}
                                          isClickable={false}
                                        />
                                      </div>
                                    )}

                                    {this.isModuleAdmin(user, "people") && (
                                      <div className="iconWrapper">
                                        <IconButton
                                          iconName={
                                            "/static/images/departments.group.react.svg"
                                          }
                                          size={16}
                                          color="#2DA7DB"
                                          isfill={true}
                                          isClickable={false}
                                        />
                                      </div>
                                    )}
                                  </div>
                                )}
                              </div>
                            </>
                          </Row>
                        );
                      })}
                    </RowContainer>
                  </div>
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
  setAddUsers: PropTypes.func.isRequired,
  updateListAdmins: PropTypes.func.isRequired,
};

export default inject(({ auth, setup }) => {
  const { admins, owner, filter, selectorIsOpen } = setup.security.accessRight;
  const { user: me } = auth.userStore;
  const { modules } = auth.moduleStore;
  const {
    setAddUsers,
    setRemoveAdmins,
    toggleSelector,
    setAdmins,
    getUsersByIds,
  } = setup;
  const {
    selectUser,
    deselectUser,
    selection,
    isUserSelected,
    setSelected,
  } = setup.selectionStore;

  return {
    groupsCaption: auth.settingsStore.customNames.groupsCaption,
    changeAdmins: setup.changeAdmins,
    fetchPeople: setup.fetchPeople,
    updateListAdmins: setup.updateListAdmins,
    modules,
    admins,
    productId: modules[0].id,
    owner,
    filter,
    me,
    setAddUsers,
    setRemoveAdmins,
    selectUser,
    deselectUser,
    selection,
    isUserSelected,
    setSelected,
    selectorIsOpen,
    toggleSelector,
    setAdmins,
    getUsersByIds,
  };
})(withTranslation("Settings")(withRouter(observer(PortalAdmins))));
