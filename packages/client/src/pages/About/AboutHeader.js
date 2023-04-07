import ArrowPathReactSvgUrl from "PUBLIC_DIR/images/arrow.path.react.svg?url";
import React from "react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
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
  const { t } = props;

  const navigate = useNavigate();

  const onBack = () => {
    navigate(-1);
  };

  return (
    <HeaderContainer>
      <IconButton
        iconName={ArrowPathReactSvgUrl}
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

export default withTranslation(["About"])(AboutHeader);
