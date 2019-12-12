import React, { Component } from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import i18n from "../../../i18n";
import { I18nextProvider, withTranslation } from "react-i18next";
import styled from "styled-components";
import {
  changeAdmins,
  getUpdateListAdmin,
  fetchPeople
} from "../../../../../../store/settings/actions";
import {
  Text,
  Avatar,
  Row,
  RowContent,
  RowContainer,
  Link,
  Paging,
  IconButton,
  AdvancedSelector,
  toastr,
  FilterInput,
  Button,
  RequestLoader,
  Loader,
  EmptyScreenContainer,
  Icons
} from "asc-web-components";
import { getUserRole } from "../../../../../../store/settings/selectors";
import isEmpty from "lodash/isEmpty";

const ToggleContentContainer = styled.div`
  .buttons_container {
    display: flex;
    width: fit-content;
  }
  .toggle_content {
    margin-bottom: 24px;
  }

  .wrapper {
    margin-top: 16px;
  }

  .remove_icon {
    margin-left: 120px;
  }

  .button_style {
    margin-right: 16px;
  }

  .advanced-selector {
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
      allOptions: [],
      options: [],
      isLoading: false,
      showLoader: true,
      selectedOptions: []
    };
  }

  componentDidMount() {
    const { admins, options, fetchPeople } = this.props;

    if (isEmpty(admins, true) || isEmpty(options, true)) {
      const newFilter = this.onAdminsFilter();
      fetchPeople(newFilter)
        .catch(error => {
          toastr.error(error);
        })
        .finally(() =>
          this.setState({
            showLoader: false,
            allOptions: this.props.options
          })
        );
    } else {
      this.setState({ showLoader: false });
    }
  }

  onChangeAdmin = (userIds, isAdmin, productId) => {
    this.onLoading(true);
    const { changeAdmins } = this.props;
    const newFilter = this.onAdminsFilter();

    changeAdmins(userIds, productId, isAdmin, newFilter)
      .catch(error => {
        toastr.error("accessRights onChangeAdmin", error);
      })
      .finally(() => {
        this.onLoading(false);
      });
  };

  onShowGroupSelector = () => {
    this.setState({
      showSelector: !this.state.showSelector,
      options: this.props.options,
      selectedOptions: []
    });
  };

  onShowFullAdminGroupSelector = () => {
    this.setState({
      showFullAdminSelector: !this.state.showFullAdminSelector,
      options: this.props.options,
      selectedOptions: []
    });
  };

  onSelect = selected => {
    const { productId } = this.props;
    this.onChangeAdmin(
      selected.map(user => user.key),
      true,
      productId
    );
    this.onShowGroupSelector();
    this.setState({ selectedOptions: selected });
  };

  onSelectFullAdmin = selected => {
    this.onChangeAdmin(
      selected.map(user => user.key),
      true,
      "00000000-0000-0000-0000-000000000000"
    );
    this.onShowFullAdminGroupSelector();
    this.setState({ selectedOptions: selected });
  };

  onSearchUsers = template => {
    const options = this.filterUserSelectorOptions(
      this.state.allOptions,
      template
    );
    this.setState({ options: options });
  };

  onChangePage = pageItem => {
    const { filter, getUpdateListAdmin } = this.props;

    const newFilter = filter.clone();
    newFilter.page = pageItem.key;
    this.onLoading(true);

    getUpdateListAdmin(newFilter)
      .catch(res => console.log(res))
      .finally(() => this.onLoading(false));
  };

  onChangePageSize = pageItem => {
    const { filter, getUpdateListAdmin } = this.props;

    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.pageCount = pageItem.key;
    this.onLoading(true);

    getUpdateListAdmin(newFilter)
      .catch(res => console.log(res))
      .finally(() => this.onLoading(false));
  };

  onPrevClick = e => {
    const { filter, getUpdateListAdmin } = this.props;

    if (!filter.hasPrev()) {
      e.preventDefault();
      return;
    }
    const newFilter = filter.clone();
    newFilter.page--;
    this.onLoading(true);
    getUpdateListAdmin(newFilter)
      .catch(res => console.log(res))
      .finally(() => this.onLoading(false));
  };

  onNextClick = e => {
    const { filter, getUpdateListAdmin } = this.props;

    if (!filter.hasNext()) {
      e.preventDefault();
      return;
    }
    const newFilter = filter.clone();
    newFilter.page++;
    this.onLoading(true);

    getUpdateListAdmin(newFilter)
      .catch(res => console.log(res))
      .finally(() => this.onLoading(false));
  };

  onLoading = status => {
    this.setState({ isLoading: status });
  };

  onAdminsFilter = () => {
    const { filter } = this.props;

    const newFilter = filter.clone();
    newFilter.page = 0;
    newFilter.role = "admin";

    return newFilter;
  };

  onFilter = data => {
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
      .catch(res => console.log(res))
      .finally(this.onLoading(false));
  };

  onResetFilter = () => {
    const { getUpdateListAdmin, filter } = this.props;

    const newFilter = filter.clone(true);

    this.onLoading(true);
    getUpdateListAdmin(newFilter)
      .catch(res => console.log(res))
      .finally(() => this.onLoading(false));
  };

  filterUserSelectorOptions = (options, template) =>
    options.filter(option => option.label.indexOf(template) > -1);

  pageItems = () => {
    const { t, filter } = this.props;
    if (filter.total < filter.pageCount) return [];
    const totalPages = Math.ceil(filter.total / filter.pageCount);
    return [...Array(totalPages).keys()].map(item => {
      return {
        key: item,
        label: t("PageOfTotalPage", { page: item + 1, totalPage: totalPages })
      };
    });
  };

  countItems = () => [
    { key: 25, label: this.props.t("CountPerPage", { count: 25 }) },
    { key: 50, label: this.props.t("CountPerPage", { count: 50 }) },
    { key: 100, label: this.props.t("CountPerPage", { count: 100 }) }
  ];

  selectedPageItem = () => {
    const { filter, t } = this.props;
    const pageItems = this.pageItems();

    const emptyPageSelection = {
      key: 0,
      label: t("PageOfTotalPage", { page: 1, totalPage: 1 })
    };

    return pageItems.find(x => x.key === filter.page) || emptyPageSelection;
  };

  selectedCountItem = () => {
    const { filter, t } = this.props;

    const emptyCountSelection = {
      key: 0,
      label: t("CountPerPage", { count: 25 })
    };

    const countItems = this.countItems();

    return (
      countItems.find(x => x.key === filter.pageCount) || emptyCountSelection
    );
  };

  getSortData = () => {
    const { t } = this.props;

    return [
      { key: "firstname", label: t("ByFirstNameSorting") },
      { key: "lastname", label: t("ByLastNameSorting") }
    ];
  };

  render() {
    const { t, admins, filter } = this.props;
    const {
      showSelector,
      options,
      selectedOptions,
      isLoading,
      showFullAdminSelector,
      showLoader
    } = this.state;

    const countElements = filter.total;

    console.log("Admins render_");

    return (
      <>
        {showLoader ? (
          <Loader className="pageLoader" type="rombs" size='40px' />
        ) : (
          <>
            <RequestLoader
              visible={isLoading}
              zIndex={256}
              loaderSize='16px'
              loaderColor={"#999"}
              label={`${t("LoadingProcessing")} ${t("LoadingDescription")}`}
              fontSize='12px'
              fontColor={"#999"}
              className="page_loader"
            />

            <ToggleContentContainer>
              <div className="buttons_container">
                <Button
                  className="button_style"
                  size="medium"
                  primary={true}
                  label="Set people admin"
                  isDisabled={isLoading}
                  onClick={this.onShowGroupSelector}
                />
                <div style={{ right: 180 }} className="advanced-selector">
                  <AdvancedSelector
                    displayType="dropdown"
                    isOpen={showSelector}
                    placeholder="placeholder"
                    options={options}
                    onSearchChanged={this.onSearchUsers}
                    //groups={groups}
                    isMultiSelect={true}
                    buttonLabel="Add members"
                    onSelect={this.onSelect}
                    onCancel={this.onShowGroupSelector}
                    onAddNewClick={() => console.log("onAddNewClick")}
                    selectAllLabel="selectorSelectAllText"
                    selectedOptions={selectedOptions}
                  />
                </div>

                <Button
                  size="medium"
                  primary={true}
                  label="Set portal admin"
                  isDisabled={isLoading}
                  onClick={this.onShowFullAdminGroupSelector}
                />
                <div style={{ right: 160 }} className="advanced-selector">
                  <AdvancedSelector
                    displayType="dropdown"
                    isOpen={showFullAdminSelector}
                    placeholder="placeholder"
                    options={options}
                    onSearchChanged={this.onSearchUsers}
                    //groups={groups}
                    isMultiSelect={true}
                    buttonLabel="Add members"
                    onSelect={this.onSelectFullAdmin}
                    onCancel={this.onShowFullAdminGroupSelector}
                    onAddNewClick={() => console.log("onAddNewClick")}
                    selectAllLabel="selectorSelectAllText"
                    selectedOptions={selectedOptions}
                  />
                </div>
              </div>

              <FilterInput
                className="filter_container"
                getFilterData={() => []}
                getSortData={this.getSortData}
                onFilter={this.onFilter}
              />

              {admins.length > 0 ? (
                <div className="wrapper">
                  <RowContainer manualHeight={`${admins.length * 50}px`}>
                    {admins.map(user => {
                      const element = (
                        <Avatar
                          size="small"
                          role={getUserRole(user)}
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
                              containerWidth="120px"
                              type="page"
                              title={user.displayName}
                              isBold={true}
                              fontSize='15px'
                              color={nameColor}
                              href={user.profileUrl}
                            >
                              {user.displayName}
                            </Link>
                            <div style={{ maxWidth: 120 }} />

                            <Text>
                              {user.isAdmin
                                ? "Full access"
                                : "People module admin"}
                            </Text>

                            {!user.isOwner ? (
                              <IconButton
                                className="remove_icon"
                                size="16"
                                isDisabled={isLoading}
                                onClick={this.onChangeAdmin.bind(
                                  this,
                                  [user.id],
                                  false,
                                  "00000000-0000-0000-0000-000000000000"
                                )}
                                iconName={"CatalogTrashIcon"}
                                isFill={true}
                                isClickable={false}
                              />
                            ) : (
                              <div />
                            )}
                          </RowContent>
                        </Row>
                      );
                    })}
                  </RowContainer>
                </div>
              ) : countElements > 25 ? (
                <div className="wrapper">
                  <Paging
                    previousLabel={t("PreviousPage")}
                    nextLabel={t("NextPage")}
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
              ) : (
                <EmptyScreenContainer
                  imageSrc="products/people/images/empty_screen_filter.png"
                  imageAlt="Empty Screen Filter image"
                  headerText={t("NotFoundTitle")}
                  descriptionText={t("NotFoundDescription")}
                  buttons={
                    <>
                      <Icons.CrossIcon
                        size="small"
                        style={{ marginRight: "4px" }}
                      />
                      <Link
                        type="action"
                        isHovered={true}
                        onClick={this.onResetFilter}
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

const AccessRightsContainer = withTranslation()(PureAdminsSettings);

const AdminsSettings = props => {
  const { language } = props;

  i18n.changeLanguage(language);

  return (
    <I18nextProvider i18n={i18n}>
      <AccessRightsContainer {...props} />
    </I18nextProvider>
  );
};

function mapStateToProps(state) {
  const {
    admins,
    options,
    owner,
    filter
  } = state.settings.security.accessRight;

  return {
    admins,
    productId: state.auth.modules[0].id,
    owner,
    options,
    filter
  };
}

AdminsSettings.defaultProps = {
  admins: [],
  productId: "",
  owner: {},
  options: []
};

AdminsSettings.propTypes = {
  admins: PropTypes.arrayOf(PropTypes.object),
  productId: PropTypes.string,
  owner: PropTypes.object,
  options: PropTypes.arrayOf(PropTypes.object)
};

export default connect(mapStateToProps, {
  changeAdmins,
  fetchPeople,
  getUpdateListAdmin
})(withRouter(AdminsSettings));
