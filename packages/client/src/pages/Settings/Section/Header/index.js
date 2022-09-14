import React from "react";
import styled, { css } from "styled-components";
import { useTranslation } from "react-i18next";
import Headline from "@docspace/common/components/Headline";
import { Consumer } from "@docspace/components/utils/context";
import { isMobile, isMobileOnly } from "react-device-detect";
import { tablet, mobile } from "@docspace/components/utils/device";

const StyledContainer = styled.div`
  width: 100%;
  height: 69px;

  @media ${tablet} {
    height: 61px;
  }

  ${isMobile &&
  css`
    height: 61px;
  `}

  @media ${mobile} {
    height: 53px;
  }

  ${isMobileOnly &&
  css`
    height: 53px;
  `}

  .header-container {
    position: relative;

    width: 100%;
    height: 100%;

    display: grid;
    align-items: center;

    grid-template-columns: auto auto 1fr;

    @media ${tablet} {
      grid-template-columns: 1fr auto;
    }

    ${isMobile &&
    css`
      grid-template-columns: 1fr auto;
    `}

    .headline-header {
      line-height: 24px;

      @media ${tablet} {
        font-size: 21px;
        line-height: 28px;
      }

      ${isMobile &&
      css`
        line-height: 28px;
      `}

      @media ${mobile} {
        line-height: 24px;
      }

      ${isMobile &&
      css`
        line-height: 24px;
      `}
    }

    .action-button {
      margin-left: 16px;

      @media ${tablet} {
        display: none;
      }

      ${isMobile &&
      css`
        display: none;
      `}
    }
  }
`;

const SectionHeaderContent = () => {
  const { t } = useTranslation("Common");
  return (
    <Consumer>
      {(context) => (
        <StyledContainer isHeaderVisible={true} width={context.sectionWidth}>
          <div className="header-container">
            <Headline
              fontSize="18px"
              className="headline-header"
              type="content"
              truncate={true}
            >
              {t("Common:Settings")}
            </Headline>
          </div>
        </StyledContainer>
      )}
    </Consumer>
  );
};

export default SectionHeaderContent;
