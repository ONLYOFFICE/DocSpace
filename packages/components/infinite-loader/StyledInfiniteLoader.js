import { List } from "react-virtualized";
import styled from "styled-components";
import Base from "../themes/base";

const StyledScroll = styled.div`
  overflow: scroll;

  /* Chrome, Edge и Safari */

  ::-webkit-scrollbar {
    width: 8px;
    height: 8px;
  }

  ::-webkit-scrollbar-thumb {
    background-color: ${({ theme }) => theme.scrollbar.backgroundColorVertical};
    border-radius: 3px;

    :hover {
      background-color: ${({ theme }) =>
        theme.scrollbar.hoverBackgroundColorVertical};
    }
  }

  /* Firefox */

  scrollbar-width: thin;
  scrollbar-color: ${({ theme }) => theme.scrollbar.backgroundColorVertical};
`;

const StyledList = styled(List)`
  outline: none;
  overflow: hidden !important;
`;

StyledScroll.defaultProps = {
  theme: Base,
};

export { StyledScroll, StyledList };
