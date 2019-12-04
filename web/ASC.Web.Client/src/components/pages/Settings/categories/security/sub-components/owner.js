import React, { Component } from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import i18n from "../../../i18n";
import { I18nextProvider, withTranslation } from "react-i18next";
import styled from "styled-components";
import {
  getPortalOwner,
  getUsersOptions
} from "../../../../../../store/settings/actions";
import {
  Text,
  Avatar,
  Link,
  toastr,
  Button,
  RequestLoader,
  AdvancedSelector,
  Loader
} from "asc-web-components";
import isEmpty from "lodash/isEmpty";

const OwnerContainer = styled.div`
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
`;
const HeaderContainer = styled.div`
  margin: 40px 0 16px 0;
`;

const BodyContainer = styled.div`
  display: flex;
  align-items: flex-start;
  flex-direction: row;
  flex-wrap: wrap;
  margin-bottom: 24px;
`;

const AvatarContainer = styled.div`
  display: flex;
  width: 330px;
  height: 120px;
  margin-right: 130px;
  margin-bottom: 24px;
  padding: 8px;
  border: 1px solid lightgrey;

  .avatar_wrapper {
    width: 100px;
    height: 100px;
  }

  .avatar_body {
    margin-left: 24px;
    max-width: 190px;
    word-wrap: break-word;
    overflow: hidden;
  }
`;

const ProjectsBody = styled.div`
  width: 280px;
`;

class PureOwnerSettings extends Component {
  constructor(props) {
    super(props);

    this.state = {
      isLoading: false,
      showSelector: false,
      options: [],
      allOptions: [],
      showLoader: true,
      selectedOwner: null
    };
  }

  componentDidMount() {
    const {
      owner,
      getPortalOwner,
      ownerId,
      options,
      getUsersOptions
    } = this.props;

    if (isEmpty(owner, true)) {
      getPortalOwner(ownerId)
        .catch(error => {
          toastr.error(error);
        })
        .finally(() => this.setState({ showLoader: false }));
    }
    if (isEmpty(options, true)) {
      getUsersOptions()
        .catch(error => {
          toastr.error(error);
        })
        .finally(() =>
          this.setState({
            showLoader: false,
            allOptions: this.props.options
          })
        );
    }
    this.setState({ showLoader: false });
  }

  onChangeOwner = () => {
    const { t, owner } = this.props;
    toastr.success(t("DnsChangeMsg", { email: owner.email }));
  };

  onLoading = status => this.setState({ isLoading: status });

  onShowSelector = status => {
    this.setState({
      options: this.props.options,
      showSelector: status
    });
  };

  onSearchUsers = template => {
    const options = this.filterUserSelectorOptions(
      this.state.allOptions,
      template
    );
    this.setState({ options: options });
  };

  onSelect = selected => {
    this.onShowSelector(false);
    this.setState({ selectedOwner: selected });
  };

  filterUserSelectorOptions = (options, template) =>
    options.filter(option => option.label.indexOf(template) > -1);

  render() {
    const { t, owner } = this.props;
    const {
      isLoading,
      showLoader,
      showSelector,
      options,
      selectedOwner
    } = this.state;

    const OwnerOpportunities = t("AccessRightsOwnerOpportunities").split("|");

    console.log("Owner render_");

    return (
      <>
        {showLoader ? (
          <Loader className="pageLoader" type="rombs" size={40} />
        ) : (
          <OwnerContainer>
            <RequestLoader
              visible={isLoading}
              zIndex={256}
              loaderSize={16}
              loaderColor={"#999"}
              label={`${t("LoadingProcessing")} ${t("LoadingDescription")}`}
              fontSize={12}
              fontColor={"#999"}
              className="page_loader"
            />
            <HeaderContainer>
              <Text fontSize={18}>{t("PortalOwner")}</Text>
            </HeaderContainer>

            <BodyContainer>
              <AvatarContainer>
                <Avatar
                  className="avatar_wrapper"
                  size="big"
                  role="owner"
                  userName={owner.userName}
                  source={owner.avatar}
                />
                <div className="avatar_body">
                  <Text
                    className="avatar_text"
                    fontSize={16}
                    isBold={true}
                  >
                    {owner.displayName}
                  </Text>
                  {owner.groups &&
                    owner.groups.map(group => (
                      <Link
                        fontSize={12}
                        key={group.id}
                        href={owner.profileUrl}
                      >
                        {group.name}
                      </Link>
                    ))}
                </div>
              </AvatarContainer>
              <ProjectsBody>
                <Text className="portal_owner" fontSize={12}>
                  {t("AccessRightsOwnerCan")}:
                </Text>
                <Text fontSize={12}>
                  {OwnerOpportunities.map((item, key) => (
                    <li key={key}>{item};</li>
                  ))}
                </Text>
              </ProjectsBody>
            </BodyContainer>

            <Text fontSize={12} className="text-body_wrapper">
              {t("AccessRightsChangeOwnerText")}
            </Text>

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
              label="Change portal owner"
              isDisabled={!isLoading ? selectedOwner === null : false}
              onClick={this.onChangeOwner}
            />
            <Text
              className="text-body_inline"
              fontSize={12}
              color="#A3A9AE"
            >
              {t("AccessRightsChangeOwnerConfirmText")}
            </Text>

            <div className="advanced-selector">
              <AdvancedSelector
                displayType="dropdown"
                isOpen={showSelector}
                placeholder="placeholder"
                options={options}
                onSearchChanged={this.onSearchUsers}
                //groups={groups}
                buttonLabel="Add members"
                onSelect={this.onSelect}
                onCancel={this.onShowSelector.bind(this, false)}
                onAddNewClick={() => console.log("onAddNewClick")}
                selectAllLabel="selectorSelectAllText"
              />
            </div>
          </OwnerContainer>
        )}
      </>
    );
  }
}

const AccessRightsContainer = withTranslation()(PureOwnerSettings);

const OwnerSettings = props => {
  const { language } = props;

  i18n.changeLanguage(language);

  return (
    <I18nextProvider i18n={i18n}>
      <AccessRightsContainer {...props} />
    </I18nextProvider>
  );
};

function mapStateToProps(state) {
  const { owner, users } = state.settings.security.accessRight;

  return {
    ownerId: state.auth.settings.ownerId,
    owner,
    options: users
  };
}

OwnerSettings.defaultProps = {
  owner: {}
};

OwnerSettings.propTypes = {
  owner: PropTypes.object
};

export default connect(mapStateToProps, { getPortalOwner, getUsersOptions })(
  withRouter(OwnerSettings)
);
