import styled from "styled-components";
import { isMobile } from "react-device-detect";

import { mobile, tablet } from "@docspace/components/utils/device";

import { Base } from "@docspace/components/themes";

const StyledInfoBody = styled.div`
  height: auto;
  padding-left: 20px;

  background-color: ${(props) => props.theme.infoPanel.backgroundColor};
  color: ${(props) => props.theme.infoPanel.textColor};

  @media ${tablet} {
    padding-left: 16px;
  }

  ${isMobile &&
  css`
    padding-left: 16px;
  `}
`;

StyledInfoBody.defaultProps = { theme: Base };

const StyledInfoHeaderContainer = styled.div`
  min-height: 104px;
  height: 104px;
  max-height: 104px;

  padding-top: 24px;

  display: flex;
  align-items: end;
  justify-content: start;

  .info-panel__info-text {
    padding-left: 16px;
    padding-bottom: 19px;

    display: flex;
    flex-direction: column;

    .info-text__name {
      font-weight: 700;
      font-size: 16px;
      line-height: 22px;
    }

    .info-text__email {
      font-weight: 600;
      font-size: 13px;
      line-height: 20px;
      color: ${(props) => props.theme.text.disableColor};
    }
  }
`;

StyledInfoHeaderContainer.defaultProps = { theme: Base };

const StyledInfoDataContainer = styled.div`
  .data__header {
    width: 100%;
    padding: 24px 0;

    .header__text {
      font-weight: 600;
      font-size: 14px;
      line-height: 16px;
    }
  }

  .data__body {
    display: grid;
    grid-template-rows: 24px 24px 24px 24px;
    grid-template-columns: 72px 1fr;

    grid-gap: 0 24px;
  }
`;

export { StyledInfoBody, StyledInfoHeaderContainer, StyledInfoDataContainer };
