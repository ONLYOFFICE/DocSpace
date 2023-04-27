import styled from "styled-components";
import { Base } from "@docspace/components/themes";
import { hugeMobile, tablet } from "@docspace/components/utils/device";

const StyledAccountsItemTitle = styled.div`
  min-height: 80px;
  height: 80px;
  max-height: 104px;
  display: flex;
  align-items: center;
  justify-content: start;
  gap: 16px;
  position: fixed;
  margin-top: -80px;
  margin-left: -20px;
  width: calc(100% - 40px);
  padding: 24px 0 24px 20px;
  background: ${(props) => props.theme.infoPanel.backgroundColor};
  z-index: 100;

  @media ${tablet} {
    width: 440px;
    padding: 24px 20px 24px 20px;
  }

  @media ${hugeMobile} {
    width: calc(100vw - 32px);
    padding: 24px 0 24px 16px;
  }

  .avatar {
    min-width: 80px;
  }

  .info-panel__info-text {
    display: flex;
    flex-direction: column;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;

    .info-panel__info-wrapper {
      display: flex;
      flex-direction: row;
    }

    .badges {
      height: 22px;
      margin-left: 8px;
    }

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
      user-select: text;
    }

    .sso-badge {
      margin-top: 8px;
    }
  }

  .context-button {
    padding-top: 24px;
    margin-left: auto;
  }
`;

StyledAccountsItemTitle.defaultProps = { theme: Base };

const StyledAccountContent = styled.div`
  margin: 80px auto 0;

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
    grid-template-rows: 28px 28px 28px 28px;
    grid-template-columns: 80px 1fr;
    grid-gap: 0 24px;
    align-items: center;

    .type-combobox {
      margin-left: -8px;

      .combo-button {
        padding-left: 8px;
      }

      .backdrop-active {
        height: 100%;
        width: 100%;
        z-index: 1000;
      }
    }
  }
`;

export { StyledAccountsItemTitle, StyledAccountContent };
