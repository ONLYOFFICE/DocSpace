import styled from "styled-components";
import Base from "../themes/base";

const StyledTableLoader = styled.div`
  grid-column-start: 1;
  grid-column-end: -1;
  display: grid;
  padding-top: 16px;
`;

const StyledRowLoader = styled.div`
  padding-top: 16px;
`;

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

StyledScroll.defaultProps = {
  theme: Base,
};

export { StyledTableLoader, StyledRowLoader, StyledScroll };
