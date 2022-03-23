import React from "react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import Text from "@appserver/components/text";
import IconButton from "@appserver/components/icon-button";

import { Base } from "@appserver/components/themes";

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

const DownloadAppListContainer = React.memo(({ t }) => {
  const desktopLink = "https://www.onlyoffice.com/desktop.aspx";
  const androidLink = "https://www.onlyoffice.com/office-for-android.aspx";
  const iosLink = "https://www.onlyoffice.com/office-for-ios.aspx";

  return (
    <StyledDownloadAppList>
      <Text className="download-app-text" fontSize="14px">
        {t("Translations:DownloadApps")}
      </Text>
      <div className="download-app-list">
        <IconButton
          onClick={() => window.open(desktopLink)}
          className="icon-button"
          iconName="/static/images/windows.react.svg"
          size="25"
          isfill={true}
          hoverColor="#3785D3"
          title={t("Translations:MobileWin")}
        />
        <IconButton
          onClick={() => window.open(desktopLink)}
          className="icon-button"
          iconName="/static/images/macOS.react.svg"
          size="25"
          isfill={true}
          hoverColor="#000000"
          title={t("Translations:MobileMac")}
        />
        <IconButton
          onClick={() => window.open(desktopLink)}
          className="icon-button"
          iconName="/static/images/linux.react.svg"
          size="25"
          isfill={true}
          hoverColor="#FFB800"
          title={t("Translations:MobileLinux")}
        />
        <IconButton
          onClick={() => window.open(androidLink)}
          className="icon-button"
          iconName="/static/images/android.react.svg"
          size="25"
          isfill={true}
          hoverColor="#9BD71C"
          title={t("Translations:MobileAndroid")}
        />
        <IconButton
          onClick={() => window.open(iosLink)}
          className="icon-button"
          iconName="/static/images/iOS.react.svg"
          size="25"
          isfill={true}
          hoverColor="#000000"
          title={t("Translations:MobileIos")}
        />
      </div>
    </StyledDownloadAppList>
  );
});

const DownloadAppList = withTranslation(["Translations"])(
  DownloadAppListContainer
);

export default DownloadAppList;
