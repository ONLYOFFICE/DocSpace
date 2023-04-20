import React, { useState } from "react";
import Text from "@docspace/components/text";
import Checkbox from "@docspace/components/checkbox";
import Button from "@docspace/components/button";
import Link from "@docspace/components/link";

import {
  StyledSimpleNav,
  StyledDeepLink,
  StyledBodyWrapper,
  StyledFileTile,
  StyledActionsWrapper,
} from "./StyledDeepLink";

const DeepLink = ({ fileInfo }) => {
  const [isRemember, setIsRemember] = useState(false);

  const onChangeCheckbox = () => {
    setIsRemember(!isRemember);
  };

  const onOpenAppClick = () => {
    if (isRemember) localStorage.setItem("defaultOpenDocument", "app");
    console.log("onOpenAppClick");
  };

  const onStayBrowserClick = () => {
    if (isRemember) localStorage.setItem("defaultOpenDocument", "web");
    console.log("onStayBrowserClick");
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

  return (
    <>
      <StyledSimpleNav>
        <img src="/static/images/logo/lightsmall.svg" />
      </StyledSimpleNav>
      <StyledDeepLink>
        <StyledBodyWrapper>
          <Text fontSize="23px" fontWeight="700">
            Opening a document
          </Text>
          <StyledFileTile>
            <img src={getFileIcon()} />
            <Text fontSize="14px" fontWeight="600" truncate>
              {getFileTitle()}
            </Text>
          </StyledFileTile>
          <Text>
            You can open the document on the portal or in the mobile application
          </Text>
        </StyledBodyWrapper>
        <StyledActionsWrapper>
          <Checkbox
            label={"Remember"}
            isChecked={isRemember}
            onChange={onChangeCheckbox}
          />
          <Button
            size="medium"
            primary
            label="Open in the app"
            onClick={onOpenAppClick}
          />
          <Link
            className="stay-link"
            type="action"
            fontSize="13px"
            fontWeight="600"
            isHovered
            color="#316DAA"
            onClick={onStayBrowserClick}
          >
            Stay in the browser
          </Link>
        </StyledActionsWrapper>
      </StyledDeepLink>
    </>
  );
};

export default DeepLink;
