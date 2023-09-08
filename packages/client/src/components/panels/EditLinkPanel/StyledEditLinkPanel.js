import styled, { css } from "styled-components";
import Box from "@docspace/components/box";
import Scrollbar from "@docspace/components/scrollbar";

const StyledEditLinkPanel = styled.div`
  .edit-link-panel {
    .scroll-body {
      ${(props) =>
        props.theme.interfaceDirection === "rtl"
          ? css`
              padding-left: 0 !important;
            `
          : css`
              padding-right: 0 !important;
            `}
    }
  }

  .field-label-icon {
    display: none;
  }

  .edit-link_body {
    padding: 22px 0px 20px;

    .edit-link_link-block {
      padding: 0px 16px 20px 16px;

      .edit-link-text {
        display: inline-flex;
        margin-bottom: 4px;
      }

      .edit-link_required-icon {
        display: inline-flex;
        ${(props) =>
          props.theme.interfaceDirection === "rtl"
            ? css`
                margin-right: 2px;
              `
            : css`
                margin-left: 2px;
              `}
      }

      .edit-link_link-input {
        margin-bottom: 8px;
        margin-top: 16px;
      }
    }

    .edit-link-toggle-block {
      ${(props) =>
        props.theme.interfaceDirection === "rtl"
          ? css`
              padding: 0 20px 16px;
            `
          : css`
              padding: 0 16px 20px;
            `}

      border-top: ${(props) => props.theme.filesPanels.sharing.borderBottom};

      .edit-link-toggle-header {
        display: flex;
        padding-top: 20px;
        padding-bottom: 8px;

        .edit-link-toggle {
          ${(props) =>
            props.theme.interfaceDirection === "rtl"
              ? css`
                  margin-right: auto;
                  margin-left: 28px;
                `
              : css`
                  margin-left: auto;
                  margin-right: 28px;
                `}
        }
      }
      .edit-link_password-block {
        margin-top: 8px;
      }

      .password-field-wrapper {
        width: 100%;
      }
    }

    .edit-link-toggle-description {
      color: ${({ theme }) => theme.editLink.text.color};
    }

    .edit-link-toggle-description_expired {
      color: ${({ theme }) => theme.editLink.text.errorColor};
    }

    .edit-link_password-block {
      width: 100%;
      display: flex;

      .edit-link_password-input {
        width: 100%;
      }

      .edit-link_generate-icon {
        ${(props) =>
          props.theme.interfaceDirection === "rtl"
            ? css`
                margin: 16px 8px 0px 0px;
              `
            : css`
                margin: 16px 0px 0px 8px;
              `}
      }
    }
  }

  .edit-link_password-links {
    display: flex;
    gap: 12px;
    margin-top: -8px;
  }

  .edit-link_header {
    padding: 0 16px;
    border-bottom: ${(props) => props.theme.filesPanels.sharing.borderBottom};

    .edit-link_heading {
      font-weight: 700;
      font-size: 18px;
    }
  }

  .public-room_date-picker {
    padding-top: 8px;
    ${({ isExpired }) =>
      isExpired &&
      css`
        color: ${({ theme }) => theme.datePicker.errorColor};
      `};
  }
`;

const StyledScrollbar = styled(Scrollbar)`
  position: relative;
  padding: 16px 0;
  height: calc(100vh - 87px) !important;
`;

const StyledButtons = styled(Box)`
  padding: 16px;
  display: flex;
  align-items: center;
  gap: 10px;

  position: absolute;
  bottom: 0px;
  width: 100%;
  background: ${(props) => props.theme.filesPanels.sharing.backgroundButtons};
  border-top: ${(props) => props.theme.filesPanels.sharing.borderTop};
`;

export { StyledEditLinkPanel, StyledScrollbar, StyledButtons };
