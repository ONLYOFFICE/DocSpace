import React, { useEffect } from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import { setDocumentTitle } from "../../../../../../helpers/utils";
import { MainContainer, StyledArrowRightIcon } from "../StyledSecurity";

const MobileView = (props) => {
  const { t, history } = props;

  useEffect(() => {
    setDocumentTitle(t("PortalAccess"));
  }, []);

  const onClickLink = (e) => {
    e.preventDefault();
    history.push(e.target.pathname);
  };

  return (
    <MainContainer>
      <div className="category-item-wrapper">
        <div className="category-item-heading">
          <Link
            className="inherit-title-link header"
            onClick={onClickLink}
            truncate={true}
            href={combineUrl(
              AppServerConfig.proxyURL,
              "/settings/security/access-portal/password"
            )}
          >
            {t("SettingPasswordStrength")}
          </Link>
          <StyledArrowRightIcon size="small" />
        </div>
        <Text className="category-item-description">
          {t("SettingPasswordStrengthDescription")}
        </Text>
      </div>

      <div className="category-item-wrapper">
        <div className="category-item-heading">
          <Link
            className="inherit-title-link header"
            onClick={onClickLink}
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
};

export default withTranslation("Settings")(withRouter(MobileView));
