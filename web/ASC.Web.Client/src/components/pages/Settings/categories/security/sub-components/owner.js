import React, { Component } from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import i18n from "../../../i18n";
import { I18nextProvider, withTranslation } from "react-i18next";
import styled from "styled-components";
import { getPortalOwner } from "../../../../../../store/settings/actions";
import {
  Text,
  Avatar,
  Link,
  toastr,
  Button,
  RequestLoader
} from "asc-web-components";

import { isArrayEqual } from "../../../utils/isArrayEqual";

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
      isLoading: false
    };
  }

  componentDidMount() {
    const { getPortalOwner, ownerId } = this.props;
    this.onLoading(true);

    getPortalOwner(ownerId)
      .catch(error => {
        toastr.error(error);
      })
      .finally(() => this.onLoading(false));
  }

  shouldComponentUpdate(nextProps, nextState) {
    const { owner, ownerId } = this.props;
    if (
      ownerId === nextProps.ownerId &&
      this.state.isLoading === nextState.isLoading &&
      isArrayEqual(owner, nextProps.owner)
    ) {
      return false;
    }
    return true;
  }

  onChangeOwner = () => {
    toastr.warning("onChangeOwner");
  };

  onLoading = status => this.setState({ isLoading: status });

  render() {
    const { t, owner } = this.props;
    const { isLoading } = this.state;

    const OwnerOpportunities = t("AccessRightsOwnerOpportunities").split("|");

    console.log("Owner render_");

    return (
      <>
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
          <Text.Body fontSize={18}>{t("PortalOwner")}</Text.Body>
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
              <Text.Body className="avatar_text" fontSize={16} isBold={true}>
                {owner.displayName}
              </Text.Body>
              {owner.groups &&
                owner.groups.map(group => (
                  <Link fontSize={12} key={group.id} href={owner.profileUrl}>
                    {group.name}
                  </Link>
                ))}
            </div>
          </AvatarContainer>
          <ProjectsBody>
            <Text.Body className="portal_owner" fontSize={12}>
              {t("AccessRightsOwnerCan")}:
            </Text.Body>
            <Text.Body fontSize={12}>
              {OwnerOpportunities.map((item, key) => (
                <li key={key}>{item};</li>
              ))}
            </Text.Body>
          </ProjectsBody>
        </BodyContainer>
        <Button
          size="medium"
          primary={true}
          label="Change portal owner"
          isDisabled={isLoading}
          onClick={this.onChangeOwner}
        />
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
  return {
    ownerId: state.auth.settings.ownerId,
    owner: state.settings.security.accessRight.owner
  };
}

OwnerSettings.defaultProps = {
  owner: {}
};

OwnerSettings.propTypes = {
  owner: PropTypes.object
};

export default connect(mapStateToProps, { getPortalOwner })(
  withRouter(OwnerSettings)
);
