import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import Text from "@docspace/components/text";
import Checkbox from "@docspace/components/checkbox";
import Button from "@docspace/components/button";
import Link from "@docspace/components/link";

import { getLogoFromPath } from "@docspace/common/utils";
import { getDeepLink } from "../../helpers/deepLinkHelper";

import {
  StyledSimpleNav,
  StyledDeepLink,
  StyledBodyWrapper,
  StyledFileTile,
  StyledActionsWrapper,
} from "./StyledDeepLink";

const DeepLink = ({
  fileInfo,
  logoUrls,
  userEmail,
  setIsShowDeepLink,
  currentColorScheme,
  deepLinkUrl,
  theme,
}) => {
  const { t } = useTranslation(["DeepLink", "Common"]);

  const [isRemember, setIsRemember] = useState(false);
  const onChangeCheckbox = () => {
    setIsRemember(!isRemember);
  };

  const onOpenAppClick = () => {
    if (isRemember) localStorage.setItem("defaultOpenDocument", "app");
    window.location = getDeepLink(
      window.location.origin,
      userEmail,
      fileInfo,
      deepLinkUrl
    );
  };

  const onStayBrowserClick = () => {
    if (isRemember) localStorage.setItem("defaultOpenDocument", "web");
    setIsShowDeepLink(false);
  };

  const getFileIcon = () => {
    const fileExst = fileInfo.fileExst.slice(1);
    const iconPath = "/static/images/icons/32/";
    return `${iconPath}${fileExst}.svg`;
  };

  const getFileTitle = () => {
    return fileInfo.fileExst
      ? fileInfo.title.split(".").slice(0, -1).join(".")
      : fileInfo.title;
  };

  const logoPath = theme.isBase
    ? logoUrls[0]?.path?.light
    : logoUrls[0]?.path?.dark;
  const logo = getLogoFromPath(logoPath);

  return (
    <>
      <StyledSimpleNav>
        <img src={logo} />
      </StyledSimpleNav>
      <StyledDeepLink>
        <StyledBodyWrapper>
          <Text fontSize="23px" fontWeight="700">
            {t("OpeningDocument")}
          </Text>
          <StyledFileTile>
            <img src={getFileIcon()} />
            <Text fontSize="14px" fontWeight="600" truncate>
              {getFileTitle()}
            </Text>
          </StyledFileTile>
          <Text>{t("DeepLinkText")}</Text>
        </StyledBodyWrapper>
        <StyledActionsWrapper>
          <Checkbox
            label={t("Common:Remember")}
            isChecked={isRemember}
            onChange={onChangeCheckbox}
          />
          <Button
            size="medium"
            primary
            label={t("OpenInApp")}
            onClick={onOpenAppClick}
          />
          <Link
            className="stay-link"
            type="action"
            fontSize="13px"
            fontWeight="600"
            isHovered
            color={currentColorScheme?.main?.accent}
            onClick={onStayBrowserClick}
          >
            {t("StayInBrowser")}
          </Link>
        </StyledActionsWrapper>
      </StyledDeepLink>
    </>
  );
};

export default inject(({ auth }) => {
  const { theme } = auth.settingsStore;

  return {
    theme,
  };
})(observer(DeepLink));
