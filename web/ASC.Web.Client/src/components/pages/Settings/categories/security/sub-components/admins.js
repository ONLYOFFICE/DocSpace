import React, { Component } from "react";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";

import Text from "@appserver/components/text";
import Avatar from "@appserver/components/avatar";
import Row from "@appserver/components/row";
import RowContent from "@appserver/components/row-content";
import RowContainer from "@appserver/components/row-container";
import Link from "@appserver/components/link";
import Paging from "@appserver/components/paging";
import IconButton from "@appserver/components/icon-button";
import toastr from "@appserver/components/toast/toastr";
import Button from "@appserver/components/button";
import RequestLoader from "@appserver/components/request-loader";
import Loader from "@appserver/components/loader";
import EmptyScreenContainer from "@appserver/components/empty-screen-container";
import { showLoader, hideLoader } from "@appserver/common/utils";
import FilterInput from "@appserver/common/components/FilterInput";
import PeopleSelector from "people/PeopleSelector";

import isEmpty from "lodash/isEmpty";
import { inject, observer } from "mobx-react";

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
  .cross_icon {
    margin-right: 8px;
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

  .filter_container {
    margin-top: 16px;
  }

  *,
  *::before,
  *::after {
    box-sizing: border-box;
  }
`;

class PureAdminsSettings extends Component {
  constructor(props) {
    super(props);

    this.state = {
      showSelector: false,
      showFullAdminSelector: false,
      isLoading: false,
      showLoader: true,
      selectedOptions: [],
    };
  }

  componentDidMount() {
    const { admins, fetchPeople } = this.props;
    showLoader();
    if (isEmpty(admins, true)) {
      const newFilter = this.onAdminsFilter();
      fetchPeople(newFilter)
        .catch((error) => {
          toastr.error(error);
        })
        .finally(() =>
          this.setState({
            showLoader: false,
          })
        );
    } else {
      this.setState({ showLoader: false });
    }
    hideLoader();
  }

  onChangeAdmin = (userIds, isAdmin, productId) => {
    this.onLoading(true);
    const { changeAdmins } = this.props;
    const newFilter = this.onAdminsFilter();

    changeAdmins(userIds + "", productId, isAdmin, newFilter)
      .catch((error) => toastr.error(error)) //TODO: add translation to toast if need
      .finally(() => this.onLoading(false));
  };

  onShowGroupSelector = () => {
    /* console.log(
      `onShowGroupSelector(showSelector: ${!this.state.showSelector})`
    ); */

    this.setState({
      showSelector: !this.state.showSelector,
    });
  };

  onShowFullAdminGroupSelector = () => {
    /* console.log(
      `onShowFullAdminGroupSelector(showFullAdminSelector: ${!this.state
        .showFullAdminSelector})`
    ); */

    this.setState({
      showFullAdminSelector: !this.state.showFullAdminSelector,
    });
  };

  onCancelSelector = (e) => {
    /* console.log(
      `onCancelSelector(showSelector: false, showFullAdminSelector: false`,
      e
    ); */

    if (
      (this.state.showSelector &&
        e.target.id === "people-admin-selector_button") ||
      (this.state.showFullAdminSelector &&
        e.target.id === "full-admin-selector_button")
    ) {
      // Skip double set of isOpen property
      return;
    }

    this.setState({
      showSelector: false,
      showFullAdminSelector: false,
    });
  };

  onSelect = (selected) => {
    const { productId } = this.props;
    this.onChangeAdmin(
      selected.map((user) => user.key),
      true,
      productId
    );
    this.onShowGroupSelector();
  };

  onSelectFullAdmin = (selected) => {
    this.onChangeAdmin(
      selected.map((user) => user.key),
      true,
      "00000000-0000-0000-0000-000000000000"
    );
    this.onShowFullAdminGroupSelector();
  };

  onChangePage = (pageItem) => {
    const { filter, getUpdateListAdmin } = this.props;

    const newFilter = filter.clone();
    newFilter.page = pageItem.key;
    this.onLoading(true);

    getUpdateListAdmin(newFilter)
      .catch((res) => console.log(res))
      .finally(() => this.onLoading(false));
  };

  onChangePageSize = (pageItem) => {
    const { filter, getUpdateListAdmin } = this.props;

    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.pageCount = pageItem.key;
    this.onLoading(true);

    getUpdateListAdmin(newFilter)
      .catch((res) => console.log(res))
      .finally(() => this.onLoading(false));
  };

  onPrevClick = (e) => {
    const { filter, getUpdateListAdmin } = this.props;

    if (!filter.hasPrev()) {
      e.preventDefault();
      return;
    }
    const newFilter = filter.clone();
    newFilter.page--;
    this.onLoading(true);
    getUpdateListAdmin(newFilter)
      .catch((res) => console.log(res))
      .finally(() => this.onLoading(false));
  };

  onNextClick = (e) => {
    const { filter, getUpdateListAdmin } = this.props;

    if (!filter.hasNext()) {
      e.preventDefault();
      return;
    }
    const newFilter = filter.clone();
    newFilter.page++;
    this.onLoading(true);

    getUpdateListAdmin(newFilter)
      .catch((res) => console.log(res))
      .finally(() => this.onLoading(false));
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

  onFilter = (data) => {
    const { filter, getUpdateListAdmin } = this.props;

    const search = data.inputValue || null;
    const sortBy = data.sortId;
    const sortOrder =
      data.sortDirection === "desc" ? "descending" : "ascending";

    const newFilter = filter.clone();

    newFilter.sortBy = sortBy;
    newFilter.sortOrder = sortOrder;
    newFilter.page = 0;
    newFilter.role = "admin";
    newFilter.search = search;
    this.onLoading(true);

    getUpdateListAdmin(newFilter)
      .catch((res) => console.log(res))
      .finally(this.onLoading(false));
  };

  onResetFilter = () => {
    const { getUpdateListAdmin, filter } = this.props;

    const newFilter = filter.clone(true);

    this.onLoading(true);
    getUpdateListAdmin(newFilter)
      .catch((res) => console.log(res))
      .finally(() => this.onLoading(false));
  };

  pageItems = () => {
    const { t, filter } = this.props;
    if (filter.total < filter.pageCount) return [];
    const totalPages = Math.ceil(filter.total / filter.pageCount);
    return [...Array(totalPages).keys()].map((item) => {
      return {
        key: item,
        label: t("Common:PageOfTotalPage", {
          page: item + 1,
          totalPage: totalPages,
        }),
      };
    });
  };

  countItems = () => [
    { key: 25, label: this.props.t("Common:CountPerPage", { count: 25 }) },
    { key: 50, label: this.props.t("Common:CountPerPage", { count: 50 }) },
    { key: 100, label: this.props.t("Common:CountPerPage", { count: 100 }) },
  ];

  selectedPageItem = () => {
    const { filter, t } = this.props;
    const pageItems = this.pageItems();

    const emptyPageSelection = {
      key: 0,
      label: t("Common:PageOfTotalPage", { page: 1, totalPage: 1 }),
    };

    return pageItems.find((x) => x.key === filter.page) || emptyPageSelection;
  };

  selectedCountItem = () => {
    const { filter, t } = this.props;

    const emptyCountSelection = {
      key: 0,
      label: t("Common:CountPerPage", { count: 25 }),
    };

    const countItems = this.countItems();

    return (
      countItems.find((x) => x.key === filter.pageCount) || emptyCountSelection
    );
  };

  getSortData = () => {
    const { t } = this.props;

    return [
      {
        key: "firstname",
        label: t("Common:ByFirstNameSorting"),
        default: true,
      },
      { key: "lastname", label: t("Common:ByLastNameSorting"), default: true },
    ];
  };

  getUserRole = (user) => {
    if (user.isOwner) return "owner";
    else if (user.isAdmin) return "admin";
    else if (
      user.listAdminModules !== undefined &&
      user.listAdminModules.includes("people")
    )
      return "admin";
    else if (user.isVisitor) return "guest";
    else return "user";
  };

  render() {
    const { t, admins, filter, me, groupsCaption } = this.props;
    const {
      showSelector,
      isLoading,
      showFullAdminSelector,
      showLoader,
    } = this.state;

    console.log("Admins render_");

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
              label={`${t("Common:LoadingProcessing")} ${t(
                "Common:LoadingDescription"
              )}`}
              fontSize="12px"
              fontColor={"#999"}
              className="page_loader"
            />

            <ToggleContentContainer>
              <div className="buttons_container">
                <div className="people-admin_container">
                  <Button
                    id="people-admin-selector_button"
                    size="medium"
                    primary={true}
                    label={t("SetPeopleAdmin")}
                    isDisabled={isLoading}
                    onClick={this.onShowGroupSelector}
                  />
                  <PeopleSelector
                    id="people-admin-selector"
                    isOpen={showSelector}
                    isMultiSelect={true}
                    role="user"
                    onSelect={this.onSelect}
                    onCancel={this.onCancelSelector}
                    defaultOption={me}
                    defaultOptionLabel={t("Common:MeLabel")}
                    groupsCaption={groupsCaption}
                  />
                </div>
                <div className="full-admin_container">
                  <Button
                    id="full-admin-selector_button"
                    size="medium"
                    primary={true}
                    label={t("SetPortalAdmin")}
                    isDisabled={isLoading}
                    onClick={this.onShowFullAdminGroupSelector}
                  />
                  <PeopleSelector
                    id="full-admin-selector"
                    isOpen={showFullAdminSelector}
                    isMultiSelect={true}
                    role="user"
                    onSelect={this.onSelectFullAdmin}
                    onCancel={this.onCancelSelector}
                    defaultOption={me}
                    defaultOptionLabel={t("Common:MeLabel")}
                    groupsCaption={groupsCaption}
                  />
                </div>
              </div>

              <FilterInput
                className="filter_container"
                getFilterData={() => []}
                getSortData={this.getSortData}
                onFilter={this.onFilter}
                directionAscLabel={t("Common:DirectionAscLabel")}
                directionDescLabel={t("Common:DirectionDescLabel")}
              />

              {admins.length > 0 ? (
                <>
                  <div className="wrapper">
                    <RowContainer manualHeight={`${admins.length * 50}px`}>
                      {admins.map((user) => {
                        const element = (
                          <Avatar
                            size="min"
                            role={this.getUserRole(user)}
                            userName={user.displayName}
                            source={user.avatar}
                          />
                        );
                        const nameColor =
                          user.status === "pending" ? "#A3A9AE" : "#333333";

                        return (
                          <Row
                            key={user.id}
                            status={user.status}
                            data={user}
                            element={element}
                          >
                            <RowContent disableSideInfo={true}>
                              <Link
                                containerWidth="50%"
                                type="page"
                                title={user.displayName}
                                isBold={true}
                                fontSize="15px"
                                color={nameColor}
                                href={user.profileUrl}
                              >
                                {user.displayName}
                              </Link>
                              <></>
                              <Text containerWidth="10%">
                                {user.isAdmin
                                  ? t("Common:FullAccess")
                                  : t("PeopleAdmin")}
                              </Text>
                              {!user.isOwner ? (
                                <IconButton
                                  containerWidth="5%"
                                  className="remove_icon"
                                  size="16"
                                  isDisabled={isLoading}
                                  onClick={this.onChangeAdmin.bind(
                                    this,
                                    [user.id],
                                    false,
                                    "00000000-0000-0000-0000-000000000000"
                                  )}
                                  iconName={
                                    "static/images/catalog.trash.react.svg"
                                  }
                                  isFill={true}
                                  isClickable={false}
                                />
                              ) : (
                                <div containerWidth="5%" />
                              )}
                            </RowContent>
                          </Row>
                        );
                      })}
                    </RowContainer>
                  </div>
                  <div className="wrapper">
                    <Paging
                      previousLabel={t("Common:Previous")}
                      nextLabel={t("Common:Next")}
                      openDirection="top"
                      countItems={this.countItems()}
                      pageItems={this.pageItems()}
                      displayItems={false}
                      selectedPageItem={this.selectedPageItem()}
                      selectedCountItem={this.selectedCountItem()}
                      onSelectPage={this.onChangePage}
                      onSelectCount={this.onChangePageSize}
                      previousAction={this.onPrevClick}
                      nextAction={this.onNextClick}
                      disablePrevious={!filter.hasPrev()}
                      disableNext={!filter.hasNext()}
                    />
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
                      <IconButton
                        className="cross_icon"
                        size="12"
                        onClick={this.onResetFilter}
                        iconName="/static/images/cross.react.svg"
                        isFill
                        color="#657077"
                      />
                      <Link
                        type="action"
                        isHovered={true}
                        fontWeight="600"
                        color="#555f65"
                        onClick={this.onResetFilter}
                      >
                        {t("Common:ClearButton")}
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

const AdminsSettings = withTranslation(["Settings", "Common"])(
  PureAdminsSettings
);

AdminsSettings.defaultProps = {
  admins: [],
  productId: "",
  owner: {},
};

AdminsSettings.propTypes = {
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
})(withRouter(observer(AdminsSettings)));
