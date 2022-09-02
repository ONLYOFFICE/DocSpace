import styled, { css } from "styled-components";
import { tablet } from "@docspace/components/utils/device";

export const StyledWrapper = styled.div`
  display: flex;
  flex-direction: column;
  border: ${(props) => props.theme.profile.themePreview.border};
  border-radius: 12px;
  height: 284px;
  width: 318px;

  @media ${tablet} {
    width: 100%;
  }

  .header {
    padding: 12px 20px;
    border-bottom: ${(props) => props.theme.profile.themePreview.border};
  }
`;

export const StyledPreview = styled.div`
  display: flex;
  flex-direction: row;
  overflow: hidden;

  .article {
    width: 211px;
    background: ${(props) => props.theme.article.background};
    padding: 0 20px;
    border: none;
    border-radius: 0 0 0 12px;

    .logo {
      padding: 22px 0;
    }

    .main-button {
      width: 100%;
      display: flex;
      justify-content: space-between;
      margin-bottom: 24px;
      background: ${(props) => props.theme.mainButton.backgroundColor};
      padding: ${(props) => props.theme.mainButton.padding};
      border: none;
      border-radius: ${(props) => props.theme.mainButton.borderRadius};

      .text {
        color: ${(props) => props.theme.mainButton.textColor};
      }
    }

    .catalog-header {
      color: ${(props) => props.theme.article.catalogItemHeader};
      padding-left: 12px;
      padding-bottom: 4px;
    }

    .item {
      display: flex;
      gap: 8px;
      padding: 10px 0 10px 10px;
      border-radius: 3px;

      .label {
        color: ${(props) => props.theme.article.catalogItemText};
      }
    }

    .item-active {
      background: ${(props) => props.theme.article.catalogItemActiveBackground};
    }
  }

  .body {
    width: 49px;
    background: ${(props) => props.theme.backgroundColor};
    border: none;
    border-radius: 0 0 12px 0;
    padding-left: 20px;
    padding-top: 22px;

    .search-input {
      width: 40px;
      margin-top: 28px;
      margin-bottom: 4px;
      padding: 6px 8px;
      border: 1px solid ${(props) => props.theme.input.borderColor};
      border-radius: 3px;
      background: ${(props) => props.theme.input.backgroundColor};

      .placeholder {
        color: ${(props) => props.theme.input.borderColor};
      }
    }

    .row-header {
      height: 40px;
      display: flex;
      gap: 4px;
      align-items: center;
    }

    .row {
      display: flex;
      flex-wrap: nowrap;
      gap: 8px;
      align-items: center;
      padding: 8px 0;
      border-top: 1px solid ${(props) => props.theme.row.borderBottom};

      .icon {
        border-radius: 6px;
      }
    }
  }
`;
