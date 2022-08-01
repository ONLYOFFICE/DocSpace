import React, { PureComponent } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import styled from "styled-components";

import OwnerSettings from "./owner";
// import ModulesSettings from "./sub-components/modules";

import { setDocumentTitle } from "SRC_DIR/helpers/utils";
//import Link from "@docspace/components/link";
import Text from "@docspace/components/text";
import toastr from "@docspace/components/toast/toastr";
import { inject } from "mobx-react";
import isEmpty from "lodash/isEmpty";
import { /*combineUrl,*/ showLoader, hideLoader } from "@docspace/common/utils";
//import { AppServerConfig } from "@docspace/common/constants";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import ArrowRightIcon from "@docspace/client/public/images/arrow.right.react.svg";
import Loader from "@docspace/components/loader";
//import commonSettingsStyles from "../../../utils/commonSettingsStyles";
import { Base } from "@docspace/components/themes";

const StyledArrowRightIcon = styled(ArrowRightIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.client.settings.security.arrowFill};
  }
`;

StyledArrowRightIcon.defaultProps = { theme: Base };

const MainContainer = styled.div`
  width: 100%;
  max-width: 700px;
  position: relative;

  .subtitle {
    margin-bottom: 20px;
  }

  .page-loader {
    position: absolute;
    left: calc(50% - 35px);
    top: 35%;
  }

  .settings_tabs {
    padding-bottom: 16px;
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
      color: ${(props) =>
        props.theme.client.settings.security.descriptionColor};
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

MainContainer.defaultProps = { theme: Base };

class AccessRights extends PureComponent {
  constructor(props) {
    super(props);

    const { t } = props;

    this.state = {
      isLoading: false,
    };

    setDocumentTitle(t("Common:AccessRights"));
  }

  /*async componentDidMount() {
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
  }*/

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
        <Text className="subtitle">{t("AccessRightsSubTitle")} </Text>
        <OwnerSettings />
        {/* {
          <div className="category-item-wrapper">
            <div className="category-item-heading">
              <Link
                className="inherit-title-link header"
                truncate={true}
                onClick={this.onClickLink}
                href={combineUrl(
                  AppServerConfig.proxyURL,
                  "/portal-settings/security/access-rights/admins"
                )}
              >
                {t("PortalAdmins")}
              </Link>
              <StyledArrowRightIcon size="small" />
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
        } */}
      </MainContainer>
    );
  }
}

export default inject(({ auth, setup }) => {
  // const { updateListAdmins } = setup;
  // const { admins, adminsTotal } = setup.security.accessRight;
  return {
    // admins,
    // adminsTotal,
    // updateListAdmins,
    organizationName: auth.settingsStore.organizationName,
    owner: auth.settingsStore.owner,
  };
})(withTranslation(["Settings", "Common"])(withRouter(AccessRights)));
