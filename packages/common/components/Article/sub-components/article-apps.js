import WindowsReactSvgUrl from "PUBLIC_DIR/images/windows.react.svg?url";
import MacOSReactSvgUrl from "PUBLIC_DIR/images/macOS.react.svg?url";
import LinuxReactSvgUrl from "PUBLIC_DIR/images/linux.react.svg?url";
import AndroidReactSvgUrl from "PUBLIC_DIR/images/android.react.svg?url";
import IOSReactSvgUrl from "PUBLIC_DIR/images/iOS.react.svg?url";
import IOSHoverReactSvgUrl from "PUBLIC_DIR/images/iOSHover.react.svg?url";

import React from "react";
import styled, { css } from "styled-components";
import { useTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";

import { Base } from "@docspace/components/themes";
import { desktop, tablet, hugeMobile } from "@docspace/components/utils/device";
import { isTablet } from "react-device-detect";

const StyledArticleApps = styled.div`
  display: flex;
  flex-direction: column;
  gap: 8px;
  position: relative;
  margin-top: auto;
  margin-bottom: 16px;

  @media ${tablet} {
    ${(props) =>
      props.showText &&
      css`
        ${({ theme }) =>
          theme.interfaceDirection === "rtl"
            ? `margin-right: 8px;`
            : `margin-left: 8px;`}
      `}
  }

  @media ${hugeMobile} {
    position: relative;
    bottom: 0px;
    margin-top: 32px;
  }

  .download-app-text {
    color: ${(props) => props.theme.filesArticleBody.downloadAppList.color};
  }

  .download-app-list {
    display: flex;
    gap: 8px;
  }
`;

StyledArticleApps.defaultProps = { theme: Base };

const ArticleApps = React.memo(({ theme, showText }) => {
  const { t } = useTranslation(["Translations"]);

  const desktopLink = "https://www.onlyoffice.com/desktop.aspx";
  const androidLink = "https://www.onlyoffice.com/office-for-android.aspx";
  const iosLink = "https://www.onlyoffice.com/office-for-ios.aspx";

  if (!showText) return <></>;

  return (
    <StyledArticleApps showText={showText}>
      <Text className="download-app-text" fontSize="14px" noSelect={true}>
        {t("Translations:DownloadApps")}
      </Text>
      <div className="download-app-list">
        <IconButton
          onClick={() => window.open(desktopLink)}
          iconName={WindowsReactSvgUrl}
          size="32"
          isFill={true}
          hoverColor={theme.filesArticleBody.downloadAppList.winHoverColor}
          title={t("Translations:MobileWin")}
        />
        <IconButton
          onClick={() => window.open(desktopLink)}
          iconName={MacOSReactSvgUrl}
          size="32"
          isFill={true}
          hoverColor={theme.filesArticleBody.downloadAppList.macHoverColor}
          title={t("Translations:MobileMac")}
        />
        <IconButton
          onClick={() => window.open(desktopLink)}
          iconName={LinuxReactSvgUrl}
          size="32"
          isFill={true}
          hoverColor={theme.filesArticleBody.downloadAppList.linuxHoverColor}
          title={t("Translations:MobileLinux")}
        />
        <IconButton
          onClick={() => window.open(androidLink)}
          iconName={AndroidReactSvgUrl}
          size="32"
          isFill={true}
          hoverColor={theme.filesArticleBody.downloadAppList.androidHoverColor}
          title={t("Translations:MobileAndroid")}
        />
        <IconButton
          iconName={IOSReactSvgUrl}
          iconHoverName={IOSHoverReactSvgUrl}
          size="32"
          isFill={false}
          title={t("Translations:MobileIos")}
          onMouseDown={() => window.open(iosLink)}
          isClickable={true}
        />
      </div>
    </StyledArticleApps>
  );
});

ArticleApps.defaultProps = { theme: Base };

export default ArticleApps;
