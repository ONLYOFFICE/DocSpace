import styled from "styled-components";

import { tablet } from "../utils/device";

export const StyledSubmenu = styled.div`
  display: flex;
  flex-direction: column;

  .scrollbar {
    width: 100%;
    height: auto;
  }

  .text {
    width: auto;
    display: inline-block;
    position: absolute;
  }
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
  overflow: scroll;

  display: flex;
  flex-direction: row;
  gap: 4px;
  padding: 0 20px;
  @media ${tablet} {
    padding: 0 16px;
  }

  &::-webkit-scrollbar {
    display: none;
  }
`;

export const StyledSubmenuItem = styled.div.attrs((props) => ({
  id: props.id,
}))`
  scroll-behavior: smooth;
  cursor: pointer;
  display: flex;
  gap: 4px;
  flex-direction: column;
  padding: 4px 14px 0;
  line-height: 20px;
`;

export const StyledSubmenuItemText = styled.div`
  width: 100%;
  display: flex;
`;

export const StyledSubmenuItemLabel = styled.div`
  z-index: 1;
  width: calc(100% + 28px);
  margin-left: -14px;
  height: 4px;
  bottom: 0px;
  border-radius: 4px 4px 0 0;
  background-color: ${(props) => props.color};
`;
