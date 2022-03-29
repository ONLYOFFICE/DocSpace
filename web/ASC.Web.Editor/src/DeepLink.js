import React, { useState } from "react";
import styled from "styled-components";
import Text from "@appserver/components/text";
import Button from "@appserver/components/button";
import Link from "@appserver/components/link";
import Checkbox from "@appserver/components/checkbox";
import { useTranslation } from "react-i18next";

const StyledBody = styled.div`
  display: flex;
  flex-direction: column;
  padding: 32px;
  z-index: 320;

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
  background: #f3f4f4;
  border-radius: 3px;
  align-items: center;
`;

const StyledHeader = styled.div`
  display: flex;
  height: 48px;
  background-color: #0f4071;
  align-items: center;

  img {
    padding-left: 32px;
  }
`;

const SimpleHeader = () => {
  return (
    <StyledHeader>
      <img alt="logo" src="/static/images/nav.logo.opened.react.svg" />
    </StyledHeader>
  );
};

const DeepLinkPage = (props) => {
  const { fileInfo, onStayBrowser } = props;
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

  const onStayWeb = () => {
    onStayBrowser();
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
            {fileInfo.title}
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
          //onClick={onOpenAppClick}
          primary
          scale
          size="normal"
        />

        <Link
          className="link"
          color="#316DAA"
          fontWeight="600"
          onClick={onStayWeb}
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
