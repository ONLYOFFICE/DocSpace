import styled from "styled-components";

import { Base } from "../../../themes";

const StyledEmptyScreen = styled.div<{ withSearch: boolean }>`
  width: 100%;

  display: flex;
  align-items: center;
  flex-direction: column;

  margin-top: ${(props) => (props.withSearch ? "80px" : "64px")};
  padding: 0 28px;

  box-sizing: border-box;

  .empty-image {
    max-width: 72px;
    max-height: 72px;

    margin-bottom: 32px;
  }

  .empty-header {
    font-weight: 700;
    font-size: 16px;
    line-height: 22px;

    margin: 0;
  }

  .empty-description {
    font-weight: 400;
    font-size: 12px;
    line-height: 16px;

    text-align: center;

    color: ${(props) => props.theme.selector.emptyScreen.descriptionColor};

    margin-top: 8px;
  }
`;

StyledEmptyScreen.defaultProps = { theme: Base };

export default StyledEmptyScreen;
