import { Base } from "@appserver/components/themes";
import styled, { css } from "styled-components";

import { isMobile, isMobileOnly } from "react-device-detect";

const StyledContent = styled.div`
  width: 100%;
  height: 100%;

  display: grid;

  grid-template-columns: 1fr;
  grid-template-rows: auto 1fr auto;
`;

const StyledHeaderContent = styled.div`
  width: calc(100% - 32px);
  max-width: calc(100% - 32px);
  height: 53px;

  border-bottom: ${(props) => props.theme.filesPanels.sharing.borderBottom};

  padding: 0 16px;

  display: flex;
  align-items: center;
  justify-content: space-between;

  @media (max-width: 480px) {
    width: calc(100vw - 32px);
    max-width: calc(100vw - 32px);
  }

  ${isMobileOnly &&
  css`
    width: calc(100vw - 32px);
    max-width: calc(100vw - 32px);
  `}

  .sharing_panel-header-info {
    display: flex;
    align-items: center;
    justify-content: start;

    width: calc(100% - 33px);
    max-width: calc(100% - 33px);

    .sharing_panel-arrow {
      margin-right: 16px;
    }
  }

  .embedding_panel {
    width: 100%;
    display: flex;
    align-items: center;
    justify-content: start;
    margin-bottom: 16px;
  }

  .sharing_panel-icons-container {
    display: flex;
    margin-left: 16px;
  }
`;

StyledHeaderContent.defaultProps = { theme: Base };

const StyledBodyContent = styled.div`
  width: 100%;
  height: 100%;

  display: flex;
  flex-direction: column;
  align-items: start;
`;

const StyledExternalLink = styled.div`
  width: 100%;

  padding: 20px 16px;

  border-bottom: ${(props) => props.theme.filesPanels.sharing.borderBottom};

  box-sizing: border-box;

  display: flex;
  flex-direction: column;
  align-items: start;

  .external-link__base-line {
    display: flex;
    align-items: center;
    justify-content: start;
    flex-direction: row;

    .external-link__text {
      font-weight: 700;
      font-size: 16px;
      line-height: 22px;

      margin-right: 16px;
    }

    .external-link__toggler {
      position: relative;
    }
  }

  .external-link__checked {
    margin-top: 16px;
    width: 100%;

    display: flex;
    align-items: center;
    justify-content: start;

    .external-link__input-link {
      flex-direction: row-reverse;
      padding-right: 0px;

      .external-link__buttons {
        position: relative;

        height: 100%;

        display: flex;
        align-items: center;

        padding: 4px 16px;

        .external-link__code-icon {
          margin-right: 12px;

          cursor: pointer;
          path {
            fill: ${(props) =>
              props.theme.filesPanels.sharing.externalLinkSvg} !important;
          }
        }

        .external-link__share-icon {
          cursor: pointer;
          path {
            fill: ${(props) =>
              props.theme.filesPanels.sharing.externalLinkSvg} !important;
          }
        }

        external-link__share-dropdown {
        }
      }

      .append {
        display: none;
      }
    }

    .external-link__copy {
      margin-left: 8px;
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

  .external-link__access-rights {
    display: flex;
    align-items: center;
    justify-content: start;
    flex-direction: row;

    margin-top: 16px;

    .external-link__access-rights_text {
      color: #a3a9ae;
      margin-right: 8px;
    }
  }
`;

StyledExternalLink.defaultProps = { theme: Base };

const StyledInternalLink = styled.div`
  width: 100%;

  display: flex;
  align-items: center;
  justify-content: space-between;

  padding: 20px 16px;

  box-sizing: border-box;

  .internal-link__link-text {
    line-height: 22px !important;
    font-size: 16px !important;
    font-weight: 700 !important;
  }

  .internal-link__copy-text {
    line-height: 15px !important;
    font-weight: 600 !important;

    border-bottom: ${(props) =>
      props.theme.filesPanels.sharing.internalLinkBorder};

    cursor: pointer;
  }
`;

StyledInternalLink.defaultProps = { theme: Base };

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

    border-bottom: ${(props) => props.theme.filesPanels.sharing.itemBorder};

    cursor: pointer;
  }

  .item__owner {
    color: ${(props) => props.theme.filesPanels.sharing.itemOwnerColor};
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

StyledItem.defaultProps = { theme: Base };

const StyledFooterContent = styled.div`
  width: calc(100% - 32px);
  border-top: ${(props) => props.theme.filesPanels.sharing.borderTop};

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

StyledFooterContent.defaultProps = { theme: Base };

export {
  StyledContent,
  StyledHeaderContent,
  StyledBodyContent,
  StyledExternalLink,
  StyledInternalLink,
  StyledItem,
  StyledFooterContent,
};
