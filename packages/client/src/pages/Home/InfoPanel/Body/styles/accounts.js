import styled, { css } from "styled-components";
import { Base } from "@docspace/components/themes";

const StyledAccountsItemTitle = styled.div`
  min-height: 104px;
  height: 104px;
  max-height: 104px;

  display: flex;
  align-items: center;
  justify-content: start;
  gap: 16px;

  .avatar {
    padding-top: 24px;
    min-width: 80px;
  }

  .info-panel__info-text {
    padding-top: 24px;

    display: flex;
    flex-direction: ${(props) => (props.isPending ? "row" : "column")};

    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;

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
    }
  }

  .context-button {
    padding-top: 24px;

    margin-left: auto;
  }
`;

StyledAccountsItemTitle.defaultProps = { theme: Base };

const StyledAccountContent = styled.div`
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

      .backdrop-active {
        height: 100%;
        width: 100%;
        z-index: 1000;
      }
    }
  }
`;

export { StyledAccountsItemTitle, StyledAccountContent };
