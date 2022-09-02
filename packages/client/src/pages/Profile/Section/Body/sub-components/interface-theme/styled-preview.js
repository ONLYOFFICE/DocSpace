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
    width: 251px;
    background: ${(props) => props.theme.article.background};
    padding: 0 20px;
    border-radius: 0 0 12px 12px;

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
  }
`;
