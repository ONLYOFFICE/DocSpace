import React, { PureComponent } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";

import OwnerSettings from "./sub-components/owner";
// import ModulesSettings from "./sub-components/modules";

import { setDocumentTitle } from "../../../../../helpers/utils";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import toastr from "@appserver/components/toast/toastr";
import { inject } from "mobx-react";
import isEmpty from "lodash/isEmpty";
import { combineUrl, showLoader, hideLoader } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import ArrowRightIcon from "@appserver/studio/public/images/arrow.right.react.svg";
import Loader from "@appserver/components/loader";
import commonSettingsStyles from "../../utils/commonSettingsStyles";

const StyledArrowRightIcon = styled(ArrowRightIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.color};
  }
`;

const MainContainer = styled.div`
  width: 100%;
  position: relative;

  .page-loader {
    position: absolute;
    left: calc(50% - 35px);
    top: 35%;
  }

  .settings_tabs {
    padding-bottom: 16px;
  }

  ${commonSettingsStyles}
`;

class AccessRights extends PureComponent {
  constructor(props) {
    super(props);

    const { t } = props;

    this.state = {
      isLoading: false,
    };

    setDocumentTitle(t("AccessRights"));
  }

  async componentDidMount() {
    const { admins, updateListAdmins } = this.props;

    if (isEmpty(admins, true)) {
      this.setIsLoading(true);
      showLoader();
      try {
        await updateListAdmins(null, true);
      } catch (error) {
        toastr.error(error);
      }
      this.setIsLoading(false);
      hideLoader();
    }
  }

  onClickLink = (e) => {
    e.preventDefault();
    const { history } = this.props;
    history.push(e.target.pathname);
  };

  setIsLoading = (isLoading) => {
    this.setState({
      isLoading,
    });
  };

  render() {
    const { t, adminsTotal } = this.props;
    const { isLoading } = this.state;
    return isLoading ? (
      <MainContainer>
        <Loader className="page-loader" type="rombs" size="40px" />
      </MainContainer>
    ) : (
      <MainContainer>
        <OwnerSettings />
        {
          <div className="category-item-wrapper">
            <div className="category-item-heading">
              <Link
                className="inherit-title-link header"
                truncate={true}
                onClick={this.onClickLink}
                href={combineUrl(
                  AppServerConfig.proxyURL,
                  "/settings/security/access-rights/admins"
                )}
              >
                {t("PortalAdmins")}
              </Link>
              <StyledArrowRightIcon size="small" color="#333333" />
            </div>
            {adminsTotal > 0 && (
              <Text className="category-item-subheader" truncate={true}>
                {adminsTotal} {t("Admins")}
              </Text>
            )}
            <Text className="category-item-description">
              {t("PortalAdminsDescription")}
            </Text>
          </div>
        }
      </MainContainer>
    );
  }
}

export default inject(({ auth, setup }) => {
  const { updateListAdmins } = setup;
  const { admins, adminsTotal } = setup.security.accessRight;
  return {
    admins,
    adminsTotal,
    updateListAdmins,
    organizationName: auth.settingsStore.organizationName,
    owner: auth.settingsStore.owner,
  };
})(withTranslation("Settings")(withRouter(AccessRights)));
