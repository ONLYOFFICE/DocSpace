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
import HelpButton from "@appserver/components/help-button";
import PeopleSelector from "@appserver/people/src/components/PeopleSelector";
import isEmpty from "lodash/isEmpty";
import { inject } from "mobx-react";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";

const StyledWrapper = styled.div`
  .portal-owner-description {
    margin-left: 16px;
    overflow: hidden;
    width: 100%;
    display: flex;
    flex-flow: column;
    justify-content: center;
  }

  .portal-owner-name {
    display: flex;
    align-items: center;
    flex-wrap: wrap;
  }
`;

const OwnerContainer = styled.div`
  margin-bottom: 50px;

  .owner-content-wrapper {
    display: flex;
    padding: 16px;
    background-color: #f8f9f9;
    border-radius: 12px;

    .avatar_wrapper {
      width: 88px;
      height: 88px;
      flex: none;
    }

    .avatar_text {
      margin-right: 3px;
    }

    .portal-owner-heading {
      margin: 0;
      margin-bottom: 4px;
    }

    .group-wrapper {
      display: inline-block;
      margin-right: 3px;
    }
  }

  .link_style {
    margin-right: 3px;
  }
  .advanced-selector {
    position: relative;
  }
  .button_offset {
    margin-right: 16px;
  }
  .chooseOwnerWrap {
    margin-top: 8px;
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

class OwnerSettings extends Component {
  constructor(props) {
    super(props);

    this.state = {
      isLoading: false,
      showSelector: false,
      selectorIsLoaded: false,
      showLocalLoader: true,
    };
  }

  async componentDidMount() {
    const { owner, getPortalOwner } = this.props;

    if (isEmpty(owner, true)) {
      try {
        await getPortalOwner();
      } catch (error) {
        toastr.error(error);
      }
    }
  }

  ownerInfo = () => (
    <Text>{this.props.t("AccessRightsOwnerOpportunities")}</Text>
  );

  changeOwner = (selectedOwner) => {
    const { sendOwnerChange, t, owner } = this.props;
    sendOwnerChange(selectedOwner.key)
      .then(() =>
        toastr.success(
          t("ConfirmEmailSended", { ownerName: owner.displayName })
        )
      )
      .catch((err) => toastr.error(err));
  };

  onLoading = (status) => this.setState({ isLoading: status });

  onToggleSelector = (status = !this.state.showSelector) => {
    this.setState({
      showSelector: status,
      selectorIsLoaded: true,
    });
  };

  onCancelSelector = () => {
    this.onToggleSelector(false);
  };

  onSelect = (items) => {
    this.onToggleSelector(false);
    this.changeOwner(items[0]);
  };

  render() {
    const { t, owner, me, groupsCaption } = this.props;
    const {
      isLoading,
      showSelector,
      selectedOwner,
      selectorIsLoaded,
    } = this.state;

    const formattedDepartments =
      owner.department && getFormattedDepartments(owner.groups);

    return (
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
              <div className="portal-owner-name">
                <Link
                  className="avatar_text"
                  fontSize="16px"
                  fontWeight={600}
                  isBold={true}
                  color="#316DAA"
                  href={owner.profileUrl}
                >
                  {owner.displayName}
                </Link>

                <div className="group-wrapper">
                  <Text fontSize="16px" as="span">{`(${t(
                    "PortalOwner"
                  )})`}</Text>
                </div>
                <HelpButton
                  displayType="dropdown"
                  place="right"
                  className="option-info"
                  offsetRight={0}
                  tooltipContent={this.ownerInfo()}
                  tooltipColor="#F8F7BF"
                />
              </div>
              <div className="chooseOwnerWrap">
                <Link
                  className="link_style"
                  isHovered={true}
                  onClick={this.onToggleSelector}
                  fontSize="12px"
                  type="action"
                >
                  {selectedOwner ? selectedOwner.label : t("ChooseOwner")}
                </Link>
                <Text as="span" fontSize="12px" color="#A3A9AE">
                  {t("AccessRightsChangeOwnerConfirmText")}
                </Text>
              </div>
            </div>
          </div>
          {selectorIsLoaded && (
            <div className="advanced-selector">
              <PeopleSelector
                isOpen={showSelector}
                size={"full"}
                onSelect={this.onSelect}
                onCancel={this.onCancelSelector}
                defaultOption={me}
                defaultOptionLabel={t("Common:MeLabel")}
                groupsCaption={groupsCaption}
              />
            </div>
          )}
        </OwnerContainer>
      </StyledWrapper>
    );
  }
}

OwnerSettings.defaultProps = {
  owner: {},
};

OwnerSettings.propTypes = {
  owner: PropTypes.object,
};

export default inject(({ auth, setup }) => {
  const { customNames, getPortalOwner, owner } = auth.settingsStore;
  const { sendOwnerChange } = setup;
  return {
    groupsCaption: customNames.groupsCaption,
    getPortalOwner,
    owner,
    me: auth.userStore.user,
    sendOwnerChange,
  };
})(withTranslation("Settings")(withRouter(OwnerSettings)));
