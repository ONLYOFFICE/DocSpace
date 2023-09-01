import { css } from "styled-components";

export const commonSettingsStyles = css`
  .category-item-wrapper {
    margin-bottom: 40px;

    .category-item-heading {
      display: flex;
      align-items: center;
      margin-bottom: 5px;
    }

    .category-item-subheader {
      font-size: 13px;
      font-weight: 600;
      margin-bottom: 5px;
    }

    .category-item-description {
      color: #555f65;
      font-size: 12px;
      max-width: 1024px;
    }

    .inherit-title-link {
      ${props =>
        props.theme.interfaceDirection === "rtl"
          ? css`
              margin-left: 7px;
            `
          : css`
              margin-right: 7px;
            `}
      font-size: 19px;
      font-weight: 600;
    }

    .link-text {
      margin: 0;
    }
  }
`;
export const UnavailableStyles = css`
  .settings_unavailable {
    color: ${props => props.theme.text.disableColor};
    pointer-events: none;
    cursor: default;

    label {
      color: ${props => props.theme.text.disableColor};
    }

    path {
      fill: ${(props) => props.theme.text.disableColor};
    }
  }
`;
