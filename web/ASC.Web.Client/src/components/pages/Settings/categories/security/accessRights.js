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

const StyledArrowRightIcon = styled(ArrowRightIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.color};
  }
`;

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

class AccessRights extends PureComponent {
  constructor(props) {
    super(props);

    const { t } = props;

    setDocumentTitle(t("ManagementCategorySecurity"));
  }

  async componentDidMount() {
    const { admins, getUpdateListAdmin } = this.props;

    showLoader();
    if (isEmpty(admins, true)) {
      try {
        await getUpdateListAdmin();
      } catch (error) {
        toastr.error(error);
      }
    }

    hideLoader();
  }

  render() {
    const { t, admins } = this.props;
    return (
      <MainContainer>
        <OwnerSettings />
        {/*<div className="category-item-wrapper">
          <div className="category-item-heading">
            <Link
              className="inherit-title-link header"
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
          </div>*/}
      </MainContainer>
    );
  }
}

export default inject(({ auth, setup }) => {
  const { getUpdateListAdmin } = setup;
  const { admins } = setup.security.accessRight;
  return {
    admins,
    getUpdateListAdmin,
    organizationName: auth.settingsStore.organizationName,
  };
})(withTranslation("Settings")(withRouter(AccessRights)));
