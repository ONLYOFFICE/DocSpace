import ArrowPathReactSvgUrl from "PUBLIC_DIR/images/arrow.path.react.svg?url";
import React from "react";
import styled, { css } from "styled-components";
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
    ${({ theme }) =>
      theme.interfaceDirection === "rtl"
        ? `margin-left: 12px;`
        : `margin-right: 12px;`}

    @media ${tablet} {
      ${({ theme }) =>
        theme.interfaceDirection === "rtl"
          ? css`
              padding: 8px 8px 8px 0;
              margin-right: -8px;
            `
          : css`
              padding: 8px 0 8px 8px;
              margin-left: -8px;
            `}
    }

    svg {
      ${({ theme }) =>
        theme.interfaceDirection === "rtl" && "transform: scaleX(-1);"}
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
