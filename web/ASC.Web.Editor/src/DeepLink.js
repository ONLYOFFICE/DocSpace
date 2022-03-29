import React, { useState } from "react";
import Text from "@appserver/components/text";
import Button from "@appserver/components/button";
import Link from "@appserver/components/link";
import Checkbox from "@appserver/components/checkbox";
import { useTranslation } from "react-i18next";
import { StyledHeader, StyledBody, StyledFileTile } from "./StyledDeepLink";
import { getDeepLink } from "./helpers/utils";

const SimpleHeader = () => {
  return (
    <StyledHeader>
      <img alt="logo" src="/static/images/nav.logo.opened.react.svg" />
    </StyledHeader>
  );
};

const DeepLinkPage = (props) => {
  const { settings, fileInfo, userEmail, onStayBrowser } = props;
  const [isChecked, setIsChecked] = useState(false);
  const { t } = useTranslation(["Editor", "Common"]);

  const onChangeCheckbox = () => {
    setIsChecked(!isChecked);
  };

  const getFileIcon = () => {
    const fileExst = fileInfo.fileExst.slice(1);
    const iconPath = "/static/images/icons/32/";
    return `${iconPath}${fileExst}.svg`;
  };

  const getTitle = () => {
    return fileInfo.fileExst
      ? fileInfo.title.split(".").slice(0, -1).join(".")
      : fileInfo.title;
  };

  const onOpenApp = () => {
    window.location = getDeepLink(
      window.location.origin,
      userEmail,
      fileInfo,
      settings
    );
  };

  const onStayWeb = () => {
    onStayBrowser();
  };

  const onOpenAppClick = () => {
    if (isChecked) localStorage.setItem("defaultOpen", "app");
    onOpenApp();
  };

  const onOpenBrowserClick = () => {
    if (isChecked) localStorage.setItem("defaultOpen", "web");
    onStayWeb();
  };

  return (
    <>
      <SimpleHeader />
      <StyledBody>
        <Text fontSize="23px" fontWeight="700">
          {t("Common:OpeningDocument")}
        </Text>
        <StyledFileTile>
          <img src={getFileIcon()} />
          <Text fontSize="14px" fontWeight="600" truncate>
            {getTitle()}
          </Text>
        </StyledFileTile>
        <Text className="description" fontSize="13px" fontWeight="400">
          {t("Common:DeepLinkDescription")}
        </Text>
        <Checkbox
          label={t("Common:Remember")}
          onChange={onChangeCheckbox}
          isChecked={isChecked}
        />
        <Button
          className="button"
          label={t("Common:OpenInApp")}
          onClick={onOpenAppClick}
          primary
          scale
          size="normal"
        />

        <Link
          className="link"
          color="#316DAA"
          fontWeight="600"
          onClick={onOpenBrowserClick}
          target="_self"
          type="action"
        >
          {t("Common:StayBrowser")}
        </Link>
      </StyledBody>
    </>
  );
};

export default DeepLinkPage;
