import styled from "styled-components";

import Base from "../../../themes/base";

const StyledBody = styled.div<{
  footerVisible: boolean;
  footerHeight: number;
  headerHeight: number;
}>`
  width: 100%;

  height: ${(props) =>
    props.footerVisible
      ? `calc(100% - 16px - ${props.footerHeight}px - ${props.headerHeight}px)`
      : `calc(100% - 16px - ${props.headerHeight}px)`};

  padding: 16px 0 0 0;

  .search-input,
  .search-loader {
    padding: 0 16px;

    margin-bottom: 12px;
  }
`;

StyledBody.defaultProps = { theme: Base };

export default StyledBody;
