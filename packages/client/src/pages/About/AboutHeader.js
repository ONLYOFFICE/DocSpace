import React from "react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import Headline from "@docspace/common/components/Headline";
import IconButton from "@docspace/components/icon-button";
import { tablet } from "@docspace/components/utils/device";

const HeaderContainer = styled.div`
  position: relative;
  display: flex;
  align-items: center;
  max-width: calc(100vw - 32px);

  .arrow-button {
    margin-right: 12px;

    @media ${tablet} {
      padding: 8px 0 8px 8px;
      margin-left: -8px;
    }
  }
`;

const AboutHeader = (props) => {
  const { t, history } = props;

  const onBack = () => {
    history.goBack();
  };

  return (
    <HeaderContainer>
      <IconButton
        iconName="/static/images/arrow.path.react.svg"
        size="17"
        isFill={true}
        onClick={onBack}
        className="arrow-button"
      />
      <Headline type="content" truncate={true}>
        {t("AboutHeader")}
      </Headline>
    </HeaderContainer>
  );
};

export default withRouter(withTranslation(["About"])(AboutHeader));
