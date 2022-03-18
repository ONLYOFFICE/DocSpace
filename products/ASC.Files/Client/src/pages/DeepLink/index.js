import React, { useEffect, useState } from "react";
import styled from "styled-components";
import Text from "@appserver/components/text";
import Button from "@appserver/components/button";
import Link from "@appserver/components/link";
import Checkbox from "@appserver/components/checkbox";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import queryString from "query-string";
import api from "@appserver/common/api";
import { getTitleWithoutExst } from "../../helpers/files-helpers";
import { inject, observer } from "mobx-react";
import AppLoader from "@appserver/common/components/AppLoader";
import { Base } from "@appserver/components/themes";

const StyledBody = styled.div`
  display: flex;
  flex-direction: column;
  width: 100%;
  padding: 72px 16px 0 32px;

  .description {
    margin-bottom: 32px;
  }

  .button {
    margin-top: 32px;
    margin-bottom: 24px;
  }

  .link {
    text-align: center;
  }
`;

const StyledFileTile = styled.div`
  display: flex;
  gap: 16px;
  padding: 16px;
  margin: 16px 0;
  background: ${(props) => props.theme.deeplink.tile.background};
  border-radius: 3px;
  align-items: center;
`;

const DeepLinkPage = (props) => {
  const { t, location, getIconSrc, user, history, deepLinkSettings } = props;
  const defaultOpen = localStorage.getItem("deeplink");

  const [currentFile, setCurrentFile] = useState([]);
  const [isChecked, setIsChecked] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    const fileId = queryString.parse(location.search).fileId;
    const file = await api.files.getFileInfo(fileId);
    setCurrentFile(file);

    if (defaultOpen) {
      if (defaultOpen === "app") onOpenApp();
      else onStayBrowser();
    } else {
      setIsLoading(true);
    }
  };

  const getDeepLink = (file) => {
    const jsonData = {
      portal: window.location.origin,
      email: user.email,
      file: {
        id: file.fileId,
      },
      folder: {
        id: file.folderId,
        parentId: file.rootFolderId,
        rootFolderType: file.rootFolderType,
      },
    };
    const deepLinkData = btoa(JSON.stringify(jsonData));

    return `${deepLinkSettings.url}?data=${deepLinkData}`;
  };

  const onOpenApp = () => {
    const nav = navigator.userAgent;
    const storeUrl =
      nav.includes("iPhone;") || nav.includes("iPad;")
        ? `https://apps.apple.com/app/id${deepLinkSettings.iosPackageId}`
        : `https://play.google.com/store/apps/details?id=${deepLinkSettings.androidPackageName}`;

    window.location = getDeepLink(currentFile);

    setTimeout(() => {
      if (document.hasFocus()) {
        window.location.replace(storeUrl);
      } else {
        history.goBack();
      }
    }, 3000);
  };

  const onOpenAppClick = () => {
    if (isChecked) localStorage.setItem("deeplink", "app");
    onOpenApp();
  };

  const onStayBrowser = () => {
    const fileId = queryString.parse(location.search).fileId;
    const url = `/products/files/doceditor?fileId=${fileId}`;
    return window.open(url, "_self");
  };

  const onStayBrowserClick = () => {
    if (isChecked) localStorage.setItem("deeplink", "web");
    onStayBrowser();
  };

  const onChangeCheckbox = () => {
    setIsChecked(!isChecked);
  };

  if (!isLoading) return <AppLoader />;
  return (
    <StyledBody>
      <Text fontSize="23px" fontWeight="700">
        {t("OpeningDocument")}
      </Text>
      <StyledFileTile>
        <img src={getIconSrc(currentFile.fileExst, 32)}></img>
        <Text fontSize="14px" fontWeight="600" truncate>
          {getTitleWithoutExst(currentFile)}
        </Text>
      </StyledFileTile>
      <Text className="description" fontSize="13px" fontWeight="400">
        {t("DeepLinkDescription")}
      </Text>
      <Checkbox
        label={t("Common:Remember")}
        onChange={onChangeCheckbox}
        isChecked={isChecked}
      />
      <Button
        className="button"
        label={t("OpenInApp")}
        onClick={onOpenAppClick}
        primary
        scale
        size="normal"
      />

      <Link
        className="link"
        color="#316DAA"
        fontWeight="600"
        onClick={onStayBrowserClick}
        target="_self"
        type="action"
      >
        {t("StayBrowser")}
      </Link>
    </StyledBody>
  );
};

DeepLinkPage.defaultProps = { theme: Base };

export default inject(({ auth, settingsStore }) => {
  return {
    getIconSrc: settingsStore.getIconSrc,
    user: auth.userStore.user,
    deepLinkSettings: auth.settingsStore.deepLink.documents,
  };
})(withRouter(withTranslation(["DeepLink"])(observer(DeepLinkPage))));
