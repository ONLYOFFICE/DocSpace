import { List } from "react-virtualized";
import styled, { css } from "styled-components";
import Base from "../themes/base";
import { desktop, mobile, tablet } from "../utils/device";

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

const rowStyles = css`
  .row-list-item {
    padding-left: 16px;
    width: calc(100% - 33px) !important;

    @media ${mobile} {
      width: calc(100% - 24px) !important;
    }
  }
`;

const tableStyles = css`
  margin-left: -20px;
  width: ${({ width }) => width + 40 + "px !important"};

  .ReactVirtualized__Grid__innerScrollContainer {
    max-width: ${({ width }) => width + 40 + "px !important"};
  }
  .table-container_body-loader {
    width: calc(100% - 48px) !important;
  }

  .table-list-item,
  .table-container_body-loader {
    padding-left: 20px;
  }
`;

const tileStyles = css`
  .files_header {
    padding-top: 11px;
  }
`;

const StyledList = styled(List)`
  outline: none;
  overflow: hidden !important;

  ${({ viewAs }) =>
    viewAs === "row"
      ? rowStyles
      : viewAs === "table"
      ? tableStyles
      : tileStyles}
`;

StyledScroll.defaultProps = {
  theme: Base,
};

export { StyledScroll, StyledList };
