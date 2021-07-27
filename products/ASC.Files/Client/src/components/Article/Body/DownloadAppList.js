import React from "react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import Text from "@appserver/components/text";
import IconButton from "@appserver/components/icon-button";
import Loaders from "@appserver/common/components/Loaders";

import withLoader from "../../../HOCs/withLoader";

const StyledDownloadAppList = styled.div`
  margin-top: 42px;

  .download-app-list {
    padding-top: 3px;
    display: flex;
    max-width: inherit;
  }

  .icon-button {
    padding: 5px;
  }
`;

const StyledRectangleLoader = styled(Loaders.Rectangle)`
  margin-top: 42px;
`;

const DownloadAppListContainer = ({ t }) => {
  return (
    <StyledDownloadAppList>
      <Text color="#555F65" fontSize="14px" fontWeight={600}>
        {t("Translations:DownloadApps")}
      </Text>
      <div className="download-app-list">
        <IconButton
          onClick={() =>
            window.open(
              "https://www.onlyoffice.com/download-desktop.aspx#windows"
            )
          }
          className="icon-button"
          iconName="/static/images/windows.react.svg"
          size="25"
          isfill={true}
          color="#A3A9AE"
          hoverColor="#3785D3"
        />
        <IconButton
          onClick={() =>
            window.open("https://www.onlyoffice.com/download-desktop.aspx#mac")
          }
          className="icon-button"
          iconName="/static/images/macOS.react.svg"
          size="25"
          isfill={true}
          color="#A3A9AE"
          hoverColor="#000000"
        />
        <IconButton
          onClick={() =>
            window.open(
              "https://www.onlyoffice.com/download-desktop.aspx#linux"
            )
          }
          className="icon-button"
          iconName="/static/images/linux.react.svg"
          size="25"
          isfill={true}
          color="#A3A9AE"
          hoverColor="#FFB800"
        />
        <IconButton
          onClick={() =>
            window.open("https://www.onlyoffice.com/office-for-android.aspx")
          }
          className="icon-button"
          iconName="/static/images/android.react.svg"
          size="25"
          isfill={true}
          color="#A3A9AE"
          hoverColor="#9BD71C"
        />
        <IconButton
          onClick={() =>
            window.open("https://www.onlyoffice.com/office-for-ios.aspx")
          }
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
  withLoader(DownloadAppListContainer)(<StyledRectangleLoader />)
);

export default DownloadAppList;
