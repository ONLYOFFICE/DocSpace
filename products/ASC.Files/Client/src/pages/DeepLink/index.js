import React, { useEffect, useState } from "react";
import styled from "styled-components";
import Text from "@appserver/components/text";
import Button from "@appserver/components/button";
import Link from "@appserver/components/link";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import queryString from "query-string";
import api from "@appserver/common/api";
import { getTitleWithoutExst } from "../../helpers/files-helpers";
import { inject, observer } from "mobx-react";
import AppLoader from "@appserver/common/components/AppLoader";

const StyledBody = styled.div`
  display: flex;
  flex-direction: column;
  width: 100%;
  padding: 72px 16px 0 32px;

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
  background: #f3f4f4;
  border-radius: 3px;
  align-items: center;
`;

const DeepLinkPage = (props) => {
  const { t, location, getIconSrc, user, openDocEditor } = props;
  const [title, setTitle] = useState("");
  const [icon, setIcon] = useState("");
  const [fileUrl, setFileUrl] = useState("");
  const [deepLink, setDeepLink] = useState("");

  const [isLoading, setIsLoading] = useState(false);

  useEffect(async () => {
    const fileId = queryString.parse(location.search).fileId;
    const file = await api.files.getFileInfo(fileId);

    setTitle(getTitleWithoutExst(file));
    setIcon(getIconSrc(file.fileExst, 32));
    setFileUrl(file.webUrl);
    setDeepLink(getDeepLink(file));
    setIsLoading(true);
  }, []);

  const getDeepLink = (file) => {
    const jsonData = {
      portal: "", //TODO: get portal url
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

    return `oodocuments:://openfile?data=${deepLinkData}`;
  };

  const onOpenAppClick = () => {
    window.location = deepLink;
  };

  if (!isLoading) return <AppLoader />;
  return (
    <StyledBody>
      <Text fontSize="23px" fontWeight="700">
        {t("OpeningDocument")}
      </Text>
      <StyledFileTile>
        <img src={icon}></img>
        <Text fontSize="14px" fontWeight="600" truncate>
          {title}
        </Text>
      </StyledFileTile>
      <Text fontSize="13px" fontWeight="400">
        {t("DeepLinkDescription")}
      </Text>
      <Button
        className="button"
        label={t("OpenInApp")}
        onClick={onOpenAppClick}
        primary
        scale
        size="large"
      />

      <Link
        className="link"
        color="#316DAA"
        fontWeight="600"
        href={fileUrl}
        target="_self"
        type="action"
      >
        {t("StayBrowser")}
      </Link>
    </StyledBody>
  );
};

export default inject(({ auth, settingsStore, filesStore }) => {
  return {
    getIconSrc: settingsStore.getIconSrc,
    user: auth.userStore.user,
    openDocEditor: filesStore.openDocEditor,
  };
})(withRouter(withTranslation(["DeepLink"])(observer(DeepLinkPage))));
