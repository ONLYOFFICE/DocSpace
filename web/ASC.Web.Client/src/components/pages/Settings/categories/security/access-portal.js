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
import { Base } from "@appserver/components/themes";
import commonSettingsStyles from "../../utils/commonSettingsStyles";

const StyledArrowRightIcon = styled(ArrowRightIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.studio.settings.security.arrowFill};
  }
`;

StyledArrowRightIcon.defaultProps = { theme: Base };

const MainContainer = styled.div`
  width: 100%;

  .settings_tabs {
    padding-bottom: 16px;
  }

  .page_loader {
    position: fixed;
    left: 50%;
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
      color: ${(props) => props.theme.studio.settings.security.descriptionColor}
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
class AccessPortal extends PureComponent {
  constructor(props) {
    super(props);

    const { t } = props;

    setDocumentTitle(t("PortalAccess"));
  }

  componentDidMount() {
    showLoader();
    hideLoader();
  }

  onClickLink = (e) => {
    e.preventDefault();
    const { history } = this.props;
    history.push(e.target.pathname);
  };

  render() {
    const { t } = this.props;
    return (
      <MainContainer>
        <div className="category-item-wrapper">
          <div className="category-item-heading">
            <Link
              className="inherit-title-link header"
              onClick={this.onClickLink}
              truncate={true}
              href={combineUrl(
                AppServerConfig.proxyURL,
                "/settings/security/access-portal/tfa"
              )}
            >
              {t("TwoFactorAuth")}
            </Link>
            <StyledArrowRightIcon size="small" />
          </div>
          <Text className="category-item-description">
            {t("TwoFactorAuthDescription")}
          </Text>
        </div>
      </MainContainer>
    );
  }
}

export default inject(({ auth }) => {
  return {
    organizationName: auth.settingsStore.organizationName,
  };
})(withTranslation("Settings")(withRouter(AccessPortal)));
