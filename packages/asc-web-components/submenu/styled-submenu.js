import styled from "styled-components";

export const StyledSubmenu = styled.div`
  display: flex;
  flex-direction: column;
`;

export const StyledSubmenuBottomLine = styled.div`
  height: 1px;
  width: 100%;
  margin: -1px 0 15px 0;
  background: #eceef1;
`;

export const StyledSubmenuContentWrapper = styled.div`
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
`;

export const StyledSubmenuItems = styled.div`
  display: flex;
  flex-direction: row;
  gap: 4px;
  padding: 0 20px;
  @media (max-width: 768px) {
    padding: 0 16px;
  }
`;

export const StyledSubmenuItem = styled.div`
  display: flex;
  flex-direction: column;
  line-height: 20px;
`;

export const StyledSubmenuItemText = styled.div`
  width: 100%;
  display: flex;
  padding: 4px 14px;
`;

export const StyledSubmenuItemLabel = styled.div`
  positin: absolute;
  width: 100%;
  height: 4px;
  bottom: 0px;
  border-radius: 4px 4px 0 0;
  background-color: ${(props) => props.color};
`;
