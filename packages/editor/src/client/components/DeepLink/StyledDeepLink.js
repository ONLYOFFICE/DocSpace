import styled from "styled-components";

export const StyledSimpleNav = styled.div`
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 12px 0;
  background-color: ${(props) => props.theme?.deepLink?.navBackground};
  margin-bottom: 32px;
`;

export const StyledDeepLink = styled.div`
  display: flex;
  flex-direction: column;
  padding: 0 16px;
`;

export const StyledBodyWrapper = styled.div`
  display: flex;
  flex-direction: column;
  gap: 16px;
  margin-bottom: 32px;
`;

export const StyledFileTile = styled.div`
  display: flex;
  gap: 16px;
  padding: 8px 16px;
  background-color: ${(props) => props.theme?.deepLink?.fileTileBackground};
  border-radius: 3px;
  align-items: center;
`;

export const StyledActionsWrapper = styled.div`
  display: flex;
  flex-direction: column;
  gap: 24px;

  .stay-link {
    text-align: center;
  }
`;
