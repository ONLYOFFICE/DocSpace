import WindowsReactSvgUrl from "PUBLIC_DIR/images/windows.react.svg?url";
import MacOSReactSvgUrl from "PUBLIC_DIR/images/macOS.react.svg?url";
import LinuxReactSvgUrl from "PUBLIC_DIR/images/linux.react.svg?url";
import AndroidReactSvgUrl from "PUBLIC_DIR/images/android.react.svg?url";
import IOSReactSvgUrl from "PUBLIC_DIR/images/iOS.react.svg?url";
import React from "react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";

import { Base } from "@docspace/components/themes";

const StyledDownloadAppList = styled.div`
  margin-top: 20px;

  .download-app-text {
    color: ${(props) => props.theme.filesArticleBody.downloadAppList.color};
  }

  .download-app-list {
    padding-top: 3px;
    display: flex;
    max-width: inherit;
  }

  .icon-button {
    padding: 5px;
  }
`;

StyledDownloadAppList.defaultProps = { theme: Base };

const DownloadAppListContainer = React.memo(({ t, theme }) => {
  const desktopLink = "https://www.onlyoffice.com/desktop.aspx";
  const androidLink = "https://www.onlyoffice.com/office-for-android.aspx";
  const iosLink = "https://www.onlyoffice.com/office-for-ios.aspx";

  return (
    <StyledDownloadAppList>
      <Text className="download-app-text" fontSize="14px" noSelect={true}>
        {t("Translations:DownloadApps")}
      </Text>
      <div className="download-app-list">
        <IconButton
          onClick={() => window.open(desktopLink)}
          className="icon-button"
          iconName={WindowsReactSvgUrl}
          size="25"
          isfill={true}
          hoverColor={theme.filesArticleBody.downloadAppList.winHoverColor}
          title={t("Translations:MobileWin")}
        />
        <IconButton
          onClick={() => window.open(desktopLink)}
          className="icon-button"
          iconName={MacOSReactSvgUrl}
          size="25"
          isfill={true}
          hoverColor={theme.filesArticleBody.downloadAppList.macHoverColor}
          title={t("Translations:MobileMac")}
        />
        <IconButton
          onClick={() => window.open(desktopLink)}
          className="icon-button"
          iconName={LinuxReactSvgUrl}
          size="25"
          isfill={true}
          hoverColor={theme.filesArticleBody.downloadAppList.linuxHoverColor}
          title={t("Translations:MobileLinux")}
        />
        <IconButton
          onClick={() => window.open(androidLink)}
          className="icon-button"
          iconName={AndroidReactSvgUrl}
          size="25"
          isfill={true}
          hoverColor={theme.filesArticleBody.downloadAppList.androidHoverColor}
          title={t("Translations:MobileAndroid")}
        />
        <IconButton
          onClick={() => window.open(iosLink)}
          className="icon-button"
          iconName={IOSReactSvgUrl}
          size="25"
          isfill={true}
          hoverColor={theme.filesArticleBody.downloadAppList.iosHoverColor}
          title={t("Translations:MobileIos")}
        />
      </div>
    </StyledDownloadAppList>
  );
});

DownloadAppListContainer.defaultProps = { theme: Base };

const DownloadAppList = withTranslation(["Translations"])(
  DownloadAppListContainer
);

export default DownloadAppList;
