import styled from "styled-components";

import Base from "../../../themes/base";

const StyledBody = styled.div<{
  footerVisible: boolean;
  withHeader?: boolean;
  footerHeight: number;
  headerHeight: number;
}>`
  width: 100%;

  height: ${(props) =>
    props.footerVisible
      ? props.withHeader
        ? `calc(100% - 16px - ${props.footerHeight}px - ${props.headerHeight}px)`
        : `calc(100% - 16px - ${props.footerHeight}px)`
      : props.withHeader
      ? `calc(100% - 16px - ${props.headerHeight}px)`
      : `calc(100% - 16px)`};

  padding: 16px 0 0 0;

  .search-input,
  .search-loader {
    padding: 0 16px;

    margin-bottom: 12px;
  }

  .body-description-text {
    font-size: 13px;
    font-weight: 600;
    line-height: 20px;
    margin-bottom: 12px;

    padding: 0 16px;

    color: ${(props) => props.theme.selector.bodyDescriptionText};
  }
`;

StyledBody.defaultProps = { theme: Base };

export default StyledBody;
