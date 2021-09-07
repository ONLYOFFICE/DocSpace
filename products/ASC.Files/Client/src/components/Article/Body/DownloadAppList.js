import React from "react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import Text from "@appserver/components/text";
import IconButton from "@appserver/components/icon-button";

import withLoader from "../../../HOCs/withLoader";

const StyledDownloadAppList = styled.div`
  margin-top: 20px;

  .download-app-list {
    padding-top: 3px;
    display: flex;
    max-width: inherit;
  }

  .icon-button {
    padding: 5px;
  }
`;

const DownloadAppListContainer = ({ t }) => {
  const windowsLink =
    "https://www.onlyoffice.com/download-desktop.aspx#windows";
  const macLink = "https://www.onlyoffice.com/download-desktop.aspx#mac";
  const linuxLink = "https://www.onlyoffice.com/download-desktop.aspx#linux";
  const androidLink = "https://www.onlyoffice.com/office-for-android.aspx";
  const iosLink = "https://www.onlyoffice.com/office-for-ios.aspx";

  return (
    <StyledDownloadAppList>
      <Text color="#83888d" fontSize="14px">
        {t("Translations:DownloadApps")}
      </Text>
      <div className="download-app-list">
        <IconButton
          onClick={() => window.open(windowsLink)}
          className="icon-button"
          iconName="/static/images/windows.react.svg"
          size="25"
          isfill={true}
          color="#A3A9AE"
          hoverColor="#3785D3"
        />
        <IconButton
          onClick={() => window.open(macLink)}
          className="icon-button"
          iconName="/static/images/macOS.react.svg"
          size="25"
          isfill={true}
          color="#A3A9AE"
          hoverColor="#000000"
        />
        <IconButton
          onClick={() => window.open(linuxLink)}
          className="icon-button"
          iconName="/static/images/linux.react.svg"
          size="25"
          isfill={true}
          color="#A3A9AE"
          hoverColor="#FFB800"
        />
        <IconButton
          onClick={() => window.open(androidLink)}
          className="icon-button"
          iconName="/static/images/android.react.svg"
          size="25"
          isfill={true}
          color="#A3A9AE"
          hoverColor="#9BD71C"
        />
        <IconButton
          onClick={() => window.open(iosLink)}
          className="icon-button"
          iconName="/static/images/iOS.react.svg"
          size="25"
          isfill={true}
          color="#A3A9AE"
          hoverColor="#000000"
        />
      </div>
    </StyledDownloadAppList>
  );
};

const DownloadAppList = withTranslation(["Translations"])(
  withLoader(DownloadAppListContainer)(<></>)
);

export default DownloadAppList;
