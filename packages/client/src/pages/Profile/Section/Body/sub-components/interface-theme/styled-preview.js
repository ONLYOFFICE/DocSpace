import styled from "styled-components";

export const StyledWrapper = styled.div`
  display: flex;
  flex-direction: column;
  border: 1px solid #eceef1;
  border-radius: 12px;

  .header {
    padding: 12px 20px;
    border-bottom: 1px solid #eceef1;
  }
`;

export const StyledPreview = styled.div`
  display: flex;

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
    width: 69px;
    background: ${(props) => props.theme.backgroundColor};
    border: none;
    border-radius: 0 0 12px 0;
  }
`;
