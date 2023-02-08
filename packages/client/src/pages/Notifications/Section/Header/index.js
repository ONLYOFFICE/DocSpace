import React from "react";
import IconButton from "@docspace/components/icon-button";
import config from "PACKAGE_FILE";
import { combineUrl } from "@docspace/common/utils";
import Headline from "@docspace/common/components/Headline";
import styled from "styled-components";

const StyledHeader = styled.div`
  display: flex;
  align-items: center;
  .arrow-button {
    margin-right: 16px;
  }
`;
const SectionHeaderContent = ({ history, t }) => {
  const onClickBack = () => {
    history.push(
      combineUrl(
        window.DocSpaceConfig?.proxy?.url,
        config.homepage,
        "/accounts/view/@self"
      )
    );
  };
  return (
    <StyledHeader>
      <IconButton
        iconName="/static/images/arrow.path.react.svg"
        size="17"
        isFill={true}
        onClick={onClickBack}
        className="arrow-button"
      />
      <Headline type="content" truncate>
        {t("Notifications")}
      </Headline>
    </StyledHeader>
  );
};

export default SectionHeaderContent;
