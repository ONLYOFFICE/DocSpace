import React, { Component } from "react";
import { ReactSVG } from "react-svg";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import Text from "@appserver/components/text";
import Avatar from "@appserver/components/avatar";
import Link from "@appserver/components/link";
import toastr from "@appserver/components/toast/toastr";
import Button from "@appserver/components/button";
import Heading from "@appserver/components/heading";
import RequestLoader from "@appserver/components/request-loader";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import Loader from "@appserver/components/loader";
import PeopleSelector from "people/PeopleSelector";
import isEmpty from "lodash/isEmpty";
import { inject } from "mobx-react";
import { showLoader, hideLoader, combineUrl } from "@appserver/common/utils";
import ArrowRightIcon from "@appserver/studio/public/images/arrow.right.react.svg";
import { AppServerConfig } from "@appserver/common/constants";

const StyledArrowRightIcon = styled(ArrowRightIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.color};
  }
`;

const StyledWrapper = styled.div`
  .portal-owner-description {
    margin-left: 16px;
    overflow: hidden;
    width: 100%;
  }

  .category-item-wrapper {
    margin-bottom: 40px;

    .category-item-heading {
      display: flex;
      align-items: center;
      margin-bottom: 5px;
    }

    .category-item-subheader {
      font-size: 13px;
      font-weight: 600;
      margin-bottom: 5px;
    }

    .category-item-description {
      color: #555f65;
      font-size: 12px;
      max-width: 1024px;
    }

    .inherit-title-link {
      margin-right: 7px;
      font-size: 19px;
      font-weight: 600;
    }

    .link-text {
      margin: 0;
    }
  }
`;

const OwnerContainer = styled.div`
  margin-bottom: 50px;

  .owner-content-wrapper {
    display: flex;
    margin-bottom: 30px;
    padding: 16px;
    background-color: #f8f9f9;
    border-radius: 12px;

    .avatar_wrapper {
      width: 88px;
      height: 88px;
      flex: none;
    }

    .portal-owner-heading {
      margin: 0;
      margin-bottom: 4px;
    }

    .portal-owner-info {
      margin-bottom: 9px;
    }

    .group-wrapper {
      display: inline-block;
      margin-left: 3px;
    }
  }

  .link_style {
    margin-right: 16px;
  }
  .text-body_wrapper {
    margin-bottom: 16px;
  }
  .advanced-selector {
    position: relative;
  }
  .text-body_inline {
    display: inline-flex;
  }
  .button_offset {
    margin-right: 16px;
  }
  .chooseOwnerWrap {
    margin-top: 16px;
    padding-top: 16px;
    border-top: 1px solid #eceef1;
  }
`;

const getFormattedDepartments = (departments) => {
  const formattedDepartments = departments.map((department, index) => {
    return (
      <span key={index}>
        <Link href={getGroupLink(department)} type="page" fontSize="12px">
          {department.name}
        </Link>
        {departments.length - 1 !== index ? ", " : ""}
      </span>
    );
  });

  return formattedDepartments;
};

const getGroupLink = (department) => {
  return combineUrl(
    AppServerConfig.proxyURL,
    "/products/people/filter?group=",
    department.id
  );
};

class PureOwnerSettings extends Component {
  constructor(props) {
    super(props);

    this.state = {
      isLoading: false,
      showSelector: false,
      showLocalLoader: true,
      selectedOwner: null,
    };
  }

  async componentDidMount() {
    const {
      owner,
      getPortalOwner,
      admins,
      filter,
      getUpdateListAdmin,
    } = this.props;

    showLoader();

    if (isEmpty(owner, true)) {
      try {
        await getPortalOwner();
      } catch (error) {
        toastr.error(error);
      }
    }

    if (isEmpty(admins, true)) {
      try {
        const newFilter = filter.clone();
        await getUpdateListAdmin(newFilter);
      } catch (error) {
        toastr.error(error);
      }
    }

    this.setState({ showLocalLoader: false });
    hideLoader();
  }

  onChangeOwner = () => {
    const { t, owner, sendOwnerChange } = this.props;
    const { selectedOwner } = this.state;
    sendOwnerChange(selectedOwner.key)
      .then((res) => toastr.success(res.message)) //toastr.success(t("DnsChangeMsg", { email: owner.email })))
      .catch((err) => toastr.error(err));
  };

  onLoading = (status) => this.setState({ isLoading: status });

  onShowSelector = (status) => {
    this.setState({
      showSelector: status,
    });
  };

  onCancelSelector = () => {
    this.onShowSelector(false);
  };

  onSelect = (items) => {
    this.onShowSelector(false);
    this.setState({ selectedOwner: items[0] });
  };

  render() {
    const { t, owner, me, groupsCaption, admins } = this.props;
    const {
      isLoading,
      showLocalLoader,
      showSelector,
      selectedOwner,
    } = this.state;

    const formattedDepartments =
      owner.department && getFormattedDepartments(owner.groups);

    return (
      <>
        {showLocalLoader ? (
          <Loader className="pageLoader" type="rombs" size="40px" />
        ) : (
          <StyledWrapper>
            <OwnerContainer>
              <div className="owner-content-wrapper">
                <Link href={owner.profileUrl}>
                  <Avatar
                    className="avatar_wrapper"
                    size="big"
                    userName={owner.userName}
                    source={owner.avatar}
                    role="user"
                  />
                </Link>

                <div className="portal-owner-description">
                  <Heading
                    className="portal-owner-heading"
                    level={3}
                    size="small"
                  >
                    {t("PortalOwner")}
                  </Heading>
                  <div className="portal-owner-info">
                    <Link
                      className="avatar_text"
                      fontSize="13px"
                      fontWeight={600}
                      isBold={true}
                      color="#316DAA"
                      href={owner.profileUrl}
                    >
                      {owner.displayName}
                    </Link>
                    {owner.groups && (
                      <div className="group-wrapper">
                        <Text as="span">(</Text>
                        {formattedDepartments}
                        <Text as="span">)</Text>
                      </div>
                    )}
                  </div>
                  <Text className="PortalOwnerDescription" color="#555F65">
                    {t("PortalOwnerDescription")}
                  </Text>
                  <div className="chooseOwnerWrap">
                    <Link
                      className="link_style"
                      isHovered={true}
                      onClick={this.onShowSelector.bind(this, !showSelector)}
                    >
                      {selectedOwner ? selectedOwner.label : t("ChooseOwner")}
                    </Link>

                    <Button
                      className="button_offset"
                      size="medium"
                      primary={true}
                      label={t("AccessRightsChangeOwnerButtonText")}
                      isDisabled={!isLoading ? selectedOwner === null : false}
                      onClick={this.onChangeOwner}
                    />
                    <Text
                      className="text-body_inline"
                      fontSize="12px"
                      color="#A3A9AE"
                    >
                      {t("AccessRightsChangeOwnerConfirmText")}
                    </Text>
                  </div>
                </div>
              </div>

              <div className="advanced-selector">
                <PeopleSelector
                  isOpen={showSelector}
                  size={"compact"}
                  onSelect={this.onSelect}
                  onCancel={this.onCancelSelector}
                  defaultOption={me}
                  defaultOptionLabel={t("MeLabel")}
                  groupsCaption={groupsCaption}
                />
              </div>
            </OwnerContainer>
            <div className="category-item-wrapper">
              <div className="category-item-heading">
                <Link
                  className="inherit-title-link header"
                  onClick={this.onClickLink}
                  truncate={true}
                  href={combineUrl(
                    AppServerConfig.proxyURL,
                    "/settings/security/access-rights/portal-admins"
                  )}
                >
                  {t("PortalAdmins")}
                </Link>
                <StyledArrowRightIcon size="small" color="#333333" />
              </div>
              {admins.length > 0 && (
                <Text className="category-item-subheader" truncate={true}>
                  {admins.length} {t("Employees")}
                </Text>
              )}
              <Text className="category-item-description">
                {t("PortalAdminsDescription")}
              </Text>
            </div>
          </StyledWrapper>
        )}
      </>
    );
  }
}

PureOwnerSettings.defaultProps = {
  owner: {},
};

PureOwnerSettings.propTypes = {
  owner: PropTypes.object,
};

export default inject(({ auth, setup }) => {
  const {
    customNames,
    getPortalOwner,
    owner,
    fetchPeople,
  } = auth.settingsStore;
  const { admins, filter } = setup.security.accessRight;
  const { sendOwnerChange, getUpdateListAdmin } = setup;
  return {
    groupsCaption: customNames.groupsCaption,
    getPortalOwner,
    owner,
    admins,
    filter,
    fetchPeople,
    getUpdateListAdmin,
    me: auth.userStore.user,
    sendOwnerChange,
  };
})(withTranslation("Settings")(withRouter(PureOwnerSettings)));
