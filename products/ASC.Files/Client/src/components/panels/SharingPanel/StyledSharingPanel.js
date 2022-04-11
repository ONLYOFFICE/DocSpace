import styled, { css } from "styled-components";

const StyledContent = styled.div`
  width: 100%;
  height: 100%;

  display: grid;

  grid-template-columns: 1fr;
  grid-template-rows: auto 1fr auto;
`;

const StyledHeaderContent = styled.div`
  width: calc(100% - 16px);
  max-width: calc(100% - 16px);
  height: 52px;

  border-bottom: 1px solid #eceef1;

  padding: 0 16px;

  margin-bottom: 24px;
  margin-right: -16px;

  display: flex;
  align-items: center;
  justify-content: space-between;

  @media (max-width: 480px) {
    width: calc(100vw - 32px);
    max-width: calc(100vw - 32px);
  }

  .sharing_panel-header-info {
    display: flex;
    align-items: center;
    justify-content: start;

    width: calc(100% - 33px);
    max-width: calc(100% - 33px);

    .sharing_panel-arrow {
      margin-right: 16px;
    }

    .sharing_panel-header {
    }
  }

  .embedding_panel {
    width: 100%;
    display: flex;
    align-items: center;
    justify-content: start;
  }

  .sharing_panel-icons-container {
    display: flex;
    margin-left: 16px;
  }
`;

const StyledBodyContent = styled.div`
  width: calc(100%);
  height: 100%;

  padding: 0 0 0 16px;
  margin-right: -16px;

  display: flex;
  flex-direction: column;
  align-items: start;

  .panel-loader-wrapper {
    display: flex;
    align-items: center;

    span {
      margin-left: 8px;
    }
  }
`;

const StyledExternalLink = styled.div`
  width: 100%;

  padding: 12px;
  margin-bottom: 32px;

  box-sizing: border-box;

  background-color: #f8f9f9;

  display: flex;
  flex-direction: column;
  align-items: center;

  border-radius: 6px;

  .external-link__unchecked {
    width: 100%;

    display: flex;
    align-items: center;
    justify-content: space-between;

    .external-link__toggler {
      position: relative;

      span {
        font-weight: 600;
        font-size: 14px;
        line-height: 16px;
      }
    }

    .external-link__share {
      .fixed-max-width {
        top: 18px;
      }
    }
  }

  .external-link__checked {
    padding-top: 12px;
    width: 100%;

    display: flex;
    align-items: center;
    justify-content: start;

    .external-link__input-link {
      flex-direction: row-reverse;
      padding-right: 0px;

      .external-link__buttons {
        height: 100%;

        display: flex;
        align-items: center;

        padding: 4px 16px 4px 24px;

        .external-link__code {
          margin-right: 12px;

          cursor: pointer;
          path {
            fill: #333333 !important;
          }
        }

        .external-link__copy {
          cursor: pointer;
          path {
            fill: #333333 !important;
          }
        }
      }

      .append {
        display: none;
      }
    }

    .panel_combo-box {
      .combo-button {
        min-width: 46px;
        height: 32px;

        .sharing-access-combo-box-icon {
          display: flex;
          align-items: center;
        }
      }
      .dropdown-container {
        top: 32px;
      }
    }
  }
`;

const StyledInternalLink = styled.div`
  width: 100%;

  display: flex;
  align-items: center;
  justify-content: space-between;

  margin-bottom: 20px;

  .internal-link__link-text {
    line-height: 22px !important;
    font-size: 16px !important;
    font-weight: 700 !important;
  }

  .internal-link__copy-text {
    line-height: 15px !important;
    font-weight: 600 !important;

    border-bottom: 1px dashed #333333;

    cursor: pointer;
  }
`;

const StyledItem = styled.div`
  width: 100%;

  display: flex;
  align-items: center;
  justify-content: space-between;

  padding: 8px 0;

  ${(props) =>
    props.isEndOfBlock &&
    css`
      margin-bottom: 16px;
    `}

  .item__info-block {
    display: flex;
    align-items: center;
    justify-content: start;

    .info-block__text {
      margin-left: 12px;
    }
  }

  .item__change-owner {
    line-height: 15px !important;
    font-weight: 600 !important;

    border-bottom: 1px dashed #333333;

    cursor: pointer;
  }

  .item__owner {
    color: rgb(163, 169, 174);
  }

  .panel_combo-box {
    .combo-button {
      min-width: 46px;
      height: 32px;

      .sharing-access-combo-box-icon {
        display: flex;
        align-items: center;
      }
    }
  }
`;

const StyledFooterContent = styled.div`
  width: calc(100% - 16px);
  border-top: 1px solid #eceef1;

  padding: 16px;

  display: flex;
  flex-direction: column;
  align-items: start;

  .sharing_panel-notification {
    margin-bottom: 16px;
  }

  .sharing_panel-checkbox {
    margin-bottom: 16px;
  }
`;

export {
  StyledContent,
  StyledHeaderContent,
  StyledBodyContent,
  StyledExternalLink,
  StyledInternalLink,
  StyledItem,
  StyledFooterContent,
};
